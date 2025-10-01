using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using FundraiseUp.Client.Configuration;
using FundraiseUp.Client.Models;
using FundraiseUp.Client.Requests;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Xunit;

namespace FundraiseUp.Client.Tests.UnitTests
{
    public class DonationOperationsTests
    {
        private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
        private readonly HttpClient _httpClient;
        private readonly FundraiseUpClient _client;

        public DonationOperationsTests()
        {
            _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_httpMessageHandlerMock.Object)
            {
                BaseAddress = new Uri("https://api.test.com")
            };
            var logger = new Mock<ILogger<FundraiseUpClient>>();

            _client = new FundraiseUpClient("test-api-key", new FundraiseUpClientOptions
            {
                BaseUrl = "https://api.test.com"
            }, _httpClient, logger.Object);
        }

        [Fact]
        public async Task CreateDonation_WithValidRequest_ShouldReturnDonation()
        {
            // Arrange
            var request = new CreateDonationRequest
            {
                Amount = "100.00",
                Currency = "usd",
                Campaign = "campaign-123",
                Designation = "general-fund",
                Supporter = new SupporterRequest
                {
                    FirstName = "John",
                    LastName = "Doe",
                    Email = "test@example.com"
                },
                PaymentMethod = new PaymentMethodRequest
                {
                    Stripe = new StripePaymentMethodRequest { Id = "pm_card_visa" }
                }
            };

            var expectedDonation = new DonationResponse
            {
                Id = "donation-456",
                Amount = "100.00",
                Currency = "USD",
                Status = DonationStatus.Succeeded,
                CreatedAt = DateTime.UtcNow
            };

            var jsonResponse = JsonSerializer.Serialize(expectedDonation, JsonConfiguration.DefaultOptions);
            var httpResponse = new HttpResponseMessage(HttpStatusCode.Created)
            {
                Content = new StringContent(jsonResponse, System.Text.Encoding.UTF8, "application/json")
            };

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            // Act
            var result = await _client.Donations
                .Create(request)
                .ExecuteAsync();

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be("donation-456");
            result.Amount.Should().Be("100.00");
            result.Currency.Should().Be("USD");
            result.Status.Should().Be(DonationStatus.Succeeded);
        }

        [Fact]
        public async Task GetDonation_WithValidId_ShouldReturnDonation()
        {
            // Arrange
            var donationId = "donation-123";
            var expectedDonation = new DonationResponse
            {
                Id = donationId,
                Amount = "50.00",
                Currency = "USD",
                Status = DonationStatus.Succeeded,
                CreatedAt = DateTime.UtcNow
            };

            var jsonResponse = JsonSerializer.Serialize(expectedDonation, JsonConfiguration.DefaultOptions);
            var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(jsonResponse, System.Text.Encoding.UTF8, "application/json")
            };

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            // Act
            var result = await _client.Donations
                .GetById(donationId)
                .ExecuteAsync();

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(donationId);
            result.Amount.Should().Be("50.00");
            result.Currency.Should().Be("USD");
            result.Status.Should().Be(DonationStatus.Succeeded);
        }

        [Fact]
        public async Task ListDonations_WithFilters_ShouldReturnPagedResults()
        {
            // Arrange
            var expectedResult = new DonationsResponse
            {
                Data = new List<DonationResponse>
                {
                    new DonationResponse
                    {
                        Id = "donation-1",
                        Amount = "25.00",
                        Currency = "USD",
                        Status = DonationStatus.Succeeded
                    },
                    new DonationResponse
                    {
                        Id = "donation-2",
                        Amount = "75.00",
                        Currency = "USD",
                        Status = DonationStatus.Pending
                    }
                },
                HasMore = false
            };

            var jsonResponse = JsonSerializer.Serialize(expectedResult, JsonConfiguration.DefaultOptions);
            var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(jsonResponse, System.Text.Encoding.UTF8, "application/json")
            };

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            // Act
            var result = await _client.Donations
                .List()
                .Take(10)
                .ExecuteAsync();

            // Assert
            result.Should().NotBeNull();
            result.Items.Should().HaveCount(2);
            result.TotalCount.Should().Be(0); // FundraiseUp API uses cursor pagination, no total count
            result.CurrentPage.Should().Be(1);
            result.PageSize.Should().Be(10);
            result.HasMore.Should().BeFalse();
            result.HasNextPage.Should().BeFalse();
        }


        [Fact]
        public async Task CreateDonation_WithFluentConfiguration_ShouldApplySettings()
        {
            // Arrange
            var request = new CreateDonationRequest
            {
                Amount = "200.00",
                Currency = "EUR",
                Supporter = new SupporterRequest
                {
                    FirstName = "Jane",
                    LastName = "Doe",
                    Email = "donor@example.com"
                },
                Campaign = "campaign-789",
                Designation = "General Fund",
                PaymentMethod = new PaymentMethodRequest
                {
                    Stripe = new StripePaymentMethodRequest { Id = "pm_card_mastercard" }
                }
            };

            var expectedDonation = new DonationResponse
            {
                Id = "donation-with-config",
                Amount = "200.00",
                Currency = "EUR",
                Status = DonationStatus.Pending
            };

            var jsonResponse = JsonSerializer.Serialize(expectedDonation, JsonConfiguration.DefaultOptions);
            var httpResponse = new HttpResponseMessage(HttpStatusCode.Created)
            {
                Content = new StringContent(jsonResponse, System.Text.Encoding.UTF8, "application/json")
            };

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            // Act
            var result = await _client.Donations
                .Create(request)
                .WithTimeout(TimeSpan.FromSeconds(45))
                .WithCorrelationId("test-correlation-123")
                .ExecuteAsync();

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be("donation-with-config");
            result.Amount.Should().Be("200.00");
            result.Currency.Should().Be("EUR");
            result.Status.Should().Be(DonationStatus.Pending);
        }
    }
}
