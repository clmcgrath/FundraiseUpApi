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
        /// Gets the supporter operations client.
        /// </summary>
        ISupporterOperations Supporters { get; }

        /// <summary>
        /// Gets the recurring plan operations client.
        /// </summary>
        IRecurringPlanOperations RecurringPlans { get; }

        /// <summary>
        /// Gets the events (audit log) operations client.
        /// </summary>
        IEventOperations Events { get; }

        /// <summary>
        /// Gets the fundraiser operations client.
        /// </summary>
        IFundraiserOperations Fundraisers { get; }

        /// <summary>
        /// Gets the donor portal operations client.
        /// </summary>
        IDonorPortalOperations DonorPortal { get; }
    }

    /// <summary>
    /// Interface for donation operations in FundraiseUp API.
    /// </summary>
    public interface IDonationOperations
    {
        /// <summary>
        /// Creates a new donation builder for the specified request.
        /// </summary>
        /// <param name="request">The donation creation request.</param>
        /// <returns>A donation operation builder.</returns>
        IDonationOperationBuilder<DonationResponse> Create(CreateDonationRequest request);

        /// <summary>
        /// Gets a donation by its identifier.
        /// </summary>
        /// <param name="donationId">The donation identifier (format: D[A-Z]{8}).</param>
        /// <returns>A donation operation builder.</returns>
        IDonationOperationBuilder<DonationResponse> GetById(string donationId);

        /// <summary>
        /// Lists donations with optional filtering using cursor-based pagination.
        /// </summary>
        /// <returns>A donation list operation builder.</returns>
        IDonationListOperationBuilder List();

        /// <summary>
        /// Updates an existing donation. Only works for API-created donations within 24 hours of creation.
        /// </summary>
        /// <param name="donationId">The donation identifier (format: D[A-Z]{8}).</param>
        /// <param name="request">The donation update request.</param>
        /// <returns>A donation operation builder.</returns>
        IDonationOperationBuilder<DonationResponse> Update(string donationId, UpdateDonationRequest request);
    }

    /// <summary>
    /// Interface for campaign operations in FundraiseUp API.
    /// Note: FundraiseUp API does not provide direct campaign endpoints.
    /// Campaign data is only available embedded within donation, supporter, and other entity responses.
    /// Campaign management must be done through the FundraiseUp Dashboard.
    /// </summary>
    public interface ICampaignOperations
    {
        // No operations available - campaigns are only accessible as embedded data
        // within other API responses (donations, supporters, etc.)
    }

    /// <summary>
    /// Interface for supporter operations in FundraiseUp API.
    /// Note: Supporters are read-only in the FundraiseUp API and cannot be created or modified directly.
    /// They are created automatically when donations are made.
    /// </summary>
    [Obsolete("Use ISupporterOperations for new code. This interface is maintained for backward compatibility.")]
    public interface IDonorOperations
    {
        /// <summary>
        /// Gets a supporter by their identifier.
        /// </summary>
        /// <param name="supporterId">The supporter identifier (format: S[A-Z]{8}).</param>
        /// <returns>A supporter operation builder.</returns>
        IDonorOperationBuilder<Donor> GetById(string supporterId);

        /// <summary>
        /// Lists supporters with optional filtering using cursor-based pagination.
        /// </summary>
        /// <returns>A supporter list operation builder.</returns>
        IDonorSearchOperationBuilder Search();

        /// <summary>
        /// Gets donations for a specific supporter.
        /// </summary>
        /// <param name="supporterId">The supporter identifier.</param>
        /// <returns>A donation list operation builder.</returns>
        IDonationListOperationBuilder GetDonations(string supporterId);
    }

    /// <summary>
    /// Interface for supporter operations in FundraiseUp API.
    /// Note: Supporters are read-only in the FundraiseUp API and cannot be created or modified directly.
    /// They are created automatically when donations are made.
    /// </summary>
    public interface ISupporterOperations
    {
        /// <summary>
        /// Gets a supporter by their identifier.
        /// </summary>
        /// <param name="supporterId">The supporter identifier (format: S[A-Z]{8}).</param>
        /// <returns>A supporter operation builder.</returns>
        ISupporterOperationBuilder<SupporterResponse> GetById(string supporterId);

        /// <summary>
        /// Lists supporters with optional filtering using cursor-based pagination.
        /// </summary>
        /// <returns>A supporter list operation builder.</returns>
        ISupporterSearchOperationBuilder Search();

        /// <summary>
        /// Gets donations for a specific supporter.
        /// </summary>
        /// <param name="supporterId">The supporter identifier.</param>
        /// <returns>A donation list operation builder.</returns>
        IDonationListOperationBuilder GetDonations(string supporterId);
    }

    /// <summary>
    /// Interface for recurring plan operations in FundraiseUp API.
    /// Note: Recurring plans are read-only in the FundraiseUp API and cannot be created or modified directly.
    /// They are created automatically when donations include recurring plan parameters.
    /// </summary>
    public interface IRecurringPlanOperations
    {
        /// <summary>
        /// Gets a recurring plan by its identifier.
        /// </summary>
        /// <param name="recurringPlanId">The recurring plan identifier (format: R[A-Z]{8}).</param>
        /// <returns>A recurring plan operation builder.</returns>
        IRecurringPlanOperationBuilder<RecurringPlanResponse> GetById(string recurringPlanId);

        /// <summary>
        /// Lists recurring plans with optional filtering using cursor-based pagination.
        /// </summary>
        /// <returns>A recurring plan search operation builder.</returns>
        IRecurringPlanSearchOperationBuilder Search();
    }

    /// <summary>
    /// Interface for event operations in FundraiseUp API.
    /// Events are read-only audit log records of all significant actions in the system.
    /// </summary>
    public interface IEventOperations
    {
        /// <summary>
        /// Gets an event by its identifier.
        /// </summary>
        /// <param name="eventId">The event identifier (format: EVT[A-Z]{8}).</param>
        /// <returns>An event operation builder.</returns>
        IEventOperationBuilder<EventResponse> GetById(string eventId);

        /// <summary>
        /// Lists events with optional filtering using cursor-based pagination.
        /// </summary>
        /// <returns>An event search operation builder.</returns>
        IEventSearchOperationBuilder Search();
    }

    /// <summary>
    /// Interface for fundraiser operations in FundraiseUp API.
    /// Fundraisers can be created, updated, and retrieved through these operations.
    /// </summary>
    public interface IFundraiserOperations
    {
        /// <summary>
        /// Creates a new fundraiser.
        /// </summary>
        /// <param name="request">The fundraiser creation request.</param>
        /// <returns>A fundraiser operation builder.</returns>
        IFundraiserOperationBuilder<FundraiserResponse> Create(CreateFundraiserRequest request);

        /// <summary>
        /// Gets a fundraiser by its identifier.
        /// </summary>
        /// <param name="fundraiserId">The fundraiser identifier (format: FND[A-Z]{8}).</param>
        /// <returns>A fundraiser operation builder.</returns>
        IFundraiserOperationBuilder<FundraiserResponse> GetById(string fundraiserId);

        /// <summary>
        /// Updates an existing fundraiser.
        /// </summary>
        /// <param name="fundraiserId">The fundraiser identifier.</param>
        /// <param name="request">The fundraiser update request.</param>
        /// <returns>A fundraiser operation builder.</returns>
        IFundraiserOperationBuilder<FundraiserResponse> Update(string fundraiserId, UpdateFundraiserRequest request);

        /// <summary>
        /// Lists fundraisers with optional filtering using cursor-based pagination.
        /// </summary>
        /// <returns>A fundraiser search operation builder.</returns>
        IFundraiserSearchOperationBuilder Search();
    }
}
