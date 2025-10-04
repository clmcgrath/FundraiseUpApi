using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace FundraiseUp.Client.Models
{
    /// <summary>
    /// Represents an event (audit log entry) response from the FundraiseUp API.
    /// Events track all significant actions and changes in the system.
    /// Based on the official FundraiseUp API specification.
    /// </summary>
    public class EventResponse
    {
        /// <summary>
        /// Unique identifier for the event (format: EVT[A-Z]{8}).
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Type of event that occurred (e.g., "donation.created", "recurring_plan.updated").
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;

        /// <summary>
        /// Timestamp when the event occurred in UTC.
        /// </summary>
        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Test mode indicator. True for live mode, false for test mode.
        /// </summary>
        [JsonPropertyName("livemode")]
        public bool LiveMode { get; set; }

        /// <summary>
        /// API version used when the event was created.
        /// </summary>
        [JsonPropertyName("api_version")]
        public string? ApiVersion { get; set; }

        /// <summary>
        /// Request ID that triggered this event (if applicable).
        /// </summary>
        [JsonPropertyName("request_id")]
        public string? RequestId { get; set; }

        /// <summary>
        /// Donation ID if this event is related to a donation.
        /// </summary>
        [JsonPropertyName("donation")]
        public string? Donation { get; set; }

        /// <summary>
        /// Recurring plan ID if this event is related to a recurring plan.
        /// </summary>
        [JsonPropertyName("recurring_plan")]
        public string? RecurringPlan { get; set; }

        /// <summary>
        /// Supporter ID if this event is related to a supporter.
        /// </summary>
        [JsonPropertyName("supporter")]
        public string? Supporter { get; set; }

        /// <summary>
        /// Campaign ID if this event is related to a campaign.
        /// </summary>
        [JsonPropertyName("campaign")]
        public string? Campaign { get; set; }

        /// <summary>
        /// Fundraiser ID if this event is related to a fundraiser.
        /// </summary>
        [JsonPropertyName("fundraiser")]
        public string? Fundraiser { get; set; }

        /// <summary>
        /// Organization account information.
        /// </summary>
        [JsonPropertyName("account")]
        public AccountResponse Account { get; set; } = new();

        /// <summary>
        /// The data object associated with this event.
        /// This contains the actual object that triggered the event.
        /// </summary>
        [JsonPropertyName("data")]
        public EventDataResponse? Data { get; set; }

        /// <summary>
        /// Previous values of changed fields (for update events).
        /// </summary>
        [JsonPropertyName("previous_attributes")]
        public Dictionary<string, object>? PreviousAttributes { get; set; }
    }

    /// <summary>
    /// Event data wrapper containing the object that triggered the event.
    /// </summary>
    public class EventDataResponse
    {
        /// <summary>
        /// The object that triggered the event.
        /// This could be a donation, recurring plan, supporter, etc.
        /// </summary>
        [JsonPropertyName("object")]
        public object? Object { get; set; }
    }

    /// <summary>
    /// Paginated response for events list.
    /// </summary>
    public class EventsResponse
    {
        /// <summary>
        /// Array of event records, paginated by request parameters.
        /// </summary>
        [JsonPropertyName("data")]
        public List<EventResponse> Data { get; set; } = new();

        /// <summary>
        /// Indicates whether there are more records available.
        /// </summary>
        [JsonPropertyName("has_more")]
        public bool HasMore { get; set; }
    }

    /// <summary>
    /// Common event types in the FundraiseUp system.
    /// </summary>
    public static class EventType
    {
        /// <summary>
        /// Donation events.
        /// </summary>
        public static class Donation
        {
            public const string Created = "donation.created";
            public const string Updated = "donation.updated";
            public const string Succeeded = "donation.succeeded";
            public const string Failed = "donation.failed";
            public const string Refunded = "donation.refunded";
            public const string Retrying = "donation.retrying";
            public const string Scheduled = "donation.scheduled";
        }

        /// <summary>
        /// Recurring plan events.
        /// </summary>
        public static class RecurringPlan
        {
            public const string Created = "recurring_plan.created";
            public const string Updated = "recurring_plan.updated";
            public const string Activated = "recurring_plan.activated";
            public const string Paused = "recurring_plan.paused";
            public const string Resumed = "recurring_plan.resumed";
            public const string Canceled = "recurring_plan.canceled";
            public const string Completed = "recurring_plan.completed";
            public const string Failed = "recurring_plan.failed";
        }

        /// <summary>
        /// Supporter events.
        /// </summary>
        public static class Supporter
        {
            public const string Created = "supporter.created";
            public const string Updated = "supporter.updated";
            public const string Merged = "supporter.merged";
        }

        /// <summary>
        /// Campaign events.
        /// </summary>
        public static class Campaign
        {
            public const string Created = "campaign.created";
            public const string Updated = "campaign.updated";
            public const string Activated = "campaign.activated";
            public const string Deactivated = "campaign.deactivated";
        }

        /// <summary>
        /// Fundraiser events.
        /// </summary>
        public static class Fundraiser
        {
            public const string Created = "fundraiser.created";
            public const string Updated = "fundraiser.updated";
            public const string Activated = "fundraiser.activated";
            public const string Deactivated = "fundraiser.deactivated";
        }

        /// <summary>
        /// Payout events.
        /// </summary>
        public static class Payout
        {
            public const string Created = "payout.created";
            public const string Paid = "payout.paid";
            public const string Failed = "payout.failed";
        }
    }
}
