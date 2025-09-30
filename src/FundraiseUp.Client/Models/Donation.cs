using System;
using System.Collections.Generic;

namespace FundraiseUp.Client.Models
{
    /// <summary>
    /// Represents a donation in the FundraiseUp system.
    /// </summary>
    public class Donation
    {
        /// <summary>
        /// Gets or sets the unique identifier for the donation.
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the donation amount.
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Gets or sets the currency code (e.g., USD, EUR).
        /// </summary>
        public string Currency { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the donor's email address.
        /// </summary>
        public string DonorEmail { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the donor's unique identifier.
        /// </summary>
        public string DonorId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the campaign identifier associated with this donation.
        /// </summary>
        public string CampaignId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the current status of the donation.
        /// </summary>
        public DonationStatus Status { get; set; }

        /// <summary>
        /// Gets or sets additional notes for the donation.
        /// </summary>
        public string Notes { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets when the donation was created.
        /// </summary>
        public DateTimeOffset CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets when the donation was last updated.
        /// </summary>
        public DateTimeOffset UpdatedAt { get; set; }
    }

    /// <summary>
    /// Represents the status of a donation.
    /// </summary>
    public enum DonationStatus
    {
        /// <summary>
        /// The donation is pending processing.
        /// </summary>
        Pending,

        /// <summary>
        /// The donation has been completed successfully.
        /// </summary>
        Completed,

        /// <summary>
        /// The donation failed to process.
        /// </summary>
        Failed,

        /// <summary>
        /// The donation has been refunded.
        /// </summary>
        Refunded,

        /// <summary>
        /// The donation was cancelled.
        /// </summary>
        Cancelled
    }
}