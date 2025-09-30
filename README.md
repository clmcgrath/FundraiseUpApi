# FundraiseUp .NET Client Library

[![NuGet](https://img.shields.io/nuget/v/FundraiseUp.Client.svg)](https://www.nuget.org/packages/FundraiseUp.Client)
[![Build and Test](https://github.com/YourOrg/FundraiseUpApi/actions/workflows/build-test.yml/badge.svg)](https://github.com/YourOrg/FundraiseUpApi/actions/workflows/build-test.yml)
[![Codacy Badge](https://app.codacy.com/project/badge/Grade/[PROJECT_ID])](https://app.codacy.com/gh/YourOrg/FundraiseUpApi/dashboard?utm_source=gh&utm_medium=referral&utm_content=&utm_campaign=Badge_grade)
[![Codacy Badge](https://app.codacy.com/project/badge/Coverage/[PROJECT_ID])](https://app.codacy.com/gh/YourOrg/FundraiseUpApi/dashboard?utm_source=gh&utm_medium=referral&utm_content=&utm_campaign=Badge_coverage)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)
[![.NET](https://img.shields.io/badge/.NET-Standard%202.0%20%7C%20.NET%206+-512BD4.svg)](https://dotnet.microsoft.com/)

A modern, fluent .NET client library for consuming the FundraiseUp API with comprehensive support for all endpoints, dependency injection, and enterprise-grade reliability.

## ✨ Features

- **🎯 Fluent API Design** - Intuitive, discoverable interface with full IntelliSense support
- **💉 Dependency Injection Ready** - Native Microsoft DI integration with IOptions pattern
- **⚡ Async-First Architecture** - All operations use async/await with CancellationToken support
- **🛡️ Enterprise-Grade Reliability** - Configurable retry policies, rate limiting, and comprehensive error handling
- **🔄 Multi-Framework Support** - Targets .NET Standard 2.0 and .NET 6+ for maximum compatibility
- **📊 Comprehensive Observability** - Structured logging with Microsoft.Extensions.Logging
- **🔒 Security-First Design** - HTTPS enforcement, secure credential management, no sensitive data exposure
- **📜 OpenAPI Compliant** - Strict adherence to FundraiseUp OpenAPI 3.0+ specifications
- **⚖️ MIT Licensed** - Permissive open source license for commercial and personal use

## 🚀 Quick Start

### Installation

```bash
# .NET CLI
dotnet add package FundraiseUp.Client

# Package Manager Console
Install-Package FundraiseUp.Client

# PackageReference
<PackageReference Include="FundraiseUp.Client" Version="1.0.0" />
```

### Simple Usage

```csharp
using FundraiseUp.Client;

// Create client with fluent builder
var client = FundraiseUpClient.Create()
    .WithApiKey("your-api-key")
    .WithTimeout(TimeSpan.FromSeconds(30))
    .WithRateLimitStrategy(RateLimitStrategy.Queue)
    .Build();

// Create a donation
var donation = await client.CreateDonationAsync(new DonationRequest
{
    Amount = 100.00m,
    Currency = "USD",
    CampaignId = "campaign-123"
});

Console.WriteLine($"Donation created: {donation.Id}, Status: {donation.PaymentStatus}");
```

### Dependency Injection (Recommended)

```csharp
// Program.cs (.NET 6+)
using FundraiseUp.Client.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddFundraiseUpClient(options =>
{
    options.ApiKey = builder.Configuration["FundraiseUp:ApiKey"];
    options.RateLimitStrategy = RateLimitStrategy.Queue;
    options.LogLevel = LogLevel.Information;
});

var app = builder.Build();
```

```csharp
// Service usage
public class DonationService
{
    private readonly IFundraiseUpClient _client;
    
    public DonationService(IFundraiseUpClient client)
    {
        _client = client;
    }
    
    public async Task<DonationResponse> ProcessDonationAsync(decimal amount, string campaignId)
    {
        return await _client.CreateDonationAsync(new DonationRequest
        {
            Amount = amount,
            Currency = "USD",
            CampaignId = campaignId
        });
    }
}
```

## 📋 Supported Operations

### Donations
- ✅ Create donations with payment processing
- ✅ Retrieve donation details and history
- ✅ Update donation information
- ✅ Query donations with filtering and pagination

### Campaigns
- ✅ Create and manage fundraising campaigns
- ✅ Update campaign details and goals
- ✅ Retrieve campaign statistics and progress
- ✅ List campaigns with advanced filtering

### Donors
- ✅ Create and manage donor profiles
- ✅ Update donor information and preferences
- ✅ Retrieve donor history and statistics
- ✅ Search and filter donor database

## ⚙️ Configuration

### Fluent Configuration
```csharp
var client = FundraiseUpClient.Create()
    .WithApiKey("your-api-key")
    .WithBaseUrl("https://api.fundraiseup.com")
    .WithTimeout(TimeSpan.FromSeconds(30))
    .WithRateLimitStrategy(RateLimitStrategy.Retry)
    .WithRetryPolicy(new RetryConfiguration
    {
        MaxRetries = 3,
        BaseDelay = TimeSpan.FromSeconds(1),
        BackoffMultiplier = 2.0
    })
    .WithLogging(LogLevel.Information)
    .Build();
```

### Configuration File (appsettings.json)
```json
{
  "FundraiseUp": {
    "ApiKey": "your-api-key",
    "BaseUrl": "https://api.fundraiseup.com",
    "TimeoutSeconds": 30,
    "RateLimitStrategy": "Queue",
    "RetryOptions": {
      "MaxRetries": 3,
      "BaseDelaySeconds": 1,
      "MaxDelaySeconds": 30,
      "BackoffMultiplier": 2.0
    },
    "LogLevel": "Information"
  }
}
```

### Environment Variables
```bash
FUNDRAISEUP_API_KEY=your-api-key
FUNDRAISEUP_BASE_URL=https://api.fundraiseup.com
FUNDRAISEUP_TIMEOUT_SECONDS=30
```

## 🔄 Rate Limiting

FundraiseUp API has a 3 concurrent request limit per account. The client provides configurable strategies:

```csharp
// Automatic retry with exponential backoff
options.RateLimitStrategy = RateLimitStrategy.Retry;

// Queue requests when limit is reached
options.RateLimitStrategy = RateLimitStrategy.Queue;

// Throw exception immediately when rate limited
options.RateLimitStrategy = RateLimitStrategy.Exception;
```

## 🛠️ Error Handling

The library provides comprehensive error handling with specific exception types:

```csharp
try
{
    var donation = await client.CreateDonationAsync(request);
}
catch (FundraiseUpRateLimitException ex)
{
    // Handle rate limiting (429 errors)
    Console.WriteLine($"Rate limited. Retry after: {ex.RetryAfter}");
}
catch (FundraiseUpValidationException ex)
{
    // Handle validation errors (400 errors)
    Console.WriteLine($"Validation failed: {ex.Message}");
    foreach (var error in ex.ValidationErrors)
    {
        Console.WriteLine($"- {error.Field}: {error.Message}");
    }
}
catch (FundraiseUpAuthenticationException ex)
{
    // Handle authentication errors (401 errors)
    Console.WriteLine("Invalid API key or authentication failed");
}
catch (FundraiseUpApiException ex)
{
    // Handle general API errors
    Console.WriteLine($"API Error [{ex.ErrorCode}]: {ex.Message}");
}
```

## 📊 Logging

The library integrates with Microsoft.Extensions.Logging for comprehensive observability:

```csharp
// Configure logging levels
builder.Services.AddFundraiseUpClient(options =>
{
    options.LogLevel = LogLevel.Information; // Error, Warning, Information, Debug
});

// Example log output
[2025-09-29 10:30:15] [Information] FundraiseUp.Client: Creating donation for campaign campaign-123
[2025-09-29 10:30:16] [Information] FundraiseUp.Client: Donation created successfully (ID: donation-456)
[2025-09-29 10:30:16] [Debug] FundraiseUp.Client.Http: POST https://api.fundraiseup.com/v1/donations completed in 1.2s
```

## 🧪 Testing

The library includes comprehensive test support:

```csharp
// Mock the client for unit testing
var mockClient = new Mock<IFundraiseUpClient>();
mockClient.Setup(x => x.CreateDonationAsync(It.IsAny<DonationRequest>(), default))
    .ReturnsAsync(new DonationResponse { Id = "test-donation" });

// Integration testing helper
var testClient = FundraiseUpClient.Create()
    .WithApiKey("test-api-key")
    .WithBaseUrl("https://api-sandbox.fundraiseup.com") // Use sandbox
    .Build();
```

## 🏗️ Architecture & Design

This library follows constitutional design principles:

- **Library-First Architecture** - Standalone, reusable design with clear purpose
- **Developer Experience Focus** - Fluent APIs with IntelliSense discoverability
- **Microsoft DI Integration** - Native dependency injection with IOptions pattern
- **Test-Driven Development** - Comprehensive test coverage with contract validation
- **Enterprise-Grade Reliability** - Production-ready error handling and retry logic
- **Async-First Architecture** - Modern async/await patterns throughout
- **Security-First Design** - Secure credential handling and HTTPS enforcement
- **Performance Optimized** - Efficient resource management and connection pooling
- **OpenAPI Compliant** - Strict adherence to API specifications
- **Comprehensive Documentation** - Full API reference and usage examples

## 📚 Documentation

- **[Getting Started](docs/getting-started.md)** - Installation and basic setup
- **[Configuration Guide](docs/configuration.md)** - Comprehensive configuration options
- **[API Reference](docs/api-reference.md)** - Complete method documentation
- **[Examples](docs/examples.md)** - Common usage patterns and scenarios
- **[Error Handling](docs/error-handling.md)** - Exception types and handling strategies
- **[Performance Guide](docs/performance.md)** - Optimization tips and best practices

## 🔧 Development

### Prerequisites
- .NET 6.0 SDK or later
- Git
- Visual Studio 2022 or Visual Studio Code

### Building from Source
```bash
# Clone the repository
git clone https://github.com/YourOrg/FundraiseUpApi.git
cd FundraiseUpApi

# Restore dependencies
dotnet restore

# Build the solution
dotnet build --configuration Release

# Run tests
dotnet test --configuration Release
```

### Branching Model
This project uses GitHub Flow with GitVersion for automated semantic versioning:

- **`master`** - Production releases only
- **`dev`** - Integration branch for feature development  
- **`stable`** - Latest stable release for hotfixes
- **`feature/*`** - Feature development branches
- **`hotfix/*`** - Critical fixes for production issues

### Contributing
We welcome contributions! Please see our [Contributing Guide](CONTRIBUTING.md) for details on:
- Code of conduct and community guidelines
- Development workflow and branching strategy
- Testing requirements and quality gates
- Pull request process and review guidelines

## 📈 Roadmap

- [ ] **v1.1** - Advanced filtering and search capabilities
- [ ] **v1.2** - Webhook support for real-time events
- [ ] **v1.3** - Bulk operations and batch processing
- [ ] **v2.0** - GraphQL support and enhanced performance

## 🤝 Support & Community

- **📖 Documentation**: [https://yourorgg.github.io/FundraiseUpApi/](https://yourorgg.github.io/FundraiseUpApi/)
- **🐛 Bug Reports**: [GitHub Issues](https://github.com/YourOrg/FundraiseUpApi/issues)
- **💡 Feature Requests**: [GitHub Discussions](https://github.com/YourOrg/FundraiseUpApi/discussions)
- **❓ Questions**: [Stack Overflow](https://stackoverflow.com/questions/tagged/fundraiseup-dotnet) (tag: `fundraiseup-dotnet`)

## 📜 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🙏 Acknowledgments

- [FundraiseUp](https://fundraiseup.com/) for providing the API and documentation
- [Microsoft](https://microsoft.com/) for the .NET ecosystem and tooling
- All [contributors](https://github.com/YourOrg/FundraiseUpApi/contributors) who help make this library better

---

<div align="center">
  <sub>Built with ❤️ by the FundraiseUpApi team</sub>
</div>