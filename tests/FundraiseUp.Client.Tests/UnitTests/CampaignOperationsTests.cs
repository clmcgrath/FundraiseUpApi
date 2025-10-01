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

        [Fact(Skip = "Campaign creation not supported by FundraiseUp API")]
        public async Task CreateCampaign_WithValidRequest_ShouldReturnCampaign()
        {
            return;
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
                Goal = "5000.00",
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
            result.Goal.Should().Be("5000.00");
            result.Currency.Should().Be("USD");
            result.Status.Should().Be(CampaignStatus.Active);
        }

        // [Fact] - COMMENTED OUT: Campaigns cannot be updated via FundraiseUp API - they are read-only
        // public async Task UpdateCampaign_WithValidData_ShouldReturnUpdatedCampaign()
        // {
        //     // This test is disabled because campaigns are read-only in FundraiseUp API
        //     // Campaign updates are not supported by the API
        // }

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
                        Goal = "1000.00",
                        Currency = "USD",
                        Status = CampaignStatus.Active
                    },
                    new Campaign
                    {
                        Id = "campaign-2",
                        Name = "Campaign Two",
                        Goal = "2000.00",
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
                .WithLimit(10)
                .ExecuteAsync();

            // Assert
            result.Should().NotBeNull();
            result.Items.Should().HaveCount(2);
            result.TotalCount.Should().Be(2);
            result.CurrentPage.Should().Be(1);
            result.PageSize.Should().Be(10);
            result.TotalPages.Should().Be(1);
        }

        // [Fact] - COMMENTED OUT: Campaign statistics not available in current FundraiseUp API
        // public async Task GetCampaignStatistics_WithValidId_ShouldReturnStats()
        // {
        //     // This test is disabled because campaign statistics are not available in FundraiseUp API
        // }

        // [Fact] - COMMENTED OUT: Campaign activation not available in FundraiseUp API  
        // public async Task ActivateCampaign_WithValidId_ShouldChangeCampaignStatus()
        // {
        //     // This test is disabled because campaign activation is not supported by the API
        // }

        // [Fact] - COMMENTED OUT: Campaigns cannot be created via FundraiseUp API - they are read-only
        // public async Task CampaignOperations_WithFluentConfiguration_ShouldApplySettings()
        // {
        //     // This test is disabled because campaigns are read-only in FundraiseUp API
        //     // Campaign creation is not supported by the API
        // }
    }
}

