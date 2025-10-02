using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using FundraiseUp.Client.Models;

namespace FundraiseUp.Client.Requests
{
    /// <summary>
    /// Request to create a donation through the FundraiseUp API.
    /// Based on the official FundraiseUp API specification.
    /// </summary>
    public class CreateDonationRequest
    {
        /// <summary>
        /// Donation amount in the selected currency. Must be a decimal string in the format {NNN}[.nn].
        /// The amount must be greater than $1 or its equivalent in other currencies.
        /// </summary>
        [Required]
        public string Amount { get; set; } = string.Empty;

        /// <summary>
        /// Campaign ID. Must belong to the account that owns the API key and be active.
        /// </summary>
        [Required]
        public string Campaign { get; set; } = string.Empty;

        /// <summary>
        /// Three-letter ISO currency code, in lowercase (e.g., "usd", "eur").
        /// </summary>
        [Required]
        public string Currency { get; set; } = string.Empty;

        /// <summary>
        /// The ID of the fund, program, or initiative the donation goes to.
        /// You can find the ID in the Designations section of the Dashboard.
        /// </summary>
        [Required]
        public string Designation { get; set; } = string.Empty;

        /// <summary>
        /// Payment method details (Stripe payment method information).
        /// </summary>
        [Required]
        public PaymentMethodRequest PaymentMethod { get; set; } = new();

        /// <summary>
        /// Supporter personal information.
        /// </summary>
        [Required]
        public SupporterRequest Supporter { get; set; } = new();

        /// <summary>
        /// Optional comment, maximum 256 characters.
        /// </summary>
        public string? Comment { get; set; }

        /// <summary>
        /// Custom fields populated according to the campaign settings.
        /// </summary>
        public List<CustomFieldRequest>? CustomFields { get; set; }

        /// <summary>
        /// Questions answered during the donation process.
        /// </summary>
        public List<QuestionRequest>? Questions { get; set; }

        /// <summary>
        /// Recurring plan details. Include this to create a recurring donation.
        /// </summary>
        public RecurringPlanRequest? RecurringPlan { get; set; }

        /// <summary>
        /// Set to true to prevent the donation receipt email from being sent for the first installment.
        /// Available for recurring plans only.
        /// </summary>
        public bool? SkipThankYouEmail { get; set; }
    }

    /// <summary>
    /// Payment method details for creating a donation.
    /// </summary>
    public class PaymentMethodRequest
    {
        /// <summary>
        /// Stripe payment method information.
        /// </summary>
        [Required]
        public StripePaymentMethodRequest Stripe { get; set; } = new();
    }

    /// <summary>
    /// Stripe payment method details.
    /// </summary>
    public class StripePaymentMethodRequest
    {
        /// <summary>
        /// The PaymentMethod ID within the organization's Stripe account.
        /// Supported payment methods are credit cards, debit cards, Apple Pay, and Google Pay.
        /// For testing, use test payment method IDs like "pm_card_visa".
        /// </summary>
        [Required]
        public string Id { get; set; } = string.Empty;
    }

    /// <summary>
    /// Supporter personal information for creating a donation.
    /// </summary>
    public class SupporterRequest
    {
        /// <summary>
        /// Supporter's first name. Maximum 256 characters.
        /// </summary>
        [Required]
        public string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// Supporter's last name. Maximum 256 characters.
        /// </summary>
        [Required]
        public string LastName { get; set; } = string.Empty;

        /// <summary>
        /// Supporter's email address. Maximum 254 characters.
        /// </summary>
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Supporter's phone number. Maximum 20 characters.
        /// Only included if the campaign setting "Ask for phone number" is enabled.
        /// </summary>
        public string? Phone { get; set; }

        /// <summary>
        /// Supporter's mailing address.
        /// Required if the campaign requires a mailing address.
        /// </summary>
        public SupporterAddressRequest? Address { get; set; }

        /// <summary>
        /// Supporter's employer information.
        /// </summary>
        public SupporterEmployerRequest? Employer { get; set; }

        /// <summary>
        /// Supporter's title (e.g., "mr", "mrs", "ms").
        /// Available options depend on campaign settings and country.
        /// </summary>
        public string? Title { get; set; }
    }

    /// <summary>
    /// Supporter's mailing address information.
    /// </summary>
    public class SupporterAddressRequest
    {
        /// <summary>
        /// Supporter's city. Maximum 64 characters.
        /// </summary>
        [Required]
        public string City { get; set; } = string.Empty;

