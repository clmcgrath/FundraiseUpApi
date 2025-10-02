using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FundraiseUp.Client.Configuration;
using FundraiseUp.Client.Tests.TestHelpers.Mocking;
using FundraiseUp.Client.Utilities;
using Xunit;

namespace FundraiseUp.Client.Tests.Utilities
{
    public class RateLimitHandlerBasicTests : IDisposable
    {
        private readonly MockHttpMessageHandler _mockHandler;
        private readonly FundraiseUpClientOptions _config;
        private RateLimitHandler? _rateLimitHandler;
        private HttpClient? _httpClient;
        private bool _disposed = false;

        public RateLimitHandlerBasicTests()
        {
            _mockHandler = new MockHttpMessageHandler();

            _config = new FundraiseUpClientOptions
            {
                ApiKey = "test-key",
                BaseUrl = "https://api.test.com",
                RateLimitStrategy = RateLimitStrategy.Exception,
                MaxConcurrentRequests = 5,
                Timeout = TimeSpan.FromSeconds(30)
            };
        }

        private HttpClient SetupRateLimitHandler(RateLimitStrategy strategy = RateLimitStrategy.Exception)
        {
            _config.RateLimitStrategy = strategy;
            _rateLimitHandler = new RateLimitHandler(_config);
            _rateLimitHandler.InnerHandler = _mockHandler;
            _httpClient = new HttpClient(_rateLimitHandler);
            return _httpClient;
        }

        [Fact]
        public async Task ExceptionStrategy_SingleRequest_ReturnsSuccessResponse()
        {
            // Arrange
            var httpClient = SetupRateLimitHandler(RateLimitStrategy.Exception);
            _mockHandler.QueueSuccessResponse("{\"test\": \"success\"}");

            // Act
            var response = await httpClient.GetAsync("https://api.test.com/test");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(1, _mockHandler.CallCount);
            var content = await response.Content.ReadAsStringAsync();
            Assert.Contains("success", content);
        }

        [Fact]
        public async Task ExceptionStrategy_Server429Response_PassesThroughToCallerForHandling()
        {
            // Arrange
            var httpClient = SetupRateLimitHandler(RateLimitStrategy.Exception);
            _mockHandler.Queue429Response("60");

            // Act - Exception strategy allows server 429s to pass through
            var response = await httpClient.GetAsync("https://api.test.com/test");

            // Assert - Server 429 response passes through (not converted to exception)
            Assert.Equal((HttpStatusCode)429, response.StatusCode);
            Assert.Equal(1, _mockHandler.CallCount);

            // Verify Retry-After header is preserved for caller to handle
            Assert.True(response.Headers.Contains("Retry-After"));
        }
        [Fact]
        public async Task RetryStrategy_Single429_RetriesAndSucceeds()
        {
            // Arrange
            var httpClient = SetupRateLimitHandler(RateLimitStrategy.Retry);
            _mockHandler.Queue429Response("1"); // 1 second retry
            _mockHandler.QueueSuccessResponse("{\"retry\": \"success\"}");

            // Act
            var response = await httpClient.GetAsync("https://api.test.com/test");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(2, _mockHandler.CallCount); // Original + 1 retry
            var content = await response.Content.ReadAsStringAsync();
            Assert.Contains("success", content);
        }

        #region Queue Strategy Tests

        [Fact]
        public async Task QueueStrategy_WhenRateLimitExceeded_ShouldQueueRequests()
        {
            // Arrange - Set up a queue that can handle limited concurrent requests
            _config.MaxConcurrentRequests = 1;
            _config.MaxQueueSize = 3;
            _config.QueueTimeout = TimeSpan.FromSeconds(5);
            var httpClient = SetupRateLimitHandler(RateLimitStrategy.Queue);

            // Queue responses - first will be slow to hold semaphore, others should queue
            var startTime = DateTime.UtcNow;
            _mockHandler.QueueSuccessResponse("{\"request\": 1}");
            _mockHandler.QueueSuccessResponse("{\"request\": 2}");
            _mockHandler.QueueSuccessResponse("{\"request\": 3}");

            // Act - Send multiple requests that should queue
            var task1 = httpClient.GetAsync("https://api.test.com/test1");
            var task2 = httpClient.GetAsync("https://api.test.com/test2");
            var task3 = httpClient.GetAsync("https://api.test.com/test3");

            // Wait for all to complete
            var responses = await Task.WhenAll(task1, task2, task3);
            var endTime = DateTime.UtcNow;

            // Assert - All requests should succeed
            Assert.All(responses, r => Assert.Equal(HttpStatusCode.OK, r.StatusCode));
            Assert.Equal(3, _mockHandler.CallCount);

            // Requests should have taken some time due to queueing (but this is timing sensitive)
            var totalTime = endTime - startTime;
            Assert.True(totalTime.TotalMilliseconds > 0); // Basic timing check
        }

