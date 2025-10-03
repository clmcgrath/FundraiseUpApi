using System;
using System.Threading.Tasks;
using FundraiseUp.Client.Models;
using FundraiseUp.Client.Requests;
using Microsoft.Extensions.Logging;

namespace FundraiseUp.Client.Operations
{
    /// <summary>
    /// Operations for managing fundraisers in FundraiseUp API.
    /// Fundraisers can be created, updated, and retrieved through these operations.
    /// </summary>
    internal class FundraiserOperations : IFundraiserOperations
    {
        private readonly HttpClientWrapper _httpClient;
        private readonly ILogger? _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="FundraiserOperations"/> class.
        /// </summary>
        /// <param name="httpClient">The HTTP client wrapper for API communication.</param>
        /// <param name="logger">Optional logger for operation tracking.</param>
        public FundraiserOperations(HttpClientWrapper httpClient, ILogger? logger = null)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger;
        }

        /// <inheritdoc />
        public IFundraiserOperationBuilder<FundraiserResponse> Create(CreateFundraiserRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            return new FundraiserOperationBuilder<FundraiserResponse>(_httpClient, _logger)
                .WithOperation(async () =>
                {
                    var endpoint = "/v1/fundraisers";
                    return await _httpClient.PostAsync<FundraiserResponse>(endpoint, request).ConfigureAwait(false);
                });
        }

        /// <inheritdoc />
        public IFundraiserOperationBuilder<FundraiserResponse> GetById(string fundraiserId)
        {
            if (string.IsNullOrWhiteSpace(fundraiserId))
                throw new ArgumentException("Fundraiser ID cannot be null or empty", nameof(fundraiserId));

            return new FundraiserOperationBuilder<FundraiserResponse>(_httpClient, _logger)
                .WithOperation(async () =>
                {
                    var endpoint = $"/v1/fundraisers/{fundraiserId}";
                    return await _httpClient.GetAsync<FundraiserResponse>(endpoint).ConfigureAwait(false);
                });
        }

        /// <inheritdoc />
        public IFundraiserOperationBuilder<FundraiserResponse> Update(string fundraiserId, UpdateFundraiserRequest request)
        {
            if (string.IsNullOrWhiteSpace(fundraiserId))
                throw new ArgumentException("Fundraiser ID cannot be null or empty", nameof(fundraiserId));
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            return new FundraiserOperationBuilder<FundraiserResponse>(_httpClient, _logger)
                .WithOperation(async () =>
                {
                    var endpoint = $"/v1/fundraisers/{fundraiserId}";
                    return await _httpClient.PostAsync<FundraiserResponse>(endpoint, request).ConfigureAwait(false);
                });
        }

