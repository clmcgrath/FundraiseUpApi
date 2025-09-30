using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using FundraiseUp.Client.Configuration;
using FundraiseUp.Client.Models;

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

            _httpClient = new HttpClient
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
        /// </summary>
        /// <param name="options">The client options.</param>
        /// <param name="httpClient">Custom HTTP client instance.</param>
        /// <param name="logger">Optional logger instance.</param>
        public HttpClientWrapper(FundraiseUpClientOptions options, HttpClient httpClient, ILogger? logger = null)
        {
            _options = options;
            _logger = logger;
            _httpClient = httpClient;

            // Configure the provided HttpClient
            _httpClient.BaseAddress = new Uri(options.BaseUrl);
            _httpClient.Timeout = options.Timeout;

            // Set default headers
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {options.ApiKey}");
            _httpClient.DefaultRequestHeaders.Add("User-Agent", options.UserAgent);
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

            // Add any additional headers
            foreach (var header in options.AdditionalHeaders)
            {
                _httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
            }
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
                _httpClient?.Dispose();
                _disposed = true;
            }
        }
    }
}