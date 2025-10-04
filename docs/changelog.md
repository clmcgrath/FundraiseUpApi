---
layout: default
title: Changelog
nav_order: 8
description: "Complete changelog for the FundraiseUp .NET Client Library"
---

# Changelog
{: .no_toc }

All notable changes to the FundraiseUp .NET Client Library are documented in this file.
{: .fs-6 .fw-300 }

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/), and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).
{: .fs-6 .fw-300 }

## Table of contents
{: .no_toc .text-delta }

1. TOC
{:toc}

---

## [Unreleased]

### 🔄 Recent Changes (October 2025)

#### Quality & Monitoring Improvements
- **Migrated from Codecov to Codacy**: Switched all GitHub Actions workflows to use Codacy for unified code quality analysis and coverage reporting
- **Enhanced CI/CD Security**: Pinned all GitHub Actions to specific commit SHAs to prevent supply chain attacks
- **Comprehensive Documentation**: Added professional Jekyll documentation site with Just the Docs theme, full-text search, and responsive design
- **Documentation Structure**: Complete reorganization with getting started guides, configuration references, examples, and advanced topics

#### Rate Limiting & Performance
- **Advanced Rate Limiting**: Implemented sophisticated rate limiting with three strategies:
  - **Queue Strategy**: Automatically queues requests when limit reached
  - **Retry Strategy**: Exponential backoff retry with configurable attempts  
  - **Exception Strategy**: Immediate failure for predictable behavior
- **Thread-Safe Design**: Rate limiting works across all threads and connection pooling strategies
- **HttpClientFactory Integration**: Full support for connection pooling while respecting FundraiseUp's 3 concurrent request limit
- **Performance Monitoring**: Added comprehensive logging and metrics for rate limiting behavior

#### Developer Experience
- **Professional Documentation Site**: Complete Jekyll-based documentation with search, navigation, and GitHub integration
- **Advanced Testing Guide**: Comprehensive testing documentation with mocking examples and best practices
- **Rate Limiting Documentation**: Detailed guide covering thread safety, connection pooling compatibility, and troubleshooting
- **Configuration Reference**: Complete configuration options with examples and validation rules

#### API Specification Alignment
- **Complete Model Rewrite**: All response models updated to match actual FundraiseUp API specification
- **Donation Response Model**: Comprehensive rewrite with all actual API fields including fees, payment details, and nested objects
- **Campaign Model**: Updated to match read-only API structure with proper string amounts and currency handling
- **Supporter Model**: Aligned with actual supporter API including Gift Aid and communication consent support
- **Pagination Support**: Implemented cursor-based pagination matching FundraiseUp's API format
- **JSON Serialization**: All models use proper JsonPropertyName attributes for exact API compatibility

#### New API Operations
- **Events Operations**: Complete implementation for accessing FundraiseUp audit logs and system events
- **Fundraiser Operations**: Full CRUD operations for individual fundraiser management
- **Recurring Plan Operations**: Read-only access to recurring donation plan information  
- **Donor Portal Operations**: Generate secure access links for supporter self-service

### �️ Production-Ready Async Implementation (October 2, 2025)
- **ConfigureAwait(false) Throughout**: Added ConfigureAwait(false) to all 33+ async operations for deadlock prevention
- **Thread-Safe Design**: Safe for use in ASP.NET, WPF, WinForms, and all SynchronizationContext environments
- **Library Best Practices**: Follows Microsoft's async patterns - users control context capture at call sites
- **Code Quality Improvements**: Eliminated constructor duplication through refactoring

### �📊 Test Coverage & Quality
- **172 Unit Tests Passing**: Comprehensive test suite with 100% success rate across .NET 6 and .NET 8
- **Production Test Categories**: Unit, Integration, Performance, Contract, and Error Handling tests
- **Rate Limiting Tests**: Dedicated test suite for concurrent request handling and thread safety
- **Mock Framework**: Advanced mocking utilities with realistic test data for contributor testing
- **Error Handling Tests**: Complete coverage of exception scenarios and edge cases
- **API Contract Tests**: Validation of all request/response models against actual API

---

## [1.0.0] - 2025-01-XX

### 🎉 Initial Release

The FundraiseUp .NET Client Library is now available! This is a modern, fluent .NET client library for consuming the FundraiseUp API with comprehensive support for all endpoints, dependency injection, and enterprise-grade reliability.

### ✨ Features

