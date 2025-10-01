using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace FundraiseUp.Client.Models
{
    /// <summary>
    /// Represents a recurring donation plan response from the FundraiseUp API.
    /// Based on the official FundraiseUp API specification.
    /// </summary>
    public class RecurringPlanResponse
    {
        /// <summary>
        /// Unique identifier for the recurring donation plan (format: R[A-Z]{7}).
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Recurring donation amount in the plan currency.
        /// </summary>
        [JsonPropertyName("amount")]
        public string Amount { get; set; } = string.Empty;

        /// <summary>
        /// Recurring donation amount in the organization's default currency.
        /// </summary>
        [JsonPropertyName("amount_in_default_currency")]
        public string AmountInDefaultCurrency { get; set; } = string.Empty;

        /// <summary>
        /// Three-letter ISO currency code in lowercase (e.g., "usd", "eur").
        /// </summary>
        [JsonPropertyName("currency")]
        public string Currency { get; set; } = string.Empty;

        /// <summary>
        /// Current status of the recurring plan.
        /// </summary>
        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// Donation frequency.
        /// </summary>
        [JsonPropertyName("frequency")]
        public string Frequency { get; set; } = string.Empty;

        /// <summary>
        /// Timestamp when the recurring plan was created in UTC.
        /// </summary>
        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Timestamp when the recurring plan ended in UTC.
        /// </summary>
        [JsonPropertyName("ended_at")]
        public DateTime? EndedAt { get; set; }

        /// <summary>
        /// Timestamp of the next scheduled donation installment in UTC.
        /// </summary>
        [JsonPropertyName("next_installment_at")]
        public DateTime? NextInstallmentAt { get; set; }

        /// <summary>
        /// Total number of installments already processed.
        /// </summary>
        [JsonPropertyName("installments_count")]
        public int InstallmentsCount { get; set; }

        /// <summary>
        /// Test mode indicator. True for live mode, false for test mode.
        /// </summary>
        [JsonPropertyName("livemode")]
        public bool LiveMode { get; set; }

        /// <summary>
        /// Source of the recurring plan creation.
        /// </summary>
        [JsonPropertyName("source")]
        public string Source { get; set; } = string.Empty;

        /// <summary>
        /// Comment provided by the supporter.
        /// </summary>
        [JsonPropertyName("comment")]
        public string? Comment { get; set; }

        /// <summary>
        /// URL from which the recurring plan was created.
        /// </summary>
        [JsonPropertyName("url")]
        public string? Url { get; set; }

        /// <summary>
        /// Amount of fees covered by the supporter per installment.
        /// </summary>
        [JsonPropertyName("supporter_covered_fee")]
        public string SupporterCoveredFee { get; set; } = string.Empty;

        /// <summary>
        /// Amount of fees covered by supporter in default currency per installment.
        /// </summary>
        [JsonPropertyName("supporter_covered_fee_in_default_currency")]
        public string SupporterCoveredFeeInDefaultCurrency { get; set; } = string.Empty;

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
        /// Payment method details.
        /// </summary>
        [JsonPropertyName("payment_method")]
        public ChargePaymentResponse PaymentMethod { get; set; } = new();

        /// <summary>
        /// Device information used to create the recurring plan.
        /// </summary>
        [JsonPropertyName("device")]
        public DeviceResponse Device { get; set; } = new();

        /// <summary>
        /// UTM tracking parameters.
        /// </summary>
        [JsonPropertyName("utm")]
        public UtmResponse Utm { get; set; } = new();

        /// <summary>
        /// Element through which the recurring plan was created.
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
        /// Custom fields populated for this recurring plan.
        /// </summary>
        [JsonPropertyName("custom_fields")]
        public List<CustomFieldResponse> CustomFields { get; set; } = new();

        /// <summary>
        /// Questions answered during the recurring plan creation process.
        /// </summary>
        [JsonPropertyName("questions")]
        public List<QuestionResponse> Questions { get; set; } = new();
    }

    /// <summary>
    /// Paginated response for recurring plans list.
    /// </summary>
    public class RecurringPlansResponse
    {
        /// <summary>
        /// Array of recurring plan records, paginated by request parameters.
        /// </summary>
        [JsonPropertyName("data")]
        public List<RecurringPlanResponse> Data { get; set; } = new();

        /// <summary>
        /// Indicates whether there are more records available.
        /// </summary>
        [JsonPropertyName("has_more")]
        public bool HasMore { get; set; }
    }
}