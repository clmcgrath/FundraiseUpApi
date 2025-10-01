using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Moq.Protected;

namespace FundraiseUp.Client.Tests.TestHelpers
{
    /// <summary>
    /// Helper for setting up HTTP client mocks for testing
    /// </summary>
    public class HttpClientMockSetup
    {
        private readonly Mock<HttpMessageHandler> _mockHandler;

        public HttpClientMockSetup()
        {
            _mockHandler = new Mock<HttpMessageHandler>();
        }

        /// <summary>
        /// Sets up a mock response for any HTTP request
        /// </summary>
        public HttpClientMockSetup SetupAnyRequest(HttpResponseMessage response)
        {
            _mockHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(response);

            return this;
        }

        /// <summary>
        /// Sets up a mock response for a specific HTTP method and URL pattern
        /// </summary>
        public HttpClientMockSetup SetupRequest(HttpMethod method, string urlPattern, HttpResponseMessage response)
        {
            _mockHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == method &&
                        req.RequestUri!.AbsolutePath.Contains(urlPattern)),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(response);

            return this;
        }

        /// <summary>
        /// Sets up a sequence of responses for multiple requests
        /// </summary>
        public HttpClientMockSetup SetupSequence(params HttpResponseMessage[] responses)
        {
            var setup = _mockHandler
                .Protected()
                .SetupSequence<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>());

            foreach (var response in responses)
            {
                setup = setup.ReturnsAsync(response);
            }

            return this;
        }

        /// <summary>
        /// Creates an HttpClient with the configured mock handler
        /// </summary>
        public HttpClient CreateHttpClient()
        {
            return new HttpClient(_mockHandler.Object)
            {
                BaseAddress = new Uri("https://api.test.com")
            };
        }

        /// <summary>
        /// Gets the mock handler for advanced verification scenarios
        /// </summary>
        public Mock<HttpMessageHandler> GetMockHandler()
        {
            return _mockHandler;
        }

        /// <summary>
        /// Verifies that a specific HTTP request was made
        /// </summary>
        public void VerifyRequest(HttpMethod method, string urlPattern, Times times)
        {
            _mockHandler
                .Protected()
                .Verify(
                    "SendAsync",
                    times,
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == method &&
                        req.RequestUri!.AbsolutePath.Contains(urlPattern)),
                    ItExpr.IsAny<CancellationToken>());
        }

        /// <summary>
        /// Verifies that no HTTP requests were made
        /// </summary>
        public void VerifyNoRequests()
        {
            _mockHandler
                .Protected()
                .Verify(
                    "SendAsync",
                    Times.Never(),
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>());
        }
    }
}