#### Core API Client
- **Fluent API Design**: Intuitive builder pattern for all operations
- **Multi-Target Support**: Compatible with .NET Standard 2.0 and .NET 6+
- **Dependency Injection**: Native support for Microsoft.Extensions.DependencyInjection
- **Async/Await**: Fully asynchronous API with cancellation token support
- **HTTP Client Factory**: Integrated with Microsoft.Extensions.Http for optimal performance

#### API Operations
- **Donations**: Create, retrieve, list, and manage donations
- **Supporters**: Retrieve and list supporter information (created automatically via donations)
- **Campaigns**: Read campaign information and statistics (managed via Dashboard)
- **Fundraisers**: Create, retrieve, list, and update individual fundraisers
- **Recurring Plans**: Access recurring donation plan information
- **Events**: Query comprehensive audit logs and system events
- **Donor Portal**: Generate secure access links for supporter self-service

#### Enhanced Features
- **Smart Rate Limiting**: Built-in rate limiting with Queue, Retry, and Exception strategies for FundraiseUp's 3 concurrent request limit
- **Thread Safety**: Rate limiting works seamlessly across all threads and connection pooling strategies
- **Cursor-Based Pagination**: Native support for FundraiseUp's cursor pagination
- **Fluent Filtering**: Chainable filters for listing operations (by campaign, status, date ranges)

#### Error Handling & Reliability
- **Custom Exceptions**: Specific exception types for different error scenarios
  - `FundraiseUpApiException`: Base API exception with status codes
  - `FundraiseUpValidationException`: Validation errors with detailed field information
  - `FundraiseUpNotFoundException`: Resource not found errors
  - `FundraiseUpAuthenticationException`: Authentication/authorization errors
  - `FundraiseUpRateLimitException`: Rate limiting exceptions with retry guidance
- **Retry Logic**: Configurable retry policies with exponential backoff
- **Timeout Management**: Per-request and global timeout configuration
- **HTTP Status Code Handling**: Proper handling of all HTTP response codes

#### Configuration & Flexibility
- **Multiple Configuration Sources**: API key via constructor, configuration, or environment variables
- **Configuration Options**: Base URL, timeouts, retry attempts, logging levels, rate limiting strategies
- **Environment Support**: Easy switching between sandbox and production environments
- **Logging Integration**: Comprehensive logging using Microsoft.Extensions.Logging

#### Developer Experience
- **IntelliSense Support**: Full XML documentation for all public APIs
- **Type Safety**: Strong typing throughout with nullable reference types
- **Validation**: Request validation with data annotations
- **JSON Serialization**: Modern System.Text.Json with custom converters
- **Memory Efficient**: Optimized for minimal allocations and GC pressure

### 🧪 Testing & Quality

#### Unit Testing Framework
- **Comprehensive Unit Tests**: Full coverage of core functionality
- **Mock Framework**: Advanced mocking utilities for contributor testing
- **MockResponseBuilder**: Centralized test data generation
- **HttpClientMockSetup**: Simplified HTTP client mocking
- **No API Key Required**: Complete testing without live API credentials

#### Testing Tools
- **Realistic Test Data**: Sample donations, campaigns, and supporters
- **Error Scenario Testing**: Validation errors, HTTP errors, network failures
- **Pagination Testing**: Mock paginated responses for list operations
- **Async Testing**: Proper async/await testing patterns
- **Rate Limiting Tests**: Concurrent request handling validation

### 📚 Documentation

#### Comprehensive Guides
- **Professional Documentation Site**: Complete Jekyll site with Just the Docs theme
- **Getting Started Guide**: Step-by-step guide for new users
- **Configuration Reference**: Complete configuration options with validation
- **Examples Collection**: Real-world usage patterns and advanced scenarios
- **Rate Limiting Guide**: Thread safety and connection pooling compatibility
- **Testing Documentation**: Testing strategies and best practices
- **Error Handling Guide**: Exception handling patterns and troubleshooting
- **Performance Guide**: Optimization strategies and best practices

#### Usage Examples
- **Basic Operations**: Creating donations, managing supporters
- **Advanced Scenarios**: Batch processing, custom retry logic, error handling
- **Integration Patterns**: ASP.NET Core integration, dependency injection
- **Testing Examples**: Unit testing, integration testing, mocking strategies

### 🔧 Technical Implementation

