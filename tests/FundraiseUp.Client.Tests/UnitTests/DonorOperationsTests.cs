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
    public class DonorOperationsTests
    {
        private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
        private readonly HttpClient _httpClient;
        private readonly FundraiseUpClient _client;

        public DonorOperationsTests()
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
        public async Task GetDonor_WithValidId_ShouldReturnDonor()
        {
            // Arrange
            var donorId = "donor-456";
            var expectedDonor = new Donor
            {
                Id = donorId,
                Email = "jane.smith@example.com",
                FirstName = "Jane",
                LastName = "Smith",
                Phone = "+1-555-987-6543",
                CreatedAt = DateTime.UtcNow
            };

            var jsonResponse = JsonSerializer.Serialize(expectedDonor, JsonConfiguration.DefaultOptions);
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
                .GetById(donorId)
                .ExecuteAsync();

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(donorId);
            result.Email.Should().Be("jane.smith@example.com");
            result.FirstName.Should().Be("Jane");
            result.LastName.Should().Be("Smith");
            result.Phone.Should().Be("+1-555-987-6543");
        }

        // [Fact] - COMMENTED OUT: Supporters cannot be updated via FundraiseUp API - they are read-only
        // public async Task UpdateDonor_WithValidData_ShouldReturnUpdatedDonor()
        // {
        //     // This test is disabled because supporters are read-only in FundraiseUp API
        // }

        [Fact]
        public async Task SearchDonors_WithEmailFilter_ShouldReturnMatchingDonors()
        {
            // Arrange
            var expectedResult = new PagedResult<Donor>
            {
                Items = new List<Donor>
                {
                    new Donor
                    {
                        Id = "donor-search-1",
                        Email = "test@example.com",
                        FirstName = "Test",
                        LastName = "User"
                    },
                    new Donor
                    {
                        Id = "donor-search-2",
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
                .Where(d => d.Email?.Contains("example.com") == true)
                .ExecuteAsync();

            // Assert
            result.Should().NotBeNull();
            result.Data.Should().HaveCount(2);
            result.HasMore.Should().BeFalse();
            result.NextCursor.Should().BeNull();
        }

        [Fact]
        public async Task GetDonorDonations_WithValidId_ShouldReturnDonationHistory()
        {
            // Arrange
            var donorId = "donor-donations";
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
                        Supporter = new EmbeddedSupporterResponse { Id = donorId }
                    },
                    new DonationResponse
                    {
                        Id = "donation-2",
                        Amount = "250.00",
                        Currency = "USD",
                        Status = DonationStatus.Succeeded,
                        Supporter = new EmbeddedSupporterResponse { Id = donorId }
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
                .GetDonations(donorId)
                .Take(10)
                .ExecuteAsync();

            // Assert
            result.Should().NotBeNull();
            result.Items.Should().HaveCount(2);
            result.Items.Should().OnlyContain(d => d.Supporter.Id == donorId);
            // The FundraiseUp API uses cursor-based pagination and does not return a total count.
            // When converting the cursor-based response to a PagedResult, TotalCount is set to 0 to reflect this.
            result.TotalCount.Should().Be(0);
            result.CurrentPage.Should().Be(1);
            result.PageSize.Should().Be(10);
            result.HasMore.Should().BeFalse();
        }
            // result.TotalCount is not asserted because FundraiseUp API uses cursor pagination and does not provide a total count
        // [Fact] - COMMENTED OUT: Supporter statistics not available in current FundraiseUp API
        // public async Task GetDonorStatistics_WithValidId_ShouldReturnStats()
        // {
        //     // This test is disabled because supporter statistics are not available in FundraiseUp API
        // }

        // [Fact] - COMMENTED OUT: Supporter merge not available in FundraiseUp API
        // public async Task MergeDonors_WithValidIds_ShouldReturnMergedDonor()
        // {
        //     // This test is disabled because supporter merge is not supported by the API
        // }

        // [Fact] - COMMENTED OUT: Supporters cannot be created via FundraiseUp API - they are created automatically with donations
        // public async Task DonorOperations_WithFluentConfiguration_ShouldApplySettings()
        // {
        //     // This test is disabled because supporters are read-only in FundraiseUp API
        // }
    }
}