        /// <summary>
        /// The first line of the supporter's mailing address. Maximum 256 characters.
        /// </summary>
        [Required]
        public string Line1 { get; set; } = string.Empty;

        /// <summary>
        /// Supporter's postal code. Maximum 10 characters.
        /// </summary>
        [Required]
        public string PostalCode { get; set; } = string.Empty;

        /// <summary>
        /// Two-letter country code, in lowercase (e.g., "us", "ca").
        /// </summary>
        [Required]
        public string Country { get; set; } = string.Empty;

        /// <summary>
        /// The second line of the supporter's mailing address. Maximum 256 characters.
        /// </summary>
        public string? Line2 { get; set; }

        /// <summary>
        /// Supporter's region, state, or province. Maximum 64 characters.
        /// Required for most countries except GB, IL, NL, FR, DE.
        /// </summary>
        public string? Region { get; set; }
    }

    /// <summary>
    /// Supporter's employer information.
    /// </summary>
    public class SupporterEmployerRequest
    {
        /// <summary>
        /// The name of the organization or company where the supporter works. Maximum 64 characters.
        /// </summary>
        [Required]
        public string Name { get; set; } = string.Empty;
    }

    /// <summary>
    /// Custom field for donation.
    /// </summary>
    public class CustomFieldRequest
    {
        /// <summary>
        /// Name of the custom field.
        /// </summary>
        [Required]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Value of the custom field.
        /// </summary>
        [Required]
        public string Value { get; set; } = string.Empty;
    }

    /// <summary>
    /// Question answer for donation.
    /// </summary>
    public class QuestionRequest
    {
        /// <summary>
        /// Unique identifier of the question, formatted as Q[A-Z]{7}.
        /// </summary>
        [Required]
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Answer for text-based questions. Maximum 256 characters.
        /// Required for "Text box", "Text line", "Select one option", and "Dropdown menu" question types.
        /// </summary>
        public string? Text { get; set; }

        /// <summary>
        /// List of selected options.
        /// Required for "Multiple checkboxes" question types.
        /// </summary>
        public List<string>? Options { get; set; }

        /// <summary>
        /// Indicates whether the checkbox is checked.
        /// Required for "Single checkbox" question types.
        /// </summary>
        public bool? Checked { get; set; }

        /// <summary>
        /// Answer for date questions in YYYY-MM-DD format.
        /// Required for "Date" question types.
        /// </summary>
        public string? Date { get; set; }
    }

    /// <summary>
    /// Recurring plan configuration for creating recurring donations.
    /// </summary>
    public class RecurringPlanRequest
    {
        /// <summary>
        /// Recurring plan frequency. Determines how often a recurring donation is processed.
        /// </summary>
        [Required]
        public string Frequency { get; set; } = string.Empty;
    }

    /// <summary>
    /// Recurring plan frequency constants for FundraiseUp API.
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
    /// Request model for updating limited donation properties.
    /// Note: Most donation properties are read-only after creation.
    /// Only a few fields can be updated via the API within 24 hours of creation.
    /// </summary>
    public class UpdateDonationRequest
    {
        /// <summary>
        /// Update the comment/message for the donation.
        /// </summary>
        [JsonPropertyName("comment")]
        public string? Comment { get; set; }

        /// <summary>
        /// Update custom fields for the donation.
        /// </summary>
        [JsonPropertyName("custom_fields")]
        public List<CustomFieldPutRequest>? CustomFields { get; set; }

        /// <summary>
        /// Update questions answered during the donation process.
        /// </summary>
        [JsonPropertyName("questions")]
        public List<QuestionPutRequest>? Questions { get; set; }

        /// <summary>
        /// Update supporter information.
        /// </summary>
        [JsonPropertyName("supporter")]
        public SupporterPutRequest? Supporter { get; set; }

        /// <summary>
        /// Change the campaign for the donation.
        /// </summary>
        [JsonPropertyName("campaign")]
        public string? Campaign { get; set; }

        /// <summary>
        /// Change the designation for the donation.
        /// </summary>
        [JsonPropertyName("designation")]
        public string? Designation { get; set; }

