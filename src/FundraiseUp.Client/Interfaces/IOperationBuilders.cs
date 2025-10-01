using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FundraiseUp.Client.Models;

namespace FundraiseUp.Client
{
    /// <summary>
    /// Interface for building donation operations with fluent configuration.
    /// </summary>
    /// <typeparam name="TResult">The result type of the operation.</typeparam>
    public interface IDonationOperationBuilder<TResult>
    {
        /// <summary>
        /// Sets the timeout for the operation.
        /// </summary>
        /// <param name="timeout">The timeout duration.</param>
        /// <returns>The operation builder for chaining.</returns>
        IDonationOperationBuilder<TResult> WithTimeout(TimeSpan timeout);

        /// <summary>
        /// Sets the retry count for the operation.
        /// </summary>
        /// <param name="retryCount">The number of retry attempts.</param>
        /// <returns>The operation builder for chaining.</returns>
        IDonationOperationBuilder<TResult> WithRetry(int retryCount);

        /// <summary>
        /// Sets a correlation ID for the operation.
        /// </summary>
        /// <param name="correlationId">The correlation ID.</param>
        /// <returns>The operation builder for chaining.</returns>
        IDonationOperationBuilder<TResult> WithCorrelationId(string correlationId);

        /// <summary>
        /// Executes the operation asynchronously.
        /// </summary>
        /// <returns>The operation result.</returns>
        Task<TResult> ExecuteAsync();
    }

    /// <summary>
    /// Interface for building donation list operations with FundraiseUp API cursor-based pagination.
    /// </summary>
    public interface IDonationListOperationBuilder
    {
        /// <summary>
        /// Sets the cursor for pagination (obtained from previous response).
        /// </summary>
        /// <param name="cursor">The pagination cursor.</param>
        /// <returns>The operation builder for chaining.</returns>
        IDonationListOperationBuilder WithCursor(string cursor);

        /// <summary>
        /// Sets the maximum number of items to return (1-100, default 20).
        /// </summary>
        /// <param name="limit">The maximum count.</param>
        /// <returns>The operation builder for chaining.</returns>
        IDonationListOperationBuilder WithLimit(int limit);

        /// <summary>
        /// Filters donations by campaign ID.
        /// </summary>
        /// <param name="campaignId">The campaign identifier.</param>
        /// <returns>The operation builder for chaining.</returns>
        IDonationListOperationBuilder ByCampaign(string campaignId);

        /// <summary>
        /// Filters donations by supporter ID.
        /// </summary>
        /// <param name="supporterId">The supporter identifier.</param>
        /// <returns>The operation builder for chaining.</returns>
        IDonationListOperationBuilder BySupporter(string supporterId);

        /// <summary>
        /// Filters donations by status.
        /// </summary>
        /// <param name="status">The donation status.</param>
        /// <returns>The operation builder for chaining.</returns>
        IDonationListOperationBuilder ByStatus(string status);

        /// <summary>
        /// Adds a filter condition to the query (for backward compatibility).
        /// Note: Complex expressions may not be supported by FundraiseUp API.
        /// </summary>
        /// <param name="predicate">The filter predicate.</param>
        /// <returns>The operation builder for chaining.</returns>
        IDonationListOperationBuilder Where(Expression<Func<DonationResponse, bool>> predicate);

        /// <summary>
        /// Adds an ordering to the query (for backward compatibility).
        /// Note: Custom ordering may not be supported by FundraiseUp API.
        /// </summary>
        /// <param name="keySelector">The ordering key selector.</param>
        /// <returns>The operation builder for chaining.</returns>
        IDonationListOperationBuilder OrderBy<TKey>(Expression<Func<DonationResponse, TKey>> keySelector);

        /// <summary>
        /// Sets the maximum number of items to return (alias for WithLimit).
        /// </summary>
        /// <param name="count">The maximum count.</param>
        /// <returns>The operation builder for chaining.</returns>
        IDonationListOperationBuilder Take(int count);

        /// <summary>
        /// Sets the timeout for the operation.
        /// </summary>
        /// <param name="timeout">The timeout duration.</param>
        /// <returns>The operation builder for chaining.</returns>
        IDonationListOperationBuilder WithTimeout(TimeSpan timeout);

        /// <summary>
        /// Sets the retry count for the operation.
        /// </summary>
        /// <param name="retryCount">The number of retry attempts.</param>
        /// <returns>The operation builder for chaining.</returns>
        IDonationListOperationBuilder WithRetry(int retryCount);

