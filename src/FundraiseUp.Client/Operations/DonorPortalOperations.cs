using System;
using System.Threading.Tasks;
using FundraiseUp.Client.Models;
using Microsoft.Extensions.Logging;

namespace FundraiseUp.Client.Operations
{
    /// <summary>
    /// Operations for managing donor portal access links.
    /// </summary>
    internal class DonorPortalOperations : IDonorPortalOperations
    {
        private readonly HttpClientWrapper _httpClient;
        private readonly ILogger? _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="DonorPortalOperations"/> class.
        /// </summary>
        /// <param name="httpClient">The HTTP client wrapper.</param>
        /// <param name="logger">The logger.</param>
        public DonorPortalOperations(HttpClientWrapper httpClient, ILogger? logger = null)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger;
        }

        /// <summary>
        /// Creates an access link for a supporter to access their donor portal.
        /// </summary>
        /// <param name="supporterId">The supporter identifier (format: SXXXXXXXX).</param>
        /// <returns>A donor portal operation builder.</returns>
        public IDonorPortalOperationBuilder<AccessLinkResponse> CreateSupporterAccessLink(string supporterId)
        {
            if (string.IsNullOrWhiteSpace(supporterId))
                throw new ArgumentException("Supporter ID cannot be null or empty.", nameof(supporterId));

            _logger?.LogDebug("Creating supporter access link operation for supporter: {SupporterId}", supporterId);

            return new DonorPortalOperationBuilder<AccessLinkResponse>(
                _httpClient,
                _logger,
                $"donor_portal/access_links/supporters/{supporterId}",
                supporterId,
                "supporter"
            );
        }

        /// <summary>
        /// Creates an access link for a recurring plan to access the donor portal.
        /// </summary>
        /// <param name="recurringPlanId">The recurring plan identifier (format: RXXXXXXX).</param>
        /// <returns>A donor portal operation builder.</returns>
        public IDonorPortalOperationBuilder<AccessLinkResponse> CreateRecurringPlanAccessLink(string recurringPlanId)
        {
            if (string.IsNullOrWhiteSpace(recurringPlanId))
                throw new ArgumentException("Recurring plan ID cannot be null or empty.", nameof(recurringPlanId));

            _logger?.LogDebug("Creating recurring plan access link operation for plan: {RecurringPlanId}", recurringPlanId);

            return new DonorPortalOperationBuilder<AccessLinkResponse>(
                _httpClient,
                _logger,
                $"donor_portal/access_links/recurring_plans/{recurringPlanId}",
                recurringPlanId,
                "recurring_plan"
            );
        }
    }

    /// <summary>
    /// Builder for donor portal operations with fluent configuration.
    /// </summary>
    /// <typeparam name="TResult">The result type of the operation.</typeparam>
    internal class DonorPortalOperationBuilder<TResult> : IDonorPortalOperationBuilder<TResult>
    {
        private readonly HttpClientWrapper _httpClient;
        private readonly ILogger? _logger;
        private readonly string _endpoint;
        private readonly string _entityId;
        private readonly string _entityType;

        private TimeSpan? _timeout;
        private int? _retryCount;
        private string? _correlationId;

        /// <summary>
        /// Initializes a new instance of the <see cref="DonorPortalOperationBuilder{TResult}"/> class.
        /// </summary>
        /// <param name="httpClient">The HTTP client wrapper.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="endpoint">The API endpoint.</param>
        /// <param name="entityId">The entity identifier.</param>
        /// <param name="entityType">The entity type.</param>
        public DonorPortalOperationBuilder(
            HttpClientWrapper httpClient,
            ILogger? logger,
            string endpoint,
            string entityId,
            string entityType)
        {
            _httpClient = httpClient;
            _logger = logger;
            _endpoint = endpoint;
            _entityId = entityId;
            _entityType = entityType;
        }

        /// <summary>
        /// Sets the timeout for the operation.
        /// </summary>
        /// <param name="timeout">The timeout duration.</param>
        /// <returns>The operation builder for chaining.</returns>
        public IDonorPortalOperationBuilder<TResult> WithTimeout(TimeSpan timeout)
        {
            _timeout = timeout;
            return this;
        }

        /// <summary>
        /// Sets the retry count for the operation.
        /// </summary>
        /// <param name="retryCount">The number of retry attempts.</param>
        /// <returns>The operation builder for chaining.</returns>
        public IDonorPortalOperationBuilder<TResult> WithRetry(int retryCount)
        {
            _retryCount = retryCount;
            return this;
        }

        /// <summary>
        /// Sets a correlation ID for the operation.
        /// </summary>
        /// <param name="correlationId">The correlation ID.</param>
        /// <returns>The operation builder for chaining.</returns>
        public IDonorPortalOperationBuilder<TResult> WithCorrelationId(string correlationId)
        {
            _correlationId = correlationId;
            return this;
        }

        /// <summary>
        /// Executes the operation asynchronously.
        /// </summary>
        /// <returns>The operation result.</returns>
        public async Task<TResult> ExecuteAsync()
        {
            try
            {
                _logger?.LogInformation("Creating {EntityType} access link for entity: {EntityId}", _entityType, _entityId);

                var result = await _httpClient.PostAsync<TResult>(_endpoint, new { }, _correlationId);

                _logger?.LogInformation("Successfully created {EntityType} access link for entity: {EntityId}", _entityType, _entityId);

                return result;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Failed to create {EntityType} access link for entity: {EntityId}", _entityType, _entityId);
                throw;
            }
        }
    }
}
