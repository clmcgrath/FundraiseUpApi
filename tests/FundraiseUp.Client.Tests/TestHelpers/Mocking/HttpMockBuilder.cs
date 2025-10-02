using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Moq.Protected;

namespace FundraiseUp.Client.Tests.TestHelpers.Mocking
{
    /// <summary>
    /// Unified HTTP mocking builder that combines response building and message handler setup.
    /// Provides fluent API for creating comprehensive HTTP test scenarios.
    /// </summary>
    public class HttpMockBuilder
    {
        private readonly Mock<HttpMessageHandler> _mockHandler;
        private readonly List<HttpRequestMessage> _requests = new List<HttpRequestMessage>();
        private int _callCount = 0;

        public HttpMockBuilder()
        {
            _mockHandler = new Mock<HttpMessageHandler>();
        }

        /// <summary>
        /// Gets the number of HTTP calls made
        /// </summary>
        public int CallCount => _callCount;

        /// <summary>
        /// Gets all requests that were made
        /// </summary>
        public IReadOnlyList<HttpRequestMessage> Requests => _requests.AsReadOnly();

        #region Response Setup Methods

        /// <summary>
        /// Sets up a mock response for any HTTP request
        /// </summary>
        public HttpMockBuilder SetupAnyRequest(HttpResponseMessage response)
        {
            _mockHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(response)
                .Callback<HttpRequestMessage, CancellationToken>((request, _) =>
                {
                    Interlocked.Increment(ref _callCount);
                    _requests.Add(request);
                });

            return this;
        }

        /// <summary>
        /// Sets up a mock response for a specific HTTP method and URL pattern
        /// </summary>
        public HttpMockBuilder SetupRequest(HttpMethod method, string urlPattern, HttpResponseMessage response)
        {
            _mockHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == method &&
                        req.RequestUri!.AbsolutePath.Contains(urlPattern)),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(response)
                .Callback<HttpRequestMessage, CancellationToken>((request, _) =>
                {
                    Interlocked.Increment(ref _callCount);
                    _requests.Add(request);
                });

            return this;
        }

        /// <summary>
        /// Sets up a sequence of responses for multiple requests
        /// </summary>
        public HttpMockBuilder SetupSequence(params HttpResponseMessage[] responses)
        {
            var setup = _mockHandler
                .Protected()
                .SetupSequence<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>());

            foreach (var response in responses)
            {
                setup = setup.ReturnsAsync(response);
            }

            // Add callback for call counting (separate setup since sequence doesn't support callbacks)
            _mockHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .Callback<HttpRequestMessage, CancellationToken>((request, _) =>
                {
                    Interlocked.Increment(ref _callCount);
                    _requests.Add(request);
                });

