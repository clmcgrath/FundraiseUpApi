# Configuration Guide

The FundraiseUp .NET Client Library provides comprehensive configuration options to fit your application's needs.

## Configuration Methods

### 1. Fluent Builder Pattern

```csharp
var client = FundraiseUpClient.Create()
    .WithApiKey("your-api-key")
    .WithBaseUrl("https://api.fundraiseup.com")
    .WithTimeout(TimeSpan.FromSeconds(30))
    .WithRateLimitStrategy(RateLimitStrategy.Queue)
    .WithRetryPolicy(new RetryConfiguration
    {
        MaxRetries = 3,
        BaseDelay = TimeSpan.FromSeconds(1),
        MaxDelay = TimeSpan.FromSeconds(30),
        BackoffMultiplier = 2.0
    })
    .WithLogging(LogLevel.Information)
    .Build();
```

### 2. Dependency Injection with Options

```csharp
services.AddFundraiseUpClient(options =>
{
    options.ApiKey = "your-api-key";
    options.BaseUrl = "https://api.fundraiseup.com";
    options.Timeout = TimeSpan.FromSeconds(30);
    options.RateLimitStrategy = RateLimitStrategy.Queue;
    options.RetryOptions = new RetryConfiguration
    {
        MaxRetries = 3,
        BaseDelay = TimeSpan.FromSeconds(1)
    };
    options.LogLevel = LogLevel.Information;
});
```

### 3. Configuration Binding

```csharp
// appsettings.json
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

// Program.cs
services.AddFundraiseUpClient(options =>
{
    configuration.GetSection("FundraiseUp").Bind(options);
});
```

## Configuration Options

### Core Settings

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `ApiKey` | `string` | **Required** | Your FundraiseUp API key |
| `BaseUrl` | `string` | `https://api.fundraiseup.com` | API base URL |
| `Timeout` | `TimeSpan` | `30 seconds` | Request timeout |

### Rate Limiting

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `RateLimitStrategy` | `RateLimitStrategy` | `Retry` | How to handle rate limits |

#### Rate Limit Strategies

- **`Retry`** - Automatically retry with exponential backoff
- **`Exception`** - Throw `FundraiseUpRateLimitException` immediately  
- **`Queue`** - Queue requests until rate limit resets

### Retry Configuration

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `MaxRetries` | `int` | `3` | Maximum retry attempts |
| `BaseDelay` | `TimeSpan` | `1 second` | Initial delay between retries |
| `MaxDelay` | `TimeSpan` | `30 seconds` | Maximum delay between retries |
| `BackoffMultiplier` | `double` | `2.0` | Exponential backoff multiplier |

### Logging

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `LogLevel` | `LogLevel` | `Information` | Minimum log level |

#### Log Levels

- **`Minimal`** - Errors and warnings only
- **`Information`** - Key operations and errors  
- **`Debug`** - Detailed request/response logging

## Environment-Specific Configuration

### Development
```csharp
services.AddFundraiseUpClient(options =>
{
    options.ApiKey = configuration["FundraiseUp:ApiKey"];
    options.LogLevel = LogLevel.Debug; // Verbose logging
    options.Timeout = TimeSpan.FromMinutes(5); // Longer timeouts
});
```

### Production
```csharp
services.AddFundraiseUpClient(options =>
{
    options.ApiKey = configuration["FundraiseUp:ApiKey"];
    options.LogLevel = LogLevel.Information; // Standard logging
    options.RateLimitStrategy = RateLimitStrategy.Queue; // Handle load
    options.RetryOptions = new RetryConfiguration
    {
        MaxRetries = 5, // More resilient
        MaxDelay = TimeSpan.FromMinutes(1)
    };
});
```

## Secure Configuration

### Azure Key Vault
```csharp
// Use Azure Key Vault for sensitive data
services.AddFundraiseUpClient(options =>
{
    options.ApiKey = configuration["KeyVault:FundraiseUpApiKey"];
    // Other non-sensitive options...
});
```

### Environment Variables
```csharp
services.AddFundraiseUpClient(options =>
{
    options.ApiKey = Environment.GetEnvironmentVariable("FUNDRAISEUP_API_KEY")
        ?? throw new InvalidOperationException("FundraiseUp API key not configured");
});
```

### User Secrets (Development)
```bash
# Set user secret
dotnet user-secrets set "FundraiseUp:ApiKey" "your-development-api-key"
```

## Validation

The library automatically validates configuration:

```csharp
// These will throw FundraiseUpConfigurationException:
options.ApiKey = null; // ❌ Required
options.BaseUrl = "http://api.fundraiseup.com"; // ❌ Must be HTTPS
options.Timeout = TimeSpan.Zero; // ❌ Must be positive
options.RetryOptions.MaxRetries = -1; // ❌ Must be non-negative
```

## Advanced Configuration

### Custom HTTP Client Configuration
```csharp
services.AddFundraiseUpClient(options =>
{
    options.ApiKey = "your-api-key";
})
.ConfigureHttpClient(client =>
{
    client.DefaultRequestHeaders.Add("User-Agent", "MyApp/1.0");
    client.Timeout = TimeSpan.FromMinutes(2);
});
```

### Multiple Clients
```csharp
// Named clients for different environments
services.AddFundraiseUpClient("production", options =>
{
    options.ApiKey = configuration["FundraiseUp:Production:ApiKey"];
});

services.AddFundraiseUpClient("sandbox", options =>
{
    options.ApiKey = configuration["FundraiseUp:Sandbox:ApiKey"];
    options.BaseUrl = "https://api-sandbox.fundraiseup.com";
});

// Usage
public class MyService
{
    private readonly IFundraiseUpClient _prodClient;
    private readonly IFundraiseUpClient _sandboxClient;
    
    public MyService(
        [FromKeyedServices("production")] IFundraiseUpClient prodClient,
        [FromKeyedServices("sandbox")] IFundraiseUpClient sandboxClient)
    {
        _prodClient = prodClient;
        _sandboxClient = sandboxClient;
    }
}
```

## Configuration Validation

Test your configuration:

```csharp
public async Task ValidateConfigurationAsync()
{
    try
    {
        // Simple connectivity test
        var campaigns = await client.GetCampaignsAsync(new CampaignQueryOptions { Limit = 1 });
        Console.WriteLine("✅ Configuration valid");
    }
    catch (FundraiseUpAuthenticationException)
    {
        Console.WriteLine("❌ Invalid API key");
    }
    catch (FundraiseUpConfigurationException ex)
    {
        Console.WriteLine($"❌ Configuration error: {ex.Message}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Connection error: {ex.Message}");
    }
}
```