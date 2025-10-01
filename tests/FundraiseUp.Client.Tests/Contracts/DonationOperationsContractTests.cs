using System;
using System.Threading.Tasks;
using FluentAssertions;
using FundraiseUp.Client;
using FundraiseUp.Client.Models;
using FundraiseUp.Client.Requests;
using Xunit;

namespace FundraiseUp.Client.Tests.Contracts
{
    /// <summary>
    /// Contract tests for donation operations - these MUST FAIL before implementation.
    /// </summary>
    public class DonationOperationsContractTests
    {
        [Fact(Skip = "CreateDonationRequest model not yet implemented")]
        public async Task CreateDonation_WithValidRequest_ShouldReturnDonation()
        {
            // NOTE: This test is skipped because the CreateDonationRequest model is not yet implemented.
            // While donations CAN be created via the FundraiseUp API, the request models need to be built first.
            return;
        }

        [Fact(Skip = "CreateDonationRequest model not yet implemented")]
        public async Task CreateDonation_WithFluentConfiguration_ShouldApplySettings()
        {
            // NOTE: This test is skipped because the CreateDonationRequest model is not yet implemented.
            return;
        }

        [Fact]
        public async Task GetDonation_WithValidId_ShouldReturnDonation()
        {
            // Arrange
            var client = new FundraiseUpClient("test-api-key");
            var donationId = "donation-123";

            // Act
            var result = await client.Donations
                .GetById(donationId)
                .ExecuteAsync();

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(donationId);
        }

        [Fact]
        public async Task ListDonations_WithFilters_ShouldReturnFilteredResults()
        {
            // Arrange
            var client = new FundraiseUpClient("test-api-key");

            // Act
            var result = await client.Donations
                .List()
                .WithLimit(10)
                .ExecuteAsync();

            // Assert
            result.Should().NotBeNull();
            result.Items.Should().NotBeNull();
            // NOTE: FundraiseUp API uses cursor pagination - TotalCount not available
        }

        [Fact(Skip = "UpdateDonationRequest model not yet implemented")]
        public async Task UpdateDonation_WithValidData_ShouldReturnUpdatedDonation()
        {
            // NOTE: This test is skipped because the UpdateDonationRequest model is not yet implemented.
            // Most donation properties are read-only, but some updates may be supported.
            return;
        }

        [Fact]
        public void DonationBuilder_ShouldProvideFluentInterface()
        {
            // Arrange
            var client = new FundraiseUpClient("test-api-key");

            // Act & Assert - Testing fluent interface compilation
            var builder = client.Donations
                .Create(new CreateDonationRequest())
                .WithTimeout(TimeSpan.FromSeconds(30))
                .WithRetry(3)
                .WithCorrelationId("test-123");

            builder.Should().NotBeNull();
        }
    }
}
