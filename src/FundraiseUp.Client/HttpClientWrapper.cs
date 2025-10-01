using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using FundraiseUp.Client.Configuration;
using FundraiseUp.Client.Models;
using FundraiseUp.Client.Utilities;
using Microsoft.Extensions.Logging;

namespace FundraiseUp.Client
{
    /// <summary>
    /// Wrapper for HTTP client operations with FundraiseUp API specific configuration.
    /// </summary>
    internal class HttpClientWrapper : IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly FundraiseUpClientOptions _options;
        private readonly ILogger? _logger;
        private readonly bool _ownsHttpClient;
        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpClientWrapper"/> class.
        /// </summary>
        /// <param name="options">The client options.</param>
        /// <param name="logger">Optional logger instance.</param>
        public HttpClientWrapper(FundraiseUpClientOptions options, ILogger? logger = null)
        {
            _options = options;
            _logger = logger;
            _ownsHttpClient = true; // We created this HttpClient, so we own it

            // Create HTTP client with rate limiting handler
            var rateLimitHandler = new RateLimitHandler(options, logger as ILogger<RateLimitHandler>)
            {
                InnerHandler = new HttpClientHandler()
            };

            _httpClient = new HttpClient(rateLimitHandler)
            {
                BaseAddress = new Uri(options.BaseUrl),
                Timeout = options.Timeout
            };

            // Set default headers
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {options.ApiKey}");
            _httpClient.DefaultRequestHeaders.Add("User-Agent", options.UserAgent);
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpClientWrapper"/> class with a custom HttpClient.
        /// This constructor is typically used with HttpClientFactory where the HttpClient lifecycle is managed externally.
        /// </summary>
        /// <param name="options">The client options.</param>
        /// <param name="httpClient">Custom HTTP client instance (typically from HttpClientFactory).</param>
        /// <param name="logger">Optional logger instance.</param>
        public HttpClientWrapper(FundraiseUpClientOptions options, HttpClient httpClient, ILogger? logger = null)
        {
            _options = options;
            _logger = logger;
            _httpClient = httpClient;
            _ownsHttpClient = false; // External HttpClient (e.g., from HttpClientFactory), don't dispose

            // Note: When using HttpClientFactory, the HttpClient is typically pre-configured
            // in the service registration, so we don't reconfigure it here to avoid conflicts.
            // If needed, additional headers can still be added per request.
        }

        /// <summary>
        /// Sends a GET request asynchronously.
        /// </summary>
        /// <typeparam name="T">The response type.</typeparam>
        /// <param name="endpoint">The API endpoint.</param>
        /// <param name="correlationId">Optional correlation ID.</param>
        /// <returns>The deserialized response.</returns>
        public async Task<T> GetAsync<T>(string endpoint, string? correlationId = null)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, endpoint);
            if (!string.IsNullOrEmpty(correlationId))
            {
                request.Headers.Add("X-Correlation-ID", correlationId);
            }

            var response = await _httpClient.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger?.LogError("API request failed: {StatusCode} - {Content}", response.StatusCode, content);
                ThrowAppropriateException(response.StatusCode, content);
            }

            return JsonSerializer.Deserialize<T>(content, JsonConfiguration.DefaultOptions)!;
        }

        /// <summary>
        /// Sends a POST request asynchronously.
        /// </summary>
        /// <typeparam name="T">The response type.</typeparam>
        /// <param name="endpoint">The API endpoint.</param>
        /// <param name="data">The request data.</param>
        /// <param name="correlationId">Optional correlation ID.</param>
        /// <returns>The deserialized response.</returns>
        public async Task<T> PostAsync<T>(string endpoint, object data, string? correlationId = null)
        {
            var json = JsonSerializer.Serialize(data, JsonConfiguration.DefaultOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Post, endpoint) { Content = content };
            if (!string.IsNullOrEmpty(correlationId))
            {
                request.Headers.Add("X-Correlation-ID", correlationId);
            }

            var response = await _httpClient.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger?.LogError("API request failed: {StatusCode} - {Content}", response.StatusCode, responseContent);
                ThrowAppropriateException(response.StatusCode, responseContent);
            }

            return JsonSerializer.Deserialize<T>(responseContent, JsonConfiguration.DefaultOptions)!;
        }

        /// <summary>
        /// Sends a PUT request asynchronously.
        /// </summary>
        /// <typeparam name="T">The response type.</typeparam>
        /// <param name="endpoint">The API endpoint.</param>
        /// <param name="data">The request data.</param>
        /// <param name="correlationId">Optional correlation ID.</param>
        /// <returns>The deserialized response.</returns>
        public async Task<T> PutAsync<T>(string endpoint, object data, string? correlationId = null)
        {
            var json = JsonSerializer.Serialize(data, JsonConfiguration.DefaultOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Put, endpoint) { Content = content };
            if (!string.IsNullOrEmpty(correlationId))
            {
                request.Headers.Add("X-Correlation-ID", correlationId);
            }

            var response = await _httpClient.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger?.LogError("API request failed: {StatusCode} - {Content}", response.StatusCode, responseContent);
                ThrowAppropriateException(response.StatusCode, responseContent);
            }

            return JsonSerializer.Deserialize<T>(responseContent, JsonConfiguration.DefaultOptions)!;
        }

        /// <summary>
        /// Throws the appropriate exception based on the HTTP status code and response content.
        /// </summary>
        /// <param name="statusCode">The HTTP status code.</param>
        /// <param name="responseContent">The response content.</param>
        private static void ThrowAppropriateException(HttpStatusCode statusCode, string responseContent)
        {
            var statusCodeInt = (int)statusCode;

            if (statusCodeInt == 422) // HttpStatusCode.UnprocessableEntity
            {
                try
                {
                    var validationResponse = JsonSerializer.Deserialize<ValidationErrorResponse>(responseContent, JsonConfiguration.DefaultOptions);
                    var validationErrors = new Dictionary<string, string>();

                    if (validationResponse?.Errors != null)
                    {
                        foreach (var error in validationResponse.Errors)
                        {
                            validationErrors[error.Field] = error.Message;
                        }
                    }

                    throw new Exceptions.FundraiseUpValidationException(statusCodeInt, responseContent, validationErrors);
                }
                catch (JsonException)
                {
                    // If we can't parse the validation response, fall back to regular API exception
                    throw new Exceptions.FundraiseUpApiException(statusCodeInt, responseContent);
                }
            }

            throw new Exceptions.FundraiseUpApiException(statusCodeInt, responseContent);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases the unmanaged resources and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">True to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                // Only dispose the HttpClient if we own it (created it ourselves)
                // HttpClientFactory-managed clients should not be disposed manually
                if (_ownsHttpClient)
                {
                    _httpClient?.Dispose();
                }
                _disposed = true;
            }
        }
    }
}