        /// <summary>
        /// Sets a correlation ID for the operation.
        /// </summary>
        /// <param name="correlationId">The correlation ID.</param>
        /// <returns>The operation builder for chaining.</returns>
        IDonationListOperationBuilder WithCorrelationId(string correlationId);

        /// <summary>
        /// Executes the operation asynchronously.
        /// </summary>
        /// <returns>The paginated result with cursor-based pagination.</returns>
        Task<PagedResult<DonationResponse>> ExecuteAsync();
    }

    /// <summary>
    /// Interface for building campaign operations with fluent configuration.
    /// </summary>
    /// <typeparam name="TResult">The result type of the operation.</typeparam>
    public interface ICampaignOperationBuilder<TResult>
    {
        /// <summary>
        /// Sets the timeout for the operation.
        /// </summary>
        /// <param name="timeout">The timeout duration.</param>
        /// <returns>The operation builder for chaining.</returns>
        ICampaignOperationBuilder<TResult> WithTimeout(TimeSpan timeout);

        /// <summary>
        /// Sets the retry count for the operation.
        /// </summary>
        /// <param name="retryCount">The number of retry attempts.</param>
        /// <returns>The operation builder for chaining.</returns>
        ICampaignOperationBuilder<TResult> WithRetry(int retryCount);

        /// <summary>
        /// Sets a correlation ID for the operation.
        /// </summary>
        /// <param name="correlationId">The correlation ID.</param>
        /// <returns>The operation builder for chaining.</returns>
        ICampaignOperationBuilder<TResult> WithCorrelationId(string correlationId);

        /// <summary>
        /// Executes the operation asynchronously.
        /// </summary>
        /// <returns>The operation result.</returns>
        Task<TResult> ExecuteAsync();
    }

    /// <summary>
    /// Interface for building campaign list operations with FundraiseUp API cursor-based pagination.
    /// </summary>
    public interface ICampaignListOperationBuilder
    {
        /// <summary>
        /// Sets the cursor for pagination (obtained from previous response).
        /// </summary>
        /// <param name="cursor">The pagination cursor.</param>
        /// <returns>The operation builder for chaining.</returns>
        ICampaignListOperationBuilder WithCursor(string cursor);

        /// <summary>
        /// Sets the maximum number of items to return (1-100, default 20).
        /// </summary>
        /// <param name="limit">The maximum count.</param>
        /// <returns>The operation builder for chaining.</returns>
        ICampaignListOperationBuilder WithLimit(int limit);

        /// <summary>
        /// Sets the timeout for the operation.
        /// </summary>
        /// <param name="timeout">The timeout duration.</param>
        /// <returns>The operation builder for chaining.</returns>
        ICampaignListOperationBuilder WithTimeout(TimeSpan timeout);

        /// <summary>
        /// Sets the retry count for the operation.
        /// </summary>
        /// <param name="retryCount">The number of retry attempts.</param>
        /// <returns>The operation builder for chaining.</returns>
        ICampaignListOperationBuilder WithRetry(int retryCount);

        /// <summary>
        /// Sets a correlation ID for the operation.
        /// </summary>
        /// <param name="correlationId">The correlation ID.</param>
        /// <returns>The operation builder for chaining.</returns>
        ICampaignListOperationBuilder WithCorrelationId(string correlationId);

        /// <summary>
        /// Executes the operation asynchronously.
        /// </summary>
        /// <returns>The paginated result with cursor-based pagination.</returns>
        Task<PagedResult<Campaign>> ExecuteAsync();
    }

    /// <summary>
    /// Interface for building supporter operations with fluent configuration.
    /// </summary>
    /// <typeparam name="TResult">The result type of the operation.</typeparam>
    public interface ISupporterOperationBuilder<TResult>
    {
        /// <summary>
        /// Sets the timeout for the operation.
        /// </summary>
        /// <param name="timeout">The timeout duration.</param>
        /// <returns>The operation builder for chaining.</returns>
        ISupporterOperationBuilder<TResult> WithTimeout(TimeSpan timeout);

        /// <summary>
        /// Sets the retry count for the operation.
        /// </summary>
        /// <param name="retryCount">The number of retry attempts.</param>
        /// <returns>The operation builder for chaining.</returns>
        ISupporterOperationBuilder<TResult> WithRetry(int retryCount);

