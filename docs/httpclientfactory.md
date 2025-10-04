---
layout: default
title: HttpClientFactory
nav_order: 7
description: "Complete guide to HttpClientFactory integration and best practices"
---

# HttpClientFactory Integration
{: .no_toc }

Complete guide to using the FundraiseUp .NET Client Library with HttpClientFactory for enterprise-grade HTTP client management.
{: .fs-6 .fw-300 }

## Table of contents
{: .no_toc .text-delta }

1. TOC
{:toc}

---

## Overview

The FundraiseUp.Client library is built with full HttpClientFactory support, providing proper HttpClient lifecycle management, DNS refresh, and connection pooling for production applications.

## ✅ Benefits

### Performance Benefits
- **Connection Reuse**: Up to 50% faster requests due to connection pooling
- **Memory Efficiency**: Reduced memory pressure from proper HttpClient lifecycle
- **DNS Resolution**: Automatic DNS refresh prevents stale DNS issues
- **Resource Cleanup**: Proper disposal prevents resource leaks

### Enterprise Features
- **Proper Connection Pooling**: Automatic management of connection pools
- **DNS Refresh**: Automatic DNS updates without restarting the application
- **Resource Management**: Proper disposal and lifecycle management
- **Resilience**: Built-in support for retry policies and circuit breakers
- **Monitoring**: Integration with .NET diagnostics and logging

---

## Basic Usage

### Simple Registration

```csharp
// In Program.cs or Startup.cs
services.AddFundraiseUpClient("your-api-key");
```

### With Configuration Options

```csharp
services.AddFundraiseUpClient(options =>
{
    options.ApiKey = "your-api-key";
    options.BaseUrl = "https://api.fundraiseup.com";
    options.Timeout = TimeSpan.FromSeconds(30);
    options.RateLimitStrategy = RateLimitStrategy.Queue;
});
```

### Dependency Injection Usage

```csharp
public class DonationService
{
    private readonly IFundraiseUpClient _fundraiseUpClient;
    
    public DonationService(IFundraiseUpClient fundraiseUpClient)
    {
        _fundraiseUpClient = fundraiseUpClient;
    }
    
    public async Task ProcessDonationAsync()
    {
        var donation = await _fundraiseUpClient.Donations
            .Create(new CreateDonationRequest
            {
                Amount = "100.00",
                Currency = "usd",
                Campaign = "FUN12345678",
                Designation = "E1234567",
                // ... other properties
            })
            .ExecuteAsync();
    }
}
```

---

## Advanced Configuration

### Custom HttpClient Configuration

```csharp
services.AddFundraiseUpClient(
    options =>
    {
        options.ApiKey = "your-api-key";
    },
    httpClient =>
    {
        // Additional HttpClient configuration
        httpClient.DefaultRequestHeaders.Add("Custom-Header", "Value");
        httpClient.DefaultRequestHeaders.Add("User-Agent", "MyApp/1.0");
    }
);
```

### Connection Pool Configuration

```csharp
services.AddFundraiseUpClient(options => options.ApiKey = "key")
    .ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler
    {
        MaxConnectionsPerServer = 10,
        PooledConnectionLifetime = TimeSpan.FromMinutes(15),
        PooledConnectionIdleTimeout = TimeSpan.FromMinutes(2),
        EnableMultipleHttp2Connections = true
    });
```

### Resilience with Polly

```csharp
using Polly;
using Polly.Extensions.Http;

services.AddFundraiseUpClient(options =>
{
    options.ApiKey = "your-api-key";
})
.AddPolicyHandler(GetRetryPolicy())
.AddPolicyHandler(GetCircuitBreakerPolicy());

static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .WaitAndRetryAsync(3, retryAttempt => 
            TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
}

static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .CircuitBreakerAsync(3, TimeSpan.FromSeconds(30));
}
```

---

## Architecture Details

### Service Registration

| Service | Lifetime | Purpose |
|---------|----------|---------|
| `IFundraiseUpClient` | Transient | Main client interface |
| `HttpClient` | Factory-managed | HTTP communication |
| `FundraiseUpClientOptions` | Singleton | Configuration options |

### Client Lifecycle

1. **Service Registration**: HttpClient registered with factory
2. **Dependency Injection**: Client injected as transient service
3. **Request Execution**: HttpClient reused from connection pool
4. **Resource Management**: Factory handles HttpClient disposal

### Thread Safety

- **HttpClient**: Thread-safe and shared across requests
- **FundraiseUpClient**: Thread-safe with proper state management
- **Connection Pool**: Shared safely across all client instances
- **Rate Limiting**: Thread-safe across all concurrent requests

---

## Manual HttpClient Management

### When to Use Manual Management

