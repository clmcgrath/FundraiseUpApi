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

The fastest way to get started:

```csharp
using FundraiseUp.Client;
using FundraiseUp.Client.Configuration;

// Simple setup with API key
var client = new FundraiseUpClient("your-api-key-here");

// Or with configuration options
var client = new FundraiseUpClient(new FundraiseUpClientOptions
{
    ApiKey = "your-api-key-here",
    BaseUrl = "https://api.fundraiseup.com",
    Timeout = TimeSpan.FromSeconds(30),
    MaxRetryAttempts = 3,
    EnableLogging = true
});

try
{
    // List campaigns using fluent API
    var campaigns = await client.Campaigns
        .List()
        .Take(10)
        .ExecuteAsync();
    
    Console.WriteLine($"Found {campaigns.Items.Count} campaigns");
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
using FundraiseUp.Client.Requests;

public class DonationService
{
    private readonly IFundraiseUpClient _client;
    
    public DonationService(IFundraiseUpClient client)
    {
        _client = client;
    }
    
    public async Task<Donation> CreateDonationAsync(decimal amount, string campaignId, string donorEmail)
    {
        return await _client.Donations
            .Create(new CreateDonationRequest
            {
                Amount = amount,
                Currency = "USD",
                CampaignId = campaignId,
                DonorEmail = donorEmail
            })
            .ExecuteAsync();
    }
}
```

## Your First API Calls

Let's make your first API calls to verify everything is working:

### 1. Create a Donation

```csharp
using FundraiseUp.Client.Requests;
using FundraiseUp.Client.Exceptions;

public async Task CreateFirstDonationAsync()
{
    try
    {
        var donation = await client.Donations
            .Create(new CreateDonationRequest
            {
                Amount = 25.00m,
                Currency = "USD",
                DonorEmail = "supporter@example.com",
                CampaignId = "your-campaign-id",
                DonorName = "John Supporter"
            })
            .ExecuteAsync();
            
        Console.WriteLine($"✅ Success! Donation ID: {donation.Id}");
        Console.WriteLine($"Amount: ${donation.Amount}");
        Console.WriteLine($"Status: {donation.Status}");
    }
    catch (FundraiseUpValidationException ex)
    {
        Console.WriteLine("❌ Validation errors:");
        foreach (var error in ex.ValidationErrors)
        {
            Console.WriteLine($"- {error.Key}: {string.Join(", ", error.Value)}");
        }
    }
    catch (FundraiseUpApiException ex)
    {
        Console.WriteLine($"❌ API Error: {ex.Message}");
    }
}
```

### 2. List Campaigns

```csharp
public async Task ListCampaignsAsync()
{
    try
    {
        var campaigns = await client.Campaigns
            .List()
            .FilterByStatus("active")
            .Take(5)
            .ExecuteAsync();
        
        Console.WriteLine($"Found {campaigns.Items.Count} active campaigns:");
        
        foreach (var campaign in campaigns.Items)
        {
            Console.WriteLine($"Campaign: {campaign.Name}");
            Console.WriteLine($"Goal: ${campaign.GoalAmount:N2}");
            Console.WriteLine($"Progress: {campaign.Status}");
            Console.WriteLine("---");
        }
    }
    catch (FundraiseUpApiException ex)
    {
        Console.WriteLine($"❌ API Error: {ex.Message}");
    }
}
```

### 3. Get Donation Details

```csharp
public async Task GetDonationAsync(string donationId)
{
    try
    {
        var donation = await client.Donations
            .GetById(donationId)
            .ExecuteAsync();
            
        Console.WriteLine($"Donation Details:");
        Console.WriteLine($"- ID: {donation.Id}");
        Console.WriteLine($"- Amount: ${donation.Amount} {donation.Currency}");
        Console.WriteLine($"- Donor: {donation.DonorEmail}");
        Console.WriteLine($"- Status: {donation.Status}");
        Console.WriteLine($"- Created: {donation.CreatedAt}");
    }
    catch (FundraiseUpNotFoundException)
    {
        Console.WriteLine($"❌ Donation {donationId} not found");
    }
    catch (FundraiseUpApiException ex)
    {
        Console.WriteLine($"❌ API Error: {ex.Message}");
    }
}
```

## Common Operations Quick Reference

### Working with Donations
```csharp
// Create donation
var donation = await client.Donations.Create(request).ExecuteAsync();

// Get donation
var donation = await client.Donations.GetById("donation-123").ExecuteAsync();

// List donations
var donations = await client.Donations.List().Take(10).ExecuteAsync();

// List donations for a campaign
var donations = await client.Donations.List()
    .FilterByCampaign("campaign-123")
    .ExecuteAsync();
```

### Working with Campaigns
```csharp
// Create campaign
var campaign = await client.Campaigns.Create(request).ExecuteAsync();

// Get campaign
var campaign = await client.Campaigns.GetById("campaign-123").ExecuteAsync();

// List campaigns
var campaigns = await client.Campaigns.List().ExecuteAsync();

// Get campaign statistics
var stats = await client.Campaigns.GetStatistics("campaign-123").ExecuteAsync();
```

### Working with Donors
```csharp
// Create donor
var donor = await client.Donors.Create(request).ExecuteAsync();

// Get donor
var donor = await client.Donors.GetById("donor-123").ExecuteAsync();

// List donors
var donors = await client.Donors.List().ExecuteAsync();

// Get donor's donations
var donations = await client.Donors.GetDonations("donor-123").ExecuteAsync();
```

## Next Steps

Now that you have the basics working:

1. **[Comprehensive Examples](EXAMPLES.md)** - Real-world usage patterns and advanced scenarios
2. **[Full Documentation](../README.md)** - Complete API reference and configuration options
3. **[Testing Guide](../TESTING.md)** - Learn how to test your integration
4. **Practice**: Try creating donations, campaigns, and exploring the fluent API

## Need Help?

- **Documentation**: [README.md](../README.md) for complete API reference
- **Examples**: [EXAMPLES.md](EXAMPLES.md) for comprehensive usage examples
- **Issues**: [GitHub Issues](https://github.com/clmcgrath/FundraiseUpApi/issues) to report bugs
- **Discussions**: [GitHub Discussions](https://github.com/clmcgrath/FundraiseUpApi/discussions) for questions

## Troubleshooting

### Common Issues

1. **Authentication Errors**: Verify your API key is correct and has proper permissions
2. **Validation Errors**: Check required fields and data formats in request objects
3. **Network Timeouts**: Increase timeout values for slow connections
4. **Rate Limiting**: Implement proper retry logic and respect API limits

### Debug Mode

Enable detailed logging to troubleshoot issues:

```csharp
var client = new FundraiseUpClient(new FundraiseUpClientOptions
{
    ApiKey = "your-key",
    EnableLogging = true,
    LogLevel = LogLevel.Debug  // This will log request/response details
});
```

That's it! You're ready to start building amazing fundraising experiences with the FundraiseUp .NET Client Library! 🚀