using System;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using FundraiseUp.Client;
using FundraiseUp.Client.Models;
using FundraiseUp.Client.Requests;

namespace FundraiseUp.Client.Tests.Contracts
{
    /// <summary>
    /// Contract tests for donation operations - these MUST FAIL before implementation.
    /// </summary>
    public class DonationOperationsContractTests
    {
        [Fact]
        public async Task CreateDonation_WithValidRequest_ShouldReturnDonation()
        {
            // Arrange
            var client = new FundraiseUpClient("test-api-key");
            var request = new CreateDonationRequest
            {
                Amount = 100.00m,
                Currency = "USD",
                DonorEmail = "test@example.com",
                CampaignId = "campaign-123"
            };

            // Act
            var result = await client.Donations
                .Create(request)
                .ExecuteAsync();

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().NotBeNullOrEmpty();
            result.Amount.Should().Be(100.00m);
            result.Currency.Should().Be("USD");
            result.Status.Should().Be(DonationStatus.Pending);
        }

        [Fact]
        public async Task CreateDonation_WithFluentConfiguration_ShouldApplySettings()
        {
            // Arrange
            var client = new FundraiseUpClient("test-api-key");
            var request = new CreateDonationRequest
            {
                Amount = 50.00m,
                Currency = "USD",
                DonorEmail = "donor@example.com",
                CampaignId = "campaign-456"
            };

            // Act
            var result = await client.Donations
                .Create(request)
                .WithTimeout(TimeSpan.FromSeconds(30))
                .WithRetry(3)
                .WithCorrelationId("test-correlation-123")
                .ExecuteAsync();

            // Assert
            result.Should().NotBeNull();
            result.Amount.Should().Be(50.00m);
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
                .Where(d => d.Amount > 25.00m)
                .OrderBy(d => d.CreatedAt)
                .Take(10)
                .ExecuteAsync();

            // Assert
            result.Should().NotBeNull();
            result.Items.Should().NotBeNull();
            result.TotalCount.Should().BeGreaterOrEqualTo(0);
        }

        [Fact]
        public async Task UpdateDonation_WithValidData_ShouldReturnUpdatedDonation()
        {
            // Arrange
            var client = new FundraiseUpClient("test-api-key");
            var donationId = "donation-123";
            var updateRequest = new UpdateDonationRequest
            {
                Message = "Updated donation notes"
            };

            // Act
            var result = await client.Donations
                .Update(donationId, updateRequest)
                .ExecuteAsync();

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(donationId);
            result.Notes.Should().Be("Updated donation notes");
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