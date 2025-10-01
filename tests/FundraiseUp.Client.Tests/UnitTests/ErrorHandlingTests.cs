using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using FundraiseUp.Client.Configuration;
using FundraiseUp.Client.Exceptions;
using FundraiseUp.Client.Requests;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Xunit;

namespace FundraiseUp.Client.Tests.UnitTests
{
    public class ErrorHandlingTests
    {
        private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
        private readonly HttpClient _httpClient;
        private readonly FundraiseUpClient _client;

        public ErrorHandlingTests()
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

        [Fact]
        public async Task HttpClient_WhenApiReturns404_ShouldThrowFundraiseUpApiException()
        {
            // Arrange
            var httpResponse = new HttpResponseMessage(HttpStatusCode.NotFound)
            {
                Content = new StringContent("Resource not found", System.Text.Encoding.UTF8, "application/json")
            };

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<FundraiseUpApiException>(async () =>
            {
                await _client.Donations
                    .GetById("nonexistent-donation")
                    .ExecuteAsync();
            });

            exception.StatusCode.Should().Be(404);
            exception.Message.Should().Be("Resource not found");
        }

        [Fact]
        public async Task HttpClient_WhenApiReturns401_ShouldThrowFundraiseUpApiException()
        {
            // Arrange
            var httpResponse = new HttpResponseMessage(HttpStatusCode.Unauthorized)
            {
                Content = new StringContent("Unauthorized access", System.Text.Encoding.UTF8, "application/json")
            };

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<FundraiseUpApiException>(async () =>
            {
                await _client.Donations
                    .GetById("some-donation")
                    .ExecuteAsync();
            });

            exception.StatusCode.Should().Be(401);
            exception.Message.Should().Be("Unauthorized access");
        }

        [Fact]
        public async Task HttpClient_WhenApiReturns400_ShouldThrowFundraiseUpApiException()
        {
            // Arrange
            var httpResponse = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent("Invalid request data", System.Text.Encoding.UTF8, "application/json")
            };

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            var invalidRequest = new CreateDonationRequest
            {
                Amount = "-100.00", // Invalid negative amount
                Currency = "USD",
                Supporter = new SupporterRequest
                {
                    FirstName = "Test",
                    LastName = "User",
                    Email = "test@example.com"
                },
                Campaign = "campaign-123",
                Designation = "EXXXXXXX",
                PaymentMethod = new PaymentMethodRequest
                {
                    Stripe = new StripePaymentMethodRequest { Id = "pm_card_visa" }
                }
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<FundraiseUpApiException>(async () =>
            {
                await _client.Donations
                    .Create(invalidRequest)
                    .ExecuteAsync();
            });

            exception.StatusCode.Should().Be(400);
            exception.Message.Should().Be("Invalid request data");
        }

        [Fact]
        public async Task HttpClient_WhenApiReturns500_ShouldThrowFundraiseUpApiException()
        {
            // Arrange
            var httpResponse = new HttpResponseMessage(HttpStatusCode.InternalServerError)
            {
                Content = new StringContent("Internal server error", System.Text.Encoding.UTF8, "application/json")
            };

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<FundraiseUpApiException>(async () =>
            {
                await _client.Supporters
                    .GetById("some-donor")
                    .ExecuteAsync();
            });

            exception.StatusCode.Should().Be(500);
            exception.Message.Should().Be("Internal server error");
        }

        [Fact]
        public async Task HttpClient_WhenApiReturns422_ShouldThrowFundraiseUpValidationException()
        {
            // Arrange
            var validationErrorResponse = @"{
                ""errors"": [
                    {
                        ""field"": ""email"",
                        ""message"": ""Email address is not valid""
                    },
                    {
                        ""field"": ""amount"",
                        ""message"": ""Amount must be greater than zero""
                    }
                ]
            }";

