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
    /// Contract tests for campaign operations - these MUST FAIL before implementation.
    /// </summary>
    public class CampaignOperationsContractTests
    {
        [Fact(Skip = "Campaigns cannot be created via API - they are read-only")]
        public async Task CreateCampaign_WithValidRequest_ShouldReturnCampaign()
        {
            // NOTE: This test is skipped because the FundraiseUp API does not support creating campaigns.
            // Campaigns are managed through the FundraiseUp dashboard and are read-only via the API.
            return;
            
            // Original test code preserved for reference:
            /*
            // Arrange
            var client = new FundraiseUpClient("test-api-key");
            var request = new CreateCampaignRequest
            {
                Name = "Test Campaign",
                GoalAmount = "10000.00",
                Currency = "USD",
                Description = "A test campaign for validation"
            };

            // Act
            var result = await client.Campaigns
                .Create(request)
                .ExecuteAsync();

            // Test content removed - see skipped comment above
            */
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
                .WithLimit(20)
                .ExecuteAsync();

            // Assert
            result.Should().NotBeNull();
            result.Items.Should().NotBeNull();
            result.PageSize.Should().Be(20);
        }

        [Fact(Skip = "Campaigns cannot be updated via API - they are read-only")]
        public async Task UpdateCampaign_WithValidData_ShouldReturnUpdatedCampaign()
        {
            // NOTE: This test is skipped because the FundraiseUp API does not support updating campaigns.
            // Campaigns are managed through the FundraiseUp dashboard and are read-only via the API.
            return;
        }

        [Fact(Skip = "Campaign statistics not available in current FundraiseUp API")]
        public async Task GetCampaignStatistics_WithValidId_ShouldReturnStats()
        {
            // NOTE: This test is skipped because campaign statistics are not currently available 
            // in the FundraiseUp API specification.
            return;
        }

        [Fact(Skip = "Campaigns cannot be activated via API - they are read-only")]
        public async Task ActivateCampaign_WithValidId_ShouldChangeCampaignStatus()
        {
            // NOTE: This test is skipped because the FundraiseUp API does not support activating campaigns.
            // Campaigns are managed through the FundraiseUp dashboard and are read-only via the API.
            return;
        }

        // [Fact] - COMMENTED OUT: Campaigns cannot be created via FundraiseUp API - they are managed through dashboard
        // public void CampaignBuilder_ShouldProvideFluentInterface()
        // {
        //     // This test is disabled because campaigns are read-only in FundraiseUp API
        // }
    }
}
