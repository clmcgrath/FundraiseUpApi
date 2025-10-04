using System;
using System.Threading.Tasks;
using FluentAssertions;
using FundraiseUp.Client;
using FundraiseUp.Client.Configuration;
using FundraiseUp.Client.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Xunit;

namespace FundraiseUp.Client.Tests.Contracts
{
    /// <summary>
    /// Contract tests for fluent configuration validation - these MUST FAIL before implementation.
    /// </summary>
    public class FluentConfigurationContractTests
    {
        [Fact]
        public void AddFundraiseUpClient_WithValidConfiguration_ShouldRegisterServices()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            services.AddFundraiseUpClient(options =>
            {
                options.ApiKey = "test-api-key";
                options.BaseUrl = "https://api.fundraiseup.com";
                options.Timeout = TimeSpan.FromSeconds(30);
            });

            // Assert
            var serviceProvider = services.BuildServiceProvider();
            var client = serviceProvider.GetService<IFundraiseUpClient>();
            client.Should().NotBeNull();
        }

        [Fact]
        public void AddFundraiseUpClient_WithInvalidApiKey_ShouldThrowValidationException()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act & Assert
            Action act = () => services.AddFundraiseUpClient(options =>
            {
                options.ApiKey = ""; // Invalid empty API key
                options.BaseUrl = "https://api.fundraiseup.com";
            });

            act.Should().Throw<FundraiseUpConfigurationException>()
                .WithMessage("*API key*required*");
        }

        [Fact]
        public void AddFundraiseUpClient_WithInvalidBaseUrl_ShouldThrowValidationException()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act & Assert
            Action act = () => services.AddFundraiseUpClient(options =>
            {
                options.ApiKey = "valid-api-key";
                options.BaseUrl = "invalid-url"; // Invalid URL format
            });

            act.Should().Throw<FundraiseUpConfigurationException>()
                .WithMessage("*Base URL*valid*");
        }

        [Fact]
        public void FundraiseUpClientOptions_WithDefaultValues_ShouldHaveReasonableDefaults()
        {
            // Arrange & Act
            var options = new FundraiseUpClientOptions();

            // Assert
            options.Timeout.Should().Be(TimeSpan.FromSeconds(30));
            options.MaxRetryAttempts.Should().Be(3);
            options.RetryDelay.Should().Be(TimeSpan.FromSeconds(1));
            options.EnableLogging.Should().BeTrue();
            options.LogLevel.Should().Be(FundraiseUp.Client.Configuration.LogLevel.Information);
        }

        [Fact]
        public void FundraiseUpClientOptions_WithCustomValues_ShouldAcceptConfiguration()
        {
            // Arrange & Act
            var options = new FundraiseUpClientOptions
            {
                ApiKey = "custom-api-key",
                BaseUrl = "https://custom.api.com",
                Timeout = TimeSpan.FromMinutes(2),
                MaxRetryAttempts = 5,
                RetryDelay = TimeSpan.FromSeconds(2),
                EnableLogging = false,
                LogLevel = FundraiseUp.Client.Configuration.LogLevel.Warning,
                UserAgent = "CustomClient/1.0"
            };

            // Assert
            options.ApiKey.Should().Be("custom-api-key");
            options.BaseUrl.Should().Be("https://custom.api.com");
            options.Timeout.Should().Be(TimeSpan.FromMinutes(2));
            options.MaxRetryAttempts.Should().Be(5);
            options.RetryDelay.Should().Be(TimeSpan.FromSeconds(2));
            options.EnableLogging.Should().BeFalse();
            options.LogLevel.Should().Be(FundraiseUp.Client.Configuration.LogLevel.Warning);
            options.UserAgent.Should().Be("CustomClient/1.0");
        }

        [Fact]
        public void ValidateConfiguration_WithValidOptions_ShouldNotThrow()
        {
            // Arrange
            var options = new FundraiseUpClientOptions
            {
                ApiKey = "valid-api-key",
                BaseUrl = "https://api.fundraiseup.com",
                Timeout = TimeSpan.FromSeconds(30)
            };

            // Act & Assert
            Action act = () => FundraiseUpClientOptionsValidator.Validate(options);
            act.Should().NotThrow();
        }

        [Theory]
        [InlineData("", "https://api.fundraiseup.com", "API key")]
        [InlineData(null, "https://api.fundraiseup.com", "API key")]
        [InlineData("valid-key", "", "Base URL")]
        [InlineData("valid-key", null, "Base URL")]
        [InlineData("valid-key", "invalid-url", "Base URL")]
        public void ValidateConfiguration_WithInvalidOptions_ShouldThrowWithSpecificMessage(
            string apiKey, string baseUrl, string expectedErrorPart)
        {
            // Arrange
            var options = new FundraiseUpClientOptions
            {
                ApiKey = apiKey,
                BaseUrl = baseUrl
            };

            // Act & Assert
            Action act = () => FundraiseUpClientOptionsValidator.Validate(options);
            act.Should().Throw<FundraiseUpConfigurationException>()
                .WithMessage($"*{expectedErrorPart}*");
        }

        [Fact]
        public void FundraiseUpClient_WithValidOptions_ShouldInitializeCorrectly()
        {
            // Arrange
            var options = new FundraiseUpClientOptions
            {
                ApiKey = "test-api-key",
                BaseUrl = "https://api.fundraiseup.com"
            };

            // Act
            var client = new FundraiseUpClient(options);

            // Assert
            client.Should().NotBeNull();
            client.Donations.Should().NotBeNull();
            client.Campaigns.Should().NotBeNull();
            client.Supporters.Should().NotBeNull();
        }

        [Fact]
        public void FundraiseUpClient_WithStringApiKey_ShouldInitializeWithDefaults()
        {
            // Arrange
            var apiKey = "test-api-key";

            // Act
            var client = new FundraiseUpClient(apiKey);

            // Assert
            client.Should().NotBeNull();
            client.Donations.Should().NotBeNull();
            client.Campaigns.Should().NotBeNull();
            client.Supporters.Should().NotBeNull();
        }
    }


}
