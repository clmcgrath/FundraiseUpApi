using System;
using System.Threading.Tasks;
using FundraiseUp.Client.Models;
using Microsoft.Extensions.Logging;

namespace FundraiseUp.Client.Operations
{
    /// <summary>
    /// Operations for managing events (audit log) in FundraiseUp API.
    /// Events are read-only records of all significant actions in the system.
    /// </summary>
    internal class EventOperations : IEventOperations
    {
        private readonly HttpClientWrapper _httpClient;
        private readonly ILogger? _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventOperations"/> class.
        /// </summary>
        /// <param name="httpClient">The HTTP client wrapper for API communication.</param>
        /// <param name="logger">Optional logger for operation tracking.</param>
        public EventOperations(HttpClientWrapper httpClient, ILogger? logger = null)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger;
        }

        /// <inheritdoc />
        public IEventOperationBuilder<EventResponse> GetById(string eventId)
        {
            if (string.IsNullOrWhiteSpace(eventId))
                throw new ArgumentException("Event ID cannot be null or empty", nameof(eventId));

            return new EventOperationBuilder<EventResponse>(_httpClient, _logger)
                .WithOperation(async () =>
                {
                    var endpoint = $"/v1/events/{eventId}";
                    return await _httpClient.GetAsync<EventResponse>(endpoint);
                });
        }

