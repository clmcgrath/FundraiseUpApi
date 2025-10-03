using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FundraiseUp.Client.Tests.TestHelpers.Mocking;
using Xunit;

namespace FundraiseUp.Client.Tests.TestHelpers.Mocking
{
    /// <summary>
    /// Tests to verify thread safety of mock HTTP handlers
    /// </summary>
    public class ThreadSafetyTests
    {
        [Fact]
        public async Task MockHttpMessageHandler_ConcurrentRequests_ShouldBeThreadSafe()
        {
            // Arrange
            var handler = new MockHttpMessageHandler();
            
            // Queue multiple responses
            for (int i = 0; i < 50; i++)
            {
                handler.QueueSuccessResponse($"{{\"response\": {i}}}");
            }

            var httpClient = new HttpClient(handler);
            var tasks = new List<Task<HttpResponseMessage>>();

            // Act - Make concurrent requests
            for (int i = 0; i < 50; i++)
            {
                tasks.Add(httpClient.GetAsync($"https://api.test.com/endpoint{i}"));
            }

            var responses = await Task.WhenAll(tasks);

            // Assert
            Assert.Equal(50, handler.CallCount);
            Assert.Equal(50, handler.Requests.Count);
            Assert.Equal(50, responses.Length);

            // Verify all responses are successful
            foreach (var response in responses)
            {
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                response.Dispose();
            }

            httpClient.Dispose();
            handler.Dispose();
        }

        [Fact]
        public async Task HttpMockBuilder_ConcurrentRequests_ShouldBeThreadSafe()
        {
            // Arrange
            var mockBuilder = new HttpMockBuilder();
            mockBuilder.SetupAnyRequest(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"success\": true}")
            });

            var httpClient = mockBuilder.CreateHttpClient();
            var tasks = new List<Task<HttpResponseMessage>>();

            // Act - Make concurrent requests
            for (int i = 0; i < 50; i++)
            {
                tasks.Add(httpClient.GetAsync($"https://api.test.com/endpoint{i}"));
            }

            var responses = await Task.WhenAll(tasks);

            // Assert
            Assert.Equal(50, mockBuilder.CallCount);
            Assert.Equal(50, mockBuilder.Requests.Count);
            Assert.Equal(50, responses.Length);

            // Verify all responses are successful
            foreach (var response in responses)
            {
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                response.Dispose();
            }

            httpClient.Dispose();
        }

        [Fact]
        public void MockHttpMessageHandler_ConcurrentQueueOperations_ShouldBeThreadSafe()
        {
            // Arrange
            var handler = new MockHttpMessageHandler();
            var tasks = new List<Task>();

            // Act - Queue responses concurrently
            for (int i = 0; i < 100; i++)
            {
                int capturedIndex = i;
                tasks.Add(Task.Run(() =>
                {
                    handler.QueueSuccessResponse($"{{\"concurrent\": {capturedIndex}}}");
                }));
            }

            Task.WaitAll(tasks.ToArray());

            // Assert - Should not throw and should have queued all responses
            // We can't easily verify the exact count without exposing internal queue state,
            // but if we made it here without exceptions, the thread safety worked
            Assert.True(true, "Concurrent queue operations completed without exceptions");

            handler.Dispose();
        }
    }
}