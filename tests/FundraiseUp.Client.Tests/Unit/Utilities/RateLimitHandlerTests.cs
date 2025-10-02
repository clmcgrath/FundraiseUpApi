using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using FundraiseUp.Client.Configuration;
using FundraiseUp.Client.Exceptions;
using FundraiseUp.Client.Utilities;
using Microsoft.Extensions.Logging;
using Xunit;

namespace FundraiseUp.Client.Tests.Unit.Utilities
{
    /// <summary>
    /// Unit tests for the RateLimitHandler class.
    /// </summary>
    public class RateLimitHandlerTests
    {
        private readonly FundraiseUpClientOptions _defaultOptions;

        public RateLimitHandlerTests()
        {
            _defaultOptions = new FundraiseUpClientOptions
            {
                ApiKey = "test-key",
                BaseUrl = "https://api.fundraiseup.com",
                MaxConcurrentRequests = 2,
                MaxQueueSize = 5,
                QueueTimeout = TimeSpan.FromSeconds(1),
                RateLimitStrategy = RateLimitStrategy.Exception
            };
        }

        [Fact]
        public void Constructor_WithValidOptions_ShouldInitializeCorrectly()
        {
            // Arrange & Act
            var handler = new RateLimitHandler(_defaultOptions);

            // Assert
            handler.Should().NotBeNull();
        }

        [Fact]
        public void Constructor_WithNullOptions_ShouldThrowArgumentNullException()
        {
            // Arrange & Act
            Action act = () => new RateLimitHandler(null!);

            // Assert
            act.Should().Throw<ArgumentNullException>()
                .WithParameterName("options");
        }

        [Fact]
        public void Constructor_WithLogger_ShouldInitializeCorrectly()
        {
            // Arrange
            var logger = new TestLogger<RateLimitHandler>();

            // Act
            var handler = new RateLimitHandler(_defaultOptions, logger);

            // Assert
            handler.Should().NotBeNull();
        }

        [Theory]
        [InlineData(RateLimitStrategy.Exception)]
        [InlineData(RateLimitStrategy.Queue)]
        [InlineData(RateLimitStrategy.Retry)]
        public void Constructor_WithDifferentStrategies_ShouldInitializeCorrectly(RateLimitStrategy strategy)
        {
            // Arrange
            var options = new FundraiseUpClientOptions
            {
                ApiKey = "test-key",
                BaseUrl = "https://api.fundraiseup.com",
                RateLimitStrategy = strategy,
                MaxConcurrentRequests = 3,
                MaxQueueSize = 10,
                QueueTimeout = TimeSpan.FromSeconds(5)
            };

            // Act
            var handler = new RateLimitHandler(options);

            // Assert
            handler.Should().NotBeNull();
        }

        [Fact]
        public void Dispose_ShouldDisposeResourcesCorrectly()
        {
            // Arrange
            var handler = new RateLimitHandler(_defaultOptions);

            // Act & Assert (should not throw)
            handler.Dispose();
        }
    }

    /// <summary>
    /// Simple test logger for testing purposes.
    /// </summary>
    public class TestLogger<T> : ILogger<T>
    {
        private class NullScope : IDisposable
        {
            public static readonly NullScope Instance = new NullScope();
            private NullScope() { }
            public void Dispose() { }
        }

        public IDisposable BeginScope<TState>(TState state) => NullScope.Instance;
        public bool IsEnabled(Microsoft.Extensions.Logging.LogLevel logLevel) => true;
        public void Log<TState>(
            Microsoft.Extensions.Logging.LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            // Simple test logger - just store the message if needed
        }
    }
}