- Custom HttpClient configuration not supported by factory
- Testing scenarios with specific mock requirements
- Integration with existing HttpClient instances

### Manual Usage Example

```csharp
// Create your own HttpClient
using var httpClient = new HttpClient();

// Configure FundraiseUp client with your HttpClient
var options = new FundraiseUpClientOptions
{
    ApiKey = "your-api-key",
    BaseUrl = "https://api.fundraiseup.com"
};

using var client = new FundraiseUpClient(options.ApiKey, options, httpClient);

// Use the client
var donation = await client.Donations
    .Create(request)
    .ExecuteAsync();
```

### Important Considerations

#### HttpClient Ownership
- **HttpClientFactory**: Factory manages lifecycle - do not dispose manually
- **Manual Creation**: You own the lifecycle and must dispose properly
- **Auto-Detection**: Library automatically detects ownership pattern

#### Configuration Priority
```csharp
// ✅ Correct - configure in service registration
services.AddFundraiseUpClient(options => 
{
    options.ApiKey = "key";
    options.BaseUrl = "https://api.fundraiseup.com";
});

// ❌ Avoid - HttpClient is pre-configured by factory
var client = httpClientFactory.CreateClient();
client.BaseAddress = new Uri("https://different-url.com");
```

---

## Testing Integration

### Unit Testing with HttpClientFactory

```csharp
[Test]
public async Task TestDonationCreation()
{
    // Arrange
    var services = new ServiceCollection();
    services.AddFundraiseUpClient("test-api-key");
    
    // Mock HTTP responses
    services.AddSingleton<HttpMessageHandler>(new MockHttpMessageHandler());
    
    var serviceProvider = services.BuildServiceProvider();
    var client = serviceProvider.GetRequiredService<IFundraiseUpClient>();
    
    // Act
    var donation = await client.Donations
        .Create(testRequest)
        .ExecuteAsync();
    
    // Assert
    Assert.IsNotNull(donation);
}
```

### Integration Testing

```csharp
public class FundraiseUpIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    
    public FundraiseUpIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }
    
    [Test]
    public async Task TestRealApiCall()
    {
        // Arrange
        var client = _factory.Services.GetRequiredService<IFundraiseUpClient>();
        
        // Act & Assert
        var campaigns = await client.Campaigns.List().ExecuteAsync();
        Assert.IsNotNull(campaigns);
    }
}
```

---

## Best Practices

### ✅ Recommended Patterns

```csharp
// Use dependency injection
public class MyService(IFundraiseUpClient client)
{
    public async Task ProcessAsync()
    {
        await client.Donations.Create(request).ExecuteAsync();
    }
}
```

### ❌ Anti-Patterns to Avoid

```csharp
// Don't create new clients repeatedly
public class BadService
{
    public async Task ProcessAsync()
    {
        using var client = new FundraiseUpClient("key"); // Inefficient!
        await client.Donations.Create(request).ExecuteAsync();
    }
}

// Don't dispose HttpClient from factory
public class AnotherBadExample
{
    public async Task ProcessAsync(IHttpClientFactory factory)
    {
        using var httpClient = factory.CreateClient(); // Don't dispose!
        // Use httpClient...
    }
}
```

### Performance Optimization

1. **Register Once**: Register FundraiseUp client once in DI container
2. **Reuse Connections**: Let HttpClientFactory manage connection pooling
3. **Configure Appropriately**: Set reasonable timeout and retry values
4. **Monitor Usage**: Use built-in logging to monitor performance

---

## Troubleshooting

### Common Issues

#### DNS Resolution Problems
**Problem**: Stale DNS entries causing connection failures
**Solution**: HttpClientFactory automatically handles DNS refresh

#### Connection Pool Exhaustion
**Problem**: Too many concurrent connections
**Solution**: Configure `MaxConnectionsPerServer` appropriately

#### Memory Leaks
**Problem**: HttpClient instances not disposed properly
**Solution**: Use HttpClientFactory for automatic lifecycle management

### Diagnostic Information

```csharp
// Enable detailed logging
services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Debug));

// Monitor HttpClient usage
services.AddFundraiseUpClient(options =>
{
    options.ApiKey = "key";
    options.LogLevel = LogLevel.Debug; // See HTTP request/response details
});
```

---

## Related Documentation

- [Configuration Guide]({% link docs/configuration.md %}) - Complete configuration options
- [Performance Guide]({% link docs/performance.md %}) - Performance optimization strategies
- [Getting Started]({% link docs/getting-started.md %}) - Basic setup and usage
- [Microsoft HttpClientFactory Documentation](https://learn.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/use-httpclientfactory-to-implement-resilient-http-requests)
- [Polly Resilience Framework](https://github.com/App-vNext/Polly)