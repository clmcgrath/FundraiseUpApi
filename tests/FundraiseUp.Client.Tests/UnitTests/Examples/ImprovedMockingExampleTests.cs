using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using FundraiseUp.Client.Configuration;
using FundraiseUp.Client.Requests;
using FundraiseUp.Client.Tests.TestHelpers;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace FundraiseUp.Client.Tests.UnitTests.Examples
{
    /// <summary>
    /// Example showing improved mocking approach for contributors without API keys
    /// </summary>
    public class ImprovedMockingExampleTests
    {
        // [Fact] - COMMENTED OUT: Campaigns cannot be created via FundraiseUp API - they are managed through dashboard
        // public async Task CreateCampaign_WithImprovedMocking_ShouldReturnCampaign()
        // {
        //     // This test is disabled because campaigns are read-only in FundraiseUp API
        // }

        [Fact]
        public async Task ListCampaigns_WithPagination_ShouldReturnPagedResults()
        {
            // Arrange - Mock paginated response
            var campaigns = new[]
            {
                MockResponseBuilder.CreateSampleCampaign("campaign-1"),
                MockResponseBuilder.CreateSampleCampaign("campaign-2")
            };

            var mockResponse = MockResponseBuilder.CreatePaginatedResponse(
                campaigns,
                page: 1,
                pageSize: 10,
                totalCount: 25
            );

            var httpMockSetup = new HttpClientMockSetup();
            httpMockSetup.SetupRequest(HttpMethod.Get, "/campaigns", mockResponse);

            var httpClient = httpMockSetup.CreateHttpClient();
            var logger = new Mock<ILogger<FundraiseUpClient>>();

            var client = new FundraiseUpClient("test-api-key", new FundraiseUpClientOptions
            {
                BaseUrl = "https://api.test.com"
            }, httpClient, logger.Object);

            // Act
            var result = await client.Campaigns
                .List()
                .WithLimit(10)
                .ExecuteAsync();

            // Assert
            result.Should().NotBeNull();
            result.Items.Should().HaveCount(2);
            result.TotalCount.Should().Be(25);
            result.CurrentPage.Should().Be(1);
            result.PageSize.Should().Be(10);
        }

        [Fact]
        public async Task CreateDonation_WithValidationError_ShouldThrowValidationException()
        {
            // Arrange - Mock validation error response
            var validationErrors = new System.Collections.Generic.Dictionary<string, string[]>
            {
                ["amount"] = new[] { "Amount must be greater than 0" },
                ["donor_email"] = new[] { "Donor email is required" }
            };

            var mockResponse = MockResponseBuilder.CreateValidationErrorResponse(validationErrors);

            var httpMockSetup = new HttpClientMockSetup();
            httpMockSetup.SetupRequest(HttpMethod.Post, "/donations", mockResponse);

            var httpClient = httpMockSetup.CreateHttpClient();
            var logger = new Mock<ILogger<FundraiseUpClient>>();

            var client = new FundraiseUpClient("test-api-key", new FundraiseUpClientOptions
            {
                BaseUrl = "https://api.test.com"
            }, httpClient, logger.Object);

            var request = new CreateDonationRequest
            {
                Campaign = "campaign-123",
                Supporter = new SupporterRequest
                {
                    FirstName = "Test",
                    LastName = "User",
                    Email = "" // Invalid - empty
                },
                Amount = "-10", // Invalid - negative
                Currency = "USD",
                Designation = "EXXXXXXX",
                PaymentMethod = new PaymentMethodRequest
                {
                    Stripe = new StripePaymentMethodRequest { Id = "pm_card_visa" }
                }
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<FundraiseUp.Client.Exceptions.FundraiseUpApiException>(
                () => client.Donations.Create(request).ExecuteAsync()
            );

            exception.Message.Should().Contain("Validation failed");
            exception.Message.Should().Contain("amount");
            exception.Message.Should().Contain("donor_email");
        }
    }
}
