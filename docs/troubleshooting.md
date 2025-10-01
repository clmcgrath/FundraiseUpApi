---
layout: default
title: Troubleshooting
nav_order: 10
description: "Common issues and solutions"
---

# Troubleshooting Guide
{: .no_toc }

Solutions to common issues when using the FundraiseUp .NET Client Library.
{: .fs-6 .fw-300 }

## Table of contents
{: .no_toc .text-delta }

1. TOC
{:toc}

---

## Authentication Issues

### Invalid API Key Error

**Problem**: `FundraiseUpAuthenticationException: Invalid API key`

**Solutions**:
1. Verify your API key is correct and active
2. Check if you're using test vs. production API keys
3. Ensure API key has required permissions

```csharp
// Verify API key format
var isTestKey = apiKey.StartsWith("test_");
var isLiveKey = apiKey.StartsWith("live_");

if (!isTestKey && !isLiveKey)
{
    throw new InvalidOperationException("API key must start with 'test_' or 'live_'");
}
```

### Permission Denied

**Problem**: `FundraiseUpApiException: Insufficient permissions`

**Solution**: Ensure your API key has the required permissions:
- Retrieve donation data
- Create new donations
- Retrieve fundraiser data
- Create new fundraisers
- Generate Donor Portal access links

---

## Rate Limiting Issues

### Rate Limit Exceeded

**Problem**: `FundraiseUpRateLimitException: Rate limit exceeded`

**Solutions**:

1. **Use Queue Strategy** (Recommended):
```csharp
services.AddFundraiseUpClient(options =>
{
    options.RateLimitStrategy = RateLimitStrategy.Queue; // Automatically handles queuing
});
```

2. **Use Retry Strategy**:
```csharp
services.AddFundraiseUpClient(options =>
{
    options.RateLimitStrategy = RateLimitStrategy.Retry;
    options.RetryOptions = new RetryConfiguration
    {
        MaxRetries = 5,
        BaseDelay = TimeSpan.FromSeconds(1),
        MaxDelay = TimeSpan.FromSeconds(30)
    };
});
```

3. **Handle Manually**:
```csharp
try
{
    var result = await client.Donations.Create(request).ExecuteAsync();
}
catch (FundraiseUpRateLimitException ex)
{
    await Task.Delay(ex.RetryAfter ?? TimeSpan.FromSeconds(1));
    // Retry the request
}
```

### Concurrent Request Issues

**Problem**: Multiple threads causing rate limit violations

**Solution**: Use HttpClientFactory with single client instance:
```csharp
// ✅ Correct - Single rate limiter across all threads
services.AddFundraiseUpClient(options => options.ApiKey = "key");

// ❌ Wrong - Creates multiple rate limiters
public class BadService()
{
    public async Task ProcessAsync()
    {
        using var client = new FundraiseUpClient("key"); // Don't do this!
    }
}
```

---

## Network Issues

### Connection Timeouts

**Problem**: `TaskCanceledException` or timeout errors

**Solutions**:

1. **Increase Timeout**:
```csharp
services.AddFundraiseUpClient(options =>
{
    options.Timeout = TimeSpan.FromMinutes(2); // Increase from default 30s
});
```

2. **Configure HttpClient Timeout**:
```csharp
services.AddFundraiseUpClient(options => options.ApiKey = "key")
    .ConfigureHttpClient(client =>
    {
        client.Timeout = TimeSpan.FromMinutes(5);
    });
```

### DNS Resolution Issues

**Problem**: DNS-related connection failures

**Solution**: HttpClientFactory automatically handles DNS refresh:
```csharp
// ✅ Use HttpClientFactory (automatic DNS refresh)
services.AddFundraiseUpClient(options => options.ApiKey = "key");

// ❌ Avoid manual HttpClient (can have stale DNS)
using var httpClient = new HttpClient();
```

---

## Serialization Issues

### JSON Deserialization Errors

**Problem**: `JsonException` when deserializing API responses

**Solutions**:

1. **Check API Response Format**:
```csharp
// Enable detailed logging to see raw responses
services.AddFundraiseUpClient(options =>
{
    options.LogLevel = LogLevel.Debug; // Shows request/response details
});
```

2. **Verify Model Compatibility**:
```csharp
// The library handles all serialization automatically
// If you see errors, it may indicate an API change
```

### Date/Time Issues

**Problem**: DateTime parsing errors or timezone issues

**Solution**: All dates are handled as UTC:
```csharp
var donation = await client.Donations.GetById("DXXXXXXX").ExecuteAsync();
Console.WriteLine($"Created: {donation.CreatedAt:yyyy-MM-dd HH:mm:ss} UTC");
```

---

## Configuration Issues

### Dependency Injection Problems

**Problem**: `InvalidOperationException` when resolving `IFundraiseUpClient`

**Solutions**:

1. **Ensure Registration**:
```csharp
// Make sure this is called in Program.cs or Startup.cs
services.AddFundraiseUpClient(options => options.ApiKey = "key");
```

2. **Check Service Lifetime**:
```csharp
// Client is registered as Transient - this is correct
services.AddFundraiseUpClient(options => options.ApiKey = "key");
// Don't register as Singleton
```