        /// <inheritdoc />
        public IFundraiserSearchOperationBuilder Search()
        {
            return new FundraiserSearchOperationBuilder(_httpClient, _logger);
        }
    }

    /// <summary>
    /// Implementation of fundraiser operation builder for FundraiseUp API.
    /// </summary>
    /// <typeparam name="T">The type of result returned by the operation.</typeparam>
    internal class FundraiserOperationBuilder<T> : IFundraiserOperationBuilder<T>
    {
        private readonly HttpClientWrapper _httpClient;
        private readonly ILogger? _logger;
        private Func<Task<T>>? _operation;
        private TimeSpan? _timeout;
        private int _retryCount;
        private string? _correlationId;

        /// <summary>
        /// Initializes a new instance of the <see cref="FundraiserOperationBuilder{T}"/> class.
        /// </summary>
        /// <param name="httpClient">The HTTP client wrapper.</param>
        /// <param name="logger">Optional logger instance.</param>  
        public FundraiserOperationBuilder(HttpClientWrapper httpClient, ILogger? logger = null)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger;
        }

        /// <summary>
        /// Sets the operation to be executed.
        /// </summary>
        /// <param name="operation">The operation function.</param>
        /// <returns>The current builder instance.</returns>
        public FundraiserOperationBuilder<T> WithOperation(Func<Task<T>> operation)
        {
            _operation = operation ?? throw new ArgumentNullException(nameof(operation));
            return this;
        }

        /// <inheritdoc />
        public IFundraiserOperationBuilder<T> WithTimeout(TimeSpan timeout)
        {
            _timeout = timeout;
            return this;
        }

        /// <inheritdoc />
        public IFundraiserOperationBuilder<T> WithRetry(int retryCount)
        {
            _retryCount = Math.Max(0, retryCount);
            return this;
        }

        /// <inheritdoc />
        public IFundraiserOperationBuilder<T> WithCorrelationId(string correlationId)
        {
            _correlationId = correlationId;
            return this;
        }

        /// <inheritdoc />
        public async Task<T> ExecuteAsync()
        {
            if (_operation == null)
                throw new InvalidOperationException("No operation has been configured.");

            _logger?.LogDebug("Executing fundraiser operation with correlation ID: {CorrelationId}", _correlationId);

            try
            {
                return await _operation().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Fundraiser operation failed with correlation ID: {CorrelationId}", _correlationId);
                throw;
            }
        }
    }

    /// <summary>
    /// Implementation of fundraiser search operation builder.
    /// </summary>
    internal class FundraiserSearchOperationBuilder : IFundraiserSearchOperationBuilder
    {
        private readonly HttpClientWrapper _httpClient;
        private readonly ILogger? _logger;
        private string? _cursor;
        private int? _limit;
        private string? _campaignId;
        private string? _status;
        private string? _creatorEmail;
        private DateTime? _createdAfter;
        private DateTime? _createdBefore;
        private TimeSpan? _timeout;
        private int _retryCount;
        private string? _correlationId;

        /// <summary>
        /// Initializes a new instance of the <see cref="FundraiserSearchOperationBuilder"/> class.
        /// </summary>
        /// <param name="httpClient">The HTTP client wrapper.</param>
        /// <param name="logger">Optional logger instance.</param>
        public FundraiserSearchOperationBuilder(HttpClientWrapper httpClient, ILogger? logger = null)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger;
        }

        /// <inheritdoc />
        public IFundraiserSearchOperationBuilder WithCursor(string cursor)
        {
            _cursor = cursor;
            return this;
        }

        /// <inheritdoc />
        public IFundraiserSearchOperationBuilder WithLimit(int limit)
        {
            _limit = Math.Max(1, Math.Min(100, limit));
            return this;
        }

        /// <inheritdoc />
        public IFundraiserSearchOperationBuilder ByCampaign(string campaignId)
        {
            _campaignId = campaignId;
            return this;
        }

        /// <inheritdoc />
        public IFundraiserSearchOperationBuilder ByStatus(string status)
        {
            _status = status;
            return this;
        }

        /// <inheritdoc />
        public IFundraiserSearchOperationBuilder ByCreatorEmail(string creatorEmail)
        {
            _creatorEmail = creatorEmail;
            return this;
        }

        /// <inheritdoc />
        public IFundraiserSearchOperationBuilder CreatedAfter(DateTime dateTime)
        {
            _createdAfter = dateTime;
            return this;
        }

        /// <inheritdoc />
        public IFundraiserSearchOperationBuilder CreatedBefore(DateTime dateTime)
        {
            _createdBefore = dateTime;
            return this;
        }

        /// <inheritdoc />
        public IFundraiserSearchOperationBuilder WithTimeout(TimeSpan timeout)
        {
            _timeout = timeout;
            return this;
        }

        /// <inheritdoc />
        public IFundraiserSearchOperationBuilder WithRetry(int retryCount)
        {
            _retryCount = Math.Max(0, retryCount);
            return this;
        }

        /// <inheritdoc />
        public IFundraiserSearchOperationBuilder WithCorrelationId(string correlationId)
        {
            _correlationId = correlationId;
            return this;
        }

        /// <inheritdoc />
        public async Task<FundraisersResponse> ExecuteAsync()
        {
            _logger?.LogDebug("Executing fundraiser search with correlation ID: {CorrelationId}", _correlationId);

            try
            {
                var queryParams = new System.Collections.Generic.List<string>();

                if (_limit.HasValue)
                    queryParams.Add($"limit={_limit}");

                if (!string.IsNullOrEmpty(_cursor))
                    queryParams.Add($"starting_after={Uri.EscapeDataString(_cursor)}");

                if (!string.IsNullOrEmpty(_campaignId))
                    queryParams.Add($"campaign={Uri.EscapeDataString(_campaignId)}");

                if (!string.IsNullOrEmpty(_status))
                    queryParams.Add($"status={Uri.EscapeDataString(_status)}");

                if (!string.IsNullOrEmpty(_creatorEmail))
                    queryParams.Add($"creator_email={Uri.EscapeDataString(_creatorEmail)}");

                if (_createdAfter.HasValue)
                {
                    var timestamp = ((DateTimeOffset)_createdAfter.Value).ToUnixTimeSeconds();
                    queryParams.Add($"created[gte]={timestamp}");
                }

                if (_createdBefore.HasValue)
                {
                    var timestamp = ((DateTimeOffset)_createdBefore.Value).ToUnixTimeSeconds();
                    queryParams.Add($"created[lte]={timestamp}");
                }

                var endpoint = "/v1/fundraisers";
                if (queryParams.Count > 0)
                    endpoint += "?" + string.Join("&", queryParams);

                var response = await _httpClient.GetAsync<CursorPagedResponse<FundraiserResponse>>(endpoint, _correlationId).ConfigureAwait(false);

                return new FundraisersResponse
                {
                    Data = response.Data,
                    HasMore = response.HasMore
                };
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Fundraiser search failed with correlation ID: {CorrelationId}", _correlationId);
                throw;
            }
        }
    }
}