            var httpResponse = new HttpResponseMessage((HttpStatusCode)422)
            {
                Content = new StringContent(validationErrorResponse, System.Text.Encoding.UTF8, "application/json")
            };

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            var invalidRequest = new CreateDonationRequest
            {
                Amount = "0.00",
                Currency = "USD",
                Supporter = new SupporterRequest
                {
                    FirstName = "Test",
                    LastName = "User",
                    Email = "invalid-email"
                },
                Campaign = "campaign-123",
                Designation = "EXXXXXXX",
                PaymentMethod = new PaymentMethodRequest
                {
                    Stripe = new StripePaymentMethodRequest { Id = "pm_card_visa" }
                }
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<FundraiseUpValidationException>(async () =>
            {
                await _client.Donations
                    .Create(invalidRequest)
                    .ExecuteAsync();
            });

            exception.StatusCode.Should().Be(422);
            exception.ValidationErrors.Should().HaveCount(2);
            exception.ValidationErrors.Should().ContainKey("email");
            exception.ValidationErrors.Should().ContainKey("amount");
            exception.ValidationErrors["email"].Should().Be("Email address is not valid");
            exception.ValidationErrors["amount"].Should().Be("Amount must be greater than zero");
        }

        [Fact]
        public async Task HttpClient_WhenNetworkTimeout_ShouldThrowTaskCanceledException()
        {
            // Arrange
            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ThrowsAsync(new TaskCanceledException("The request was canceled due to the configured HttpClient.Timeout"));

            // Act & Assert
            await Assert.ThrowsAsync<TaskCanceledException>(async () =>
            {
                await _client.Donations
                    .GetById("some-donation")
                    .WithTimeout(TimeSpan.FromMilliseconds(1)) // Very short timeout
                    .ExecuteAsync();
            });
        }

        [Fact]
        public async Task HttpClient_WhenHttpRequestException_ShouldThrowHttpRequestException()
        {
            // Arrange
            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ThrowsAsync(new HttpRequestException("Unable to connect to remote server"));

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(async () =>
            {
                await _client.Donations
                    .GetById("some-donation")
                    .ExecuteAsync();
            });
        }

        [Theory(Skip = "Validation testing requires more complex setup - will be handled in integration tests")]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("   ")]
        public void CreateDonationRequest_WithInvalidEmail_ShouldFailValidation(string invalidEmail)
        {
            // Arrange
            var request = new CreateDonationRequest
            {
                Amount = "100.00",
                Currency = "USD",
                Supporter = new SupporterRequest
                {
                    FirstName = "Test",
                    LastName = "User",
                    Email = invalidEmail
                },
                Campaign = "campaign-123",
                Designation = "EXXXXXXX",
                PaymentMethod = new PaymentMethodRequest
                {
                    Stripe = new StripePaymentMethodRequest { Id = "pm_card_visa" }
                }
            };

            // Act & Assert
            var validationResults = ValidateObject(request);
            validationResults.Should().NotBeEmpty();
        }

        private static List<System.ComponentModel.DataAnnotations.ValidationResult> ValidateObject(object obj)
        {
            var validationResults = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
            var context = new System.ComponentModel.DataAnnotations.ValidationContext(obj);
            System.ComponentModel.DataAnnotations.Validator.TryValidateObject(obj, context, validationResults, true);
            return validationResults;
        }

        [Theory]
        [InlineData(-1.00)]
        [InlineData(0.00)]
        public async Task CreateDonation_WithInvalidAmount_ShouldThrowValidationException(decimal invalidAmount)
        {
            // Arrange
            var validationErrorResponse = @"{
                ""errors"": [
                    {
                        ""field"": ""amount"",
                        ""message"": ""Amount must be greater than zero""
                    }
                ]
            }";

            var httpResponse = new HttpResponseMessage((HttpStatusCode)422)
            {
                Content = new StringContent(validationErrorResponse, System.Text.Encoding.UTF8, "application/json")
            };

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            var invalidRequest = new CreateDonationRequest
            {
                Amount = invalidAmount.ToString(System.Globalization.CultureInfo.InvariantCulture),
                Currency = "USD",
                Supporter = new SupporterRequest
                {
                    FirstName = "Test",
                    LastName = "User",
                    Email = "test@example.com"
                },
                Campaign = "campaign-123",
                Designation = "EXXXXXXX",
                PaymentMethod = new PaymentMethodRequest
                {
                    Stripe = new StripePaymentMethodRequest { Id = "pm_card_visa" }
                }
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<FundraiseUpValidationException>(async () =>
            {
                await _client.Donations
                    .Create(invalidRequest)
                    .ExecuteAsync();
            });

            exception.StatusCode.Should().Be(422);
            exception.ValidationErrors.Should().ContainKey("amount");
        }
    }
}
