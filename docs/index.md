---
layout: default
title: Home
nav_ord| [Performance Guide](performance.md) | Optimization tips and best practices |

## 🚀 Quick Start {#quick-start}

### Installation
description: "A modern, fluent .NET client library for consuming the FundraiseUp API"
permalink: /
---

# FundraiseUp .NET Client Library

[![NuGet](https://img.shields.io/nuget/v/FundraiseUp.Client.svg)](https://www.nuget.org/packages/FundraiseUp.Client)
[![Build Status](https://github.com/clmcgrath/FundraiseUpApi/workflows/CI/badge.svg)](https://github.com/clmcgrath/FundraiseUpApi/actions)
[![License](https://img.shields.io/github/license/clmcgrath/FundraiseUpApi.svg)](https://github.com/clmcgrath/FundraiseUpApi/blob/main/LICENSE)

A modern, fluent .NET client library for consuming the FundraiseUp API with comprehensive support for all endpoints, production-ready async patterns, dependency injection, and enterprise-grade reliability.

{: .fs-6 .fw-300 }

[Get started now](#quick-start){: .btn .btn-primary .fs-5 .mb-4 .mb-md-0 .mr-2 } [View it on GitHub](https://github.com/clmcgrath/FundraiseUpApi){: .btn .fs-5 .mb-4 .mb-md-0 }

---

## ✨ Features

- **Fluent API Design** - Intuitive, discoverable interface with IntelliSense support
- **Dependency Injection Ready** - Native support for Microsoft DI with configuration options
- **Production-Ready Async** - All operations use async/await with ConfigureAwait(false) for deadlock prevention and CancellationToken support
- **Enterprise-Grade** - Configurable retry policies, rate limiting, and comprehensive error handling
- **Multi-Framework** - Targets .NET Standard 2.0 and .NET 6+ for broad compatibility
- **Comprehensive Logging** - Structured logging with Microsoft.Extensions.Logging
- **Secure by Default** - HTTPS enforcement and secure credential management

## � Documentation

| Section | Description |
|:--------|:------------|
| [Getting Started](getting-started.md) | Installation and basic setup |
| [Configuration](configuration.md) | Comprehensive configuration options |
| [Rate Limiting & Connection Pooling](RATE_LIMITING_CONNECTION_POOLING.md) | Advanced performance optimization |
| [Examples](EXAMPLES.md) | Common usage patterns and scenarios |
| [API Reference](api-reference.md) | Complete method documentation |
| [Production Readiness](production-readiness.md) | Thread safety, deadlock prevention, and enterprise deployment |
| [Error Handling](error-handling.md) | Exception types and handling strategies |
| [Performance Guide](performance.md) | Optimization tips and best practices |

## �🚀 Quick Start

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

### Core Guides
- [**Getting Started**](getting-started.md) - Installation and basic setup
- [**Configuration Guide**](configuration.md) - Comprehensive configuration options
- [**HttpClientFactory Integration**](httpclientfactory.md) - Enterprise HTTP client management
- [**API Reference**](api-reference.md) - Complete API documentation
- [**Examples**](EXAMPLES.md) - Common usage patterns and scenarios

### Advanced Topics
- [**Error Handling**](error-handling.md) - Exception types and handling strategies
- [**Performance Guide**](performance.md) - Optimization tips and best practices
- [**Best Practices**](best-practices.md) - Production-ready patterns and recommendations
- [**Rate Limiting & Connection Pooling**](RATE_LIMITING_CONNECTION_POOLING.md) - Advanced performance optimization

### Development
- [**Branching Strategy**](branching-strategy.md) - Git workflow and versioning approach
- [**Testing Guide**](../TESTING.md) - Unit testing patterns and strategies

### Support & Reference
- [**FAQ**](faq.md) - Frequently asked questions and answers
- [**Troubleshooting**](troubleshooting.md) - Common issues and solutions
- [**Migration Guide**](migration-guide.md) - Version upgrade guides
- [**Roadmap**](roadmap.md) - Future development plans and priorities
- [**Changelog**](changelog.md) - Complete version history

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

- [Issues](https://github.com/clmcgrath/FundraiseUpApi/issues) - Bug reports and feature requests
- [Discussions](https://github.com/clmcgrath/FundraiseUpApi/discussions) - Community support and questions
- [Documentation](https://clmcgrath.github.io/FundraiseUpApi/) - Comprehensive guides and API reference