using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FundraiseUp.Client.Configuration;
using FundraiseUp.Client.Exceptions;
using Microsoft.Extensions.Logging;

namespace FundraiseUp.Client.Utilities
{
    /// <summary>
    /// HTTP message handler that enforces FundraiseUp API rate limiting.
    /// </summary>
    public class RateLimitHandler : DelegatingHandler, IDisposable
    {
        private readonly FundraiseUpClientOptions _options;
        private readonly ILogger<RateLimitHandler>? _logger;
        private readonly SemaphoreSlim _semaphore;
        private readonly SemaphoreSlim _queueSemaphore;
        private long _currentRequests;

        /// <summary>
        /// Initializes a new instance of the <see cref="RateLimitHandler"/> class.
        /// </summary>
        /// <param name="options">The client configuration options.</param>
        /// <param name="logger">Optional logger for rate limiting events.</param>
        public RateLimitHandler(FundraiseUpClientOptions options, ILogger<RateLimitHandler>? logger = null)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _logger = logger;
            _semaphore = new SemaphoreSlim(options.MaxConcurrentRequests, options.MaxConcurrentRequests);
            _queueSemaphore = new SemaphoreSlim(options.MaxQueueSize, options.MaxQueueSize);
        }

        /// <summary>
        /// Sends an HTTP request with rate limiting applied based on the configured strategy.
        /// </summary>
        /// <param name="request">The HTTP request message.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The HTTP response message.</returns>
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            switch (_options.RateLimitStrategy)
            {
                case RateLimitStrategy.Exception:
                    return await SendWithExceptionStrategy(request, cancellationToken);

                case RateLimitStrategy.Queue:
                    return await SendWithQueueStrategy(request, cancellationToken);

                case RateLimitStrategy.Retry:
                    return await SendWithRetryStrategy(request, cancellationToken);

                default:
                    throw new InvalidOperationException($"Unsupported rate limit strategy: {_options.RateLimitStrategy}");
            }
        }

        /// <summary>
        /// Exception strategy: Throw immediately if rate limit would be exceeded.
        /// </summary>
        private async Task<HttpResponseMessage> SendWithExceptionStrategy(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (!_semaphore.Wait(0))
            {
                var currentRequests = Interlocked.Read(ref _currentRequests);
                _logger?.LogWarning("Rate limit exceeded using Exception strategy. Current requests: {CurrentRequests}/{MaxRequests}",
                    currentRequests, _options.MaxConcurrentRequests);

                throw new RateLimitExceededException((int)currentRequests, _options.MaxConcurrentRequests);
            }

            try
            {
                Interlocked.Increment(ref _currentRequests);
                _logger?.LogDebug("Acquired rate limit slot. Current requests: {CurrentRequests}/{MaxRequests}",
                    _currentRequests, _options.MaxConcurrentRequests);

                return await base.SendAsync(request, cancellationToken);
            }
            finally
            {
                Interlocked.Decrement(ref _currentRequests);
                _semaphore.Release();
                _logger?.LogDebug("Released rate limit slot. Current requests: {CurrentRequests}/{MaxRequests}",
                    _currentRequests, _options.MaxConcurrentRequests);
            }
        }

        /// <summary>
        /// Queue strategy: Queue requests when rate limit is reached.
        /// </summary>
        private async Task<HttpResponseMessage> SendWithQueueStrategy(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // First, try to get a queue slot
            if (!_queueSemaphore.Wait(0, cancellationToken))
            {
                _logger?.LogWarning("Request queue is full ({MaxQueueSize} requests). Rejecting request.", _options.MaxQueueSize);
                throw new RateLimitExceededException(_options.MaxQueueSize, _options.MaxQueueSize);
            }

            try
            {
                // Now wait for a rate limit slot with timeout
                using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                timeoutCts.CancelAfter(_options.QueueTimeout);

                var waitResult = await _semaphore.WaitAsync(_options.QueueTimeout, timeoutCts.Token);
                if (!waitResult)
                {
                    _logger?.LogWarning("Request timed out waiting in queue after {QueueTimeout}", _options.QueueTimeout);
                    throw new TimeoutException($"Request timed out waiting in queue after {_options.QueueTimeout}");
                }

                try
                {
                    Interlocked.Increment(ref _currentRequests);
                    _logger?.LogDebug("Processing queued request. Current requests: {CurrentRequests}/{MaxRequests}",
                        _currentRequests, _options.MaxConcurrentRequests);

                    return await base.SendAsync(request, cancellationToken);
                }
                finally
                {
                    Interlocked.Decrement(ref _currentRequests);
                    _semaphore.Release();
                }
            }
            finally
            {
                _queueSemaphore.Release();
            }
        }

        /// <summary>
        /// Retry strategy: Retry with exponential backoff when rate limit is hit.
        /// </summary>
        private async Task<HttpResponseMessage> SendWithRetryStrategy(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var retryCount = 0;
            var maxRetries = _options.MaxRetryAttempts;
            var baseDelay = _options.RetryDelay;

            while (retryCount <= maxRetries)
            {
                if (_semaphore.Wait(0, cancellationToken))
                {
                    try
                    {
                        Interlocked.Increment(ref _currentRequests);
                        _logger?.LogDebug("Acquired rate limit slot on attempt {Attempt}. Current requests: {CurrentRequests}/{MaxRequests}",
                            retryCount + 1, _currentRequests, _options.MaxConcurrentRequests);

                        var response = await base.SendAsync(request, cancellationToken);

                        // Check if we got a 429 Too Many Requests response  
                        if (response.StatusCode == HttpStatusCode.TooManyRequests)
                        {
                            var retryAfter = GetRetryAfterSeconds(response);
                            if (retryCount < maxRetries)
                            {
                                var delay = retryAfter.HasValue
                                    ? TimeSpan.FromSeconds(retryAfter.Value)
                                    : TimeSpan.FromMilliseconds(baseDelay.TotalMilliseconds * Math.Pow(2, retryCount));

                                _logger?.LogWarning("Received 429 Too Many Requests. Retrying after {Delay}ms (attempt {Attempt}/{MaxRetries})",
                                    delay.TotalMilliseconds, retryCount + 1, maxRetries);

                                await Task.Delay(delay, cancellationToken);
                                retryCount++;
                                continue;
                            }
                        }

                        return response;
                    }
                    finally
                    {
                        Interlocked.Decrement(ref _currentRequests);
                        _semaphore.Release();
                    }
                }
                else
                {
                    // Rate limit exceeded, wait and retry
                    if (retryCount >= maxRetries)
                    {
                        var currentRequests = Interlocked.Read(ref _currentRequests);
                        _logger?.LogError("Rate limit exceeded after {MaxRetries} attempts. Current requests: {CurrentRequests}/{MaxRequests}",
                            maxRetries, currentRequests, _options.MaxConcurrentRequests);

                        throw new RateLimitExceededException((int)currentRequests, _options.MaxConcurrentRequests);
                    }

                    var delay = TimeSpan.FromMilliseconds(baseDelay.TotalMilliseconds * Math.Pow(2, retryCount));
                    _logger?.LogWarning("Rate limit exceeded. Retrying after {Delay}ms (attempt {Attempt}/{MaxRetries})",
                        delay.TotalMilliseconds, retryCount + 1, maxRetries);

                    await Task.Delay(delay, cancellationToken);
                    retryCount++;
                }
            }

            // This should never be reached, but just in case
            throw new RateLimitExceededException(
                $"Fallback in SendWithRetryStrategy reached after {retryCount} attempts. " +
                $"Current requests: {_currentRequests}/{_options.MaxConcurrentRequests}. " +
                $"Request: {request.Method} {request.RequestUri?.AbsolutePath}",
                (int)_currentRequests,
                _options.MaxConcurrentRequests
            );
        }

        /// <summary>
        /// Extracts the Retry-After header value from an HTTP response.
        /// </summary>
        /// <param name="response">The HTTP response.</param>
        /// <returns>The number of seconds to wait, or null if not specified.</returns>
        private static int? GetRetryAfterSeconds(HttpResponseMessage response)
        {
            if (response.Headers.RetryAfter?.Delta.HasValue == true)
            {
                return (int)response.Headers.RetryAfter.Delta.Value.TotalSeconds;
            }

            if (response.Headers.RetryAfter?.Date.HasValue == true)
            {
                var waitTime = response.Headers.RetryAfter.Date.Value - DateTimeOffset.UtcNow;
                return waitTime.TotalSeconds > 0 ? (int)waitTime.TotalSeconds : null;
            }

            return null;
        }

        /// <summary>
        /// Disposes the rate limiting resources.
        /// </summary>
        /// <param name="disposing">True if disposing managed resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _semaphore?.Dispose();
                _queueSemaphore?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}