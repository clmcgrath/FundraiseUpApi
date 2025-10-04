using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace FundraiseUp.Client.Models
{
    /// <summary>
    /// Represents supporter information from the FundraiseUp API.
    /// Supporters are read-only entities and cannot be created or modified through the API.
    /// They are created automatically when donations are made.
    /// </summary>
    public class SupporterResponse
    {
        /// <summary>
        /// Unique identifier of the supporter (format: S[A-Z]{8}).
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Supporter's email address. Maximum 254 characters.
        /// </summary>
        [JsonPropertyName("email")]
        public string? Email { get; set; }

        /// <summary>
        /// Supporter's first name. Maximum 256 characters.
        /// </summary>
        [JsonPropertyName("first_name")]
        public string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// Supporter's last name. Maximum 256 characters.
        /// </summary>
        [JsonPropertyName("last_name")]
        public string LastName { get; set; } = string.Empty;

        /// <summary>
        /// Supporter's phone number. Maximum 20 characters.
        /// </summary>
        [JsonPropertyName("phone")]
        public string? Phone { get; set; }

        /// <summary>
        /// Supporter's title (e.g., "mr", "mrs", "ms").
        /// </summary>
        [JsonPropertyName("title")]
        public string? Title { get; set; }

        /// <summary>
        /// Supporter's preferred language (e.g., "en-US", "fr-FR").
        /// </summary>
        [JsonPropertyName("language")]
        public string? Language { get; set; }

        /// <summary>
        /// Supporter's mailing address.
        /// </summary>
        [JsonPropertyName("address")]
        public Address? Address { get; set; }

        /// <summary>
        /// Supporter's employer information.
        /// </summary>
        [JsonPropertyName("employer")]
        public EmployerResponse? Employer { get; set; }

        /// <summary>
        /// When the supporter was created (UTC).
        /// </summary>
        [JsonPropertyName("created_at")]
        public DateTimeOffset CreatedAt { get; set; }

        /// <summary>
        /// When the supporter information was last updated (UTC).
        /// </summary>
        [JsonPropertyName("updated_at")]
        public DateTimeOffset UpdatedAt { get; set; }

        /// <summary>
        /// Indicates if this is live mode data or test mode.
        /// </summary>
        [JsonPropertyName("live_mode")]
        public bool LiveMode { get; set; }

        /// <summary>
        /// Organization account information.
        /// </summary>
        [JsonPropertyName("account")]
        public AccountResponse Account { get; set; } = new();

        /// <summary>
        /// Custom fields associated with the supporter.
        /// </summary>
        [JsonPropertyName("custom_fields")]
        public List<CustomFieldResponse> CustomFields { get; set; } = new();

        /// <summary>
        /// Communication consent details for the supporter.
        /// </summary>
        [JsonPropertyName("consent")]
        public object? Consent { get; set; }
    }

    /// <summary>
    /// Collection response for supporters with cursor-based pagination.
    /// </summary>
    public class SupportersResponse
    {
        /// <summary>
        /// List of supporters.
        /// </summary>
        [JsonPropertyName("data")]
        public List<SupporterResponse> Data { get; set; } = new();

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
}
