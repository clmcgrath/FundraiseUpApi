# Getting Started

This guide will help you get up and running with the FundraiseUp .NET Client Library quickly.

## Prerequisites

- .NET Standard 2.0+ or .NET 6+
- FundraiseUp API key ([Get one here](https://fundraiseup.com/developers))

## Installation

### Package Manager Console
```powershell
Install-Package FundraiseUp.Client
```

### .NET CLI
```bash
dotnet add package FundraiseUp.Client
```

### PackageReference
```xml
<PackageReference Include="FundraiseUp.Client" Version="1.0.0" />
```

## Basic Setup

### 1. Simple Client Creation

The fastest way to get started is using the fluent builder pattern:

```csharp
using FundraiseUp.Client;

var client = FundraiseUpClient.Create()
    .WithApiKey("your-api-key-here")
    .WithTimeout(TimeSpan.FromSeconds(30))
    .Build();

try
{
    var campaigns = await client.GetCampaignsAsync();
    Console.WriteLine($"Found {campaigns.Count()} campaigns");
}
finally
{
    client.Dispose();
}
```

### 2. Dependency Injection (Recommended)

For production applications, use dependency injection:

#### ASP.NET Core / .NET 6+
```csharp
// Program.cs
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

#### Configuration File
```json
{
  "FundraiseUp": {
    "ApiKey": "your-api-key-here",
    "BaseUrl": "https://api.fundraiseup.com",
    "TimeoutSeconds": 30
  }
}
```

#### Service Usage
```csharp
public class DonationService
{
    private readonly IFundraiseUpClient _client;
    
    public DonationService(IFundraiseUpClient client)
    {
        _client = client;
    }
    
    public async Task<DonationResponse> CreateDonationAsync(decimal amount, string campaignId)
    {
        var request = new DonationRequest
        {
            Amount = amount,
            Currency = "USD",
            CampaignId = campaignId
        };
        
        return await _client.CreateDonationAsync(request);
    }
}
```

## First API Call

Let's make your first API call to verify everything is working:

```csharp
public async Task TestConnectionAsync()
{
    try
    {
        // Get a list of campaigns
        var campaigns = await client.GetCampaignsAsync(new CampaignQueryOptions
        {
            Limit = 5,
            Status = "active"
        });
        
        foreach (var campaign in campaigns)
        {
            Console.WriteLine($"Campaign: {campaign.Name}");
            Console.WriteLine($"Goal: ${campaign.GoalAmount:N2}");
            Console.WriteLine($"Raised: ${campaign.CurrentAmount:N2}");
            Console.WriteLine($"Progress: {(campaign.CurrentAmount / campaign.GoalAmount * 100):F1}%");
            Console.WriteLine("---");
        }
    }
    catch (FundraiseUpAuthenticationException)
    {
        Console.WriteLine("❌ Invalid API key - check your credentials");
    }
    catch (FundraiseUpException ex)
    {
        Console.WriteLine($"❌ API Error: {ex.Message}");
    }
}
```

## Next Steps

Now that you have the basics working:

1. **[Configuration Guide](configuration.md)** - Learn about all configuration options
2. **[Examples](examples.md)** - See common usage patterns  
3. **[Error Handling](error-handling.md)** - Implement proper error handling
4. **[API Reference](api-reference.md)** - Explore all available methods

## Need Help?

- Check our [Examples](examples.md) for common scenarios
- Review [Error Handling](error-handling.md) for troubleshooting
- [Open an issue](https://github.com/YourOrg/FundraiseUpApi/issues) if you find a bug
- [Start a discussion](https://github.com/YourOrg/FundraiseUpApi/discussions) for questions