# Quick Start Guide: FundraiseUp .NET Client Library

## Installation

### Via Package Manager Console
```powershell
Install-Package FundraiseUp.Client
```

### Via .NET CLI
```bash
dotnet add package FundraiseUp.Client
```

### Via PackageReference
```xml
<PackageReference Include="FundraiseUp.Client" Version="1.0.0" />
```

## Basic Setup

### 1. Simple Client Creation
```csharp
using FundraiseUp.Client;

// Create client with fluent API
var client = FundraiseUpClient.Create()
    .WithApiKey("your-api-key-here")
    .WithTimeout(TimeSpan.FromSeconds(30))
    .Build();

try
{
    // Use the client
    var campaigns = await client.GetCampaignsAsync();
    Console.WriteLine($"Found {campaigns.Count()} campaigns");
}
finally
{
    client.Dispose();
}
```

### 2. Dependency Injection Setup (Recommended)

#### Program.cs (.NET 6+)
```csharp
using FundraiseUp.Client.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add FundraiseUp client to DI container
builder.Services.AddFundraiseUpClient(options =>
{
    options.ApiKey = builder.Configuration["FundraiseUp:ApiKey"];
    options.RateLimitStrategy = RateLimitStrategy.Queue;
    options.LogLevel = LogLevel.Information;
});

var app = builder.Build();
```

#### appsettings.json
```json
{
  "FundraiseUp": {
    "ApiKey": "your-api-key-here"
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
    
    public async Task<DonationResponse> ProcessDonationAsync(decimal amount, string campaignId)
    {
        var request = new DonationRequest
        {
            Amount = amount,
            Currency = "USD",
            CampaignId = campaignId,
            PaymentMethod = new PaymentMethodInfo
            {
                Type = "card",
                Last4 = "1234",
                Brand = "visa"
            }
        };
        
        return await _client.CreateDonationAsync(request);
    }
}
```

## Common Usage Patterns

### 3. Working with Campaigns
```csharp
// Create a new campaign
var createRequest = new CreateCampaignRequest
{
    Name = "Annual Fundraiser 2025",
    Description = "Supporting our community programs",
    GoalAmount = 50000,
    Currency = "USD",
    StartDate = DateTimeOffset.UtcNow,
    EndDate = DateTimeOffset.UtcNow.AddMonths(3)
};

var campaign = await client.CreateCampaignAsync(createRequest);
Console.WriteLine($"Created campaign: {campaign.Id}");

// Get all campaigns with filtering
var campaigns = await client.GetCampaignsAsync(new CampaignQueryOptions
{
    Status = "active",
    Page = 1,
    Limit = 10
});

foreach (var c in campaigns)
{
    Console.WriteLine($"{c.Name}: ${c.CurrentAmount:N2} / ${c.GoalAmount:N2}");
}
```

### 4. Processing Donations
```csharp
// Create a donation
var donationRequest = new DonationRequest
{
    Amount = 100.00m,
    Currency = "USD",
    CampaignId = "campaign-123",
    DonorId = "donor-456", // Optional if donor exists
    PaymentMethod = new PaymentMethodInfo
    {
        Type = "card",
        Last4 = "4321",
        Brand = "mastercard"
    }
};

var donation = await client.CreateDonationAsync(donationRequest);
Console.WriteLine($"Donation created: {donation.Id}, Status: {donation.PaymentStatus}");

// Get donation details
var donationDetails = await client.GetDonationAsync(donation.Id);
Console.WriteLine($"Transaction ID: {donationDetails.TransactionId}");
```

### 5. Managing Donors
```csharp
// Create a new donor
var donorRequest = new CreateDonorRequest
{
    Email = "donor@example.com",
    FirstName = "John",
    LastName = "Doe",
    Phone = "+1234567890",
    Address = new AddressInfo
    {
        Street = "123 Main St",
        City = "Anytown",
        State = "CA",
        PostalCode = "12345",
        Country = "US"
    }
};

var donor = await client.CreateDonorAsync(donorRequest);

// Search donors
var donors = await client.GetDonorsAsync(new DonorQueryOptions
{
    Email = "donor@example.com"
});
```

