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
    /// Interface for building donation list operations.
    /// </summary>
    public interface IDonationListOperationBuilder
    {
        /// <summary>
        /// Adds a filter condition to the query.
        /// </summary>
        /// <param name="predicate">The filter predicate.</param>
        /// <returns>The operation builder for chaining.</returns>
        IDonationListOperationBuilder Where(Expression<Func<Donation, bool>> predicate);

        /// <summary>
        /// Adds an ordering to the query.
        /// </summary>
        /// <param name="keySelector">The ordering key selector.</param>
        /// <returns>The operation builder for chaining.</returns>
        IDonationListOperationBuilder OrderBy<TKey>(Expression<Func<Donation, TKey>> keySelector);

        /// <summary>
        /// Sets the maximum number of items to return.
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
        /// <returns>The paginated result.</returns>
        Task<PagedResult<Donation>> ExecuteAsync();
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
    /// Interface for building campaign list operations.
    /// </summary>
    public interface ICampaignListOperationBuilder
    {
        /// <summary>
        /// Sets pagination parameters.
        /// </summary>
        /// <param name="page">The page number (1-based).</param>
        /// <param name="pageSize">The page size.</param>
        /// <returns>The operation builder for chaining.</returns>
        ICampaignListOperationBuilder Page(int page, int pageSize);

        /// <summary>
        /// Adds an ordering to the query.
        /// </summary>
        /// <param name="keySelector">The ordering key selector.</param>
        /// <returns>The operation builder for chaining.</returns>
        ICampaignListOperationBuilder OrderBy<TKey>(Expression<Func<Campaign, TKey>> keySelector);

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
        /// <returns>The paginated result.</returns>
        Task<PagedResult<Campaign>> ExecuteAsync();
    }

    /// <summary>
    /// Interface for building donor operations with fluent configuration.
    /// </summary>
    /// <typeparam name="TResult">The result type of the operation.</typeparam>
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
    /// Interface for building donor search operations.
    /// </summary>
    public interface IDonorSearchOperationBuilder
    {
        /// <summary>
        /// Adds a filter condition to the search.
        /// </summary>
        /// <param name="predicate">The filter predicate.</param>
        /// <returns>The operation builder for chaining.</returns>
        IDonorSearchOperationBuilder Where(Expression<Func<Donor, bool>> predicate);

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
        /// Executes the search operation asynchronously.
        /// </summary>
        /// <returns>The search results.</returns>
        Task<PagedResult<Donor>> ExecuteAsync();
    }
}