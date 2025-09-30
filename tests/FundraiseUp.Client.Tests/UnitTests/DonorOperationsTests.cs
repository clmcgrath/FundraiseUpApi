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
            _httpClient = new HttpClient(_httpMessageHandlerMock.Object);
            var logger = new Mock<ILogger<FundraiseUpClient>>();

            _client = new FundraiseUpClient("test-api-key", new FundraiseUpClientOptions
            {
                BaseUrl = "https://api.test.com"
            }, _httpClient, logger.Object);
        }

        [Fact]
        public async Task CreateDonor_WithValidRequest_ShouldReturnDonor()
        {
            // Arrange
            var request = new CreateDonorRequest
            {
                Email = "john.doe@example.com",
                FirstName = "John",
                LastName = "Doe",
                Phone = "+1-555-123-4567",
                Address = new Address
                {
                    Street = "123 Main St",
                    City = "New York",
                    State = "NY",
                    PostalCode = "10001",
                    Country = "US"
                }
            };

            var expectedDonor = new Donor
            {
                Id = "donor-123",
                Email = "john.doe@example.com",
                FirstName = "John",
                LastName = "Doe",
                Phone = "+1-555-123-4567",
                Address = new Address
                {
                    Street = "123 Main St",
                    City = "New York",
                    State = "NY",
                    PostalCode = "10001",
                    Country = "US"
                },
                CreatedAt = DateTime.UtcNow
            };

            var jsonResponse = JsonSerializer.Serialize(expectedDonor, JsonConfiguration.DefaultOptions);
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
            var result = await _client.Donors
                .Create(request)
                .ExecuteAsync();

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be("donor-123");
            result.Email.Should().Be("john.doe@example.com");
            result.FirstName.Should().Be("John");
            result.LastName.Should().Be("Doe");
            result.Phone.Should().Be("+1-555-123-4567");
            result.Address.Should().NotBeNull();
            result.Address!.Street.Should().Be("123 Main St");
            result.Address.City.Should().Be("New York");
            result.Address.State.Should().Be("NY");
            result.Address.PostalCode.Should().Be("10001");
            result.Address.Country.Should().Be("US");
        }

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
            var result = await _client.Donors
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

        [Fact]
        public async Task UpdateDonor_WithValidData_ShouldReturnUpdatedDonor()
        {
            // Arrange
            var donorId = "donor-789";
            var updateRequest = new UpdateDonorRequest
            {
                FirstName = "Updated John",
                LastName = "Updated Doe",
                Phone = "+1-555-111-2222"
            };

            var expectedDonor = new Donor
            {
                Id = donorId,
                Email = "john.doe@example.com",
                FirstName = "Updated John",
                LastName = "Updated Doe",
                Phone = "+1-555-111-2222",
                UpdatedAt = DateTime.UtcNow
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
            var result = await _client.Donors
                .Update(donorId, updateRequest)
                .ExecuteAsync();

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(donorId);
            result.Email.Should().Be("john.doe@example.com");
            result.FirstName.Should().Be("Updated John");
            result.LastName.Should().Be("Updated Doe");
            result.Phone.Should().Be("+1-555-111-2222");
        }

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
            var result = await _client.Donors
                .Search()
                .Where(d => d.Email.Contains("example.com"))
                .ExecuteAsync();

            // Assert
            result.Should().NotBeNull();
            result.Items.Should().HaveCount(2);
            result.TotalCount.Should().Be(2);
            result.CurrentPage.Should().Be(1);
            result.PageSize.Should().Be(20);
        }

        [Fact]
        public async Task GetDonorDonations_WithValidId_ShouldReturnDonationHistory()
        {
            // Arrange
            var donorId = "donor-donations";
            var expectedResult = new PagedResult<Donation>
            {
                Items = new List<Donation>
                {
                    new Donation
                    {
                        Id = "donation-1",
                        Amount = 100.00m,
                        Currency = "USD",
                        Status = DonationStatus.Completed,
                        DonorId = donorId
                    },
                    new Donation
                    {
                        Id = "donation-2",
                        Amount = 250.00m,
                        Currency = "USD",
                        Status = DonationStatus.Completed,
                        DonorId = donorId
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
            var result = await _client.Donors
                .GetDonations(donorId)
                .Take(10)
                .ExecuteAsync();

            // Assert
            result.Should().NotBeNull();
            result.Items.Should().HaveCount(2);
            result.Items.Should().OnlyContain(d => d.DonorId == donorId);
            result.TotalCount.Should().Be(2);
            result.CurrentPage.Should().Be(1);
            result.PageSize.Should().Be(10);
        }

        [Fact]
        public async Task GetDonorStatistics_WithValidId_ShouldReturnStats()
        {
            // Arrange
            var donorId = "donor-stats";
            var expectedStats = new DonorStatistics
            {
                DonorId = donorId,
                TotalDonated = 750.00m,
                DonationCount = 5,
                AverageDonation = 150.00m,

                FirstDonationDate = DateTime.UtcNow.AddDays(-90),
                LastDonationDate = DateTime.UtcNow.AddDays(-5)
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
            var result = await _client.Donors
                .GetStatistics(donorId)
                .ExecuteAsync();

            // Assert
            result.Should().NotBeNull();
            result.DonorId.Should().Be(donorId);
            result.TotalDonated.Should().Be(750.00m);
            result.DonationCount.Should().Be(5);
            result.AverageDonation.Should().Be(150.00m);

            result.FirstDonationDate.Should().BeCloseTo(DateTime.UtcNow.AddDays(-90), TimeSpan.FromMinutes(1));
            result.LastDonationDate.Should().BeCloseTo(DateTime.UtcNow.AddDays(-5), TimeSpan.FromMinutes(1));
        }

        [Fact]
        public async Task MergeDonors_WithValidIds_ShouldReturnMergedDonor()
        {
            // Arrange
            var primaryDonorId = "donor-primary";
            var duplicateDonorId = "donor-secondary";

            var expectedDonor = new Donor
            {
                Id = "donor-primary",
                Email = "primary@example.com",
                FirstName = "Primary",
                LastName = "Donor",
                Phone = "+1-555-999-8888",
                UpdatedAt = DateTime.UtcNow
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
            var result = await _client.Donors
                .Merge(primaryDonorId, duplicateDonorId)
                .ExecuteAsync();

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be("donor-primary");
            result.Email.Should().Be("primary@example.com");
            result.FirstName.Should().Be("Primary");
            result.LastName.Should().Be("Donor");
            result.Phone.Should().Be("+1-555-999-8888");
        }

        [Fact]
        public async Task DonorOperations_WithFluentConfiguration_ShouldApplySettings()
        {
            // Arrange
            var request = new CreateDonorRequest
            {
                Email = "fluent@example.com",
                FirstName = "Fluent",
                LastName = "Donor"
            };

            var expectedDonor = new Donor
            {
                Id = "donor-fluent",
                Email = "fluent@example.com",
                FirstName = "Fluent",
                LastName = "Donor",
                CreatedAt = DateTime.UtcNow
            };

            var jsonResponse = JsonSerializer.Serialize(expectedDonor, JsonConfiguration.DefaultOptions);
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
            var result = await _client.Donors
                .Create(request)
                .WithTimeout(TimeSpan.FromSeconds(45))
                .WithCorrelationId("donor-correlation-789")
                .WithRetry(3)
                .ExecuteAsync();

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be("donor-fluent");
            result.Email.Should().Be("fluent@example.com");
            result.FirstName.Should().Be("Fluent");
            result.LastName.Should().Be("Donor");
        }
    }
}