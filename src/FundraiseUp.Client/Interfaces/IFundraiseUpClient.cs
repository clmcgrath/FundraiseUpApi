using System;
using System.Threading.Tasks;
using FundraiseUp.Client.Models;
using FundraiseUp.Client.Requests;

namespace FundraiseUp.Client
{
    /// <summary>
    /// Interface for the main FundraiseUp API client.
    /// </summary>
    public interface IFundraiseUpClient : IDisposable
    {
        /// <summary>
        /// Gets the donation operations client.
        /// </summary>
        IDonationOperations Donations { get; }

        /// <summary>
        /// Gets the campaign operations client.
        /// </summary>
        ICampaignOperations Campaigns { get; }

        /// <summary>
        /// Gets the donor operations client.
        /// </summary>
        IDonorOperations Donors { get; }
    }

    /// <summary>
    /// Interface for donation operations.
    /// </summary>
    public interface IDonationOperations
    {
        /// <summary>
        /// Creates a new donation builder for the specified request.
        /// </summary>
        /// <param name="request">The donation creation request.</param>
        /// <returns>A donation operation builder.</returns>
        IDonationOperationBuilder<Donation> Create(CreateDonationRequest request);

        /// <summary>
        /// Gets a donation by its identifier.
        /// </summary>
        /// <param name="donationId">The donation identifier.</param>
        /// <returns>A donation operation builder.</returns>
        IDonationOperationBuilder<Donation> GetById(string donationId);

        /// <summary>
        /// Lists donations with optional filtering.
        /// </summary>
        /// <returns>A donation list operation builder.</returns>
        IDonationListOperationBuilder List();

        /// <summary>
        /// Updates an existing donation.
        /// </summary>
        /// <param name="donationId">The donation identifier.</param>
        /// <param name="request">The update request.</param>
        /// <returns>A donation operation builder.</returns>
        IDonationOperationBuilder<Donation> Update(string donationId, UpdateDonationRequest request);
    }

    /// <summary>
    /// Interface for campaign operations.
    /// </summary>
    public interface ICampaignOperations
    {
        /// <summary>
        /// Creates a new campaign builder for the specified request.
        /// </summary>
        /// <param name="request">The campaign creation request.</param>
        /// <returns>A campaign operation builder.</returns>
        ICampaignOperationBuilder<Campaign> Create(CreateCampaignRequest request);

        /// <summary>
        /// Gets a campaign by its identifier.
        /// </summary>
        /// <param name="campaignId">The campaign identifier.</param>
        /// <returns>A campaign operation builder.</returns>
        ICampaignOperationBuilder<Campaign> GetById(string campaignId);

        /// <summary>
        /// Lists campaigns with optional filtering.
        /// </summary>
        /// <returns>A campaign list operation builder.</returns>
        ICampaignListOperationBuilder List();

        /// <summary>
        /// Updates an existing campaign.
        /// </summary>
        /// <param name="campaignId">The campaign identifier.</param>
        /// <param name="request">The update request.</param>
        /// <returns>A campaign operation builder.</returns>
        ICampaignOperationBuilder<Campaign> Update(string campaignId, UpdateCampaignRequest request);

        /// <summary>
        /// Gets statistics for a campaign.
        /// </summary>
        /// <param name="campaignId">The campaign identifier.</param>
        /// <returns>A campaign statistics operation builder.</returns>
        ICampaignOperationBuilder<CampaignStatistics> GetStatistics(string campaignId);

        /// <summary>
        /// Activates a campaign.
        /// </summary>
        /// <param name="campaignId">The campaign identifier.</param>
        /// <returns>A campaign operation builder.</returns>
        ICampaignOperationBuilder<Campaign> Activate(string campaignId);
    }

    /// <summary>
    /// Interface for donor operations.
    /// </summary>
    public interface IDonorOperations
    {
        /// <summary>
        /// Creates a new donor builder for the specified request.
        /// </summary>
        /// <param name="request">The donor creation request.</param>
        /// <returns>A donor operation builder.</returns>
        IDonorOperationBuilder<Donor> Create(CreateDonorRequest request);

        /// <summary>
        /// Gets a donor by their identifier.
        /// </summary>
        /// <param name="donorId">The donor identifier.</param>
        /// <returns>A donor operation builder.</returns>
        IDonorOperationBuilder<Donor> GetById(string donorId);

        /// <summary>
        /// Searches for donors with optional filtering.
        /// </summary>
        /// <returns>A donor search operation builder.</returns>
        IDonorSearchOperationBuilder Search();

        /// <summary>
        /// Updates an existing donor.
        /// </summary>
        /// <param name="donorId">The donor identifier.</param>
        /// <param name="request">The update request.</param>
        /// <returns>A donor operation builder.</returns>
        IDonorOperationBuilder<Donor> Update(string donorId, UpdateDonorRequest request);

        /// <summary>
        /// Gets donations for a specific donor.
        /// </summary>
        /// <param name="donorId">The donor identifier.</param>
        /// <returns>A donation list operation builder.</returns>
        IDonationListOperationBuilder GetDonations(string donorId);

        /// <summary>
        /// Gets statistics for a donor.
        /// </summary>
        /// <param name="donorId">The donor identifier.</param>
        /// <returns>A donor statistics operation builder.</returns>
        IDonorOperationBuilder<DonorStatistics> GetStatistics(string donorId);

        /// <summary>
        /// Merges two donor records.
        /// </summary>
        /// <param name="primaryDonorId">The primary donor identifier.</param>
        /// <param name="duplicateDonorId">The duplicate donor identifier to merge.</param>
        /// <returns>A donor operation builder.</returns>
        IDonorOperationBuilder<Donor> Merge(string primaryDonorId, string duplicateDonorId);
    }
}