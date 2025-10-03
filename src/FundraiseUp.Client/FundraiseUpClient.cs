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

            InitializeOperations();
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

            InitializeOperations();
        }

        /// <summary>
        /// Initializes all operation instances to avoid duplication across constructors.
        /// </summary>
        private void InitializeOperations()
        {
            Donations = new DonationOperations(_httpClient, _logger);
            Campaigns = new CampaignOperations(_httpClient, _logger);
            Supporters = new SupporterOperations(_httpClient, _logger);
            RecurringPlans = new RecurringPlanOperations(_httpClient, _logger);
            Events = new EventOperations(_httpClient, _logger);
            Fundraisers = new FundraiserOperations(_httpClient, _logger);
            DonorPortal = new DonorPortalOperations(_httpClient, _logger);
        }

        /// <inheritdoc />
        public IDonationOperations Donations { get; private set; } = null!;

        /// <inheritdoc />
        public ICampaignOperations Campaigns { get; private set; } = null!;

        /// <inheritdoc />
        public ISupporterOperations Supporters { get; private set; } = null!;

        /// <inheritdoc />
        public IRecurringPlanOperations RecurringPlans { get; private set; } = null!;

        /// <inheritdoc />
        public IEventOperations Events { get; private set; } = null!;

        /// <inheritdoc />
        public IFundraiserOperations Fundraisers { get; private set; } = null!;

        /// <inheritdoc />
        public IDonorPortalOperations DonorPortal { get; private set; } = null!;

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
