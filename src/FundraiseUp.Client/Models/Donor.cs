using System;
using System.Collections.Generic;

namespace FundraiseUp.Client.Models
{
    /// <summary>
    /// Represents a donor in the FundraiseUp system.
    /// </summary>
    public class Donor
    {
        /// <summary>
        /// Gets or sets the unique identifier for the donor.
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the donor's email address.
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the donor's first name.
        /// </summary>
        public string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the donor's last name.
        /// </summary>
        public string LastName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the donor's phone number.
        /// </summary>
        public string Phone { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the donor's address.
        /// </summary>
        public Address? Address { get; set; }

        /// <summary>
        /// Gets or sets when the donor record was created.
        /// </summary>
        public DateTimeOffset CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets when the donor record was last updated.
        /// </summary>
        public DateTimeOffset UpdatedAt { get; set; }

        /// <summary>
        /// Gets or sets the list of donor IDs that were merged into this donor.
        /// </summary>
        public List<string> MergedFromIds { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets additional metadata for the donor.
        /// </summary>
        public Dictionary<string, object> Metadata { get; set; } = new Dictionary<string, object>();
    }

    /// <summary>
    /// Represents donor statistics.
    /// </summary>
    public class DonorStatistics
    {
        /// <summary>
        /// Gets or sets the donor identifier.
        /// </summary>
        public string DonorId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the total amount donated by this donor.
        /// </summary>
        public decimal TotalDonated { get; set; }

        /// <summary>
        /// Gets or sets the total number of donations made.
        /// </summary>
        public int DonationCount { get; set; }

        /// <summary>
        /// Gets or sets the date of the first donation.
        /// </summary>
        public DateTimeOffset FirstDonationDate { get; set; }

        /// <summary>
        /// Gets or sets the date of the last donation.
        /// </summary>
        public DateTimeOffset LastDonationDate { get; set; }

        /// <summary>
        /// Gets or sets the average donation amount.
        /// </summary>
        public decimal AverageDonation { get; set; }

        /// <summary>
        /// Gets or sets when the statistics were last updated.
        /// </summary>
        public DateTimeOffset LastUpdated { get; set; }
    }

    /// <summary>
    /// Represents an address.
    /// </summary>
    public class Address
    {
        /// <summary>
        /// Gets or sets the street address.
        /// </summary>
        public string Street { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the city.
        /// </summary>
        public string City { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the state or province.
        /// </summary>
        public string State { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the postal code.
        /// </summary>
        public string PostalCode { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the country code.
        /// </summary>
        public string Country { get; set; } = string.Empty;
    }
}