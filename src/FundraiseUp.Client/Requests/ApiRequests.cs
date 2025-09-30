using System.ComponentModel.DataAnnotations;
using FundraiseUp.Client.Models;

namespace FundraiseUp.Client.Requests
{
    public class CreateDonationRequest
    {
        [Required]
        public decimal Amount { get; set; }

        [Required]
        public string Currency { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string DonorEmail { get; set; } = string.Empty;

        [Required]
        public string CampaignId { get; set; } = string.Empty;

        public string? DonorName { get; set; }
        public string? Message { get; set; }
        public bool IsAnonymous { get; set; }
    }

    public class UpdateDonationRequest
    {
        public decimal? Amount { get; set; }
        public string? Currency { get; set; }
        public string? DonorName { get; set; }
        public string? Message { get; set; }
        public bool? IsAnonymous { get; set; }
    }

    public class CreateCampaignRequest
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        public decimal GoalAmount { get; set; }

        [Required]
        public string Currency { get; set; } = string.Empty;

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class UpdateCampaignRequest
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal? GoalAmount { get; set; }
        public string? Currency { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool? IsActive { get; set; }
    }

    public class CreateDonorRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;

        public string? Phone { get; set; }
        public Address? Address { get; set; }
    }

    public class UpdateDonorRequest
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Phone { get; set; }
        public Address? Address { get; set; }
    }

    public class MergeDonorsRequest
    {
        [Required]
        public string PrimaryDonorId { get; set; } = string.Empty;

        [Required]
        public string SecondaryDonorId { get; set; } = string.Empty;

        public bool PreserveSecondaryDonorData { get; set; } = true;
    }
}