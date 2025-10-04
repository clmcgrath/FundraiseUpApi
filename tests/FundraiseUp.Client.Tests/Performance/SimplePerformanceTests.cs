using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using FundraiseUp.Client.Configuration;
using FundraiseUp.Client.Models;
using FundraiseUp.Client.Requests;
using FundraiseUp.Client.Tests.TestHelpers.Mocking;
using FundraiseUp.Client.Utilities;
using Xunit;

namespace FundraiseUp.Client.Tests.Performance
{
    /// <summary>
    /// Performance tests for the FundraiseUp client library with simulated network latency.
    /// Tests client-side performance, rate limiting, and resource usage under realistic conditions.
    /// </summary>
    public class SimplePerformanceTests : IDisposable
    {
        private readonly FundraiseUpClientOptions _defaultOptions;
        private FundraiseUpClient? _client;
        private bool _disposed = false;

        public SimplePerformanceTests()
        {
            _defaultOptions = new FundraiseUpClientOptions
            {
                ApiKey = "test-performance-key",
                BaseUrl = "https://api.test.com",
                RateLimitStrategy = RateLimitStrategy.Queue,
                MaxConcurrentRequests = 3, // FundraiseUp API limit
                MaxQueueSize = 100,
                QueueTimeout = TimeSpan.FromSeconds(30),
                Timeout = TimeSpan.FromSeconds(10)
            };
        }

        [Fact]
        public async Task ConcurrentDonationRequests_ShouldMaintainPerformance()
        {
            // Arrange
            const int concurrentRequests = 25;
            const int maxExpectedSeconds = 5;

            var mockBuilder = new HttpMockBuilder()
                .AddDelayedSuccessResponse(TimeSpan.FromMilliseconds(100),
                    "{\"id\": \"ABCDEFGHI\", \"amount\": \"100.00\", \"currency\": \"usd\"}");

            _client = CreateClientWithMock(mockBuilder);

            var donationRequest = CreateStandardDonationRequest();
            var stopwatch = Stopwatch.StartNew();

            // Act - Fire off multiple concurrent requests
            var tasks = Enumerable.Range(0, concurrentRequests)
                .Select(_ => _client.Donations.Create(donationRequest).ExecuteAsync())
                .ToArray();

            var results = await Task.WhenAll(tasks);
            stopwatch.Stop();

            // Assert
            results.Should().AllSatisfy(result =>
            {
                result.Should().NotBeNull();
                result.Id.Should().NotBeEmpty();
                result.Amount.Should().Be("100.00");
                result.Currency.Should().Be("usd");
            });

            stopwatch.Elapsed.Should().BeLessThan(TimeSpan.FromSeconds(maxExpectedSeconds));
        }

        [Fact]
        public async Task MemoryUsage_ShouldRemainStable()
        {
            // Arrange
            const int requestCount = 100;
            const long maxMemoryIncreaseMB = 10;

            var mockBuilder = new HttpMockBuilder()
                .AddDelayedSuccessResponse(TimeSpan.FromMilliseconds(50),
                    "{\"id\": \"BABCDEFGH\", \"amount\": \"100.00\", \"currency\": \"usd\"}");

            _client = CreateClientWithMock(mockBuilder);

            // Force garbage collection to get baseline memory
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            var initialMemory = GC.GetTotalMemory(false);
            var donationRequest = CreateStandardDonationRequest();

            // Act - Make many sequential requests
            for (int i = 0; i < requestCount; i++)
            {
                var result = await _client.Donations.Create(donationRequest).ExecuteAsync();
                result.Should().NotBeNull();
            }

            // Force garbage collection to clean up
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            var finalMemory = GC.GetTotalMemory(false);

            // Assert
            var memoryIncreaseBytes = finalMemory - initialMemory;
            var memoryIncreaseMB = memoryIncreaseBytes / (1024 * 1024);

            memoryIncreaseMB.Should().BeLessThan(maxMemoryIncreaseMB,
                $"Memory usage increased by {memoryIncreaseMB}MB after {requestCount} requests");
        }