        /// <summary>
        /// Sets a correlation ID for the operation.
        /// </summary>
        /// <param name="correlationId">The correlation ID.</param>
        /// <returns>The operation builder for chaining.</returns>
        ISupporterOperationBuilder<TResult> WithCorrelationId(string correlationId);

        /// <summary>
        /// Executes the operation asynchronously.
        /// </summary>
        /// <returns>The operation result.</returns>
        Task<TResult> ExecuteAsync();
    }

    /// <summary>
    /// Interface for building supporter search operations with fluent configuration.
    /// Uses cursor-based pagination for FundraiseUp API compatibility.
    /// </summary>
    public interface ISupporterSearchOperationBuilder
    {
        /// <summary>
        /// Sets the cursor for pagination.
        /// </summary>
        /// <param name="cursor">The cursor value from a previous response.</param>
        /// <returns>The operation builder for chaining.</returns>
        ISupporterSearchOperationBuilder WithCursor(string cursor);

        /// <summary>
        /// Sets the maximum number of items to return.
        /// </summary>
        /// <param name="limit">The limit (max 100).</param>
        /// <returns>The operation builder for chaining.</returns>
        ISupporterSearchOperationBuilder WithLimit(int limit);

        /// <summary>
        /// Sets the timeout for the operation.
        /// </summary>
        /// <param name="timeout">The timeout duration.</param>
        /// <returns>The operation builder for chaining.</returns>
        ISupporterSearchOperationBuilder WithTimeout(TimeSpan timeout);

        /// <summary>
        /// Sets the retry count for the operation.
        /// </summary>
        /// <param name="retryCount">The number of retry attempts.</param>
        /// <returns>The operation builder for chaining.</returns>
        ISupporterSearchOperationBuilder WithRetry(int retryCount);

        /// <summary>
        /// Sets a correlation ID for the operation.
        /// </summary>
        /// <param name="correlationId">The correlation ID.</param>
        /// <returns>The operation builder for chaining.</returns>
        ISupporterSearchOperationBuilder WithCorrelationId(string correlationId);

        /// <summary>
        /// Adds a filter expression for supporter search.
        /// Note: Complex expressions may not be fully supported by the FundraiseUp API.
        /// </summary>
        /// <param name="predicate">The filter predicate.</param>
        /// <returns>The operation builder for chaining.</returns>
        ISupporterSearchOperationBuilder Where(System.Linq.Expressions.Expression<Func<SupporterResponse, bool>> predicate);

        /// <summary>
        /// Executes the search operation asynchronously.
        /// </summary>
        /// <returns>The search results with cursor-based pagination.</returns>
        Task<SupportersResponse> ExecuteAsync();
    }

    /// <summary>
    /// Interface for building supporter operations with fluent configuration.
    /// </summary>
    /// <typeparam name="TResult">The result type of the operation.</typeparam>
    [Obsolete("Use ISupporterOperationBuilder for new code. This interface is maintained for backward compatibility.")]
    public interface IDonorOperationBuilder<TResult>
    {
        /// <summary>
        /// Sets the timeout for the operation.
        /// </summary>
        /// <param name="timeout">The timeout duration.</param>
        /// <returns>The operation builder for chaining.</returns>
        IDonorOperationBuilder<TResult> WithTimeout(TimeSpan timeout);

        /// <summary>
        /// Sets the retry count for the operation.
        /// </summary>
        /// <param name="retryCount">The number of retry attempts.</param>
        /// <returns>The operation builder for chaining.</returns>
        IDonorOperationBuilder<TResult> WithRetry(int retryCount);

        /// <summary>
        /// Sets a correlation ID for the operation.
        /// </summary>
        /// <param name="correlationId">The correlation ID.</param>
        /// <returns>The operation builder for chaining.</returns>
        IDonorOperationBuilder<TResult> WithCorrelationId(string correlationId);

        /// <summary>
        /// Executes the operation asynchronously.
        /// </summary>
        /// <returns>The operation result.</returns>
        Task<TResult> ExecuteAsync();
    }

    /// <summary>
    /// Interface for building supporter search operations with fluent configuration.
    /// Uses cursor-based pagination for FundraiseUp API compatibility.
    /// </summary>
    [Obsolete("Use ISupporterSearchOperationBuilder for new code. This interface is maintained for backward compatibility.")]
    public interface IDonorSearchOperationBuilder
    {
        /// <summary>
        /// Sets the cursor for pagination.
        /// </summary>
        /// <param name="cursor">The cursor value from a previous response.</param>
        /// <returns>The operation builder for chaining.</returns>
        IDonorSearchOperationBuilder WithCursor(string cursor);

