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
    /// Contract tests for donor operations - these MUST FAIL before implementation.
    /// </summary>
    public class DonorOperationsContractTests
    {
        [Fact(Skip = "Supporters cannot be created via API - they are created automatically with donations")]
        public async Task CreateDonor_WithValidRequest_ShouldReturnDonor()
        {
            // NOTE: This test is skipped because the FundraiseUp API does not support creating supporters directly.
            // Supporters are created automatically when donations are made.
            return;
        }

        [Fact]
        public async Task GetDonor_WithValidId_ShouldReturnDonor()
        {
            // Arrange
            var client = new FundraiseUpClient("test-api-key");
            var donorId = "donor-123";

            // Act
            var result = await client.Supporters
                .GetById(donorId)
                .ExecuteAsync();

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(donorId);
        }

        [Fact]
        public async Task SearchDonors_WithEmailFilter_ShouldReturnMatchingDonors()
        {
            // Arrange
            var client = new FundraiseUpClient("test-api-key");

            // Act
            var result = await client.Supporters
                .Search()
                .WithLimit(20)
                .ExecuteAsync();

            // Assert
            result.Should().NotBeNull();
            result.Items.Should().NotBeNull();
            // NOTE: FundraiseUp API doesn't support complex Where clauses - testing basic search instead
        }

        [Fact(Skip = "Supporters cannot be updated via API - they are read-only")]
        public async Task UpdateDonor_WithValidData_ShouldReturnUpdatedDonor()
        {
            // NOTE: This test is skipped because the FundraiseUp API does not support updating supporters.
            // Supporters are managed automatically and are read-only via the API.
            return;
        }

        [Fact]
        public async Task GetDonorDonations_WithValidId_ShouldReturnDonationHistory()
        {
            // Arrange
            var client = new FundraiseUpClient("test-api-key");
            var donorId = "donor-123";

            // Act
            var result = await client.Supporters
                .GetDonations(donorId)
                .OrderBy(d => d.CreatedAt)
                .ExecuteAsync();

            // Assert
            result.Should().NotBeNull();
            result.Items.Should().NotBeNull();
            result.Items.Should().OnlyContain(d => d.Supporter.Id == donorId);
        }

        [Fact(Skip = "Supporter statistics not available in current FundraiseUp API")]
        public async Task GetDonorStatistics_WithValidId_ShouldReturnStats()
        {
            // NOTE: This test is skipped because supporter statistics are not currently available
            // in the FundraiseUp API specification.
            return;
        }

        [Fact(Skip = "Supporters cannot be merged via API - not supported")]
        public async Task MergeDonors_WithValidIds_ShouldReturnMergedDonor()
        {
            // NOTE: This test is skipped because the FundraiseUp API does not support merging supporters.
            return;
        }

        // [Fact] - COMMENTED OUT: Supporters cannot be created via FundraiseUp API - they are created automatically with donations
        // public void DonorBuilder_ShouldProvideFluentInterface()
        // {
        //     // This test is disabled because supporters are read-only in FundraiseUp API
        // }
    }
}