## Advanced Configuration

### 6. Rate Limiting Configuration
```csharp
builder.Services.AddFundraiseUpClient(options =>
{
    options.ApiKey = "your-api-key";
    
    // Strategy options:
    options.RateLimitStrategy = RateLimitStrategy.Queue;  // Queue requests when limit hit
    // options.RateLimitStrategy = RateLimitStrategy.Retry;     // Retry with backoff
    // options.RateLimitStrategy = RateLimitStrategy.Exception; // Throw exception immediately
    
    // Configure retry behavior (when using Retry strategy)
    options.RetryOptions = new RetryConfiguration
    {
        MaxRetries = 3,
        BaseDelay = TimeSpan.FromSeconds(1),
        MaxDelay = TimeSpan.FromSeconds(30),
        BackoffMultiplier = 2.0
    };
});
```

### 7. Logging Configuration
```csharp
builder.Services.AddFundraiseUpClient(options =>
{
    options.ApiKey = "your-api-key";
    options.LogLevel = LogLevel.Debug; // For detailed request/response logging
});

// In appsettings.json for fine-grained control:
{
  "Logging": {
    "LogLevel": {
      "FundraiseUp.Client": "Debug"
    }
  }
}
```

### 8. Error Handling
```csharp
try
{
    var donation = await client.CreateDonationAsync(request);
}
catch (FundraiseUpRateLimitException ex)
{
    // Handle rate limiting
    Console.WriteLine($"Rate limited. Retry after: {ex.RetryAfter}");
}
catch (FundraiseUpApiException ex)
{
    // Handle API errors
    Console.WriteLine($"API Error: {ex.ErrorCode} - {ex.Message}");
    if (ex.ErrorDetails?.ContainsKey("validation_errors") == true)
    {
        // Handle validation errors
    }
}
catch (FundraiseUpAuthenticationException ex)
{
    // Handle authentication issues
    Console.WriteLine("Invalid API key or authentication failed");
}
catch (FundraiseUpException ex)
{
    // Handle general client errors
    Console.WriteLine($"Client error: {ex.Message}");
}
```

## Testing Your Integration

### 9. Basic Connectivity Test
```csharp
public async Task TestConnection()
{
    try
    {
        // Simple test to verify API connectivity
        var campaigns = await client.GetCampaignsAsync(new CampaignQueryOptions { Limit = 1 });
        Console.WriteLine("✅ API connection successful");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ API connection failed: {ex.Message}");
    }
}
```

### 10. Validation Test Scenarios
```csharp
// Test validation handling
try
{
    var invalidRequest = new DonationRequest
    {
        Amount = -100, // Invalid: negative amount
        Currency = "INVALID", // Invalid: not ISO 4217
        CampaignId = "" // Invalid: empty string
    };
    
    await client.CreateDonationAsync(invalidRequest);
}
catch (FundraiseUpApiException ex)
{
    Console.WriteLine($"Validation caught: {ex.ErrorCode}");
    // Should throw validation error
}
```

## Performance Tips

- **Use Dependency Injection**: Leverages HttpClientFactory for connection pooling
- **Implement Cancellation**: Always pass cancellation tokens for long operations
- **Monitor Rate Limits**: Use Queue strategy for high-throughput scenarios  
- **Configure Timeouts**: Set appropriate timeouts based on your use case
- **Enable Logging**: Use Information level in production, Debug for troubleshooting

## Next Steps

1. **Review API Documentation**: Check FundraiseUp's API docs for all available endpoints
2. **Set Up Monitoring**: Integrate with your logging and monitoring systems
3. **Implement Error Handling**: Add comprehensive error handling for production use
4. **Performance Testing**: Test rate limiting behavior under load
5. **Security Review**: Ensure API keys are stored securely (Azure Key Vault, etc.)

For more detailed examples and advanced scenarios, see the full documentation and API reference.