using System;
using System.Threading.Tasks;
using FundraiseUp.Client.Models;
using Microsoft.Extensions.Logging;

namespace FundraiseUp.Client.Operations
{
    /// <summary>
    /// Operations for managing recurring donation plans in FundraiseUp API.
    /// Recurring plans are read-only and are created automatically when a donation includes a recurring plan.
    /// </summary>
    internal class RecurringPlanOperations : IRecurringPlanOperations
    {
        private readonly HttpClientWrapper _httpClient;
        private readonly ILogger? _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="RecurringPlanOperations"/> class.
        /// </summary>
        /// <param name="httpClient">The HTTP client wrapper for API communication.</param>
        /// <param name="logger">Optional logger for operation tracking.</param>
        public RecurringPlanOperations(HttpClientWrapper httpClient, ILogger? logger = null)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger;
        }

        /// <inheritdoc />
        public IRecurringPlanOperationBuilder<RecurringPlanResponse> GetById(string recurringPlanId)
        {
            if (string.IsNullOrWhiteSpace(recurringPlanId))
                throw new ArgumentException("Recurring plan ID cannot be null or empty", nameof(recurringPlanId));

            return new RecurringPlanOperationBuilder<RecurringPlanResponse>(_httpClient, _logger)
                .WithOperation(async () =>
                {
                    var endpoint = $"/v1/recurring_plans/{recurringPlanId}";
                    return await _httpClient.GetAsync<RecurringPlanResponse>(endpoint);
                });
        }

