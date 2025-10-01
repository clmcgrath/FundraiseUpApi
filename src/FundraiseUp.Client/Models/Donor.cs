using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace FundraiseUp.Client.Models
{
    /// <summary>
    /// Represents supporter information from the FundraiseUp API.
    /// Supporters are read-only entities and cannot be created or modified through the API.
    /// This class maintains the "Donor" name for backward compatibility but represents FundraiseUp "supporters".
    /// </summary>
    public class Donor
    {
        /// <summary>
        /// Unique identifier of the supporter (format: S[A-Z]{8}).
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// The supporter's email address.
        /// </summary>
        [JsonPropertyName("email")]
        public string? Email { get; set; }

        /// <summary>
        /// The supporter's first name.
        /// </summary>
        [JsonPropertyName("first_name")]
        public string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// The supporter's last name.
        /// </summary>
        [JsonPropertyName("last_name")]
        public string LastName { get; set; } = string.Empty;

        /// <summary>
        /// The supporter's phone number.
        /// </summary>
        [JsonPropertyName("phone")]
        public string Phone { get; set; } = string.Empty;

        /// <summary>
        /// The supporter's title (e.g., "mr", "mrs", "ms").
        /// </summary>
        [JsonPropertyName("title")]
        public string? Title { get; set; }

        /// <summary>
        /// The supporter's locale (e.g., "en-US", "fr-FR").
        /// </summary>
        [JsonPropertyName("language")]
        public string Language { get; set; } = string.Empty;

        /// <summary>
        /// The supporter's mailing address information.
        /// </summary>
        [JsonPropertyName("address")]
        public Address? Address { get; set; }

        /// <summary>
        /// The supporter's employer information.
        /// </summary>
        [JsonPropertyName("employer")]
        public EmployerResponse? Employer { get; set; }

        /// <summary>
        /// Total amount donated by this supporter across all donations in their default currency.
        /// Represented as a string for precision (e.g., "100.50").
        /// </summary>
        [JsonPropertyName("total_donated")]
        public string TotalDonated { get; set; } = string.Empty;

        /// <summary>
        /// Total amount donated by this supporter in the organization's default currency.
        /// Represented as a string for precision (e.g., "100.50").
        /// </summary>
        [JsonPropertyName("total_donated_in_default_currency")]
        public string TotalDonatedInDefaultCurrency { get; set; } = string.Empty;

        /// <summary>
        /// Three-letter ISO currency code in lowercase for the supporter's donations.
        /// </summary>
        [JsonPropertyName("currency")]
        public string Currency { get; set; } = string.Empty;

        /// <summary>
        /// Number of donations made by this supporter.
        /// </summary>
        [JsonPropertyName("donation_count")]
        public int DonationCount { get; set; }

        /// <summary>
        /// When the supporter was first created in the system (UTC).
        /// </summary>
        [JsonPropertyName("created_at")]
        public DateTimeOffset CreatedAt { get; set; }

        /// <summary>
        /// When the supporter information was last updated (UTC).
        /// </summary>
        [JsonPropertyName("updated_at")]
        public DateTimeOffset UpdatedAt { get; set; }

        /// <summary>
        /// Custom fields associated with the supporter.
        /// </summary>
        [JsonPropertyName("custom_fields")]
        public List<CustomFieldResponse> CustomFields { get; set; } = new();

        /// <summary>
        /// Communication consent details for the supporter.
        /// </summary>
        [JsonPropertyName("communication_consent")]
        public CommunicationConsentResponse? CommunicationConsent { get; set; }

        /// <summary>
        /// Gift Aid information (UK organizations only).
        /// </summary>
        [JsonPropertyName("gift_aid")]
        public GiftAidResponse? GiftAid { get; set; }
    }

    /// <summary>
    /// Represents an address using FundraiseUp API format.
    /// </summary>
    public class Address
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

        // Legacy properties for backward compatibility
        /// <summary>
        /// Gets or sets the street address (mapped from line1).
        /// </summary>
        public string Street
        {
            get => Line1 ?? string.Empty;
            set => Line1 = value;
        }

        /// <summary>
        /// Gets or sets the state or province (mapped from region).
        /// </summary>
        public string State
        {
            get => Region ?? string.Empty;
            set => Region = value;
        }
    }
}