#### Architecture
- **Clean Architecture**: Separation of concerns with clear boundaries
- **SOLID Principles**: Adherence to SOLID design principles
- **Builder Pattern**: Fluent API using builder pattern for requests
- **Factory Pattern**: HTTP client factory integration
- **Repository Pattern**: Clean abstraction over HTTP operations

#### Performance Optimizations
- **Connection Pooling**: Efficient HTTP connection reuse with rate limiting compatibility
- **Async Operations**: Non-blocking I/O operations throughout
- **Memory Management**: Minimal allocations and proper disposal
- **JSON Performance**: Optimized serialization with System.Text.Json
- **Request Compression**: Optional GZIP compression support
- **Smart Rate Limiting**: Prevents API throttling while maximizing throughput

#### Security
- **API Key Management**: Secure handling of authentication credentials
- **HTTPS Only**: Enforced HTTPS for all API communications
- **Request Validation**: Input validation to prevent malformed requests
- **Sensitive Data Logging**: Careful handling of sensitive information in logs
- **CI/CD Security**: Pinned GitHub Actions to prevent supply chain attacks

### 🛠️ Development Tools

#### Build & CI/CD
- **Multi-Target Build**: Automatic building for multiple frameworks
- **NuGet Package**: Automated package generation and metadata
- **GitHub Actions**: Continuous integration with automated testing
- **Code Quality**: Codacy integration for comprehensive quality analysis
- **Coverage Reporting**: Automated coverage reporting with Codacy

#### Developer Workflow
- **EditorConfig**: Consistent code formatting across editors
- **Git Integration**: Proper .gitignore and Git hooks
- **IDE Support**: Full Visual Studio and VS Code integration
- **Debug Support**: Comprehensive debugging and logging capabilities

### 📦 Package Information

- **Package ID**: `FundraiseUp.Client`
- **Version**: `1.0.0`
- **Target Frameworks**: .NET Standard 2.0, .NET 6.0+
- **Dependencies**: Microsoft.Extensions.*, System.Text.Json, System.Net.Http
- **License**: MIT License
- **Repository**: https://github.com/clmcgrath/FundraiseUpApi

### 🚀 Getting Started

```bash
# Install via NuGet Package Manager
Install-Package FundraiseUp.Client

# Or via .NET CLI
dotnet add package FundraiseUp.Client
```

```csharp
// Quick start example with rate limiting
using FundraiseUp.Client;
using FundraiseUp.Client.Configuration;

var services = new ServiceCollection();
services.AddFundraiseUpClient(options =>
{
    options.ApiKey = "your-api-key";
    options.RateLimitStrategy = RateLimitStrategy.Queue; // Handle rate limiting automatically
});

var serviceProvider = services.BuildServiceProvider();
var client = serviceProvider.GetRequiredService<IFundraiseUpClient>();

var donation = await client.Donations
    .Create(new CreateDonationRequest
    {
        Amount = "25.00",
        Currency = "USD",
        DonorEmail = "supporter@example.com",
        Campaign = "FUNXXXXXXXX",
        Designation = "EXXXXXXX"
    })
    .ExecuteAsync();

Console.WriteLine($"Created donation: {donation.Id}");
```

### 🎯 What's Next

Future releases will include:
- **Integration Tests**: Comprehensive testing against live API (pending API access)
- **Webhook Support**: Handling FundraiseUp webhook events
- **Bulk Operations**: Efficient batch processing capabilities
- **Advanced Caching**: Optional response caching for improved performance
- **Metrics & Telemetry**: Built-in metrics collection and reporting
- **Additional Endpoints**: Support for new FundraiseUp API features as they become available

---

### 📋 Migration Guide

This is the initial release, so no migration is required. For future versions, migration guides will be provided here.

### 🤝 Contributing

We welcome contributions! Please see our documentation for details on how to get started with development and testing.

### 📄 License

This project is licensed under the MIT License - see the [LICENSE](https://github.com/clmcgrath/FundraiseUpApi/blob/main/LICENSE) file for details.

### 🆘 Support

- **Documentation**: [Complete Documentation Site](https://clmcgrath.github.io/FundraiseUpApi/)
- **Issues**: [GitHub Issues](https://github.com/clmcgrath/FundraiseUpApi/issues)
- **Discussions**: [GitHub Discussions](https://github.com/clmcgrath/FundraiseUpApi/discussions)

---

**Happy fundraising!** 🎉💝