        /// <inheritdoc />
        public IRecurringPlanSearchOperationBuilder Search()
        {
            return new RecurringPlanSearchOperationBuilder(_httpClient, _logger);
        }
    }

    /// <summary>
    /// Implementation of recurring plan operation builder for FundraiseUp API.
    /// </summary>
    /// <typeparam name="T">The type of result returned by the operation.</typeparam>
    internal class RecurringPlanOperationBuilder<T> : IRecurringPlanOperationBuilder<T>
    {
        private readonly HttpClientWrapper _httpClient;
        private readonly ILogger? _logger;
        private Func<Task<T>>? _operation;
        private TimeSpan? _timeout;
        private int _retryCount;
        private string? _correlationId;

        /// <summary>
        /// Initializes a new instance of the <see cref="RecurringPlanOperationBuilder{T}"/> class.
        /// </summary>
        /// <param name="httpClient">The HTTP client wrapper.</param>
        /// <param name="logger">Optional logger instance.</param>
        public RecurringPlanOperationBuilder(HttpClientWrapper httpClient, ILogger? logger = null)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger;
        }

        /// <summary>
        /// Sets the operation to be executed.
        /// </summary>
        /// <param name="operation">The operation function.</param>
        /// <returns>The current builder instance.</returns>
        public RecurringPlanOperationBuilder<T> WithOperation(Func<Task<T>> operation)
        {
            _operation = operation ?? throw new ArgumentNullException(nameof(operation));
            return this;
        }

        /// <inheritdoc />
        public IRecurringPlanOperationBuilder<T> WithTimeout(TimeSpan timeout)
        {
            _timeout = timeout;
            return this;
        }

        /// <inheritdoc />
        public IRecurringPlanOperationBuilder<T> WithRetry(int retryCount)
        {
            _retryCount = Math.Max(0, retryCount);
            return this;
        }

        /// <inheritdoc />
        public IRecurringPlanOperationBuilder<T> WithCorrelationId(string correlationId)
        {
            _correlationId = correlationId;
            return this;
        }

        /// <inheritdoc />
        public async Task<T> ExecuteAsync()
        {
            if (_operation == null)
                throw new InvalidOperationException("No operation has been configured.");

            _logger?.LogDebug("Executing recurring plan operation with correlation ID: {CorrelationId}", _correlationId);

            try
            {
                // Configure HTTP client if needed
                if (_timeout.HasValue || !string.IsNullOrEmpty(_correlationId))
                {
                    // Implementation would configure the HTTP client here
                    // For now, we rely on the HttpClientWrapper to handle these settings
                }

                return await _operation();
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Recurring plan operation failed with correlation ID: {CorrelationId}", _correlationId);
                throw;
            }
        }
    }

    /// <summary>
    /// Implementation of recurring plan search operation builder.
    /// </summary>
    internal class RecurringPlanSearchOperationBuilder : IRecurringPlanSearchOperationBuilder
    {
        private readonly HttpClientWrapper _httpClient;
        private readonly ILogger? _logger;
        private string? _cursor;
        private int? _limit;
        private string? _supporterId;
        private string? _campaignId;
        private string? _status;
        private TimeSpan? _timeout;
        private int _retryCount;
        private string? _correlationId;

        /// <summary>
        /// Initializes a new instance of the <see cref="RecurringPlanSearchOperationBuilder"/> class.
        /// </summary>
        /// <param name="httpClient">The HTTP client wrapper.</param>
        /// <param name="logger">Optional logger instance.</param>
        public RecurringPlanSearchOperationBuilder(HttpClientWrapper httpClient, ILogger? logger = null)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger;
        }

        /// <inheritdoc />
        public IRecurringPlanSearchOperationBuilder WithCursor(string cursor)
        {
            _cursor = cursor;
            return this;
        }

        /// <inheritdoc />
        public IRecurringPlanSearchOperationBuilder WithLimit(int limit)
        {
            _limit = Math.Max(1, Math.Min(100, limit));
            return this;
        }

        /// <inheritdoc />
        public IRecurringPlanSearchOperationBuilder BySupporter(string supporterId)
        {
            _supporterId = supporterId;
            return this;
        }

        /// <inheritdoc />
        public IRecurringPlanSearchOperationBuilder ByCampaign(string campaignId)
        {
            _campaignId = campaignId;
            return this;
        }

        /// <inheritdoc />
        public IRecurringPlanSearchOperationBuilder ByStatus(string status)
        {
            _status = status;
            return this;
        }

        /// <inheritdoc />
        public IRecurringPlanSearchOperationBuilder WithTimeout(TimeSpan timeout)
        {
            _timeout = timeout;
            return this;
        }

        /// <inheritdoc />
        public IRecurringPlanSearchOperationBuilder WithRetry(int retryCount)
        {
            _retryCount = Math.Max(0, retryCount);
            return this;
        }

        /// <inheritdoc />
        public IRecurringPlanSearchOperationBuilder WithCorrelationId(string correlationId)
        {
            _correlationId = correlationId;
            return this;
        }

        /// <inheritdoc />
        public async Task<RecurringPlansResponse> ExecuteAsync()
        {
            _logger?.LogDebug("Executing recurring plan search with correlation ID: {CorrelationId}", _correlationId);

            try
            {
                var queryParams = new System.Collections.Generic.List<string>();

                if (_limit.HasValue)
                    queryParams.Add($"limit={_limit}");

                if (!string.IsNullOrEmpty(_cursor))
                    queryParams.Add($"starting_after={Uri.EscapeDataString(_cursor)}");

                if (!string.IsNullOrEmpty(_supporterId))
                    queryParams.Add($"supporter={Uri.EscapeDataString(_supporterId)}");

                if (!string.IsNullOrEmpty(_campaignId))
                    queryParams.Add($"campaign={Uri.EscapeDataString(_campaignId)}");

                if (!string.IsNullOrEmpty(_status))
                    queryParams.Add($"status={Uri.EscapeDataString(_status)}");

                var endpoint = "/v1/recurring_plans";
                if (queryParams.Count > 0)
                    endpoint += "?" + string.Join("&", queryParams);

                var response = await _httpClient.GetAsync<CursorPagedResponse<RecurringPlanResponse>>(endpoint, _correlationId);

                return new RecurringPlansResponse
                {
                    Data = response.Data,
                    HasMore = response.HasMore
                };
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Recurring plan search failed with correlation ID: {CorrelationId}", _correlationId);
                throw;
            }
        }
    }
}