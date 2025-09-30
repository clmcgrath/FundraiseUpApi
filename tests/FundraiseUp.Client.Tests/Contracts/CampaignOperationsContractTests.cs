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
    /// Contract tests for campaign operations - these MUST FAIL before implementation.
    /// </summary>
    public class CampaignOperationsContractTests
    {
        [Fact]
        public async Task CreateCampaign_WithValidRequest_ShouldReturnCampaign()
        {
            // Arrange
            var client = new FundraiseUpClient("test-api-key");
            var request = new CreateCampaignRequest
            {
                Name = "Test Campaign",
                GoalAmount = 10000.00m,
                Currency = "USD",
                Description = "A test campaign for validation"
            };

            // Act
            var result = await client.Campaigns
                .Create(request)
                .ExecuteAsync();

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().NotBeNullOrEmpty();
            result.Name.Should().Be("Test Campaign");
            result.Goal.Should().Be(10000.00m);
            result.Currency.Should().Be("USD");
            result.Status.Should().Be(CampaignStatus.Draft);
        }

        [Fact]
        public async Task GetCampaign_WithValidId_ShouldReturnCampaign()
        {
            // Arrange
            var client = new FundraiseUpClient("test-api-key");
            var campaignId = "campaign-123";

            // Act
            var result = await client.Campaigns
                .GetById(campaignId)
                .ExecuteAsync();

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(campaignId);
        }

        [Fact]
        public async Task ListCampaigns_WithPagination_ShouldReturnPagedResults()
        {
            // Arrange
            var client = new FundraiseUpClient("test-api-key");

            // Act
            var result = await client.Campaigns
                .List()
                .Page(1, 20)
                .OrderBy(c => c.Name)
                .ExecuteAsync();

            // Assert
            result.Should().NotBeNull();
            result.Items.Should().NotBeNull();
            result.PageSize.Should().Be(20);
            result.CurrentPage.Should().Be(1);
        }

        [Fact]
        public async Task UpdateCampaign_WithValidData_ShouldReturnUpdatedCampaign()
        {
            // Arrange
            var client = new FundraiseUpClient("test-api-key");
            var campaignId = "campaign-123";
            var updateRequest = new UpdateCampaignRequest
            {
                Name = "Updated Campaign Name",
                GoalAmount = 15000.00m
            };

            // Act
            var result = await client.Campaigns
                .Update(campaignId, updateRequest)
                .ExecuteAsync();

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(campaignId);
            result.Name.Should().Be("Updated Campaign Name");
            result.Goal.Should().Be(15000.00m);
        }

        [Fact]
        public async Task GetCampaignStatistics_WithValidId_ShouldReturnStats()
        {
            // Arrange
            var client = new FundraiseUpClient("test-api-key");
            var campaignId = "campaign-123";

            // Act
            var result = await client.Campaigns
                .GetStatistics(campaignId)
                .ExecuteAsync();

            // Assert
            result.Should().NotBeNull();
            result.CampaignId.Should().Be(campaignId);
            result.TotalRaised.Should().BeGreaterOrEqualTo(0);
            result.DonationCount.Should().BeGreaterOrEqualTo(0);
            result.ProgressPercentage.Should().BeInRange(0, 100);
        }

        [Fact]
        public async Task ActivateCampaign_WithValidId_ShouldChangeCampaignStatus()
        {
            // Arrange
            var client = new FundraiseUpClient("test-api-key");
            var campaignId = "campaign-123";

            // Act
            var result = await client.Campaigns
                .Activate(campaignId)
                .ExecuteAsync();

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(campaignId);
            result.Status.Should().Be(CampaignStatus.Active);
        }

        [Fact]
        public void CampaignBuilder_ShouldProvideFluentInterface()
        {
            // Arrange
            var client = new FundraiseUpClient("test-api-key");

            // Act & Assert - Testing fluent interface compilation
            var builder = client.Campaigns
                .Create(new CreateCampaignRequest())
                .WithTimeout(TimeSpan.FromSeconds(45))
                .WithRetry(2)
                .WithCorrelationId("campaign-test-456");

            builder.Should().NotBeNull();
        }
    }
}