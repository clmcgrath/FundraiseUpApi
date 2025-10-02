using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace FundraiseUp.Client.Models
{
    /// <summary>
    /// Organization account information.
    /// </summary>
    public class AccountResponse
    {
        /// <summary>
        /// Unique identifier of the organization (format: A[A-Z]{7}).
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Custom identifier for the organization.
        /// </summary>
        [JsonPropertyName("code")]
        public string? Code { get; set; }

        /// <summary>
        /// The name of the organization.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
    }

    /// <summary>
    /// Campaign information.
    /// </summary>
    public class CampaignResponse
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
    }

    /// <summary>
    /// Supporter information as displayed on the Donation page.
    /// </summary>
    public class EmbeddedSupporterResponse
    {
        /// <summary>
        /// Unique supporter identifier (format: S[A-Z]{8}).
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Supporter's email address.
        /// </summary>
        [JsonPropertyName("email")]
        public string? Email { get; set; }

        /// <summary>
        /// Supporter's first name.
        /// </summary>
        [JsonPropertyName("first_name")]
        public string? FirstName { get; set; }

        /// <summary>
        /// Supporter's last name.
        /// </summary>
        [JsonPropertyName("last_name")]
        public string? LastName { get; set; }

        /// <summary>
        /// Supporter's phone number.
        /// </summary>
        [JsonPropertyName("phone")]
        public string? Phone { get; set; }

        /// <summary>
        /// Supporter's title (e.g., "mr", "mrs", "ms").
        /// </summary>
        [JsonPropertyName("title")]
        public string? Title { get; set; }

        /// <summary>
        /// Supporter's locale (e.g., "en-US", "fr-FR").
        /// </summary>
        [JsonPropertyName("language")]
        public string Language { get; set; } = string.Empty;

        /// <summary>
        /// Supporter's mailing address.
        /// </summary>
        [JsonPropertyName("address")]
        public AddressResponse? Address { get; set; }

        /// <summary>
        /// Supporter's employer information.
        /// </summary>
        [JsonPropertyName("employer")]
        public EmployerResponse? Employer { get; set; }
    }

    /// <summary>
    /// Address information.
    /// </summary>
    public class AddressResponse
    {
        /// <summary>
        /// Two-letter country code in lowercase.
        /// </summary>
        [JsonPropertyName("country")]
        public string Country { get; set; } = string.Empty;

        /// <summary>
        /// City name.
        /// </summary>
        [JsonPropertyName("city")]
        public string? City { get; set; }

        /// <summary>
        /// Postal or ZIP code.
        /// </summary>
        [JsonPropertyName("postal_code")]
        public string? PostalCode { get; set; }

        /// <summary>
        /// Region, state, or province.
        /// </summary>
        [JsonPropertyName("region")]
        public string? Region { get; set; }

        /// <summary>
        /// First address line.
        /// </summary>
        [JsonPropertyName("line1")]
        public string? Line1 { get; set; }

        /// <summary>
        /// Second address line.
        /// </summary>
        [JsonPropertyName("line2")]
        public string? Line2 { get; set; }
    }

    /// <summary>
    /// Employer information.
    /// </summary>
    public class EmployerResponse
    {
        /// <summary>
        /// Employer's name.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
    }

    /// <summary>
    /// Designation (fund/program) information.
    /// </summary>
    public class DesignationResponse
    {
        /// <summary>
        /// Unique designation identifier (format: E[A-Z]{7}).
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Designation code as set in the Dashboard.
        /// </summary>
        [JsonPropertyName("code")]
        public string? Code { get; set; }

        /// <summary>
        /// Designation name.
        /// </summary>
        [JsonPropertyName("name")]
        public string? Name { get; set; }
    }

    /// <summary>
    /// Payment processing details.
    /// </summary>
    public class ChargePaymentResponse
    {
        /// <summary>
        /// Payment ID (Stripe Charge ID, PayPal Capture ID, etc.).
        /// </summary>
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        /// <summary>
        /// Email associated with the payment method.
        /// </summary>
        [JsonPropertyName("email")]
        public string? Email { get; set; }

        /// <summary>
        /// Payment method type (e.g., "credit_card", "paypal", "apple_pay").
        /// </summary>
        [JsonPropertyName("method")]
        public string Method { get; set; } = string.Empty;

        /// <summary>
        /// Payment processor (e.g., "stripe", "paypal").
        /// </summary>
        [JsonPropertyName("processor")]
        public string Processor { get; set; } = string.Empty;

        /// <summary>
        /// Most recent payment error message (if status is failed/retrying).
        /// </summary>
        [JsonPropertyName("error_message")]
        public string? ErrorMessage { get; set; }

        /// <summary>
        /// Credit card information (if applicable).
        /// </summary>
        [JsonPropertyName("credit_card")]
        public CreditCardResponse? CreditCard { get; set; }

        /// <summary>
        /// Bank account information (if applicable).
        /// </summary>
        [JsonPropertyName("bank_account")]
        public BankAccountResponse? BankAccount { get; set; }
    }

    /// <summary>
    /// Credit card information.
    /// </summary>
    public class CreditCardResponse
    {
        /// <summary>
        /// Last four digits of the credit card number.
        /// </summary>
        [JsonPropertyName("last4")]
        public string? Last4 { get; set; }

        /// <summary>
        /// Two-digit expiration month (01 to 12).
        /// </summary>
        [JsonPropertyName("exp_month")]
        public string? ExpMonth { get; set; }

        /// <summary>
        /// Four-digit expiration year.
        /// </summary>
        [JsonPropertyName("exp_year")]
        public string? ExpYear { get; set; }

        /// <summary>
        /// Credit card type (e.g., "visa", "mastercard").
        /// </summary>
        [JsonPropertyName("type")]
        public string? Type { get; set; }
    }

    /// <summary>
    /// Bank account information.
    /// </summary>
    public class BankAccountResponse
    {
        /// <summary>
        /// Last four digits of the bank account number.
        /// </summary>
        [JsonPropertyName("last4")]
        public string? Last4 { get; set; }
    }

    /// <summary>
    /// Platform fee details.
    /// </summary>
    public class PlatformFeeResponse
    {
        /// <summary>
        /// Platform fee amount in the donation's currency.
        /// </summary>
        [JsonPropertyName("amount")]
        public string Amount { get; set; } = string.Empty;

        /// <summary>
        /// Platform fee amount in the organization's default currency.
        /// </summary>
        [JsonPropertyName("amount_in_default_currency")]
        public string AmountInDefaultCurrency { get; set; } = string.Empty;

        /// <summary>
        /// Three-letter ISO currency code in lowercase.
        /// </summary>
        [JsonPropertyName("currency")]
        public string Currency { get; set; } = string.Empty;
    }

    /// <summary>
    /// Processing fee details.
    /// </summary>
    public class ProcessingFeeResponse
    {
        /// <summary>
        /// Processing fee amount in the donation's currency.
        /// </summary>
        [JsonPropertyName("amount")]
        public string Amount { get; set; } = string.Empty;

        /// <summary>
        /// Processing fee amount in the organization's default currency.
        /// </summary>
        [JsonPropertyName("amount_in_default_currency")]
        public string AmountInDefaultCurrency { get; set; } = string.Empty;

        /// <summary>
        /// Three-letter ISO currency code in lowercase.
        /// </summary>
        [JsonPropertyName("currency")]
        public string Currency { get; set; } = string.Empty;
    }

    /// <summary>
    /// Payout details.
    /// </summary>
    public class PayoutResponse
    {
        /// <summary>
        /// Payout amount in the donation's currency.
        /// </summary>
        [JsonPropertyName("amount")]
        public string Amount { get; set; } = string.Empty;

        /// <summary>
        /// Payout amount in the organization's default currency.
        /// </summary>
        [JsonPropertyName("amount_in_default_currency")]
        public string AmountInDefaultCurrency { get; set; } = string.Empty;

        /// <summary>
        /// Three-letter ISO currency code in lowercase.
        /// </summary>
        [JsonPropertyName("currency")]
        public string Currency { get; set; } = string.Empty;
    }

    /// <summary>
    /// Device information used for the donation.
    /// </summary>
    public class DeviceResponse
    {
        /// <summary>
        /// Browser name (can be null if not detected).
        /// </summary>
        [JsonPropertyName("browser")]
        public string? Browser { get; set; }

        /// <summary>
        /// Operating system name (can be null if not detected).
        /// </summary>
        [JsonPropertyName("os")]
        public string? Os { get; set; }

        /// <summary>
        /// Device type (can be null if not detected).
        /// </summary>
        [JsonPropertyName("type")]
        public string? Type { get; set; }

        /// <summary>
        /// IP address details.
        /// </summary>
        [JsonPropertyName("ip")]
        public IpResponse Ip { get; set; } = new();
    }

    /// <summary>
    /// IP address information.
    /// </summary>
    public class IpResponse
    {
        /// <summary>
        /// IP address.
        /// </summary>
        [JsonPropertyName("address")]
        public string? Address { get; set; }

        /// <summary>
        /// Two-letter country code in lowercase.
        /// </summary>
        [JsonPropertyName("country_name")]
        public string? CountryName { get; set; }

        /// <summary>
        /// Region name.
        /// </summary>
        [JsonPropertyName("region")]
        public string? Region { get; set; }

        /// <summary>
        /// City name.
        /// </summary>
        [JsonPropertyName("city")]
        public string? City { get; set; }
    }

    /// <summary>
    /// UTM tracking parameters.
    /// </summary>
    public class UtmResponse
    {
        /// <summary>
        /// UTM source value.
        /// </summary>
        [JsonPropertyName("source")]
        public string? Source { get; set; }

        /// <summary>
        /// UTM campaign value.
        /// </summary>
        [JsonPropertyName("campaign")]
        public string? Campaign { get; set; }

        /// <summary>
        /// UTM content value.
        /// </summary>
        [JsonPropertyName("content")]
        public string? Content { get; set; }

        /// <summary>
        /// UTM medium value.
        /// </summary>
        [JsonPropertyName("medium")]
        public string? Medium { get; set; }

        /// <summary>
        /// UTM term value.
        /// </summary>
        [JsonPropertyName("term")]
        public string? Term { get; set; }
    }

    /// <summary>
    /// Element through which the donation was made.
    /// </summary>
    public class ElementResponse
    {
        /// <summary>
        /// Unique element identifier.
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Name of the element.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Type of element used for the donation.
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;
    }

    /// <summary>
    /// Fundraiser information.
    /// </summary>
    public class FundraiserResponse
    {
        /// <summary>
        /// Unique identifier for the fundraiser (format: FND[A-Z]{8}).
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// The title of the fundraiser.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Fundraiser title (alias for Name for consistency with API).
        /// </summary>
        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Fundraiser description.
        /// </summary>
        [JsonPropertyName("description")]
        public string? Description { get; set; }

        /// <summary>
        /// Current status of the fundraiser.
        /// </summary>
        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// Fundraiser goal amount in the organization's default currency.
        /// </summary>
        [JsonPropertyName("goal")]
        public string? Goal { get; set; }

        /// <summary>
        /// Total amount raised so far.
        /// </summary>
        [JsonPropertyName("amount_raised")]
        public string AmountRaised { get; set; } = string.Empty;

        /// <summary>
        /// Total number of donations received.
        /// </summary>
        [JsonPropertyName("donations_count")]
        public int DonationsCount { get; set; }

        /// <summary>
        /// Three-letter ISO currency code in lowercase (e.g., "usd", "eur").
        /// </summary>
        [JsonPropertyName("currency")]
        public string Currency { get; set; } = string.Empty;

        /// <summary>
        /// Fundraiser URL slug.
        /// </summary>
        [JsonPropertyName("slug")]
        public string? Slug { get; set; }

        /// <summary>
        /// Custom fundraiser URL.
        /// </summary>
        [JsonPropertyName("url")]
        public string? Url { get; set; }

        /// <summary>
        /// Timestamp when the fundraiser was created in UTC.
        /// </summary>
        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Timestamp when the fundraiser was last updated in UTC.
        /// </summary>
        [JsonPropertyName("updated_at")]
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// Test mode indicator. True for live mode, false for test mode.
        /// </summary>
        [JsonPropertyName("livemode")]
        public bool LiveMode { get; set; }
    }

    /// <summary>
    /// Tribute information.
    /// </summary>
    public class TributeResponse
    {
        /// <summary>
        /// Unique tribute identifier (format: T[A-Z]{7}).
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Tribute type ("in_honor" or "in_memory").
        /// </summary>
        [JsonPropertyName("type")]
        public string? Type { get; set; }

        /// <summary>
        /// Name of the person honored or remembered.
        /// </summary>
        [JsonPropertyName("honoree")]
        public string Honoree { get; set; } = string.Empty;

        /// <summary>
        /// Tribute sharing details.
        /// </summary>
        [JsonPropertyName("sharing")]
        public TributeSharingResponse? Sharing { get; set; }
    }

    /// <summary>
    /// Tribute sharing information.
    /// </summary>
    public class TributeSharingResponse
    {
        /// <summary>
        /// Sharing method ("email" or "address").
        /// </summary>
        [JsonPropertyName("type")]
        public string? Type { get; set; }

        /// <summary>
        /// Name of the sender.
        /// </summary>
        [JsonPropertyName("from")]
        public string? From { get; set; }

        /// <summary>
        /// Custom message included in the tribute.
        /// </summary>
        [JsonPropertyName("message")]
        public string? Message { get; set; }

        /// <summary>
        /// Tribute recipient information.
        /// </summary>
        [JsonPropertyName("recipient")]
        public TributeRecipientResponse? Recipient { get; set; }
    }

    /// <summary>
    /// Tribute recipient information.
    /// </summary>
    public class TributeRecipientResponse
    {
        /// <summary>
        /// Tribute recipient's title.
        /// </summary>
        [JsonPropertyName("title")]
        public string? Title { get; set; }

        /// <summary>
        /// Tribute recipient's first name.
        /// </summary>
        [JsonPropertyName("first_name")]
        public string? FirstName { get; set; }

        /// <summary>
        /// Tribute recipient's last name.
        /// </summary>
        [JsonPropertyName("last_name")]
        public string? LastName { get; set; }

        /// <summary>
        /// Email address for email sharing type.
        /// </summary>
        [JsonPropertyName("email")]
        public string? Email { get; set; }

        /// <summary>
        /// Postal address for the tribute recipient.
        /// </summary>
        [JsonPropertyName("address")]
        public RecipientAddressResponse? Address { get; set; }
    }

    /// <summary>
    /// Recipient postal address.
    /// </summary>
    public class RecipientAddressResponse
    {
        /// <summary>
        /// Two-letter country code in lowercase.
        /// </summary>
        [JsonPropertyName("country")]
        public string Country { get; set; } = string.Empty;

        /// <summary>
        /// City name.
        /// </summary>
        [JsonPropertyName("city")]
        public string? City { get; set; }

        /// <summary>
        /// Postal or ZIP code.
        /// </summary>
        [JsonPropertyName("postal_code")]
        public string? PostalCode { get; set; }

        /// <summary>
        /// Region, state, or province.
        /// </summary>
        [JsonPropertyName("region")]
        public string? Region { get; set; }

        /// <summary>
        /// First address line.
        /// </summary>
        [JsonPropertyName("line1")]
        public string? Line1 { get; set; }

        /// <summary>
        /// Second address line.
        /// </summary>
        [JsonPropertyName("line2")]
        public string? Line2 { get; set; }
    }

    /// <summary>
    /// Gift Aid information (UK organizations only).
    /// </summary>
    public class GiftAidResponse
    {
        /// <summary>
        /// Indicates whether the Gift Aid was claimed.
        /// </summary>
        [JsonPropertyName("claimed")]
        public bool? Claimed { get; set; }

        /// <summary>
        /// Legal text associated with Gift Aid.
        /// </summary>
        [JsonPropertyName("legal_text")]
        public string LegalText { get; set; } = string.Empty;
    }

    /// <summary>
    /// Communication consent details.
    /// </summary>
    public class CommunicationConsentResponse
    {
        /// <summary>
        /// Overall communication consent status.
        /// </summary>
        [JsonPropertyName("general")]
        public string General { get; set; } = string.Empty;

        /// <summary>
        /// Customized consent for individual channels.
        /// </summary>
        [JsonPropertyName("customized")]
        public CustomizedCommunicationConsentResponse Customized { get; set; } = new();
    }

    /// <summary>
    /// Detailed consent for individual communication channels.
    /// </summary>
    public class CustomizedCommunicationConsentResponse
    {
        /// <summary>
        /// Email communication consent.
        /// </summary>
        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Phone call communication consent.
        /// </summary>
        [JsonPropertyName("phone_calls")]
        public string PhoneCalls { get; set; } = string.Empty;

        /// <summary>
        /// SMS communication consent.
        /// </summary>
        [JsonPropertyName("sms")]
        public string Sms { get; set; } = string.Empty;

        /// <summary>
        /// Postal mail communication consent.
        /// </summary>
        [JsonPropertyName("postal_mail")]
        public string PostalMail { get; set; } = string.Empty;

        /// <summary>
        /// Social media communication consent.
        /// </summary>
        [JsonPropertyName("social_media")]
        public string SocialMedia { get; set; } = string.Empty;
    }

    /// <summary>
    /// Custom field response.
    /// </summary>
    public class CustomFieldResponse
    {
        /// <summary>
        /// Name of the custom field.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Value of the custom field.
        /// </summary>
        [JsonPropertyName("value")]
        public string Value { get; set; } = string.Empty;
    }

    /// <summary>
    /// Question response.
    /// </summary>
    public class QuestionResponse
    {
        /// <summary>
        /// Unique identifier of the question (format: Q[A-Z]{7}).
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Question code as set in the Dashboard.
        /// </summary>
        [JsonPropertyName("code")]
        public string? Code { get; set; }

        /// <summary>
        /// Answer text for text-based questions.
        /// </summary>
        [JsonPropertyName("text")]
        public string? Text { get; set; }

        /// <summary>
        /// Selected options for multiple choice questions.
        /// </summary>
        [JsonPropertyName("options")]
        public List<string>? Options { get; set; }

        /// <summary>
        /// Checkbox selection status.
        /// </summary>
        [JsonPropertyName("checked")]
        public bool? Checked { get; set; }
    }

    /// <summary>
    /// Recurring plan information within a donation response.
    /// </summary>
    public class DonationRecurringPlanResponse
    {
        /// <summary>
        /// Unique identifier for the recurring donation plan (format: R[A-Z]{7}).
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

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
    }

    /// <summary>
    /// Communication consent status values.
    /// </summary>
    public static class ConsentStatus
    {
        public const string NotSubmitted = "not_submitted";
        public const string OptedIn = "opted_in";
        public const string OptedOut = "opted_out";
    }

    /// <summary>
    /// Recurring plan status values.
    /// </summary>
    public static class RecurringPlanStatus
    {
        public const string Active = "active";
        public const string Scheduled = "scheduled";
        public const string Paused = "paused";
        public const string Retrying = "retrying";
        public const string Completed = "completed";
        public const string Failed = "failed";
        public const string Canceled = "canceled";
    }

    /// <summary>
    /// Recurring plan frequency values.
    /// </summary>
    public static class RecurringPlanFrequency
    {
        public const string Daily = "daily";
        public const string Weekly = "weekly";
        public const string Biweekly = "biweekly";
        public const string Every4Weeks = "every4weeks";
        public const string Monthly = "monthly";
        public const string Bimonthly = "bimonthly";
        public const string Quarterly = "quarterly";
        public const string Semiannual = "semiannual";
        public const string Annual = "annual";
    }

    /// <summary>
    /// Tribute type values.
    /// </summary>
    public static class TributeType
    {
        public const string InHonor = "in_honor";
        public const string InMemory = "in_memory";
    }

    /// <summary>
    /// Device type values.
    /// </summary>
    public static class DeviceType
    {
        public const string Desktop = "desktop";
        public const string Mobile = "mobile";
        public const string Tablet = "tablet";
    }

    /// <summary>
    /// Paginated response for fundraisers list.
    /// </summary>
    public class FundraisersResponse
    {
        /// <summary>
        /// Array of fundraiser records, paginated by request parameters.
        /// </summary>
        [JsonPropertyName("data")]
        public List<FundraiserResponse> Data { get; set; } = new();

        /// <summary>
        /// Indicates whether there are more records available.
        /// </summary>
        [JsonPropertyName("has_more")]
        public bool HasMore { get; set; }
    }

    /// <summary>
    /// FundraiseUp fundraiser status values.
    /// </summary>
    public static class FundraiserStatus
    {
        public const string Draft = "draft";
        public const string Active = "active";
        public const string Paused = "paused";
        public const string Completed = "completed";
        public const string Canceled = "canceled";
    }

    /// <summary>
    /// Response model for donor portal access links.
    /// </summary>
    public class AccessLinkResponse
    {
        /// <summary>
        /// The unique identifier for the access link.
        /// </summary>
        [JsonPropertyName("url")]
        public string Url { get; set; } = string.Empty;

        /// <summary>
        /// The expiration timestamp of the access link.
        /// </summary>
        [JsonPropertyName("expires_at")]
        public DateTime ExpiresAt { get; set; }
    }
}
