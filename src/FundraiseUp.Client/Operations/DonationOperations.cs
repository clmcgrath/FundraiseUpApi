using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FundraiseUp.Client.Models;
using FundraiseUp.Client.Requests;
using Microsoft.Extensions.Logging;

namespace FundraiseUp.Client.Operations
{
    /// <summary>
    /// Implementation of donation operations for FundraiseUp API.
    /// </summary>
    internal class DonationOperations : IDonationOperations
    {
        private readonly HttpClientWrapper _httpClient;
        private readonly ILogger? _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="DonationOperations"/> class.
        /// </summary>
        /// <param name="httpClient">The HTTP client wrapper.</param>
        /// <param name="logger">Optional logger instance.</param>
        public DonationOperations(HttpClientWrapper httpClient, ILogger? logger = null)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        /// <inheritdoc />
        public IDonationOperationBuilder<DonationResponse> Create(CreateDonationRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            return new DonationOperationBuilder<DonationResponse>(_httpClient, _logger, async (correlationId) =>
            {
                _logger?.LogInformation("Creating new donation with correlation ID: {CorrelationId}", correlationId);
                return await _httpClient.PostAsync<DonationResponse>("/v1/donations", request, correlationId);
            });
        }

        /// <inheritdoc />
        public IDonationOperationBuilder<DonationResponse> GetById(string donationId)
        {
            if (string.IsNullOrWhiteSpace(donationId))
                throw new ArgumentException("Donation ID cannot be null or empty", nameof(donationId));

            return new DonationOperationBuilder<DonationResponse>(_httpClient, _logger, async (correlationId) =>
            {
                return await _httpClient.GetAsync<DonationResponse>($"/v1/donations/{donationId}", correlationId);
            });
        }

        /// <inheritdoc />
        public IDonationListOperationBuilder List()
        {
            return new DonationListOperationBuilder(_httpClient, _logger);
        }

        /// <inheritdoc />
        public IDonationOperationBuilder<DonationResponse> Update(string donationId, UpdateDonationRequest request)
        {
            if (string.IsNullOrWhiteSpace(donationId))
                throw new ArgumentException("Donation ID cannot be null or empty", nameof(donationId));
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            // Validate the update request
            var validationErrors = request.GetValidationErrors();
            if (validationErrors.Count > 0)
            {
                throw new ArgumentException($"Invalid update request: {string.Join("; ", validationErrors)}", nameof(request));
            }

            return new DonationOperationBuilder<DonationResponse>(_httpClient, _logger, async (correlationId) =>
            {
                _logger?.LogInformation("Updating donation {DonationId} with correlation ID: {CorrelationId}", donationId, correlationId);
                return await _httpClient.PostAsync<DonationResponse>($"/v1/donations/{donationId}", request, correlationId);
            });
        }
    }

    /// <summary>
    /// Implementation of donation operation builder.
    /// </summary>
    /// <typeparam name="TResult">The result type.</typeparam>
    internal class DonationOperationBuilder<TResult> : IDonationOperationBuilder<TResult>
    {
        private readonly HttpClientWrapper _httpClient;
        private readonly ILogger? _logger;
        private readonly Func<string?, Task<TResult>> _operation;
        private TimeSpan? _timeout;
        private int? _retryCount;
        private string? _correlationId;

        /// <summary>
        /// Initializes a new instance of the <see cref="DonationOperationBuilder{TResult}"/> class.
        /// </summary>
        /// <param name="httpClient">The HTTP client wrapper.</param>
        /// <param name="logger">Optional logger instance.</param>
        /// <param name="operation">The operation to execute.</param>
        public DonationOperationBuilder(HttpClientWrapper httpClient, ILogger? logger, Func<string?, Task<TResult>> operation)
        {
            _httpClient = httpClient;
            _logger = logger;
            _operation = operation;
        }

        /// <inheritdoc />
        public IDonationOperationBuilder<TResult> WithTimeout(TimeSpan timeout)
        {
            _timeout = timeout;
            return this;
        }

        /// <inheritdoc />
        public IDonationOperationBuilder<TResult> WithRetry(int retryCount)
        {
            _retryCount = retryCount;
            return this;
        }

