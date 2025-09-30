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
            _httpClient = new HttpClient(_httpMessageHandlerMock.Object);
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
                Amount = 100.00m,
                Currency = "USD",
                DonorEmail = "test@example.com",
                CampaignId = "campaign-123"
            };

            var expectedDonation = new Donation
            {
                Id = "donation-456",
                Amount = 100.00m,
                Currency = "USD",
                Status = DonationStatus.Completed,
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
            result.Amount.Should().Be(100.00m);
            result.Currency.Should().Be("USD");
            result.Status.Should().Be(DonationStatus.Completed);
        }

        [Fact]
        public async Task GetDonation_WithValidId_ShouldReturnDonation()
        {
            // Arrange
            var donationId = "donation-123";
            var expectedDonation = new Donation
            {
                Id = donationId,
                Amount = 50.00m,
                Currency = "USD",
                Status = DonationStatus.Completed,
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
            result.Amount.Should().Be(50.00m);
            result.Currency.Should().Be("USD");
            result.Status.Should().Be(DonationStatus.Completed);
        }

        [Fact]
        public async Task ListDonations_WithFilters_ShouldReturnPagedResults()
        {
            // Arrange
            var expectedResult = new PagedResult<Donation>
            {
                Items = new List<Donation>
                {
                    new Donation
                    {
                        Id = "donation-1",
                        Amount = 25.00m,
                        Currency = "USD",
                        Status = DonationStatus.Completed
                    },
                    new Donation
                    {
                        Id = "donation-2",
                        Amount = 75.00m,
                        Currency = "USD",
                        Status = DonationStatus.Pending
                    }
                },
                TotalCount = 2,
                CurrentPage = 1,
                PageSize = 10
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
            result.TotalCount.Should().Be(2);
            result.CurrentPage.Should().Be(1);
            result.PageSize.Should().Be(10);
            result.TotalPages.Should().Be(1);
        }

        [Fact]
        public async Task UpdateDonation_WithValidData_ShouldReturnUpdatedDonation()
        {
            // Arrange
            var donationId = "donation-123";
            var updateRequest = new UpdateDonationRequest
            {
                Amount = 150.00m,
                Currency = "USD"
            };

            var expectedDonation = new Donation
            {
                Id = donationId,
                Amount = 150.00m,
                Currency = "USD",
                Status = DonationStatus.Completed,
                UpdatedAt = DateTime.UtcNow
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
                .Update(donationId, updateRequest)
                .ExecuteAsync();

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(donationId);
            result.Amount.Should().Be(150.00m);
            result.Currency.Should().Be("USD");
            result.Status.Should().Be(DonationStatus.Completed);
        }

        [Fact]
        public async Task CreateDonation_WithFluentConfiguration_ShouldApplySettings()
        {
            // Arrange
            var request = new CreateDonationRequest
            {
                Amount = 200.00m,
                Currency = "EUR",
                DonorEmail = "donor@example.com",
                CampaignId = "campaign-789"
            };

            var expectedDonation = new Donation
            {
                Id = "donation-with-config",
                Amount = 200.00m,
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
            result.Amount.Should().Be(200.00m);
            result.Currency.Should().Be("EUR");
            result.Status.Should().Be(DonationStatus.Pending);
        }
    }
}