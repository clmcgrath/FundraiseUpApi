using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using FundraiseUp.Client.Configuration;
using FundraiseUp.Client.Requests;
using FundraiseUp.Client.Tests.TestHelpers.Mocking;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace FundraiseUp.Client.Tests.Core
{
    /// <summary>
    /// Example showing improved mocking approach for contributors without API keys
    /// </summary>
    public class ImprovedMockingExampleTests
    {

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

            var httpMockSetup = new HttpMockBuilder();
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
                    Email = "" // Invalid: empty email
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