        /// <summary>
        /// Sets the maximum number of items to return.
        /// </summary>
        /// <param name="limit">The limit (max 100).</param>
        /// <returns>The operation builder for chaining.</returns>
        IDonorSearchOperationBuilder WithLimit(int limit);

        /// <summary>
        /// Sets the timeout for the operation.
        /// </summary>
        /// <param name="timeout">The timeout duration.</param>
        /// <returns>The operation builder for chaining.</returns>
        IDonorSearchOperationBuilder WithTimeout(TimeSpan timeout);

        /// <summary>
        /// Sets the retry count for the operation.
        /// </summary>
        /// <param name="retryCount">The number of retry attempts.</param>
        /// <returns>The operation builder for chaining.</returns>
        IDonorSearchOperationBuilder WithRetry(int retryCount);

        /// <summary>
        /// Sets a correlation ID for the operation.
        /// </summary>
        /// <param name="correlationId">The correlation ID.</param>
        /// <returns>The operation builder for chaining.</returns>
        IDonorSearchOperationBuilder WithCorrelationId(string correlationId);

        /// <summary>
        /// Adds a filter expression for supporter search.
        /// Note: Complex expressions may not be fully supported by the FundraiseUp API.
        /// </summary>
        /// <param name="predicate">The filter predicate.</param>
        /// <returns>The operation builder for chaining.</returns>
        IDonorSearchOperationBuilder Where(System.Linq.Expressions.Expression<Func<Donor, bool>> predicate);

        /// <summary>
        /// Executes the search operation asynchronously.
        /// </summary>
        /// <returns>The search results with cursor-based pagination.</returns>
        Task<PagedResult<Donor>> ExecuteAsync();
    }

    /// <summary>
    /// Interface for building recurring plan operations with fluent configuration.
    /// </summary>
    /// <typeparam name="TResult">The result type of the operation.</typeparam>
    public interface IRecurringPlanOperationBuilder<TResult>
    {
        /// <summary>
        /// Sets the timeout for the operation.
        /// </summary>
        /// <param name="timeout">The timeout duration.</param>
        /// <returns>The operation builder for chaining.</returns>
        IRecurringPlanOperationBuilder<TResult> WithTimeout(TimeSpan timeout);

        /// <summary>
        /// Sets the retry count for the operation.
        /// </summary>
        /// <param name="retryCount">The number of retry attempts.</param>
        /// <returns>The operation builder for chaining.</returns>
        IRecurringPlanOperationBuilder<TResult> WithRetry(int retryCount);

        /// <summary>
        /// Sets a correlation ID for the operation.
        /// </summary>
        /// <param name="correlationId">The correlation ID.</param>
        /// <returns>The operation builder for chaining.</returns>
        IRecurringPlanOperationBuilder<TResult> WithCorrelationId(string correlationId);

        /// <summary>
        /// Executes the operation asynchronously.
        /// </summary>
        /// <returns>The operation result.</returns>
        Task<TResult> ExecuteAsync();
    }

    /// <summary>
    /// Interface for building recurring plan search operations with FundraiseUp API cursor-based pagination.
    /// </summary>
    public interface IRecurringPlanSearchOperationBuilder
    {
        /// <summary>
        /// Sets the cursor for pagination (obtained from previous response).
        /// </summary>
        /// <param name="cursor">The pagination cursor.</param>
        /// <returns>The operation builder for chaining.</returns>
        IRecurringPlanSearchOperationBuilder WithCursor(string cursor);

        /// <summary>
        /// Sets the maximum number of items to return (1-100, default 10).
        /// </summary>
        /// <param name="limit">The page size limit.</param>
        /// <returns>The operation builder for chaining.</returns>
        IRecurringPlanSearchOperationBuilder WithLimit(int limit);

        /// <summary>
        /// Filters recurring plans by supporter ID.
        /// </summary>
        /// <param name="supporterId">The supporter ID to filter by.</param>
        /// <returns>The operation builder for chaining.</returns>
        IRecurringPlanSearchOperationBuilder BySupporter(string supporterId);

        /// <summary>
        /// Filters recurring plans by campaign ID.
        /// </summary>
        /// <param name="campaignId">The campaign ID to filter by.</param>
        /// <returns>The operation builder for chaining.</returns>
        IRecurringPlanSearchOperationBuilder ByCampaign(string campaignId);

