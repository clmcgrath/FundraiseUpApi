using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace FundraiseUp.Client.Models
{
    /// <summary>
    /// Represents a donation response from the FundraiseUp API.
    /// Based on the official FundraiseUp API specification.
    /// </summary>
    public class DonationResponse
    {
        /// <summary>
        /// Unique identifier for the donation (format: D[A-Z]{7}).
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Donation amount in the transaction currency (string format like "99.99").
        /// </summary>
        [JsonPropertyName("amount")]
        public string Amount { get; set; } = string.Empty;

        /// <summary>
        /// Donation amount converted to the organization's default currency.
        /// </summary>
        [JsonPropertyName("amount_in_default_currency")]
        public string AmountInDefaultCurrency { get; set; } = string.Empty;

        /// <summary>
        /// Amount before fees were covered by supporter.
        /// </summary>
        [JsonPropertyName("amount_before_fees_covered")]
        public string AmountBeforeFeesCovered { get; set; } = string.Empty;

        /// <summary>
        /// Amount before fees covered, in organization's default currency.
        /// </summary>
        [JsonPropertyName("amount_before_fees_covered_in_default_currency")]
        public string AmountBeforeFeesCoveredInDefaultCurrency { get; set; } = string.Empty;

        /// <summary>
        /// Three-letter ISO currency code in lowercase (e.g., "usd", "eur").
        /// </summary>
        [JsonPropertyName("currency")]
        public string Currency { get; set; } = string.Empty;

        /// <summary>
        /// Indicates if the donation was made anonymously.
        /// </summary>
        [JsonPropertyName("anonymous")]
        public bool Anonymous { get; set; }

        /// <summary>
        /// Current status of the donation.
        /// </summary>
        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// Optional comment provided by the supporter.
        /// </summary>
        [JsonPropertyName("comment")]
        public string? Comment { get; set; }

        /// <summary>
        /// Timestamp when the donation was created in UTC.
        /// </summary>
        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Timestamp when the donation succeeded in UTC.
        /// </summary>
        [JsonPropertyName("succeeded_at")]
        public DateTime? SucceededAt { get; set; }

        /// <summary>
        /// Timestamp when the donation failed in UTC.
        /// </summary>
        [JsonPropertyName("failed_at")]
        public DateTime? FailedAt { get; set; }

        /// <summary>
        /// Timestamp when the donation was refunded in UTC.
        /// </summary>
        [JsonPropertyName("refunded_at")]
        public DateTime? RefundedAt { get; set; }

        /// <summary>
        /// Test mode indicator. True for live mode, false for test mode.
        /// </summary>
        [JsonPropertyName("livemode")]
        public bool LiveMode { get; set; }

        /// <summary>
        /// Source of the donation (e.g., "api", "website", "campaign_page").
        /// </summary>
        [JsonPropertyName("source")]
        public string Source { get; set; } = string.Empty;

        /// <summary>
        /// Installment number for recurring donations (null for one-time).
        /// </summary>
        [JsonPropertyName("installment")]
        public string? Installment { get; set; }

        /// <summary>
        /// Name of organization the donation was made on behalf of.
        /// </summary>
        [JsonPropertyName("on_behalf_of")]
        public string? OnBehalfOf { get; set; }

        /// <summary>
        /// Amount of fees covered by the supporter.
        /// </summary>
        [JsonPropertyName("supporter_covered_fee")]
        public string SupporterCoveredFee { get; set; } = string.Empty;

        /// <summary>
        /// Amount of fees covered by supporter in default currency.
        /// </summary>
        [JsonPropertyName("supporter_covered_fee_in_default_currency")]
        public string SupporterCoveredFeeInDefaultCurrency { get; set; } = string.Empty;

        /// <summary>
        /// URL from which the donation was made.
        /// </summary>
        [JsonPropertyName("url")]
        public string? Url { get; set; }

        /// <summary>
        /// Unique identifier for the donation receipt.
        /// </summary>
        [JsonPropertyName("receipt_id")]
        public string? ReceiptId { get; set; }

        // Nested objects
        /// <summary>
        /// Organization account information.
        /// </summary>
        [JsonPropertyName("account")]
        public AccountResponse Account { get; set; } = new();

        /// <summary>
        /// Campaign details.
        /// </summary>
        [JsonPropertyName("campaign")]
        public CampaignResponse Campaign { get; set; } = new();

        /// <summary>
        /// Supporter information.
        /// </summary>
        [JsonPropertyName("supporter")]
        public EmbeddedSupporterResponse Supporter { get; set; } = new();

        /// <summary>
        /// Designation (fund/program) information.
        /// </summary>
        [JsonPropertyName("designation")]
        public DesignationResponse? Designation { get; set; }

        /// <summary>
        /// Payment processing details.
        /// </summary>
        [JsonPropertyName("payment")]
        public ChargePaymentResponse Payment { get; set; } = new();

        /// <summary>
        /// Platform fee information.
        /// </summary>
        [JsonPropertyName("platform_fee")]
        public PlatformFeeResponse PlatformFee { get; set; } = new();

        /// <summary>
        /// Processing fee information.
        /// </summary>
        [JsonPropertyName("processing_fee")]
        public ProcessingFeeResponse ProcessingFee { get; set; } = new();

        /// <summary>
        /// Payout information.
        /// </summary>
        [JsonPropertyName("payout")]
        public PayoutResponse Payout { get; set; } = new();

        /// <summary>
        /// Device information used for the donation.
        /// </summary>
        [JsonPropertyName("device")]
        public DeviceResponse Device { get; set; } = new();

        /// <summary>
        /// UTM tracking parameters.
        /// </summary>
        [JsonPropertyName("utm")]
        public UtmResponse Utm { get; set; } = new();

        /// <summary>
        /// Element through which the donation was made.
        /// </summary>
        [JsonPropertyName("element")]
        public ElementResponse? Element { get; set; }

        /// <summary>
        /// Fundraiser information if applicable.
        /// </summary>
        [JsonPropertyName("fundraiser")]
        public FundraiserResponse? Fundraiser { get; set; }

        /// <summary>
        /// Tribute information if applicable.
        /// </summary>
        [JsonPropertyName("tribute")]
        public TributeResponse? Tribute { get; set; }

        /// <summary>
        /// Gift Aid information (UK organizations only).
        /// </summary>
        [JsonPropertyName("gift_aid")]
        public GiftAidResponse GiftAid { get; set; } = new();

        /// <summary>
        /// Communication consent details.
        /// </summary>
        [JsonPropertyName("consent")]
        public CommunicationConsentResponse Consent { get; set; } = new();

        /// <summary>
        /// Custom fields populated for this donation.
        /// </summary>
        [JsonPropertyName("custom_fields")]
        public List<CustomFieldResponse> CustomFields { get; set; } = new();

        /// <summary>
        /// Questions answered during the donation process.
        /// </summary>
        [JsonPropertyName("questions")]
        public List<QuestionResponse> Questions { get; set; } = new();

        /// <summary>
        /// Recurring plan information if this is a recurring donation.
        /// </summary>
        [JsonPropertyName("recurring_plan")]
        public DonationRecurringPlanResponse? RecurringPlan { get; set; }
    }

    /// <summary>
    /// Paginated response for donations list.
    /// </summary>
    public class DonationsResponse
    {
        /// <summary>
        /// Array of donation records, paginated by request parameters.
        /// </summary>
        [JsonPropertyName("data")]
        public List<DonationResponse> Data { get; set; } = new();

        /// <summary>
        /// Indicates whether there are more records available.
        /// </summary>
        [JsonPropertyName("has_more")]
        public bool HasMore { get; set; }
    }

    /// <summary>
    /// FundraiseUp donation status values.
    /// </summary>
    public static class DonationStatus
    {
        public const string Succeeded = "succeeded";
        public const string Failed = "failed";
        public const string Refunded = "refunded";
        public const string Pending = "pending";
        public const string Retrying = "retrying";
        public const string Scheduled = "scheduled";
    }

    /// <summary>
    /// FundraiseUp donation source values.
    /// </summary>
    public static class DonationSource
    {
        public const string Website = "website";
        public const string CampaignPage = "campaign_page";
        public const string VirtualTerminal = "virtual_terminal";
        public const string DonorPortal = "donor_portal";
        public const string Dashboard = "dashboard";
        public const string RecurringMigration = "recurring_migration";
        public const string Api = "api";
    }
}