            return this;
        }

        #endregion

        #region Latency Simulation Methods

        /// <summary>
        /// Sets up a mock response with simulated network latency
        /// </summary>
        public HttpMockBuilder SetupDelayedRequest(TimeSpan delay, HttpResponseMessage response)
        {
            _mockHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .Returns(async (HttpRequestMessage request, CancellationToken cancellationToken) =>
                {
                    Interlocked.Increment(ref _callCount);
                    _requests.Add(request);

                    // Simulate network latency
                    await Task.Delay(delay, cancellationToken);

                    return response;
                });

            return this;
        }

        /// <summary>
        /// Sets up a mock response with variable network latency (simulates real-world conditions)
        /// </summary>
        public HttpMockBuilder SetupVariableLatencyRequest(TimeSpan minDelay, TimeSpan maxDelay, HttpResponseMessage response)
        {
            var random = new Random();

            _mockHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .Returns(async (HttpRequestMessage request, CancellationToken cancellationToken) =>
                {
                    Interlocked.Increment(ref _callCount);
                    _requests.Add(request);

                    // Simulate variable network latency
                    var delayMs = random.Next((int)minDelay.TotalMilliseconds, (int)maxDelay.TotalMilliseconds);
                    await Task.Delay(delayMs, cancellationToken);

                    return response;
                });

            return this;
        }

        /// <summary>
        /// Sets up a sequence of responses with different latencies (useful for testing retry scenarios)
        /// </summary>
        public HttpMockBuilder SetupLatencySequence(params (TimeSpan delay, HttpResponseMessage response)[] delayedResponses)
        {
            var responseQueue = new Queue<(TimeSpan, HttpResponseMessage)>(delayedResponses);

            _mockHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .Returns(async (HttpRequestMessage request, CancellationToken cancellationToken) =>
                {
                    Interlocked.Increment(ref _callCount);
                    _requests.Add(request);

                    if (responseQueue.Count > 0)
                    {
                        var (delay, response) = responseQueue.Dequeue();
                        await Task.Delay(delay, cancellationToken);
                        return response;
                    }

                    // Default response if queue is empty
                    return new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent("{\"default\": true}", System.Text.Encoding.UTF8, "application/json")
                    };
                });

            return this;
        }

        #endregion

        #region Quick Response Builders

        /// <summary>
        /// Adds a successful 200 OK response to the sequence
        /// </summary>
        public HttpMockBuilder AddSuccessResponse(string content = "{\"success\": true}")
        {
            _mockHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .Returns((HttpRequestMessage request, CancellationToken cancellationToken) =>
                {
                    Interlocked.Increment(ref _callCount);
                    _requests.Add(request);
                    
                    // Create a new response for each request to avoid sharing
                    var response = new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(content, System.Text.Encoding.UTF8, "application/json")
                    };
                    return Task.FromResult(response);
                });

            return this;
        }

        /// <summary>
        /// Adds a successful response with simulated network latency
        /// </summary>
        public HttpMockBuilder AddDelayedSuccessResponse(TimeSpan delay, string content = "{\"success\": true}")
        {
            // Create a factory function that returns a new response each time
            _mockHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .Returns(async (HttpRequestMessage request, CancellationToken cancellationToken) =>
                {
                    await Task.Delay(delay, cancellationToken);
                    Interlocked.Increment(ref _callCount);
                    _requests.Add(request);
                    
                    // Create a new response for each request to avoid sharing
                    return new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(content, System.Text.Encoding.UTF8, "application/json")
                    };
                });

            return this;
        }

        /// <summary>
        /// Adds a successful response with variable latency (simulates realistic network conditions)
        /// </summary>
        public HttpMockBuilder AddVariableLatencyResponse(TimeSpan minDelay, TimeSpan maxDelay, string content = "{\"success\": true}")
        {
            var random = new Random();
            
            _mockHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .Returns(async (HttpRequestMessage request, CancellationToken cancellationToken) =>
                {
                    // Generate random delay between min and max
                    var delayMs = random.Next((int)minDelay.TotalMilliseconds, (int)maxDelay.TotalMilliseconds);
                    await Task.Delay(delayMs, cancellationToken);
                    
                    Interlocked.Increment(ref _callCount);
                    _requests.Add(request);
                    
                    // Create a new response for each request to avoid sharing
                    return new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(content, System.Text.Encoding.UTF8, "application/json")
                    };
                });

            return this;
        }

        /// <summary>
        /// Adds a 429 (Too Many Requests) response with optional Retry-After header
        /// </summary>
        public HttpMockBuilder Add429Response(string? retryAfter = null)
        {
            var response = new HttpResponseMessage((HttpStatusCode)429);
            if (!string.IsNullOrEmpty(retryAfter))
            {
                response.Headers.Add("Retry-After", retryAfter);
            }
            return SetupAnyRequest(response);
        }

        /// <summary>
        /// Adds an error response with the specified status code
        /// </summary>
        public HttpMockBuilder AddErrorResponse(HttpStatusCode statusCode, string? message = null)
        {
            var errorContent = message ?? GetDefaultErrorMessage(statusCode);
            var response = new HttpResponseMessage(statusCode)
            {
                Content = new StringContent(errorContent, System.Text.Encoding.UTF8, "application/json")
            };
            return SetupAnyRequest(response);
        }

        /// <summary>
        /// Adds multiple 429 responses followed by a success response (useful for retry testing)
        /// </summary>
        public HttpMockBuilder AddRateLimitSequence(int rateLimitCount, string? retryAfter = null)
        {
            var responses = new List<HttpResponseMessage>();

            for (int i = 0; i < rateLimitCount; i++)
            {
                var rateLimitResponse = new HttpResponseMessage((HttpStatusCode)429);
                if (!string.IsNullOrEmpty(retryAfter))
                {
                    rateLimitResponse.Headers.Add("Retry-After", retryAfter);
                }
                responses.Add(rateLimitResponse);
            }

            responses.Add(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"success\": true}", System.Text.Encoding.UTF8, "application/json")
            });

            return SetupSequence(responses.ToArray());
        }

        #endregion

        #region Client and Handler Creation

        /// <summary>
        /// Creates an HttpClient with the configured mock handler
        /// </summary>
        public HttpClient CreateHttpClient(string? baseAddress = "https://api.test.com")
        {
            var client = new HttpClient(_mockHandler.Object);
            if (!string.IsNullOrEmpty(baseAddress))
            {
                client.BaseAddress = new Uri(baseAddress);
            }
            return client;
        }

        /// <summary>
        /// Gets the mock handler for advanced verification scenarios
        /// </summary>
        public Mock<HttpMessageHandler> GetMockHandler()
        {
            return _mockHandler;
        }

        /// <summary>
        /// Creates a raw HttpMessageHandler for direct use
        /// </summary>
        public HttpMessageHandler CreateHandler()
        {
            return _mockHandler.Object;
        }

        #endregion

        #region Verification Methods

        /// <summary>
        /// Verifies that a specific HTTP request was made
        /// </summary>
        public void VerifyRequest(HttpMethod method, string urlPattern, Times times)
        {
            _mockHandler
                .Protected()
                .Verify(
                    "SendAsync",
                    times,
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == method &&
                        req.RequestUri!.AbsolutePath.Contains(urlPattern)),
                    ItExpr.IsAny<CancellationToken>());
        }

        /// <summary>
        /// Verifies that no HTTP requests were made
        /// </summary>
        public void VerifyNoRequests()
        {
            _mockHandler
                .Protected()
                .Verify(
                    "SendAsync",
                    Times.Never(),
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>());
        }

        /// <summary>
        /// Verifies the total number of requests made
        /// </summary>
        public void VerifyRequestCount(int expectedCount)
        {
            if (_callCount != expectedCount)
            {
                throw new InvalidOperationException($"Expected {expectedCount} requests, but {_callCount} were made.");
            }
        }

        #endregion

        #region Helper Methods

        private static string GetDefaultErrorMessage(HttpStatusCode statusCode)
        {
            return statusCode switch
            {
                HttpStatusCode.NotFound => "{\"message\":\"Resource not found\"}",
                HttpStatusCode.Unauthorized => "{\"message\":\"Unauthorized access\"}",
                HttpStatusCode.BadRequest => "{\"message\":\"Bad request\"}",
                HttpStatusCode.InternalServerError => "{\"message\":\"Internal server error\"}",
                _ => $"{{\"message\":\"HTTP {(int)statusCode} error\"}}"
            };
        }

        #endregion
    }
}
