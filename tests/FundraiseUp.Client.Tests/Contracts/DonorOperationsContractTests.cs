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
    /// Contract tests for donor operations - these MUST FAIL before implementation.
    /// </summary>
    public class DonorOperationsContractTests
    {
        [Fact]
        public async Task CreateDonor_WithValidRequest_ShouldReturnDonor()
        {
            // Arrange
            var client = new FundraiseUpClient("test-api-key");
            var request = new CreateDonorRequest
            {
                Email = "donor@example.com",
                FirstName = "John",
                LastName = "Doe",
                Phone = "+1-555-0123"
            };

            // Act
            var result = await client.Donors
                .Create(request)
                .ExecuteAsync();

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().NotBeNullOrEmpty();
            result.Email.Should().Be("donor@example.com");
            result.FirstName.Should().Be("John");
            result.LastName.Should().Be("Doe");
            result.Phone.Should().Be("+1-555-0123");
        }

        [Fact]
        public async Task GetDonor_WithValidId_ShouldReturnDonor()
        {
            // Arrange
            var client = new FundraiseUpClient("test-api-key");
            var donorId = "donor-123";

            // Act
            var result = await client.Donors
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
            var email = "test@example.com";

            // Act
            var result = await client.Donors
                .Search()
                .Where(d => d.Email == email)
                .ExecuteAsync();

            // Assert
            result.Should().NotBeNull();
            result.Items.Should().NotBeNull();
            result.Items.Should().OnlyContain(d => d.Email == email);
        }

        [Fact]
        public async Task UpdateDonor_WithValidData_ShouldReturnUpdatedDonor()
        {
            // Arrange
            var client = new FundraiseUpClient("test-api-key");
            var donorId = "donor-123";
            var updateRequest = new UpdateDonorRequest
            {
                FirstName = "Jane",
                LastName = "Smith",
                Phone = "+1-555-9876"
            };

            // Act
            var result = await client.Donors
                .Update(donorId, updateRequest)
                .ExecuteAsync();

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(donorId);
            result.FirstName.Should().Be("Jane");
            result.LastName.Should().Be("Smith");
            result.Phone.Should().Be("+1-555-9876");
        }

        [Fact]
        public async Task GetDonorDonations_WithValidId_ShouldReturnDonationHistory()
        {
            // Arrange
            var client = new FundraiseUpClient("test-api-key");
            var donorId = "donor-123";

            // Act
            var result = await client.Donors
                .GetDonations(donorId)
                .OrderBy(d => d.CreatedAt)
                .ExecuteAsync();

            // Assert
            result.Should().NotBeNull();
            result.Items.Should().NotBeNull();
            result.Items.Should().OnlyContain(d => d.DonorId == donorId);
        }

        [Fact]
        public async Task GetDonorStatistics_WithValidId_ShouldReturnStats()
        {
            // Arrange
            var client = new FundraiseUpClient("test-api-key");
            var donorId = "donor-123";

            // Act
            var result = await client.Donors
                .GetStatistics(donorId)
                .ExecuteAsync();

            // Assert
            result.Should().NotBeNull();
            result.DonorId.Should().Be(donorId);
            result.TotalDonated.Should().BeGreaterOrEqualTo(0);
            result.DonationCount.Should().BeGreaterOrEqualTo(0);
            result.FirstDonationDate.Should().BeOnOrBefore(DateTimeOffset.UtcNow);
        }

        [Fact]
        public async Task MergeDonors_WithValidIds_ShouldReturnMergedDonor()
        {
            // Arrange
            var client = new FundraiseUpClient("test-api-key");
            var primaryDonorId = "donor-123";
            var duplicateDonorId = "donor-456";

            // Act
            var result = await client.Donors
                .Merge(primaryDonorId, duplicateDonorId)
                .ExecuteAsync();

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(primaryDonorId);
            result.MergedFromIds.Should().Contain(duplicateDonorId);
        }

        [Fact]
        public void DonorBuilder_ShouldProvideFluentInterface()
        {
            // Arrange
            var client = new FundraiseUpClient("test-api-key");

            // Act & Assert - Testing fluent interface compilation
            var builder = client.Donors
                .Create(new CreateDonorRequest())
                .WithTimeout(TimeSpan.FromSeconds(20))
                .WithRetry(2)
                .WithCorrelationId("donor-test-789");

            builder.Should().NotBeNull();
        }
    }
}