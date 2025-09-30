# FundraiseUp .NET Client Library

[![NuGet](https://img.shields.io/nuget/v/FundraiseUp.Client.svg)](https://www.nuget.org/packages/FundraiseUp.Client)
[![Build Status](https://github.com/YourOrg/FundraiseUpApi/workflows/CI/badge.svg)](https://github.com/YourOrg/FundraiseUpApi/actions)
[![License](https://img.shields.io/github/license/YourOrg/FundraiseUpApi.svg)](LICENSE)

A modern, fluent .NET client library for consuming the FundraiseUp API with comprehensive support for all endpoints, dependency injection, and enterprise-grade reliability.

## ✨ Features

- **Fluent API Design** - Intuitive, discoverable interface with IntelliSense support
- **Dependency Injection Ready** - Native support for Microsoft DI with configuration options
- **Async-First** - All operations use async/await patterns with CancellationToken support
- **Enterprise-Grade** - Configurable retry policies, rate limiting, and comprehensive error handling
- **Multi-Framework** - Targets .NET Standard 2.0 and .NET 6+ for broad compatibility
- **Comprehensive Logging** - Structured logging with Microsoft.Extensions.Logging
- **Secure by Default** - HTTPS enforcement and secure credential management

## 🚀 Quick Start

### Installation

```bash
dotnet add package FundraiseUp.Client
```

### Basic Usage

```csharp
using FundraiseUp.Client;

// Simple client creation
var client = FundraiseUpClient.Create()
    .WithApiKey("your-api-key")
    .WithTimeout(TimeSpan.FromSeconds(30))
    .Build();

// Create a donation
var donation = await client.CreateDonationAsync(new DonationRequest
{
    Amount = 100.00m,
    Currency = "USD",
    CampaignId = "campaign-123"
});
```

### Dependency Injection

```csharp
// Program.cs
builder.Services.AddFundraiseUpClient(options =>
{
    options.ApiKey = builder.Configuration["FundraiseUp:ApiKey"];
    options.RateLimitStrategy = RateLimitStrategy.Queue;
});

// Usage in service
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

## 📚 Documentation

- [**Getting Started**](getting-started.md) - Installation and basic setup
- [**Configuration Guide**](configuration.md) - Comprehensive configuration options
- [**API Reference**](api-reference.md) - Complete API documentation
- [**Examples**](examples.md) - Common usage patterns and scenarios
- [**Error Handling**](error-handling.md) - Exception types and handling strategies
- [**Performance Guide**](performance.md) - Optimization tips and best practices

## 🏗️ Architecture

This library follows constitutional principles ensuring:

- **Library-First Architecture** - Standalone, reusable design
- **Developer Experience Focus** - Fluent, discoverable APIs
- **Microsoft DI Integration** - Native dependency injection support
- **Test-Driven Development** - Comprehensive test coverage
- **Enterprise-Grade Reliability** - Production-ready error handling
- **Async-First Architecture** - Modern async/await patterns
- **Security-First Design** - Secure credential handling
- **Performance Optimized** - Efficient resource management

## 🤝 Contributing

We welcome contributions! Please see our [Contributing Guide](CONTRIBUTING.md) for details.

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](../LICENSE) file for details.

## 🆘 Support

- [Issues](https://github.com/YourOrg/FundraiseUpApi/issues) - Bug reports and feature requests
- [Discussions](https://github.com/YourOrg/FundraiseUpApi/discussions) - Community support and questions
- [Documentation](https://yourorgg.github.io/FundraiseUpApi/) - Comprehensive guides and API reference