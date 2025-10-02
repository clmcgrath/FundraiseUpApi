using System;
using FluentAssertions;
using FundraiseUp.Client.Configuration;
using FundraiseUp.Client.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace FundraiseUp.Client.Tests.Integration
{
    /// <summary>
    /// Integration tests for configuration validation scenarios.
    /// Tests all configuration validation including startup-time and runtime validation.
    /// </summary>
    public class ConfigurationValidationTests
    {
        [Fact]
        public void Configuration_WithMissingApiKey_ShouldThrowConfigurationException()
        {
            // Arrange
            var options = new FundraiseUpClientOptions
            {
                // ApiKey intentionally missing
                BaseUrl = "https://api.fundraiseup.com"
            };

            // Act & Assert
            var act = () => FundraiseUpClientOptionsValidator.Validate(options);
            act.Should().Throw<FundraiseUpConfigurationException>()
                .WithMessage("*API key*");
        }

        [Fact]
        public void Configuration_WithEmptyApiKey_ShouldThrowConfigurationException()
        {
            // Arrange
            var options = new FundraiseUpClientOptions
            {
                ApiKey = "",
                BaseUrl = "https://api.fundraiseup.com"
            };

            // Act & Assert
            var act = () => FundraiseUpClientOptionsValidator.Validate(options);
            act.Should().Throw<FundraiseUpConfigurationException>()
                .WithMessage("*API key*");
        }

        [Fact]
        public void Configuration_WithWhitespaceApiKey_ShouldThrowConfigurationException()
        {
            // Arrange
            var options = new FundraiseUpClientOptions
            {
                ApiKey = "   ",
                BaseUrl = "https://api.fundraiseup.com"
            };

            // Act & Assert
            var act = () => FundraiseUpClientOptionsValidator.Validate(options);
            act.Should().Throw<FundraiseUpConfigurationException>()
                .WithMessage("*API key*");
        }

        [Fact]
        public void Configuration_WithMissingBaseUrl_ShouldThrowConfigurationException()
        {
            // Arrange
            var options = new FundraiseUpClientOptions
            {
                ApiKey = "test-api-key",
                BaseUrl = "" // BaseUrl explicitly set to empty
            };

            // Act & Assert
            var act = () => FundraiseUpClientOptionsValidator.Validate(options);
            act.Should().Throw<FundraiseUpConfigurationException>()
                .WithMessage("*Base URL*");
        }

        [Fact]
        public void Configuration_WithEmptyBaseUrl_ShouldThrowConfigurationException()
        {
            // Arrange
            var options = new FundraiseUpClientOptions
            {
                ApiKey = "test-api-key",
                BaseUrl = ""
            };

            // Act & Assert
            var act = () => FundraiseUpClientOptionsValidator.Validate(options);
            act.Should().Throw<FundraiseUpConfigurationException>()
                .WithMessage("*Base URL*");
        }

        [Theory]
        [InlineData("not-a-url")]
        [InlineData("ftp://invalid.com")]
        [InlineData("http://")]
        [InlineData("invalid-protocol://api.fundraiseup.com")]
        public void Configuration_WithMalformedBaseUrl_ShouldThrowConfigurationException(string invalidUrl)
        {
            // Arrange
            var options = new FundraiseUpClientOptions
            {
                ApiKey = "test-api-key",
                BaseUrl = invalidUrl
            };

            // Act & Assert
            var act = () => FundraiseUpClientOptionsValidator.Validate(options);
            act.Should().Throw<FundraiseUpConfigurationException>()
                .WithMessage("*Base URL*");
        }

        [Fact]
        public void Configuration_WithHttpBaseUrl_ShouldThrowConfigurationException()
        {
            // Arrange - HTTP should not be allowed, only HTTPS
            var options = new FundraiseUpClientOptions
            {
                ApiKey = "test-api-key",
                BaseUrl = "http://api.fundraiseup.com" // HTTP not allowed
            };

            // Act & Assert
            var act = () => FundraiseUpClientOptionsValidator.Validate(options);
            act.Should().Throw<FundraiseUpConfigurationException>()
                .WithMessage("*HTTPS*");
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(11)] // Max should be 10 per FundraiseUp API limits
        public void Configuration_WithInvalidMaxRetryAttempts_ShouldThrowConfigurationException(int invalidRetries)
        {
            // Arrange
            var options = new FundraiseUpClientOptions
            {
                ApiKey = "test-api-key",
                BaseUrl = "https://api.fundraiseup.com",
                MaxRetryAttempts = invalidRetries
            };

            // Act & Assert
            var act = () => FundraiseUpClientOptionsValidator.Validate(options);
            act.Should().Throw<FundraiseUpConfigurationException>()
                .WithMessage("*retry attempts*");
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(11)] // Max should be 10 per FundraiseUp API limits
        public void Configuration_WithInvalidMaxConcurrentRequests_ShouldThrowConfigurationException(int invalidConcurrency)
        {
            // Arrange
            var options = new FundraiseUpClientOptions
            {
                ApiKey = "test-api-key",
                BaseUrl = "https://api.fundraiseup.com",
                MaxConcurrentRequests = invalidConcurrency
            };

            // Act & Assert
            var act = () => FundraiseUpClientOptionsValidator.Validate(options);
            act.Should().Throw<FundraiseUpConfigurationException>()
                .WithMessage("*concurrent*");
        }

        [Fact]
        public void Configuration_WithNegativeTimeout_ShouldThrowConfigurationException()
        {
            // Arrange
            var options = new FundraiseUpClientOptions
            {
                ApiKey = "test-api-key",
                BaseUrl = "https://api.fundraiseup.com",
                Timeout = TimeSpan.FromSeconds(-1)
            };

            // Act & Assert
            var act = () => FundraiseUpClientOptionsValidator.Validate(options);
            act.Should().Throw<FundraiseUpConfigurationException>()
                .WithMessage("*timeout*");
        }

        [Fact]
        public void Configuration_WithValidOptions_ShouldNotThrow()
        {
            // Arrange
            var options = new FundraiseUpClientOptions
            {
                ApiKey = "test-api-key",
                BaseUrl = "https://api.fundraiseup.com",
                MaxRetryAttempts = 3,
                MaxConcurrentRequests = 3,
                Timeout = TimeSpan.FromSeconds(30),
                EnableLogging = true,
                LogLevel = LogLevel.Information
            };

            // Act & Assert
            var act = () => FundraiseUpClientOptionsValidator.Validate(options);
            act.Should().NotThrow();
        }

        [Fact]
        public void DependencyInjection_WithInvalidConfiguration_ShouldThrowOnStartup()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act & Assert - Invalid configuration should be caught during DI setup
            var act = () => services.AddFundraiseUpClient(options =>
            {
                options.ApiKey = ""; // Invalid empty API key
                options.BaseUrl = "https://api.fundraiseup.com";
            });

            // Configuration validation should occur during service registration
            act.Should().Throw<FundraiseUpConfigurationException>()
                .WithMessage("*API key*");
        }

        [Fact]
        public void DependencyInjection_WithValidConfiguration_ShouldCreateClient()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddFundraiseUpClient(options =>
            {
                options.ApiKey = "test-api-key";
                options.BaseUrl = "https://api.fundraiseup.com";
                options.MaxRetryAttempts = 3;
                options.EnableLogging = true;
            });

            // Act
            var serviceProvider = services.BuildServiceProvider();
            var client = serviceProvider.GetRequiredService<IFundraiseUpClient>();

            // Assert
            client.Should().NotBeNull();
            client.Should().BeAssignableTo<IFundraiseUpClient>();
        }

        [Fact]
        public void RuntimeConfiguration_WithFluentBuilder_ShouldValidateConfiguration()
        {
            // Arrange & Act - Test runtime configuration validation
            var act = () => new FundraiseUpClient(new FundraiseUpClientOptions
            {
                ApiKey = "", // Invalid
                BaseUrl = "https://api.fundraiseup.com"
            });

            // Assert
            act.Should().Throw<FundraiseUpConfigurationException>()
                .WithMessage("*API key*");
        }

        [Fact]
        public void Configuration_LogicalConflicts_ShouldBeDetected()
        {
            // Arrange - Test logical conflicts in configuration
            var options = new FundraiseUpClientOptions
            {
                ApiKey = "test-api-key",
                BaseUrl = "https://api.test.com",
                MaxRetryAttempts = 5,
                RetryDelay = TimeSpan.FromSeconds(10),
                Timeout = TimeSpan.FromSeconds(5) // Timeout shorter than retry delay
            };

            // Act & Assert - Timeout should be longer than retry delay * max attempts
            var act = () => FundraiseUpClientOptionsValidator.Validate(options);
            act.Should().Throw<FundraiseUpConfigurationException>()
                .WithMessage("*timeout*");
        }
    }
}
