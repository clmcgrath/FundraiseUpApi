using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace FundraiseUp.Client.Models
{
    /// <summary>
    /// Represents a fundraising campaign from the FundraiseUp API.
    /// Campaigns are read-only entities and cannot be created or modified through the API.
    /// </summary>
    public class Campaign
    {
        /// <summary>
        /// Unique campaign identifier (format: FUN[A-Z]{8}).
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Campaign code as set in the Dashboard.
        /// </summary>
        [JsonPropertyName("code")]
        public string? Code { get; set; }

        /// <summary>
        /// Campaign name.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Campaign description.
        /// </summary>
        [JsonPropertyName("description")]
        public string? Description { get; set; }

        /// <summary>
        /// The fundraising goal amount represented as a string for precision (e.g., "10000.00").
        /// </summary>
        [JsonPropertyName("goal")]
        public string? Goal { get; set; }

        /// <summary>
        /// Three-letter ISO currency code in lowercase (e.g., "usd", "eur").
        /// </summary>
        [JsonPropertyName("currency")]
        public string Currency { get; set; } = string.Empty;

        /// <summary>
        /// Current status of the campaign.
        /// </summary>
        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// Campaign visibility setting.
        /// </summary>
        [JsonPropertyName("visibility")]
        public string Visibility { get; set; } = string.Empty;

        /// <summary>
        /// Total amount raised for this campaign represented as a string for precision.
        /// </summary>
        [JsonPropertyName("total_raised")]
        public string TotalRaised { get; set; } = string.Empty;

        /// <summary>
        /// Total amount raised in the organization's default currency.
        /// </summary>
        [JsonPropertyName("total_raised_in_default_currency")]
        public string TotalRaisedInDefaultCurrency { get; set; } = string.Empty;

        /// <summary>
        /// Number of donations received for this campaign.
        /// </summary>
        [JsonPropertyName("donation_count")]
        public int DonationCount { get; set; }

        /// <summary>
        /// When the campaign was created in the system (UTC).
        /// </summary>
        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// When the campaign was last updated (UTC).
        /// </summary>
        [JsonPropertyName("updated_at")]
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// Campaign start date (if set).
        /// </summary>
        [JsonPropertyName("start_date")]
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// Campaign end date (if set).
        /// </summary>
        [JsonPropertyName("end_date")]
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// URL of the campaign page.
        /// </summary>
        [JsonPropertyName("url")]
        public string? Url { get; set; }

        /// <summary>
        /// Custom fields associated with the campaign.
        /// </summary>
        [JsonPropertyName("custom_fields")]
        public List<CustomFieldResponse> CustomFields { get; set; } = new();
    }

    /// <summary>
    /// Collection response for campaigns with pagination support.
    /// </summary>
    public class CampaignsResponse
    {
        /// <summary>
        /// List of campaigns.
        /// </summary>
        [JsonPropertyName("data")]
        public List<Campaign> Data { get; set; } = new();

        /// <summary>
        /// Pagination cursor for next page of results.
        /// </summary>
        [JsonPropertyName("next_cursor")]
        public string? NextCursor { get; set; }

        /// <summary>
        /// Indicates if there are more results available.
        /// </summary>
        [JsonPropertyName("has_more")]
        public bool HasMore { get; set; }
    }

    /// <summary>
    /// Campaign status values as returned by the FundraiseUp API.
    /// </summary>
    public static class CampaignStatus
    {
        public const string Active = "active";
        public const string Paused = "paused";
        public const string Ended = "ended";
        public const string Draft = "draft";
        public const string Archived = "archived";
    }

    /// <summary>
    /// Campaign visibility values.
    /// </summary>
    public static class CampaignVisibility
    {
        public const string Public = "public";
        public const string Private = "private";
        public const string Unlisted = "unlisted";
    }
}