        /// <inheritdoc />
        public IEventSearchOperationBuilder Search()
        {
            return new EventSearchOperationBuilder(_httpClient, _logger);
        }
    }

    /// <summary>
    /// Implementation of event operation builder for FundraiseUp API.
    /// </summary>
    /// <typeparam name="T">The type of result returned by the operation.</typeparam>
    internal class EventOperationBuilder<T> : IEventOperationBuilder<T>
    {
        private readonly HttpClientWrapper _httpClient;
        private readonly ILogger? _logger;
        private Func<Task<T>>? _operation;
        private TimeSpan? _timeout;
        private int _retryCount;
        private string? _correlationId;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventOperationBuilder{T}"/> class.
        /// </summary>
        /// <param name="httpClient">The HTTP client wrapper.</param>
        /// <param name="logger">Optional logger instance.</param>
        public EventOperationBuilder(HttpClientWrapper httpClient, ILogger? logger = null)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger;
        }

        /// <summary>
        /// Sets the operation to be executed.
        /// </summary>
        /// <param name="operation">The operation function.</param>
        /// <returns>The current builder instance.</returns>
        public EventOperationBuilder<T> WithOperation(Func<Task<T>> operation)
        {
            _operation = operation ?? throw new ArgumentNullException(nameof(operation));
            return this;
        }

        /// <inheritdoc />
        public IEventOperationBuilder<T> WithTimeout(TimeSpan timeout)
        {
            _timeout = timeout;
            return this;
        }

        /// <inheritdoc />
        public IEventOperationBuilder<T> WithRetry(int retryCount)
        {
            _retryCount = Math.Max(0, retryCount);
            return this;
        }

        /// <inheritdoc />
        public IEventOperationBuilder<T> WithCorrelationId(string correlationId)
        {
            _correlationId = correlationId;
            return this;
        }

        /// <inheritdoc />
        public async Task<T> ExecuteAsync()
        {
            if (_operation == null)
                throw new InvalidOperationException("No operation has been configured.");

            _logger?.LogDebug("Executing event operation with correlation ID: {CorrelationId}", _correlationId);

            try
            {
                return await _operation();
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Event operation failed with correlation ID: {CorrelationId}", _correlationId);
                throw;
            }
        }
    }

    /// <summary>
    /// Implementation of event search operation builder.
    /// </summary>
    internal class EventSearchOperationBuilder : IEventSearchOperationBuilder
    {
        private readonly HttpClientWrapper _httpClient;
        private readonly ILogger? _logger;
        private string? _cursor;
        private int? _limit;
        private string? _type;
        private string? _donationId;
        private string? _recurringPlanId;
        private string? _supporterId;
        private string? _campaignId;
        private string? _fundraiserId;
        private DateTime? _createdAfter;
        private DateTime? _createdBefore;
        private TimeSpan? _timeout;
        private int _retryCount;
        private string? _correlationId;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventSearchOperationBuilder"/> class.
        /// </summary>
        /// <param name="httpClient">The HTTP client wrapper.</param>
        /// <param name="logger">Optional logger instance.</param>
        public EventSearchOperationBuilder(HttpClientWrapper httpClient, ILogger? logger = null)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger;
        }

        /// <inheritdoc />
        public IEventSearchOperationBuilder WithCursor(string cursor)
        {
            _cursor = cursor;
            return this;
        }

        /// <inheritdoc />
        public IEventSearchOperationBuilder WithLimit(int limit)
        {
            _limit = Math.Max(1, Math.Min(100, limit));
            return this;
        }

        /// <inheritdoc />
        public IEventSearchOperationBuilder ByType(string eventType)
        {
            _type = eventType;
            return this;
        }

        /// <inheritdoc />
        public IEventSearchOperationBuilder ByDonation(string donationId)
        {
            _donationId = donationId;
            return this;
        }

        /// <inheritdoc />
        public IEventSearchOperationBuilder ByRecurringPlan(string recurringPlanId)
        {
            _recurringPlanId = recurringPlanId;
            return this;
        }

        /// <inheritdoc />
        public IEventSearchOperationBuilder BySupporter(string supporterId)
        {
            _supporterId = supporterId;
            return this;
        }

        /// <inheritdoc />
        public IEventSearchOperationBuilder ByCampaign(string campaignId)
        {
            _campaignId = campaignId;
            return this;
        }

        /// <inheritdoc />
        public IEventSearchOperationBuilder ByFundraiser(string fundraiserId)
        {
            _fundraiserId = fundraiserId;
            return this;
        }

        /// <inheritdoc />
        public IEventSearchOperationBuilder CreatedAfter(DateTime dateTime)
        {
            _createdAfter = dateTime;
            return this;
        }

        /// <inheritdoc />
        public IEventSearchOperationBuilder CreatedBefore(DateTime dateTime)
        {
            _createdBefore = dateTime;
            return this;
        }

        /// <inheritdoc />
        public IEventSearchOperationBuilder WithTimeout(TimeSpan timeout)
        {
            _timeout = timeout;
            return this;
        }

        /// <inheritdoc />
        public IEventSearchOperationBuilder WithRetry(int retryCount)
        {
            _retryCount = Math.Max(0, retryCount);
            return this;
        }

        /// <inheritdoc />
        public IEventSearchOperationBuilder WithCorrelationId(string correlationId)
        {
            _correlationId = correlationId;
            return this;
        }

        /// <inheritdoc />
        public async Task<EventsResponse> ExecuteAsync()
        {
            _logger?.LogDebug("Executing event search with correlation ID: {CorrelationId}", _correlationId);

            try
            {
                var queryParams = new System.Collections.Generic.List<string>();

                if (_limit.HasValue)
                    queryParams.Add($"limit={_limit}");

                if (!string.IsNullOrEmpty(_cursor))
                    queryParams.Add($"starting_after={Uri.EscapeDataString(_cursor)}");

                if (!string.IsNullOrEmpty(_type))
                    queryParams.Add($"type={Uri.EscapeDataString(_type)}");

                if (!string.IsNullOrEmpty(_donationId))
                    queryParams.Add($"donation={Uri.EscapeDataString(_donationId)}");

                if (!string.IsNullOrEmpty(_recurringPlanId))
                    queryParams.Add($"recurring_plan={Uri.EscapeDataString(_recurringPlanId)}");

                if (!string.IsNullOrEmpty(_supporterId))
                    queryParams.Add($"supporter={Uri.EscapeDataString(_supporterId)}");

                if (!string.IsNullOrEmpty(_campaignId))
                    queryParams.Add($"campaign={Uri.EscapeDataString(_campaignId)}");

                if (!string.IsNullOrEmpty(_fundraiserId))
                    queryParams.Add($"fundraiser={Uri.EscapeDataString(_fundraiserId)}");

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

                var endpoint = "/v1/events";
                if (queryParams.Count > 0)
                    endpoint += "?" + string.Join("&", queryParams);

                var response = await _httpClient.GetAsync<CursorPagedResponse<EventResponse>>(endpoint, _correlationId);

                return new EventsResponse
                {
                    Data = response.Data,
                    HasMore = response.HasMore
                };
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Event search failed with correlation ID: {CorrelationId}", _correlationId);
                throw;
            }
        }
    }
}
