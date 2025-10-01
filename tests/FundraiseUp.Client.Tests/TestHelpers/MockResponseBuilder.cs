using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using FundraiseUp.Client.Configuration;
using FundraiseUp.Client.Models;

namespace FundraiseUp.Client.Tests.TestHelpers
{
    /// <summary>
    /// Builder for creating mock HTTP responses for testing
    /// </summary>
    public class MockResponseBuilder
    {
        /// <summary>
        /// Creates a successful response with JSON content
        /// </summary>
        public static HttpResponseMessage CreateJsonResponse<T>(T data, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            var json = JsonSerializer.Serialize(data, JsonConfiguration.DefaultOptions);
            return new HttpResponseMessage(statusCode)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
        }

        /// <summary>
        /// Creates a paginated list response
        /// </summary>
        public static HttpResponseMessage CreatePaginatedResponse<T>(IEnumerable<T> items, int page = 1, int pageSize = 10, int totalCount = 100)
        {
            var pagedResult = new Models.PagedResult<T>
            {
                Items = new List<T>(items),
                CurrentPage = page,
                PageSize = pageSize,
                TotalCount = totalCount
            };

            return CreateJsonResponse(pagedResult);
        }

        /// <summary>
        /// Creates an error response
        /// </summary>
        public static HttpResponseMessage CreateErrorResponse(HttpStatusCode statusCode, string? message = null)
        {
            var errorContent = message ?? GetDefaultErrorMessage(statusCode);
            return new HttpResponseMessage(statusCode)
            {
                Content = new StringContent(errorContent, Encoding.UTF8, "application/json")
            };
        }

        /// <summary>
        /// Creates a validation error response (422)
        /// </summary>
        public static HttpResponseMessage CreateValidationErrorResponse(Dictionary<string, string[]> errors)
        {
            var errorResponse = new
            {
                message = "Validation failed",
                errors = errors
            };

            var json = JsonSerializer.Serialize(errorResponse, JsonConfiguration.DefaultOptions);
            return new HttpResponseMessage((HttpStatusCode)422)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
        }

        /// <summary>
        /// Creates sample campaign data for testing
        /// </summary>
        public static Campaign CreateSampleCampaign(string id = "campaign-123")
        {
            return new Campaign
            {
                Id = id,
                Name = "Test Campaign",
                Description = "A sample campaign for testing",
                Goal = "10000.00",
                Currency = "usd",
                Status = CampaignStatus.Active,
                CreatedAt = DateTime.UtcNow.AddDays(-30),
                UpdatedAt = DateTime.UtcNow
            };
        }

        /// <summary>
        /// Creates sample donor data for testing
        /// </summary>
        public static Donor CreateSampleDonor(string id = "donor-123")
        {
            return new Donor
            {
                Id = id,
                Email = "test@example.com",
                FirstName = "John",
                LastName = "Doe",
                Phone = "+1234567890",
                CreatedAt = DateTimeOffset.UtcNow.AddDays(-60),
                UpdatedAt = DateTimeOffset.UtcNow,
                Address = new Address
                {
                    Street = "123 Test St",
                    City = "Test City",
                    State = "TS",
                    PostalCode = "12345",
                    Country = "US"
                }
            };
        }

        /// <summary>
        /// Creates sample donation response data for testing
        /// </summary>
        public static DonationResponse CreateSampleDonation(string id = "donation-123", string campaignId = "campaign-123", string donorId = "donor-123")
        {
            return new DonationResponse
            {
                Id = id,
                Amount = "100.00",
                Currency = "usd",
                Status = DonationStatus.Succeeded,
                CreatedAt = DateTime.UtcNow.AddHours(-2),
                Campaign = new CampaignResponse { Id = campaignId, Name = "Test Campaign" },
                Supporter = new EmbeddedSupporterResponse { Id = donorId, Email = "test@example.com" }
            };
        }

        private static string GetDefaultErrorMessage(HttpStatusCode statusCode)
        {
            return statusCode switch
            {
                HttpStatusCode.NotFound => "{\"message\":\"Resource not found\"}",
                HttpStatusCode.Unauthorized => "{\"message\":\"Unauthorized access\"}",
                HttpStatusCode.BadRequest => "{\"message\":\"Bad request\"}",
                HttpStatusCode.InternalServerError => "{\"message\":\"Internal server error\"}",
                _ => $"{{\"message\":\"HTTP {(int)statusCode} error\"}}"
            };
        }
    }
}
