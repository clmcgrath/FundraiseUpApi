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
    public class CampaignOperationsTests
    {
        private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
        private readonly HttpClient _httpClient;
        private readonly FundraiseUpClient _client;

        public CampaignOperationsTests()
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
        public async Task CreateCampaign_WithValidRequest_ShouldReturnCampaign()
        {
            // Arrange
            var request = new CreateCampaignRequest
            {
                Name = "Test Campaign",
                Description = "A test campaign",
                GoalAmount = 10000.00m,
                Currency = "USD",
                IsActive = true
            };

            var expectedCampaign = new Campaign
            {
                Id = "campaign-123",
                Name = "Test Campaign",
                Description = "A test campaign",
                Goal = 10000.00m,
                Currency = "USD",
                Status = CampaignStatus.Active,
                CreatedAt = DateTime.UtcNow
            };

            var jsonResponse = JsonSerializer.Serialize(expectedCampaign, JsonConfiguration.DefaultOptions);
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
            var result = await _client.Campaigns
                .Create(request)
                .ExecuteAsync();

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be("campaign-123");
            result.Name.Should().Be("Test Campaign");
            result.Description.Should().Be("A test campaign");
            result.Goal.Should().Be(10000.00m);
            result.Currency.Should().Be("USD");
            result.Status.Should().Be(CampaignStatus.Active);
        }

        [Fact]
        public async Task GetCampaign_WithValidId_ShouldReturnCampaign()
        {
            // Arrange
            var campaignId = "campaign-456";
            var expectedCampaign = new Campaign
            {
                Id = campaignId,
                Name = "Existing Campaign",
                Description = "An existing campaign",
                Goal = 5000.00m,
                Currency = "USD",
                Status = CampaignStatus.Active,
                CreatedAt = DateTime.UtcNow
            };

            var jsonResponse = JsonSerializer.Serialize(expectedCampaign, JsonConfiguration.DefaultOptions);
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
            var result = await _client.Campaigns
                .GetById(campaignId)
                .ExecuteAsync();

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(campaignId);
            result.Name.Should().Be("Existing Campaign");
            result.Description.Should().Be("An existing campaign");
            result.Goal.Should().Be(5000.00m);
            result.Currency.Should().Be("USD");
            result.Status.Should().Be(CampaignStatus.Active);
        }

        [Fact]
        public async Task UpdateCampaign_WithValidData_ShouldReturnUpdatedCampaign()
        {
            // Arrange
            var campaignId = "campaign-789";
            var updateRequest = new UpdateCampaignRequest
            {
                Name = "Updated Campaign Name",
                GoalAmount = 15000.00m,
                Description = "Updated description"
            };

            var expectedCampaign = new Campaign
            {
                Id = campaignId,
                Name = "Updated Campaign Name",
                Description = "Updated description",
                Goal = 15000.00m,
                Currency = "USD",
                Status = CampaignStatus.Active,
                UpdatedAt = DateTime.UtcNow
            };

            var jsonResponse = JsonSerializer.Serialize(expectedCampaign, JsonConfiguration.DefaultOptions);
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
            var result = await _client.Campaigns
                .Update(campaignId, updateRequest)
                .ExecuteAsync();

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(campaignId);
            result.Name.Should().Be("Updated Campaign Name");
            result.Description.Should().Be("Updated description");
            result.Goal.Should().Be(15000.00m);
            result.Status.Should().Be(CampaignStatus.Active);
        }

        [Fact]
        public async Task ListCampaigns_WithPagination_ShouldReturnPagedResults()
        {
            // Arrange
            var expectedResult = new PagedResult<Campaign>
            {
                Items = new List<Campaign>
                {
                    new Campaign
                    {
                        Id = "campaign-1",
                        Name = "Campaign One",
                        Goal = 1000.00m,
                        Currency = "USD",
                        Status = CampaignStatus.Active
                    },
                    new Campaign
                    {
                        Id = "campaign-2",
                        Name = "Campaign Two",
                        Goal = 2000.00m,
                        Currency = "USD",
                        Status = CampaignStatus.Draft
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
            var result = await _client.Campaigns
                .List()
                .Page(1, 10)
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
        public async Task GetCampaignStatistics_WithValidId_ShouldReturnStats()
        {
            // Arrange
            var campaignId = "campaign-stats";
            var expectedStats = new CampaignStatistics
            {
                CampaignId = campaignId,
                TotalRaised = 7500.00m,
                DonationCount = 25,
                AverageDonation = 300.00m,
                ProgressPercentage = 75.0m
            };

            var jsonResponse = JsonSerializer.Serialize(expectedStats, JsonConfiguration.DefaultOptions);
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
            var result = await _client.Campaigns
                .GetStatistics(campaignId)
                .ExecuteAsync();

            // Assert
            result.Should().NotBeNull();
            result.CampaignId.Should().Be(campaignId);
            result.TotalRaised.Should().Be(7500.00m);
            result.DonationCount.Should().Be(25);
            result.AverageDonation.Should().Be(300.00m);
            result.ProgressPercentage.Should().Be(75.0m);
        }

        [Fact]
        public async Task ActivateCampaign_WithValidId_ShouldChangeCampaignStatus()
        {
            // Arrange
            var campaignId = "campaign-activate";
            var expectedCampaign = new Campaign
            {
                Id = campaignId,
                Name = "Activated Campaign",
                Status = CampaignStatus.Active,
                Goal = 5000.00m,
                Currency = "USD",
                UpdatedAt = DateTime.UtcNow
            };

            var jsonResponse = JsonSerializer.Serialize(expectedCampaign, JsonConfiguration.DefaultOptions);
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
            var result = await _client.Campaigns
                .Activate(campaignId)
                .ExecuteAsync();

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(campaignId);
            result.Status.Should().Be(CampaignStatus.Active);
            result.Name.Should().Be("Activated Campaign");
        }

        [Fact]
        public async Task CampaignOperations_WithFluentConfiguration_ShouldApplySettings()
        {
            // Arrange
            var request = new CreateCampaignRequest
            {
                Name = "Fluent Campaign",
                Description = "Campaign with fluent config",
                GoalAmount = 20000.00m,
                Currency = "EUR"
            };

            var expectedCampaign = new Campaign
            {
                Id = "campaign-fluent",
                Name = "Fluent Campaign",
                Description = "Campaign with fluent config",
                Goal = 20000.00m,
                Currency = "EUR",
                Status = CampaignStatus.Draft
            };

            var jsonResponse = JsonSerializer.Serialize(expectedCampaign, JsonConfiguration.DefaultOptions);
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
            var result = await _client.Campaigns
                .Create(request)
                .WithTimeout(TimeSpan.FromSeconds(60))
                .WithCorrelationId("campaign-correlation-456")
                .WithRetry(2)
                .ExecuteAsync();

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be("campaign-fluent");
            result.Name.Should().Be("Fluent Campaign");
            result.Goal.Should().Be(20000.00m);
            result.Currency.Should().Be("EUR");
            result.Status.Should().Be(CampaignStatus.Draft);
        }
    }
}

