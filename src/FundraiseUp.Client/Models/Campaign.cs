using System;
using System.Collections.Generic;

namespace FundraiseUp.Client.Models
{
    /// <summary>
    /// Represents a fundraising campaign in the FundraiseUp system.
    /// </summary>
    public class Campaign
    {
        /// <summary>
        /// Gets or sets the unique identifier for the campaign.
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the campaign name.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the campaign description.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the fundraising goal amount.
        /// </summary>
        public decimal Goal { get; set; }

        /// <summary>
        /// Gets or sets the currency code (e.g., USD, EUR).
        /// </summary>
        public string Currency { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the current status of the campaign.
        /// </summary>
        public CampaignStatus Status { get; set; }

        /// <summary>
        /// Gets or sets when the campaign was created.
        /// </summary>
        public DateTimeOffset CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets when the campaign was last updated.
        /// </summary>
        public DateTimeOffset UpdatedAt { get; set; }

        /// <summary>
        /// Gets or sets the campaign start date.
        /// </summary>
        public DateTimeOffset? StartDate { get; set; }

        /// <summary>
        /// Gets or sets the campaign end date.
        /// </summary>
        public DateTimeOffset? EndDate { get; set; }
    }

    /// <summary>
    /// Represents the status of a campaign.
    /// </summary>
    public enum CampaignStatus
    {
        /// <summary>
        /// The campaign is in draft status.
        /// </summary>
        Draft,

        /// <summary>
        /// The campaign is active and accepting donations.
        /// </summary>
        Active,

        /// <summary>
        /// The campaign is paused.
        /// </summary>
        Paused,

        /// <summary>
        /// The campaign has ended.
        /// </summary>
        Ended,

        /// <summary>
        /// The campaign has been archived.
        /// </summary>
        Archived
    }

    /// <summary>
    /// Represents campaign statistics.
    /// </summary>
    public class CampaignStatistics
    {
        /// <summary>
        /// Gets or sets the campaign identifier.
        /// </summary>
        public string CampaignId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the total amount raised.
        /// </summary>
        public decimal TotalRaised { get; set; }

        /// <summary>
        /// Gets or sets the number of donations received.
        /// </summary>
        public int DonationCount { get; set; }

        /// <summary>
        /// Gets or sets the progress percentage towards the goal.
        /// </summary>
        public decimal ProgressPercentage { get; set; }

        /// <summary>
        /// Gets or sets the average donation amount.
        /// </summary>
        public decimal AverageDonation { get; set; }

        /// <summary>
        /// Gets or sets when the statistics were last updated.
        /// </summary>
        public DateTimeOffset LastUpdated { get; set; }
    }
}