        /// <summary>
        /// Filters recurring plans by status.
        /// </summary>
        /// <param name="status">The status to filter by.</param>
        /// <returns>The operation builder for chaining.</returns>
        IRecurringPlanSearchOperationBuilder ByStatus(string status);

        /// <summary>
        /// Sets the timeout for the operation.
        /// </summary>
        /// <param name="timeout">The timeout duration.</param>
        /// <returns>The operation builder for chaining.</returns>
        IRecurringPlanSearchOperationBuilder WithTimeout(TimeSpan timeout);

        /// <summary>
        /// Sets the retry count for the operation.
        /// </summary>
        /// <param name="retryCount">The number of retry attempts.</param>
        /// <returns>The operation builder for chaining.</returns>
        IRecurringPlanSearchOperationBuilder WithRetry(int retryCount);

        /// <summary>
        /// Sets a correlation ID for the operation.
        /// </summary>
        /// <param name="correlationId">The correlation ID.</param>
        /// <returns>The operation builder for chaining.</returns>
        IRecurringPlanSearchOperationBuilder WithCorrelationId(string correlationId);

        /// <summary>
        /// Executes the search operation asynchronously.
        /// </summary>
        /// <returns>The search results with cursor-based pagination.</returns>
        Task<RecurringPlansResponse> ExecuteAsync();
    }

    /// <summary>
    /// Interface for building event operations with fluent configuration.
    /// </summary>
    /// <typeparam name="TResult">The result type of the operation.</typeparam>
    public interface IEventOperationBuilder<TResult>
    {
        /// <summary>
        /// Sets the timeout for the operation.
        /// </summary>
        /// <param name="timeout">The timeout duration.</param>
        /// <returns>The operation builder for chaining.</returns>
        IEventOperationBuilder<TResult> WithTimeout(TimeSpan timeout);

        /// <summary>
        /// Sets the retry count for the operation.
        /// </summary>
        /// <param name="retryCount">The number of retry attempts.</param>
        /// <returns>The operation builder for chaining.</returns>
        IEventOperationBuilder<TResult> WithRetry(int retryCount);

        /// <summary>
        /// Sets a correlation ID for the operation.
        /// </summary>
        /// <param name="correlationId">The correlation ID.</param>
        /// <returns>The operation builder for chaining.</returns>
        IEventOperationBuilder<TResult> WithCorrelationId(string correlationId);

        /// <summary>
        /// Executes the operation asynchronously.
        /// </summary>
        /// <returns>The operation result.</returns>
        Task<TResult> ExecuteAsync();
    }

    /// <summary>
    /// Interface for building event search operations with FundraiseUp API cursor-based pagination.
    /// </summary>
    public interface IEventSearchOperationBuilder
    {
        /// <summary>
        /// Sets the cursor for pagination (obtained from previous response).
        /// </summary>
        /// <param name="cursor">The pagination cursor.</param>
        /// <returns>The operation builder for chaining.</returns>
        IEventSearchOperationBuilder WithCursor(string cursor);

        /// <summary>
        /// Sets the maximum number of items to return (1-100, default 10).
        /// </summary>
        /// <param name="limit">The page size limit.</param>
        /// <returns>The operation builder for chaining.</returns>
        IEventSearchOperationBuilder WithLimit(int limit);

        /// <summary>
        /// Filters events by event type.
        /// </summary>
        /// <param name="eventType">The event type to filter by (e.g., "donation.created").</param>
        /// <returns>The operation builder for chaining.</returns>
        IEventSearchOperationBuilder ByType(string eventType);

        /// <summary>
        /// Filters events by donation ID.
        /// </summary>
        /// <param name="donationId">The donation ID to filter by.</param>
        /// <returns>The operation builder for chaining.</returns>
        IEventSearchOperationBuilder ByDonation(string donationId);

        /// <summary>
        /// Filters events by recurring plan ID.
        /// </summary>
        /// <param name="recurringPlanId">The recurring plan ID to filter by.</param>
        /// <returns>The operation builder for chaining.</returns>
        IEventSearchOperationBuilder ByRecurringPlan(string recurringPlanId);

        /// <summary>
        /// Filters events by supporter ID.
        /// </summary>
        /// <param name="supporterId">The supporter ID to filter by.</param>
        /// <returns>The operation builder for chaining.</returns>
        IEventSearchOperationBuilder BySupporter(string supporterId);

