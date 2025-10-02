using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace FundraiseUp.Client.Tests.TestHelpers.Mocking
{
    /// <summary>
    /// Simplified mock HTTP message handler for testing rate limiting scenarios
    /// </summary>
    public class MockHttpMessageHandler : HttpMessageHandler
    {
        private readonly Queue<HttpResponseMessage> _responses = new Queue<HttpResponseMessage>();
        private readonly List<HttpRequestMessage> _requests = new List<HttpRequestMessage>();
        private int _callCount = 0;

        /// <summary>
        /// Gets the number of HTTP calls made
        /// </summary>
        public int CallCount => _callCount;

        /// <summary>
        /// Gets all requests that were made
        /// </summary>
        public IReadOnlyList<HttpRequestMessage> Requests => _requests.AsReadOnly();

        /// <summary>
        /// Queues a response to be returned on the next HTTP call
        /// </summary>
        public void QueueResponse(HttpResponseMessage response)
        {
            _responses.Enqueue(response);
        }

        /// <summary>
        /// Queues a 429 (Too Many Requests) response with optional Retry-After header
        /// </summary>
        public void Queue429Response(string? retryAfter = null)
        {
            var response = new HttpResponseMessage((HttpStatusCode)429);
            if (!string.IsNullOrEmpty(retryAfter))
            {
                response.Headers.Add("Retry-After", retryAfter);
            }
            QueueResponse(response);
        }

        /// <summary>
        /// Queues a successful 200 OK response
        /// </summary>
        public void QueueSuccessResponse(string content = "{\"success\": true}")
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(content)
            };
            QueueResponse(response);
        }

        /// <summary>
        /// Queues multiple 429 responses followed by a success response
        /// </summary>
        public void QueueRateLimitSequence(int rateLimitCount, string? retryAfter = null)
        {
            for (int i = 0; i < rateLimitCount; i++)
            {
                Queue429Response(retryAfter);
            }
            QueueSuccessResponse();
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Interlocked.Increment(ref _callCount);
            _requests.Add(request);

            if (_responses.Count == 0)
            {
                // Default to success if no responses queued
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("{\"default\": true}")
                });
            }

            return Task.FromResult(_responses.Dequeue());
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                while (_responses.Count > 0)
                {
                    _responses.Dequeue()?.Dispose();
                }

                foreach (var request in _requests)
                {
                    request?.Dispose();
                }
            }
            base.Dispose(disposing);
        }
    }
}