        /// <summary>
        /// Validates the update request to ensure it contains at least one field to update
        /// and that all provided values are valid.
        /// </summary>
        /// <returns>True if the request is valid, false otherwise.</returns>
        public bool IsValid()
        {
            // At least one field must be provided for update
            return !string.IsNullOrEmpty(Comment) ||
                   (CustomFields != null && CustomFields.Count > 0) ||
                   (Questions != null && Questions.Count > 0) ||
                   Supporter != null ||
                   !string.IsNullOrEmpty(Campaign) ||
                   !string.IsNullOrEmpty(Designation);
        }

        /// <summary>
        /// Validates the update request and returns validation errors if any.
        /// </summary>
        /// <returns>List of validation error messages, empty if valid.</returns>
        public List<string> GetValidationErrors()
        {
            var errors = new List<string>();

            if (!IsValid())
            {
                errors.Add("At least one field must be provided for update.");
            }

            // Validate custom fields
            if (CustomFields != null)
            {
                for (int i = 0; i < CustomFields.Count; i++)
                {
                    var field = CustomFields[i];
                    if (string.IsNullOrWhiteSpace(field.Name))
                    {
                        errors.Add($"Custom field at index {i} must have a name.");
                    }
                    if (field.Value == null)
                    {
                        errors.Add($"Custom field '{field.Name}' at index {i} must have a value (can be empty string).");
                    }
                }
            }

            // Validate questions
            if (Questions != null)
            {
                for (int i = 0; i < Questions.Count; i++)
                {
                    var question = Questions[i];
                    if (string.IsNullOrWhiteSpace(question.Id))
                    {
                        errors.Add($"Question at index {i} must have an ID.");
                    }
                    if (question.Answer == null)
                    {
                        errors.Add($"Question '{question.Id}' at index {i} must have an answer (can be empty string).");
                    }
                }
            }

            // Validate supporter information
            if (Supporter != null)
            {
                if (!string.IsNullOrEmpty(Supporter.Email) && !IsValidEmail(Supporter.Email))
                {
                    errors.Add("Supporter email format is invalid.");
                }
            }

            return errors;
        }

        private static bool IsValidEmail(string? email)
        {
            if (string.IsNullOrEmpty(email))
                return false;

            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }

    /// <summary>
    /// Custom field update for donation modifications.
    /// </summary>
    public class CustomFieldPutRequest
    {
        /// <summary>
        /// Name of the custom field.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Updated value of the custom field.
        /// </summary>
        [JsonPropertyName("value")]
        public string Value { get; set; } = string.Empty;
    }

    /// <summary>
    /// Question answer update for donation modifications.
    /// </summary>
    public class QuestionPutRequest
    {
        /// <summary>
        /// Unique identifier of the question.
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Updated answer to the question.
        /// </summary>
        [JsonPropertyName("answer")]
        public string Answer { get; set; } = string.Empty;
    }

    /// <summary>
    /// Supporter information update for donation modifications.
    /// </summary>
    public class SupporterPutRequest
    {
        /// <summary>
        /// Updated first name.
        /// </summary>
        [JsonPropertyName("first_name")]
        public string? FirstName { get; set; }

        /// <summary>
        /// Updated last name.
        /// </summary>
        [JsonPropertyName("last_name")]
        public string? LastName { get; set; }

        /// <summary>
        /// Updated email address.
        /// </summary>
        [JsonPropertyName("email")]
        public string? Email { get; set; }

        /// <summary>
        /// Updated phone number.
        /// </summary>
        [JsonPropertyName("phone")]
        public string? Phone { get; set; }

        /// <summary>
        /// Updated address information.
        /// </summary>
        [JsonPropertyName("address")]
        public SupporterAddressRequest? Address { get; set; }
    }

    /// <summary>
    /// Placeholder request model for campaign operations.
    /// Note: Campaigns are read-only in FundraiseUp API and cannot be created or updated.
    /// This model exists only to satisfy test compilation.
    /// </summary>
    // Note: Campaigns cannot be created via FundraiseUp API - they are read-only.
    public class CreateCampaignRequest
    {
        /// <summary>
        /// Campaign name.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Campaign description.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Goal amount as string.
        /// </summary>
        public string? GoalAmount { get; set; }

        /// <summary>
        /// Currency code.
        /// </summary>
        public string Currency { get; set; } = string.Empty;

        /// <summary>
        /// Whether campaign is active.
        /// </summary>
        public bool IsActive { get; set; }
    }

