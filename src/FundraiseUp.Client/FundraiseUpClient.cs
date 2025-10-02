using System;
using FundraiseUp.Client.Configuration;
using FundraiseUp.Client.Operations;
using Microsoft.Extensions.Logging;

namespace FundraiseUp.Client
{
    /// <summary>
    /// Main implementation of the FundraiseUp API client.
    /// </summary>
    public class FundraiseUpClient : IFundraiseUpClient
    {
        private readonly FundraiseUpClientOptions _options;
        private readonly ILogger<FundraiseUpClient>? _logger;
        private readonly HttpClientWrapper _httpClient;
        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="FundraiseUpClient"/> class.
        /// </summary>
        /// <param name="apiKey">The API key for authentication.</param>
        public FundraiseUpClient(string apiKey)
            : this(new FundraiseUpClientOptions { ApiKey = apiKey })
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FundraiseUpClient"/> class.
        /// </summary>
        /// <param name="options">The client configuration options.</param>
        /// <param name="logger">Optional logger instance.</param>
        public FundraiseUpClient(FundraiseUpClientOptions options, ILogger<FundraiseUpClient>? logger = null)
        {
            FundraiseUpClientOptionsValidator.Validate(options);

            _options = options;
            _logger = logger;
            _httpClient = new HttpClientWrapper(options, logger);

            Donations = new DonationOperations(_httpClient, logger);
            Campaigns = new CampaignOperations(_httpClient, logger);
            Supporters = new SupporterOperations(_httpClient, logger);
            RecurringPlans = new RecurringPlanOperations(_httpClient, logger);
            Events = new EventOperations(_httpClient, logger);
            Fundraisers = new FundraiserOperations(_httpClient, logger);
            DonorPortal = new DonorPortalOperations(_httpClient, logger);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FundraiseUpClient"/> class for testing.
        /// </summary>
        /// <param name="apiKey">The API key for authentication.</param>
        /// <param name="options">The client configuration options.</param>
        /// <param name="httpClient">Custom HTTP client for testing.</param>
        /// <param name="logger">Optional logger instance.</param>
        public FundraiseUpClient(string apiKey, FundraiseUpClientOptions options, HttpClient httpClient, ILogger<FundraiseUpClient>? logger = null)
        {
            options.ApiKey = apiKey;
            FundraiseUpClientOptionsValidator.Validate(options);

            _options = options;
            _logger = logger;
            _httpClient = new HttpClientWrapper(options, httpClient, logger);

            Donations = new DonationOperations(_httpClient, logger);
            Campaigns = new CampaignOperations(_httpClient, logger);
            Supporters = new SupporterOperations(_httpClient, logger);
            RecurringPlans = new RecurringPlanOperations(_httpClient, logger);
            Events = new EventOperations(_httpClient, logger);
            Fundraisers = new FundraiserOperations(_httpClient, logger);
            DonorPortal = new DonorPortalOperations(_httpClient, logger);
        }

        /// <inheritdoc />
        public IDonationOperations Donations { get; }

        /// <inheritdoc />
        public ICampaignOperations Campaigns { get; }

        /// <inheritdoc />
        public ISupporterOperations Supporters { get; }

        /// <inheritdoc />
        public IRecurringPlanOperations RecurringPlans { get; }

        /// <inheritdoc />
        public IEventOperations Events { get; }

        /// <inheritdoc />
        public IFundraiserOperations Fundraisers { get; }

        /// <inheritdoc />
        public IDonorPortalOperations DonorPortal { get; }

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