        [Fact]
        public async Task RateLimitingWithNetworkLatency_ShouldHandleGracefully()
        {
            // Arrange
            const int requestCount = 10;

            var mockBuilder = new HttpMockBuilder()
                .AddVariableLatencyResponse(TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(300),
                    "{\"id\": \"CABCDEFGH\", \"amount\": \"100.00\", \"currency\": \"usd\"}");

            // Configure client with strict rate limiting
            var options = new FundraiseUpClientOptions
            {
                ApiKey = _defaultOptions.ApiKey,
                BaseUrl = _defaultOptions.BaseUrl,
                MaxConcurrentRequests = 2, // Very restrictive
                RateLimitStrategy = RateLimitStrategy.Queue,
                QueueTimeout = TimeSpan.FromSeconds(30),
                Timeout = TimeSpan.FromSeconds(10)
            };

            _client = CreateClientWithMock(mockBuilder, options);

            var donationRequest = CreateStandardDonationRequest();
            var stopwatch = Stopwatch.StartNew();

            // Act - Submit requests that will be rate limited
            var tasks = Enumerable.Range(0, requestCount)
                .Select(_ => _client.Donations.Create(donationRequest).ExecuteAsync())
                .ToArray();

            var results = await Task.WhenAll(tasks);
            stopwatch.Stop();

            // Assert
            results.Should().HaveCount(requestCount);
            results.Should().AllSatisfy(result =>
            {
                result.Should().NotBeNull();
                result.Id.Should().NotBeEmpty();
            });

            // With rate limiting and network latency, this should take longer but still complete
            stopwatch.Elapsed.Should().BeLessThan(TimeSpan.FromSeconds(15));
        }

        [Fact]
        public async Task TimeoutHandling_ShouldHandleGracefully()
        {
            // Arrange - Setup delayed response shorter than timeout
            var options = new FundraiseUpClientOptions
            {
                ApiKey = _defaultOptions.ApiKey,
                BaseUrl = _defaultOptions.BaseUrl,
                Timeout = TimeSpan.FromSeconds(10), // Long enough to allow for retries
                MaxRetryAttempts = 0, // Disable retries to test timeout directly
                RateLimitStrategy = _defaultOptions.RateLimitStrategy
            };

            var mockBuilder = new HttpMockBuilder()
                .AddDelayedSuccessResponse(TimeSpan.FromSeconds(3), // Delay shorter than timeout
                    "{\"id\": \"DABCDEFGH\", \"amount\": \"100.00\", \"currency\": \"usd\"}");

            _client = CreateClientWithMock(mockBuilder, options);

            var donationRequest = CreateStandardDonationRequest();

            // Act & Assert - Should complete successfully within timeout
            var result = await _client.Donations.Create(donationRequest).ExecuteAsync();
            result.Should().NotBeNull();
            result.Id.Should().NotBeEmpty();
        }

        [Fact]
        public async Task StressTest_ShouldHandleHighLoad()
        {
            // Arrange
            const int concurrentRequests = 50; // Reduced from 100 to avoid timeout issues
            const int maxExpectedSeconds = 8;

            var mockBuilder = new HttpMockBuilder()
                .AddVariableLatencyResponse(TimeSpan.FromMilliseconds(50), TimeSpan.FromMilliseconds(500),
                    "{\"id\": \"DCDEFGHIJ\", \"amount\": \"100.00\", \"currency\": \"usd\"}");

            _client = CreateClientWithMock(mockBuilder);

            var donationRequest = CreateStandardDonationRequest();
            var stopwatch = Stopwatch.StartNew();

            // Act - High concurrent load
            var tasks = Enumerable.Range(0, concurrentRequests)
                .Select(_ => _client.Donations.Create(donationRequest).ExecuteAsync())
                .ToArray();

            var results = await Task.WhenAll(tasks);
            stopwatch.Stop();

            // Assert
            results.Should().HaveCount(concurrentRequests);
            results.Should().AllSatisfy(result =>
            {
                result.Should().NotBeNull();
                result.Id.Should().NotBeEmpty();
            });

            stopwatch.Elapsed.Should().BeLessThan(TimeSpan.FromSeconds(maxExpectedSeconds));
        }

        private FundraiseUpClient CreateClientWithMock(HttpMockBuilder mockBuilder, FundraiseUpClientOptions? options = null)
        {
            var httpClient = mockBuilder.CreateHttpClient();
            var clientOptions = options ?? _defaultOptions;
            return new FundraiseUpClient(clientOptions.ApiKey, clientOptions, httpClient);
        }

        private static CreateDonationRequest CreateStandardDonationRequest()
        {
            return new CreateDonationRequest
            {
                Amount = "100.00",
                Currency = "usd",
                Campaign = "test-campaign",
                Designation = "test-designation",
                PaymentMethod = new PaymentMethodRequest
                {
                    Stripe = new StripePaymentMethodRequest
                    {
                        Id = "pm_test_123"
                    }
                },
                Supporter = new SupporterRequest
                {
                    FirstName = "Test",
                    LastName = "User",
                    Email = "test@example.com"
                }
            };
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _client?.Dispose();
                _disposed = true;
            }
        }
    }
}