    /// <summary>
    /// Placeholder request model for campaign updates.
    /// Note: Campaigns are read-only in FundraiseUp API and cannot be updated.
    /// This model exists only to satisfy test compilation.
    /// </summary>
    [Obsolete("Campaigns cannot be updated via FundraiseUp API - they are read-only")]
    public class UpdateCampaignRequest
    {
        /// <summary>
        /// Campaign name.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Goal amount as string.
        /// </summary>
        public string? GoalAmount { get; set; }
    }

    /// <summary>
    /// Placeholder request model for supporter operations.
    /// Note: Supporters are read-only in FundraiseUp API and cannot be created directly.
    /// This model exists only to satisfy test compilation.
    /// </summary>
    [Obsolete("Supporters cannot be created via FundraiseUp API - they are created automatically with donations")]
    public class CreateDonorRequest
    {
        /// <summary>
        /// Supporter email.
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// First name.
        /// </summary>
        public string? FirstName { get; set; }

        /// <summary>
        /// Last name.
        /// </summary>
        public string? LastName { get; set; }

        /// <summary>
        /// Phone number.
        /// </summary>
        public string? Phone { get; set; }
    }

    /// <summary>
    /// Placeholder request model for supporter updates.
    /// Note: Supporters are read-only in FundraiseUp API and cannot be updated.
    /// This model exists only to satisfy test compilation.
    /// </summary>
    [Obsolete("Supporters cannot be updated via FundraiseUp API - they are read-only")]
    public class UpdateDonorRequest
    {
        /// <summary>
        /// First name.
        /// </summary>
        public string? FirstName { get; set; }

        /// <summary>
        /// Last name.
        /// </summary>
        public string? LastName { get; set; }

        /// <summary>
        /// Phone number.
        /// </summary>
        public string? Phone { get; set; }
    }

    /// <summary>
    /// Placeholder model for campaign statistics.
    /// Note: Campaign statistics are not currently available in the FundraiseUp API.
    /// This model exists only to satisfy test compilation.
    /// </summary>
    [Obsolete("Campaign statistics not available in current FundraiseUp API")]
    public class CampaignStatistics
    {
        /// <summary>
        /// Campaign ID.
        /// </summary>
        public string CampaignId { get; set; } = string.Empty;

        /// <summary>
        /// Total amount raised.
        /// </summary>
        public string TotalRaised { get; set; } = string.Empty;

        /// <summary>
        /// Number of donations.
        /// </summary>
        public int DonationCount { get; set; }

        /// <summary>
        /// Progress percentage.
        /// </summary>
        public double ProgressPercentage { get; set; }
    }

    /// <summary>
    /// Placeholder model for supporter statistics.
    /// Note: Supporter statistics are not currently available in the FundraiseUp API.
    /// This model exists only to satisfy test compilation.
    /// </summary>
    [Obsolete("Supporter statistics not available in current FundraiseUp API")]
    public class DonorStatistics
    {
        /// <summary>
        /// Supporter ID.
        /// </summary>
        public string DonorId { get; set; } = string.Empty;

        /// <summary>
        /// Total donated amount.
        /// </summary>
        public string TotalDonated { get; set; } = string.Empty;

        /// <summary>
        /// Number of donations.
        /// </summary>
        public int DonationCount { get; set; }

        /// <summary>
        /// First donation date.
        /// </summary>
        public DateTimeOffset FirstDonationDate { get; set; }
    }

    /// <summary>
    /// Request model for creating a new fundraiser.
    /// </summary>
    public class CreateFundraiserRequest
    {
        /// <summary>
        /// Fundraiser title (required).
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Fundraiser description.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Campaign ID to associate with this fundraiser (required).
        /// </summary>
        public string Campaign { get; set; } = string.Empty;

        /// <summary>
        /// Fundraiser goal amount in the organization's default currency.
        /// </summary>
        public string? Goal { get; set; }

        /// <summary>
        /// Custom fundraiser URL slug.
        /// </summary>
        public string? Slug { get; set; }

        /// <summary>
        /// Fundraiser image URL.
        /// </summary>
        public string? ImageUrl { get; set; }

        /// <summary>
        /// Video URL for the fundraiser.
        /// </summary>
        public string? VideoUrl { get; set; }

        /// <summary>
        /// End date for the fundraiser.
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Indicates if the fundraiser allows anonymous donations.
        /// </summary>
        public bool AllowAnonymous { get; set; } = true;

        /// <summary>
        /// Indicates if the fundraiser shows the donor list publicly.
        /// </summary>
        public bool ShowDonorList { get; set; } = true;

