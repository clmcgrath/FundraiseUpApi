using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FundraiseUp.Client.Models;
using Microsoft.Extensions.Logging;

namespace FundraiseUp.Client.Operations
{
    /// <summary>
    /// Implementation of campaign operations for FundraiseUp API.
    /// Note: FundraiseUp API does not provide direct campaign endpoints.
    /// Campaign data is only available embedded within donation, supporter, and other entity responses.
    /// Campaign management must be done through the FundraiseUp Dashboard.
    /// </summary>
    internal class CampaignOperations : ICampaignOperations
    {
        private readonly HttpClientWrapper _httpClient;
        private readonly ILogger? _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="CampaignOperations"/> class.
        /// </summary>
        /// <param name="httpClient">The HTTP client wrapper.</param>
        /// <param name="logger">Optional logger instance.</param>
        public CampaignOperations(HttpClientWrapper httpClient, ILogger? logger = null)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        // No operations implemented - campaigns are only accessible as embedded data
        // within other API responses (donations, supporters, etc.)
    }
}