### Configuration Validation Errors

**Problem**: `FundraiseUpConfigurationException` during startup

**Common Issues**:
```csharp
// ❌ Missing API key
options.ApiKey = null; // Will throw

// ❌ Invalid URL
options.BaseUrl = "http://api.fundraiseup.com"; // Must be HTTPS

// ❌ Invalid timeout
options.Timeout = TimeSpan.Zero; // Must be positive

// ❌ Invalid retry configuration
options.RetryOptions.MaxRetries = -1; // Must be non-negative
```

---

## Performance Issues

### Slow Response Times

**Problem**: API calls taking longer than expected

**Solutions**:

1. **Enable Connection Pooling**:
```csharp
services.AddFundraiseUpClient(options => options.ApiKey = "key")
    .ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler
    {
        MaxConnectionsPerServer = 10,
        PooledConnectionLifetime = TimeSpan.FromMinutes(15)
    });
```

2. **Use Appropriate Rate Limiting**:
```csharp
// Queue strategy may add latency but ensures reliability
options.RateLimitStrategy = RateLimitStrategy.Queue;

// Exception strategy is fastest but requires manual handling
options.RateLimitStrategy = RateLimitStrategy.Exception;
```

### Memory Usage Issues

**Problem**: High memory consumption

**Solutions**:

1. **Use HttpClientFactory**:
```csharp
// ✅ Proper resource management
services.AddFundraiseUpClient(options => options.ApiKey = "key");

// ❌ Avoid creating multiple clients
for (int i = 0; i < 100; i++)
{
    using var client = new FundraiseUpClient("key"); // Memory leak!
}
```

2. **Dispose Clients Properly**:
```csharp
// When using manual client creation
using var client = new FundraiseUpClient("key");
// Client automatically disposed
```

---

## Testing Issues

### Mock Setup Problems

**Problem**: Difficulty setting up mocks for testing

**Solution**: Use the provided test helpers:
```csharp
[Test]
public async Task TestDonationCreation()
{
    // Use the built-in mock helpers
    var mockSetup = new HttpClientMockSetup();
    mockSetup.SetupSuccessResponse("/v1/donations", new DonationResponse
    {
        Id = "DTEST123",
        Amount = "100.00",
        Status = "succeeded"
    });
    
    var client = new FundraiseUpClient("test-key", mockSetup.HttpClient);
    // Test your code...
}
```

### Integration Test Failures

**Problem**: Tests failing against real API

**Solutions**:

1. **Use Test Environment**:
```csharp
var client = new FundraiseUpClient("test_your-test-api-key");
```

2. **Use Test Payment Methods**:
```csharp
var request = new CreateDonationRequest
{
    PaymentMethod = new PaymentMethodRequest
    {
        Stripe = new StripePaymentMethodRequest
        {
            Id = "pm_card_visa" // Stripe test payment method
        }
    }
};
```

---

## Error Logging

### Enable Detailed Logging

To diagnose issues, enable detailed logging:

```csharp
// In Program.cs or Startup.cs
services.AddLogging(builder =>
{
    builder.AddConsole()
           .AddDebug()
           .SetMinimumLevel(LogLevel.Debug);
});

services.AddFundraiseUpClient(options =>
{
    options.ApiKey = "your-key";
    options.LogLevel = LogLevel.Debug; // Shows HTTP requests/responses
});
```

### Log Output Examples

**Normal Operation**:
```
[2025-10-01 10:30:15] [Information] FundraiseUp.Client: Creating donation
[2025-10-01 10:30:16] [Debug] FundraiseUp.Client.Http: POST /v1/donations completed in 1.2s
```

**Rate Limiting**:
```
[2025-10-01 10:30:15] [Debug] FundraiseUp.Client.RateLimit: Acquired rate limit slot. Current requests: 2/3
[2025-10-01 10:30:16] [Debug] FundraiseUp.Client.RateLimit: Released rate limit slot. Current requests: 1/3
```

**Errors**:
```
[2025-10-01 10:30:15] [Error] FundraiseUp.Client: API request failed with status 422
[2025-10-01 10:30:15] [Error] FundraiseUp.Client: Response: {"errors":[{"field":"amount","message":"Amount must be greater than 0"}]}
```

---

## Getting Help

If you can't find a solution here:

1. **Check the [Examples](EXAMPLES.md)** for working code samples
2. **Review the [Configuration Guide](configuration.md)** for setup options
3. **Search [GitHub Issues](https://github.com/clmcgrath/FundraiseUpApi/issues)** for similar problems
4. **Create a new issue** with:
   - Your .NET version
   - Library version
   - Complete error message
   - Minimal code example
   - Steps to reproduce

---

## Diagnostic Checklist

When reporting issues, please include:

- [ ] .NET version and target framework
- [ ] FundraiseUp.Client library version
- [ ] Complete exception message and stack trace
- [ ] Minimal code example that reproduces the issue
- [ ] Whether you're using test or production API keys
- [ ] Any relevant configuration settings
- [ ] Log output (with sensitive data removed)