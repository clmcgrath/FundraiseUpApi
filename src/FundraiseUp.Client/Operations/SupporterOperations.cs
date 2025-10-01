using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using FundraiseUp.Client.Models;

namespace FundraiseUp.Client.Operations
{
    /// <summary>
    /// Implementation of supporter operations for FundraiseUp API.
    /// Note: Supporters are read-only in the FundraiseUp API and cannot be created or modified directly.
    /// They are created automatically when donations are made.
    /// </summary>
    internal class SupporterOperations : ISupporterOperations
    {
        private readonly HttpClientWrapper _httpClient;
        private readonly ILogger? _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="SupporterOperations"/> class.
        /// </summary>
        /// <param name="httpClient">The HTTP client wrapper.</param>
        /// <param name="logger">Optional logger instance.</param>
        public SupporterOperations(HttpClientWrapper httpClient, ILogger? logger = null)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        /// <inheritdoc />
        public ISupporterOperationBuilder<SupporterResponse> GetById(string supporterId)
        {
            if (string.IsNullOrWhiteSpace(supporterId))
                throw new ArgumentException("Supporter ID cannot be null or empty", nameof(supporterId));

            return new SupporterOperationBuilder<SupporterResponse>(_httpClient, _logger, async (correlationId) =>
            {
                return await _httpClient.GetAsync<SupporterResponse>($"/v1/supporters/{supporterId}", correlationId);
            });
        }

        /// <inheritdoc />
        public ISupporterSearchOperationBuilder Search()
        {
            return new SupporterSearchOperationBuilder(_httpClient, _logger);
        }

        /// <inheritdoc />
        public IDonationListOperationBuilder GetDonations(string supporterId)
        {
            if (string.IsNullOrWhiteSpace(supporterId))
                throw new ArgumentException("Supporter ID cannot be null or empty", nameof(supporterId));

            return new DonationListOperationBuilder(_httpClient, _logger).BySupporter(supporterId);
        }
    }

    /// <summary>
    /// Implementation of supporter operation builder for FundraiseUp API.
    /// </summary>
    /// <typeparam name="TResult">The result type.</typeparam>
    internal class SupporterOperationBuilder<TResult> : ISupporterOperationBuilder<TResult>
    {
        private readonly HttpClientWrapper _httpClient;
        private readonly ILogger? _logger;
        private readonly Func<string?, Task<TResult>> _operation;
        private TimeSpan? _timeout;
        private int? _retryCount;
        private string? _correlationId;

        /// <summary>
        /// Initializes a new instance of the <see cref="SupporterOperationBuilder{TResult}"/> class.
        /// </summary>
        /// <param name="httpClient">The HTTP client wrapper.</param>
        /// <param name="logger">Optional logger instance.</param>
        /// <param name="operation">The operation to execute.</param>
        public SupporterOperationBuilder(HttpClientWrapper httpClient, ILogger? logger, Func<string?, Task<TResult>> operation)
        {
            _httpClient = httpClient;
            _logger = logger;
            _operation = operation;
        }

        /// <inheritdoc />
        public ISupporterOperationBuilder<TResult> WithTimeout(TimeSpan timeout)
        {
            _timeout = timeout;
            return this;
        }

        /// <inheritdoc />
        public ISupporterOperationBuilder<TResult> WithRetry(int retryCount)
        {
            _retryCount = retryCount;
            return this;
        }

        /// <inheritdoc />
        public ISupporterOperationBuilder<TResult> WithCorrelationId(string correlationId)
        {
            _correlationId = correlationId;
            return this;
        }

        /// <inheritdoc />
        public async Task<TResult> ExecuteAsync()
        {
            return await _operation(_correlationId);
        }
    }

    /// <summary>
    /// Implementation of supporter search operation builder for FundraiseUp API with cursor-based pagination.
    /// </summary>
    internal class SupporterSearchOperationBuilder : ISupporterSearchOperationBuilder
    {
        private readonly HttpClientWrapper _httpClient;
        private readonly ILogger? _logger;
        private readonly Dictionary<string, string> _queryParameters = new();
        private TimeSpan? _timeout;
        private int? _retryCount;
        private string? _correlationId;

        /// <summary>
        /// Initializes a new instance of the <see cref="SupporterSearchOperationBuilder"/> class.
        /// </summary>
        /// <param name="httpClient">The HTTP client wrapper.</param>
        /// <param name="logger">Optional logger instance.</param>
        public SupporterSearchOperationBuilder(HttpClientWrapper httpClient, ILogger? logger = null)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        /// <inheritdoc />
        public ISupporterSearchOperationBuilder WithCursor(string cursor)
        {
            if (!string.IsNullOrWhiteSpace(cursor))
                _queryParameters["cursor"] = cursor;
            return this;
        }

        /// <inheritdoc />
        public ISupporterSearchOperationBuilder WithLimit(int limit)
        {
            if (limit > 0 && limit <= 100)
                _queryParameters["limit"] = limit.ToString();
            return this;
        }

        /// <inheritdoc />
        public ISupporterSearchOperationBuilder WithTimeout(TimeSpan timeout)
        {
            _timeout = timeout;
            return this;
        }

        /// <inheritdoc />
        public ISupporterSearchOperationBuilder WithRetry(int retryCount)
        {
            _retryCount = retryCount;
            return this;
        }

        /// <inheritdoc />
        public ISupporterSearchOperationBuilder WithCorrelationId(string correlationId)
        {
            _correlationId = correlationId;
            return this;
        }

        /// <inheritdoc />
        public ISupporterSearchOperationBuilder Where(System.Linq.Expressions.Expression<Func<SupporterResponse, bool>> predicate)
        {
            _logger?.LogWarning("Complex where expressions may not be fully supported by FundraiseUp API. Consider using API-specific filtering methods.");
            // For now, we'll store the expression but not process it since FundraiseUp API has limited filtering
            return this;
        }

        /// <inheritdoc />
        public async Task<SupportersResponse> ExecuteAsync()
        {
            var queryString = string.Join("&", _queryParameters.Select(kvp => $"{kvp.Key}={Uri.EscapeDataString(kvp.Value)}"));
            var endpoint = string.IsNullOrEmpty(queryString) ? "/v1/supporters" : $"/v1/supporters?{queryString}";

            var response = await _httpClient.GetAsync<SupportersResponse>(endpoint, _correlationId);
            return response;
        }
    }
}