        /// <summary>
        /// Minimum donation amount allowed.
        /// </summary>
        public string? MinimumAmount { get; set; }

        /// <summary>
        /// Suggested donation amounts.
        /// </summary>
        public List<string> SuggestedAmounts { get; set; } = new();

        /// <summary>
        /// Fundraiser creator information.
        /// </summary>
        public FundraiserCreatorRequest? Creator { get; set; }

        /// <summary>
        /// Beneficiary information.
        /// </summary>
        public FundraiserBeneficiaryRequest? Beneficiary { get; set; }

        /// <summary>
        /// Custom fields for the fundraiser.
        /// </summary>
        public List<CustomFieldRequest> CustomFields { get; set; } = new();

        /// <summary>
        /// Social sharing settings.
        /// </summary>
        public SocialSharingRequest? SocialSharing { get; set; }

        /// <summary>
        /// SEO metadata.
        /// </summary>
        public SeoRequest? Seo { get; set; }
    }

    /// <summary>
    /// Request model for updating an existing fundraiser.
    /// </summary>
    public class UpdateFundraiserRequest
    {
        /// <summary>
        /// Updated fundraiser title.
        /// </summary>
        public string? Title { get; set; }

        /// <summary>
        /// Updated fundraiser description.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Updated fundraiser goal amount.
        /// </summary>
        public string? Goal { get; set; }

        /// <summary>
        /// Updated fundraiser status.
        /// </summary>
        public string? Status { get; set; }

        /// <summary>
        /// Updated custom fundraiser URL slug.
        /// </summary>
        public string? Slug { get; set; }

        /// <summary>
        /// Updated fundraiser image URL.
        /// </summary>
        public string? ImageUrl { get; set; }

        /// <summary>
        /// Updated video URL for the fundraiser.
        /// </summary>
        public string? VideoUrl { get; set; }

        /// <summary>
        /// Updated end date for the fundraiser.
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Updated setting for allowing anonymous donations.
        /// </summary>
        public bool? AllowAnonymous { get; set; }

        /// <summary>
        /// Updated setting for showing the donor list publicly.
        /// </summary>
        public bool? ShowDonorList { get; set; }

        /// <summary>
        /// Updated minimum donation amount allowed.
        /// </summary>
        public string? MinimumAmount { get; set; }

        /// <summary>
        /// Updated suggested donation amounts.
        /// </summary>
        public List<string>? SuggestedAmounts { get; set; }

        /// <summary>
        /// Updated custom fields for the fundraiser.
        /// </summary>
        public List<CustomFieldRequest>? CustomFields { get; set; }

        /// <summary>
        /// Updated social sharing settings.
        /// </summary>
        public SocialSharingRequest? SocialSharing { get; set; }

        /// <summary>
        /// Updated SEO metadata.
        /// </summary>
        public SeoRequest? Seo { get; set; }
    }

    /// <summary>
    /// Fundraiser creator information for requests.
    /// </summary>
    public class FundraiserCreatorRequest
    {
        /// <summary>
        /// Creator's name.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Creator's email address.
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// Creator's profile image URL.
        /// </summary>
        public string? AvatarUrl { get; set; }
    }

    /// <summary>
    /// Fundraiser beneficiary information for requests.
    /// </summary>
    public class FundraiserBeneficiaryRequest
    {
        /// <summary>
        /// Beneficiary's name.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Beneficiary's relationship to the creator.
        /// </summary>
        public string? Relationship { get; set; }
    }

    /// <summary>
    /// Social sharing configuration for requests.
    /// </summary>
    public class SocialSharingRequest
    {
        /// <summary>
        /// Facebook sharing enabled.
        /// </summary>
        public bool FacebookEnabled { get; set; } = true;

        /// <summary>
        /// Twitter sharing enabled.
        /// </summary>
        public bool TwitterEnabled { get; set; } = true;

        /// <summary>
        /// Email sharing enabled.
        /// </summary>
        public bool EmailEnabled { get; set; } = true;

        /// <summary>
        /// Custom sharing message.
        /// </summary>
        public string? ShareMessage { get; set; }
    }

    /// <summary>
    /// SEO metadata for requests.
    /// </summary>
    public class SeoRequest
    {
        /// <summary>
        /// Meta title for SEO.
        /// </summary>
        public string? Title { get; set; }

        /// <summary>
        /// Meta description for SEO.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Keywords for SEO.
        /// </summary>
        public List<string> Keywords { get; set; } = new();
    }
}