        /// <summary>
        /// Filters events by campaign ID.
        /// </summary>
        /// <param name="campaignId">The campaign ID to filter by.</param>
        /// <returns>The operation builder for chaining.</returns>
        IEventSearchOperationBuilder ByCampaign(string campaignId);

        /// <summary>
        /// Filters events by fundraiser ID.
        /// </summary>
        /// <param name="fundraiserId">The fundraiser ID to filter by.</param>
        /// <returns>The operation builder for chaining.</returns>
        IEventSearchOperationBuilder ByFundraiser(string fundraiserId);

        /// <summary>
        /// Filters events created after the specified date.
        /// </summary>
        /// <param name="dateTime">The minimum creation date.</param>
        /// <returns>The operation builder for chaining.</returns>
        IEventSearchOperationBuilder CreatedAfter(DateTime dateTime);

        /// <summary>
        /// Filters events created before the specified date.
        /// </summary>
        /// <param name="dateTime">The maximum creation date.</param>
        /// <returns>The operation builder for chaining.</returns>
        IEventSearchOperationBuilder CreatedBefore(DateTime dateTime);

        /// <summary>
        /// Sets the timeout for the operation.
        /// </summary>
        /// <param name="timeout">The timeout duration.</param>
        /// <returns>The operation builder for chaining.</returns>
        IEventSearchOperationBuilder WithTimeout(TimeSpan timeout);

        /// <summary>
        /// Sets the retry count for the operation.
        /// </summary>
        /// <param name="retryCount">The number of retry attempts.</param>
        /// <returns>The operation builder for chaining.</returns>
        IEventSearchOperationBuilder WithRetry(int retryCount);

        /// <summary>
        /// Sets a correlation ID for the operation.
        /// </summary>
        /// <param name="correlationId">The correlation ID.</param>
        /// <returns>The operation builder for chaining.</returns>
        IEventSearchOperationBuilder WithCorrelationId(string correlationId);

        /// <summary>
        /// Executes the search operation asynchronously.
        /// </summary>
        /// <returns>The search results with cursor-based pagination.</returns>
        Task<EventsResponse> ExecuteAsync();
    }

    /// <summary>
    /// Interface for building fundraiser operations with fluent configuration.
    /// </summary>
    /// <typeparam name="TResult">The result type of the operation.</typeparam>
    public interface IFundraiserOperationBuilder<TResult>
    {
        /// <summary>
        /// Sets the timeout for the operation.
        /// </summary>
        /// <param name="timeout">The timeout duration.</param>
        /// <returns>The operation builder for chaining.</returns>
        IFundraiserOperationBuilder<TResult> WithTimeout(TimeSpan timeout);

        /// <summary>
        /// Sets the retry count for the operation.
        /// </summary>
        /// <param name="retryCount">The number of retry attempts.</param>
        /// <returns>The operation builder for chaining.</returns>
        IFundraiserOperationBuilder<TResult> WithRetry(int retryCount);

        /// <summary>
        /// Sets a correlation ID for the operation.
        /// </summary>
        /// <param name="correlationId">The correlation ID.</param>
        /// <returns>The operation builder for chaining.</returns>
        IFundraiserOperationBuilder<TResult> WithCorrelationId(string correlationId);

        /// <summary>
        /// Executes the operation asynchronously.
        /// </summary>
        /// <returns>The operation result.</returns>
        Task<TResult> ExecuteAsync();
    }

    /// <summary>
    /// Interface for building fundraiser search operations with FundraiseUp API cursor-based pagination.
    /// </summary>
    public interface IFundraiserSearchOperationBuilder
    {
        /// <summary>
        /// Sets the cursor for pagination (obtained from previous response).
        /// </summary>
        /// <param name="cursor">The pagination cursor.</param>
        /// <returns>The operation builder for chaining.</returns>
        IFundraiserSearchOperationBuilder WithCursor(string cursor);

        /// <summary>
        /// Sets the maximum number of items to return (1-100, default 10).
        /// </summary>
        /// <param name="limit">The page size limit.</param>
        /// <returns>The operation builder for chaining.</returns>
        IFundraiserSearchOperationBuilder WithLimit(int limit);

        /// <summary>
        /// Filters fundraisers by campaign ID.
        /// </summary>
        /// <param name="campaignId">The campaign ID to filter by.</param>
        /// <returns>The operation builder for chaining.</returns>
        IFundraiserSearchOperationBuilder ByCampaign(string campaignId);

