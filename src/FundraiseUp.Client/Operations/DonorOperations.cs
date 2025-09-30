using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using FundraiseUp.Client.Models;
using FundraiseUp.Client.Requests;

namespace FundraiseUp.Client.Operations
{
    /// <summary>
    /// Implementation of donor operations.
    /// </summary>
    internal class DonorOperations : IDonorOperations
    {
        private readonly HttpClientWrapper _httpClient;
        private readonly ILogger? _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="DonorOperations"/> class.
        /// </summary>
        /// <param name="httpClient">The HTTP client wrapper.</param>
        /// <param name="logger">Optional logger instance.</param>
        public DonorOperations(HttpClientWrapper httpClient, ILogger? logger = null)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        /// <inheritdoc />
        public IDonorOperationBuilder<Donor> Create(CreateDonorRequest request)
        {
            return new DonorOperationBuilder<Donor>(_httpClient, _logger, async (correlationId) =>
            {
                return await _httpClient.PostAsync<Donor>("/api/v1/donors", request, correlationId);
            });
        }

        /// <inheritdoc />
        public IDonorOperationBuilder<Donor> GetById(string donorId)
        {
            return new DonorOperationBuilder<Donor>(_httpClient, _logger, async (correlationId) =>
            {
                return await _httpClient.GetAsync<Donor>($"/api/v1/donors/{donorId}", correlationId);
            });
        }

        /// <inheritdoc />
        public IDonorSearchOperationBuilder Search()
        {
            return new DonorSearchOperationBuilder(_httpClient, _logger);
        }

        /// <inheritdoc />
        public IDonorOperationBuilder<Donor> Update(string donorId, UpdateDonorRequest request)
        {
            return new DonorOperationBuilder<Donor>(_httpClient, _logger, async (correlationId) =>
            {
                return await _httpClient.PutAsync<Donor>($"/api/v1/donors/{donorId}", request, correlationId);
            });
        }

        /// <inheritdoc />
        public IDonationListOperationBuilder GetDonations(string donorId)
        {
            return new DonorDonationListOperationBuilder(_httpClient, _logger, donorId);
        }

        /// <inheritdoc />
        public IDonorOperationBuilder<DonorStatistics> GetStatistics(string donorId)
        {
            return new DonorOperationBuilder<DonorStatistics>(_httpClient, _logger, async (correlationId) =>
            {
                return await _httpClient.GetAsync<DonorStatistics>($"/api/v1/donors/{donorId}/statistics", correlationId);
            });
        }

        /// <inheritdoc />
        public IDonorOperationBuilder<Donor> Merge(string primaryDonorId, string duplicateDonorId)
        {
            return new DonorOperationBuilder<Donor>(_httpClient, _logger, async (correlationId) =>
            {
                var request = new { DuplicateDonorId = duplicateDonorId };
                return await _httpClient.PostAsync<Donor>($"/api/v1/donors/{primaryDonorId}/merge", request, correlationId);
            });
        }
    }

    /// <summary>
    /// Implementation of donor operation builder.
    /// </summary>
    /// <typeparam name="TResult">The result type.</typeparam>
    internal class DonorOperationBuilder<TResult> : IDonorOperationBuilder<TResult>
    {
        private readonly HttpClientWrapper _httpClient;
        private readonly ILogger? _logger;
        private readonly Func<string?, Task<TResult>> _operation;
        private TimeSpan? _timeout;
        private int? _retryCount;
        private string? _correlationId;

        /// <summary>
        /// Initializes a new instance of the <see cref="DonorOperationBuilder{TResult}"/> class.
        /// </summary>
        /// <param name="httpClient">The HTTP client wrapper.</param>
        /// <param name="logger">Optional logger instance.</param>
        /// <param name="operation">The operation to execute.</param>
        public DonorOperationBuilder(HttpClientWrapper httpClient, ILogger? logger, Func<string?, Task<TResult>> operation)
        {
            _httpClient = httpClient;
            _logger = logger;
            _operation = operation;
        }

        /// <inheritdoc />
        public IDonorOperationBuilder<TResult> WithTimeout(TimeSpan timeout)
        {
            _timeout = timeout;
            return this;
        }

        /// <inheritdoc />
        public IDonorOperationBuilder<TResult> WithRetry(int retryCount)
        {
            _retryCount = retryCount;
            return this;
        }

        /// <inheritdoc />
        public IDonorOperationBuilder<TResult> WithCorrelationId(string correlationId)
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
    /// Implementation of donor search operation builder.
    /// </summary>
    internal class DonorSearchOperationBuilder : IDonorSearchOperationBuilder
    {
        private readonly HttpClientWrapper _httpClient;
        private readonly ILogger? _logger;
        private TimeSpan? _timeout;
        private int? _retryCount;
        private string? _correlationId;

        /// <summary>
        /// Initializes a new instance of the <see cref="DonorSearchOperationBuilder"/> class.
        /// </summary>
        /// <param name="httpClient">The HTTP client wrapper.</param>
        /// <param name="logger">Optional logger instance.</param>
        public DonorSearchOperationBuilder(HttpClientWrapper httpClient, ILogger? logger = null)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        /// <inheritdoc />
        public IDonorSearchOperationBuilder Where(Expression<Func<Donor, bool>> predicate)
        {
            // For now, just return this - actual query building would be implemented here
            return this;
        }

        /// <inheritdoc />
        public IDonorSearchOperationBuilder WithTimeout(TimeSpan timeout)
        {
            _timeout = timeout;
            return this;
        }

        /// <inheritdoc />
        public IDonorSearchOperationBuilder WithRetry(int retryCount)
        {
            _retryCount = retryCount;
            return this;
        }

        /// <inheritdoc />
        public IDonorSearchOperationBuilder WithCorrelationId(string correlationId)
        {
            _correlationId = correlationId;
            return this;
        }

        /// <inheritdoc />
        public async Task<PagedResult<Donor>> ExecuteAsync()
        {
            // Mock implementation for now - would build actual query parameters
            return await _httpClient.GetAsync<PagedResult<Donor>>("/api/v1/donors/search", _correlationId);
        }
    }

    /// <summary>
    /// Implementation of donor donation list operation builder.
    /// </summary>
    internal class DonorDonationListOperationBuilder : IDonationListOperationBuilder
    {
        private readonly HttpClientWrapper _httpClient;
        private readonly ILogger? _logger;
        private readonly string _donorId;
        private TimeSpan? _timeout;
        private int? _retryCount;
        private string? _correlationId;

        /// <summary>
        /// Initializes a new instance of the <see cref="DonorDonationListOperationBuilder"/> class.
        /// </summary>
        /// <param name="httpClient">The HTTP client wrapper.</param>
        /// <param name="logger">Optional logger instance.</param>
        /// <param name="donorId">The donor identifier.</param>
        public DonorDonationListOperationBuilder(HttpClientWrapper httpClient, ILogger? logger, string donorId)
        {
            _httpClient = httpClient;
            _logger = logger;
            _donorId = donorId;
        }

        /// <inheritdoc />
        public IDonationListOperationBuilder Where(Expression<Func<Donation, bool>> predicate)
        {
            return this;
        }

        /// <inheritdoc />
        public IDonationListOperationBuilder OrderBy<TKey>(Expression<Func<Donation, TKey>> keySelector)
        {
            return this;
        }

        /// <inheritdoc />
        public IDonationListOperationBuilder Take(int count)
        {
            return this;
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
        public async Task<PagedResult<Donation>> ExecuteAsync()
        {
            return await _httpClient.GetAsync<PagedResult<Donation>>($"/api/v1/donors/{_donorId}/donations", _correlationId);
        }
    }
}