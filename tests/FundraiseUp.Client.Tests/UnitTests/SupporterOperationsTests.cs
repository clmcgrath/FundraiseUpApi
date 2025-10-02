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
    public class SupporterOperationsTests
    {
        private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
        private readonly HttpClient _httpClient;
        private readonly FundraiseUpClient _client;

        public SupporterOperationsTests()
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

        // Note: Supporters cannot be created via FundraiseUp API; they are created automatically with donations.

        [Fact]
        public async Task GetSupporter_WithValidId_ShouldReturnSupporter()
        {
            // Arrange
            var supporterId = "supporter-456";
            var expectedSupporter = new SupporterResponse
            {
                Id = supporterId,
                Email = "jane.smith@example.com",
                FirstName = "Jane",
                LastName = "Smith",
                Phone = "+1-555-987-6543",
                CreatedAt = DateTime.UtcNow
            };

            var jsonResponse = JsonSerializer.Serialize(expectedSupporter, JsonConfiguration.DefaultOptions);
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
            var result = await _client.Supporters
                .GetById(supporterId)
                .ExecuteAsync();

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(supporterId);
            result.Email.Should().Be("jane.smith@example.com");
            result.FirstName.Should().Be("Jane");
            result.LastName.Should().Be("Smith");
            result.Phone.Should().Be("+1-555-987-6543");
        }


        [Fact]
        public async Task SearchSupporters_WithEmailFilter_ShouldReturnMatchingSupporters()
        {
            // Arrange
            var expectedResult = new PagedResult<SupporterResponse>
            {
                Items = new List<SupporterResponse>
                {
                    new SupporterResponse
                    {
                        Id = "supporter-search-1",
                        Email = "test@example.com",
                        FirstName = "Test",
                        LastName = "User"
                    },
                    new SupporterResponse
                    {
                        Id = "supporter-search-2",
                        Email = "another@example.com",
                        FirstName = "Another",
                        LastName = "User"
                    }
                },
                TotalCount = 2,
                CurrentPage = 1,
                PageSize = 20
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
            var result = await _client.Supporters
                .Search()
                .Where(d => d.Email != null && d.Email.Contains("example.com"))
                .ExecuteAsync();

            // Assert
            result.Should().NotBeNull();
            result.Data.Should().HaveCount(2);
            result.HasMore.Should().BeFalse();
            result.NextCursor.Should().BeNull();
        }

        [Fact]
        public async Task GetSupporterDonations_WithValidId_ShouldReturnDonationHistory()
        {
            // Arrange
            var supporterId = "supporter-donations";
            var expectedResult = new DonationsResponse
            {
                Data = new List<DonationResponse>
                {
                    new DonationResponse
                    {
                        Id = "donation-1",
                        Amount = "100.00",
                        Currency = "USD",
                        Status = DonationStatus.Succeeded,
                        Supporter = new EmbeddedSupporterResponse { Id = supporterId }
                    },
                    new DonationResponse
                    {
                        Id = "donation-2",
                        Amount = "250.00",
                        Currency = "USD",
                        Status = DonationStatus.Succeeded,
                        Supporter = new EmbeddedSupporterResponse { Id = supporterId }
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
            var result = await _client.Supporters
                .GetDonations(supporterId)
                .Take(10)
                .ExecuteAsync();

            // Assert
            result.Should().NotBeNull();
            result.Items.Should().HaveCount(2);
            result.Items.Should().BeEquivalentTo(expectedResult.Data);
            // The FundraiseUp API uses cursor-based pagination and does not return a total count.
            // When converting the cursor-based response to a PagedResult, TotalCount is set to 0 to reflect this.
            result.TotalCount.Should().Be(0);
            result.CurrentPage.Should().Be(1);
            result.PageSize.Should().Be(10);
            result.HasMore.Should().BeFalse();
        }
    }
}