        /// <summary>
        /// Filters fundraisers by status.
        /// </summary>
        /// <param name="status">The status to filter by.</param>
        /// <returns>The operation builder for chaining.</returns>
        IFundraiserSearchOperationBuilder ByStatus(string status);

        /// <summary>
        /// Filters fundraisers by creator email.
        /// </summary>
        /// <param name="creatorEmail">The creator email to filter by.</param>
        /// <returns>The operation builder for chaining.</returns>
        IFundraiserSearchOperationBuilder ByCreatorEmail(string creatorEmail);

        /// <summary>
        /// Filters fundraisers created after the specified date.
        /// </summary>
        /// <param name="dateTime">The minimum creation date.</param>
        /// <returns>The operation builder for chaining.</returns>
        IFundraiserSearchOperationBuilder CreatedAfter(DateTime dateTime);

        /// <summary>
        /// Filters fundraisers created before the specified date.
        /// </summary>
        /// <param name="dateTime">The maximum creation date.</param>
        /// <returns>The operation builder for chaining.</returns>
        IFundraiserSearchOperationBuilder CreatedBefore(DateTime dateTime);

        /// <summary>
        /// Sets the timeout for the operation.
        /// </summary>
        /// <param name="timeout">The timeout duration.</param>
        /// <returns>The operation builder for chaining.</returns>
        IFundraiserSearchOperationBuilder WithTimeout(TimeSpan timeout);

        /// <summary>
        /// Sets the retry count for the operation.
        /// </summary>
        /// <param name="retryCount">The number of retry attempts.</param>
        /// <returns>The operation builder for chaining.</returns>
        IFundraiserSearchOperationBuilder WithRetry(int retryCount);

        /// <summary>
        /// Sets a correlation ID for the operation.
        /// </summary>
        /// <param name="correlationId">The correlation ID.</param>
        /// <returns>The operation builder for chaining.</returns>
        IFundraiserSearchOperationBuilder WithCorrelationId(string correlationId);

        /// <summary>
        /// Executes the search operation asynchronously.
        /// </summary>
        /// <returns>The search results with cursor-based pagination.</returns>
        Task<FundraisersResponse> ExecuteAsync();
    }

    /// <summary>
    /// Interface for donor portal operations.
    /// </summary>
    public interface IDonorPortalOperations
    {
        /// <summary>
        /// Creates an access link for a supporter to access their donor portal.
        /// </summary>
        /// <param name="supporterId">The supporter identifier (format: SXXXXXXXX).</param>
        /// <returns>A donor portal operation builder.</returns>
        IDonorPortalOperationBuilder<AccessLinkResponse> CreateSupporterAccessLink(string supporterId);

        /// <summary>
        /// Creates an access link for a recurring plan to access the donor portal.
        /// </summary>
        /// <param name="recurringPlanId">The recurring plan identifier (format: RXXXXXXX).</param>
        /// <returns>A donor portal operation builder.</returns>
        IDonorPortalOperationBuilder<AccessLinkResponse> CreateRecurringPlanAccessLink(string recurringPlanId);
    }

    /// <summary>
    /// Interface for building donor portal operations with fluent configuration.
    /// </summary>
    /// <typeparam name="TResult">The result type of the operation.</typeparam>
    public interface IDonorPortalOperationBuilder<TResult>
    {
        /// <summary>
        /// Sets the timeout for the operation.
        /// </summary>
        /// <param name="timeout">The timeout duration.</param>
        /// <returns>The operation builder for chaining.</returns>
        IDonorPortalOperationBuilder<TResult> WithTimeout(TimeSpan timeout);

        /// <summary>
        /// Sets the retry count for the operation.
        /// </summary>
        /// <param name="retryCount">The number of retry attempts.</param>
        /// <returns>The operation builder for chaining.</returns>
        IDonorPortalOperationBuilder<TResult> WithRetry(int retryCount);

        /// <summary>
        /// Sets a correlation ID for the operation.
        /// </summary>
        /// <param name="correlationId">The correlation ID.</param>
        /// <returns>The operation builder for chaining.</returns>
        IDonorPortalOperationBuilder<TResult> WithCorrelationId(string correlationId);

        /// <summary>
        /// Executes the operation asynchronously.
        /// </summary>
        /// <returns>The operation result.</returns>
        Task<TResult> ExecuteAsync();
    }
}