        /// <inheritdoc />
        public IDonationOperationBuilder<TResult> WithCorrelationId(string correlationId)
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
    /// Implementation of donation list operation builder for FundraiseUp API.
    /// </summary>
    internal class DonationListOperationBuilder : IDonationListOperationBuilder
    {
        private readonly HttpClientWrapper _httpClient;
        private readonly ILogger? _logger;
        private readonly Dictionary<string, string> _queryParameters = new();
        private TimeSpan? _timeout;
        private int? _retryCount;
        private string? _correlationId;

        /// <summary>
        /// Initializes a new instance of the <see cref="DonationListOperationBuilder"/> class.
        /// </summary>
        /// <param name="httpClient">The HTTP client wrapper.</param>
        /// <param name="logger">Optional logger instance.</param>
        public DonationListOperationBuilder(HttpClientWrapper httpClient, ILogger? logger = null)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        /// <inheritdoc />
        public IDonationListOperationBuilder WithCursor(string cursor)
        {
            if (!string.IsNullOrWhiteSpace(cursor))
                _queryParameters["cursor"] = cursor;
            return this;
        }

        /// <inheritdoc />
        public IDonationListOperationBuilder WithLimit(int limit)
        {
            if (limit > 0 && limit <= 100)
                _queryParameters["limit"] = limit.ToString();
            return this;
        }

        /// <inheritdoc />
        public IDonationListOperationBuilder ByCampaign(string campaignId)
        {
            if (!string.IsNullOrWhiteSpace(campaignId))
                _queryParameters["campaign_id"] = campaignId;
            return this;
        }

        /// <inheritdoc />
        public IDonationListOperationBuilder BySupporter(string supporterId)
        {
            if (!string.IsNullOrWhiteSpace(supporterId))
                _queryParameters["supporter_id"] = supporterId;
            return this;
        }

        /// <inheritdoc />
        public IDonationListOperationBuilder ByStatus(string status)
        {
            if (!string.IsNullOrWhiteSpace(status))
                _queryParameters["status"] = status;
            return this;
        }

        /// <inheritdoc />
        public IDonationListOperationBuilder Where(Expression<Func<DonationResponse, bool>> predicate)
        {
            // Note: FundraiseUp API has limited filtering capabilities
            // This is kept for compatibility but may not translate to API filters
            _logger?.LogWarning("Complex where expressions may not be supported by FundraiseUp API");
            return this;
        }

        /// <inheritdoc />
        public IDonationListOperationBuilder OrderBy<TKey>(Expression<Func<DonationResponse, TKey>> keySelector)
        {
            // Note: FundraiseUp API has limited sorting capabilities
            // This is kept for compatibility but may not translate to API sorting
            _logger?.LogWarning("Custom ordering may not be supported by FundraiseUp API");
            return this;
        }

        /// <inheritdoc />
        public IDonationListOperationBuilder Take(int count)
        {
            return WithLimit(count);
        }

        /// <inheritdoc />
        public IDonationListOperationBuilder WithTimeout(TimeSpan timeout)
        {
            _timeout = timeout;
            return this;
        }

        /// <inheritdoc />
        public IDonationListOperationBuilder WithRetry(int retryCount)
        {
            _retryCount = retryCount;
            return this;
        }

        /// <inheritdoc />
        public IDonationListOperationBuilder WithCorrelationId(string correlationId)
        {
            _correlationId = correlationId;
            return this;
        }

        /// <inheritdoc />
        public async Task<PagedResult<DonationResponse>> ExecuteAsync()
        {
            var queryString = string.Join("&", _queryParameters.Select(kvp => $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}"));
            var endpoint = string.IsNullOrEmpty(queryString) ? "/v1/donations" : $"/v1/donations?{queryString}";

            var response = await _httpClient.GetAsync<DonationsResponse>(endpoint, _correlationId);

            // Convert to PagedResult for backward compatibility
            return new PagedResult<DonationResponse>
            {
                Items = response.Data,
                NextCursor = null, // DonationsResponse uses HasMore instead of cursor
                HasMore = response.HasMore,
                PageSize = _queryParameters.ContainsKey("limit") ? int.Parse(_queryParameters["limit"]) : 20
            };
        }
    }
}