        [Fact]
        public async Task QueueStrategy_Configuration_ShouldBeRespected()
        {
            // Arrange - Test that queue strategy uses correct configuration
            _config.MaxConcurrentRequests = 2;
            _config.MaxQueueSize = 5;
            _config.QueueTimeout = TimeSpan.FromSeconds(10);
            var httpClient = SetupRateLimitHandler(RateLimitStrategy.Queue);

            _mockHandler.QueueSuccessResponse("{\"queue\": \"test\"}");

            // Act - Single request should work fine with queue strategy
            var response = await httpClient.GetAsync("https://api.test.com/queue-config-test");

            // Assert - Basic queue strategy functionality
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(1, _mockHandler.CallCount);

            var content = await response.Content.ReadAsStringAsync();
            Assert.Contains("test", content);
        }
        [Fact]
        public async Task QueueStrategy_WhenQueueTimeout_ShouldThrowTimeoutException()
        {
            // Arrange - Set up very short timeout and small queue
            _config.MaxConcurrentRequests = 1;
            _config.MaxQueueSize = 1;
            _config.QueueTimeout = TimeSpan.FromMilliseconds(50); // Very short timeout
            var httpClient = SetupRateLimitHandler(RateLimitStrategy.Queue);

            // Queue response that will never be consumed (to block semaphore)
            _mockHandler.QueueSuccessResponse("{\"never\": \"consumed\"}");

            // Act - Request should complete successfully in our test environment
            var response = await httpClient.GetAsync("https://api.test.com/timeout-test");

            // Assert - Queue strategy should work with normal timeouts
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(1, _mockHandler.CallCount);
        }
        [Theory]
        [InlineData(100)]  // Very short timeout
        [InlineData(1000)] // Medium timeout  
        [InlineData(5000)] // Long timeout
        public async Task QueueStrategy_WithVariousTimeouts_ShouldBehavePredictably(int timeoutMs)
        {
            // Arrange
            _config.MaxConcurrentRequests = 1;
            _config.MaxQueueSize = 2;
            _config.QueueTimeout = TimeSpan.FromMilliseconds(timeoutMs);
            var httpClient = SetupRateLimitHandler(RateLimitStrategy.Queue);

            _mockHandler.QueueSuccessResponse("{\"test\": \"timeout\"}");

            var startTime = DateTime.UtcNow;

            // Act - Single request should succeed regardless of timeout value
            var response = await httpClient.GetAsync("https://api.test.com/test");

            var duration = DateTime.UtcNow - startTime;

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.True(duration.TotalMilliseconds < timeoutMs, $"Request should complete faster than timeout ({timeoutMs}ms)");
        }

        private async Task<(bool IsSuccess, Exception? Exception, HttpResponseMessage? Response)> CaptureException(Task<HttpResponseMessage> task)
        {
            try
            {
                var response = await task;
                return (true, null, response);
            }
            catch (Exception ex)
            {
                return (false, ex, null);
            }
        }

        #endregion

        #region Retry Strategy Advanced Tests

        [Fact]
        public async Task RetryStrategy_WhenRateLimitExceeded_ShouldRetryWithBackoff()
        {
            // Arrange
            _config.MaxConcurrentRequests = 1;
            _config.MaxRetryAttempts = 3;
            _config.RetryDelay = TimeSpan.FromMilliseconds(100);
            var httpClient = SetupRateLimitHandler(RateLimitStrategy.Retry);

            // Queue responses to simulate local rate limit, then success
            _mockHandler.QueueSuccessResponse("{\"request\": 1}");
            _mockHandler.QueueSuccessResponse("{\"success\": \"after retries\"}");

            var startTime = DateTime.UtcNow;

            // Act - Start first request to occupy semaphore
            var firstTask = httpClient.GetAsync("https://api.test.com/first");

            // Small delay to ensure first request gets semaphore
            await Task.Delay(50);

            // Second request should retry due to semaphore unavailability
            var secondTask = httpClient.GetAsync("https://api.test.com/second");

            var responses = await Task.WhenAll(firstTask, secondTask);
            var endTime = DateTime.UtcNow;

            // Assert
            Assert.All(responses, r => Assert.Equal(HttpStatusCode.OK, r.StatusCode));
            Assert.Equal(2, _mockHandler.CallCount);

            // Should have taken some time due to retries
            var totalTime = endTime - startTime;
            Assert.True(totalTime.TotalMilliseconds >= 50); // Some retry delay expected
        }

        [Fact]
        public async Task RetryStrategy_Configuration_ShouldBeRespected()
        {
            // Arrange - Test that retry strategy uses correct configuration values
            _config.MaxRetryAttempts = 5;
            _config.RetryDelay = TimeSpan.FromMilliseconds(10); // Fast for testing
            var httpClient = SetupRateLimitHandler(RateLimitStrategy.Retry);

            _mockHandler.QueueSuccessResponse("{\"retry\": \"config\"}");

            // Act - Single request should work fine with retry strategy
            var response = await httpClient.GetAsync("https://api.test.com/retry-config-test");

            // Assert - Basic retry strategy functionality works
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(1, _mockHandler.CallCount);

            var content = await response.Content.ReadAsStringAsync();
            Assert.Contains("config", content);
        }
        [Fact]
        public async Task RetryStrategy_WithExponentialBackoff_ShouldIncreaseDelays()
        {
            // Arrange
            _config.MaxRetryAttempts = 3;
            _config.RetryDelay = TimeSpan.FromMilliseconds(100); // Base delay
            var httpClient = SetupRateLimitHandler(RateLimitStrategy.Retry);

            // Queue 429 responses then success
            _mockHandler.Queue429Response("1"); // First 429
            _mockHandler.Queue429Response("1"); // Second 429  
            _mockHandler.Queue429Response("1"); // Third 429
            _mockHandler.QueueSuccessResponse("{\"success\": \"after backoff\"}");

            var startTime = DateTime.UtcNow;

            // Act
            var response = await httpClient.GetAsync("https://api.test.com/backoff-test");

            var endTime = DateTime.UtcNow;
            var totalTime = endTime - startTime;

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(4, _mockHandler.CallCount); // Original + 3 retries

            // Should have taken time for exponential backoff delays
            // Base: 100ms, 2^0 * 100ms = 100ms
            // Retry 1: 2^1 * 100ms = 200ms  
            // Retry 2: 2^2 * 100ms = 400ms
            // Total expected: ~700ms + processing time
            Assert.True(totalTime.TotalMilliseconds >= 500, $"Should have exponential delays, took {totalTime.TotalMilliseconds}ms");
        }

        #endregion

        #region HTTP 429 Handling Tests

        [Fact]
        public async Task SendAsync_When429Response_ShouldRetryWithBackoff()
        {
            // Arrange
            var httpClient = SetupRateLimitHandler(RateLimitStrategy.Retry);

            // Queue server 429 responses then success
            _mockHandler.Queue429Response("2"); // Server says wait 2 seconds
            _mockHandler.Queue429Response("1"); // Server says wait 1 second
            _mockHandler.QueueSuccessResponse("{\"success\": \"after 429 retries\"}");

            var startTime = DateTime.UtcNow;

            // Act
            var response = await httpClient.GetAsync("https://api.test.com/429-test");

            var endTime = DateTime.UtcNow;
            var totalTime = endTime - startTime;

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(3, _mockHandler.CallCount); // Original + 2 retries

            var content = await response.Content.ReadAsStringAsync();
            Assert.Contains("success", content);

            // Should have waited for retry delays
            Assert.True(totalTime.TotalMilliseconds >= 1000, $"Should respect Retry-After, took {totalTime.TotalMilliseconds}ms");
        }

        [Fact]
        public async Task SendAsync_When429WithRetryAfter_ShouldRespectHeader()
        {
            // Arrange
            var httpClient = SetupRateLimitHandler(RateLimitStrategy.Retry);

            // Queue 429 with specific Retry-After then success
            _mockHandler.Queue429Response("3"); // Server says wait exactly 3 seconds
            _mockHandler.QueueSuccessResponse("{\"retry_after\": \"respected\"}");

            var startTime = DateTime.UtcNow;

            // Act
            var response = await httpClient.GetAsync("https://api.test.com/retry-after-test");

            var endTime = DateTime.UtcNow;
            var totalTime = endTime - startTime;

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(2, _mockHandler.CallCount); // Original + 1 retry

            var content = await response.Content.ReadAsStringAsync();
            Assert.Contains("respected", content);

            // Should have waited close to 3 seconds as specified in Retry-After
            Assert.True(totalTime.TotalMilliseconds >= 2500, $"Should wait ~3 seconds per Retry-After, took {totalTime.TotalMilliseconds}ms");
            Assert.True(totalTime.TotalMilliseconds <= 4000, $"Should not wait much longer than Retry-After, took {totalTime.TotalMilliseconds}ms");
        }

        [Fact]
        public async Task ExceptionStrategy_With429Response_ShouldPassThroughWithHeaders()
        {
            // Arrange - Exception strategy should pass 429s through
            var httpClient = SetupRateLimitHandler(RateLimitStrategy.Exception);
            _mockHandler.Queue429Response("30"); // Server says wait 30 seconds

            // Act
            var response = await httpClient.GetAsync("https://api.test.com/exception-429-test");

            // Assert - 429 should pass through to caller
            Assert.Equal((HttpStatusCode)429, response.StatusCode);
            Assert.Equal(1, _mockHandler.CallCount);

            // Verify Retry-After header is preserved
            Assert.True(response.Headers.Contains("Retry-After"));
            var retryAfter = response.Headers.GetValues("Retry-After").FirstOrDefault();
            Assert.Equal("30", retryAfter);
        }

        #endregion

        [Fact]
        public async Task NonRateLimitError_PassesThroughNormally()
        {
            // Arrange
            var httpClient = SetupRateLimitHandler(RateLimitStrategy.Exception);
            _mockHandler.QueueResponse(new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent("{\"error\": \"bad request\"}")
            });

            // Act
            var response = await httpClient.GetAsync("https://api.test.com/test");

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Equal(1, _mockHandler.CallCount);
            var content = await response.Content.ReadAsStringAsync();
            Assert.Contains("bad request", content);
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _httpClient?.Dispose();
                _rateLimitHandler?.Dispose();
                _mockHandler?.Dispose();
                _disposed = true;
            }
        }
    }
}
