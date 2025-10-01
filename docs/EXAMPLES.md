# FundraiseUp .NET Client - Usage Examples

This document provides comprehensive examples of how to use the FundraiseUp .NET Client Library for various scenarios.

## Table of Contents

- [Getting Started](#getting-started)
- [Donation Management](#donation-management)
- [Supporter Management](#supporter-management)
- [Fundraiser Management](#fundraiser-management)
- [Recurring Plan Operations](#recurring-plan-operations)
- [Event Operations (Audit Logs)](#event-operations-audit-logs)
- [Donor Portal Integration](#donor-portal-integration)
- [Error Handling](#error-handling)
- [Advanced Scenarios](#advanced-scenarios)
- [Testing Examples](#testing-examples)

## Getting Started

### Basic Setup

```csharp
using FundraiseUp.Client;
using FundraiseUp.Client.Configuration;

// Simple client setup
var client = new FundraiseUpClient("your-api-key");

// Advanced setup with configuration
var client = new FundraiseUpClient(new FundraiseUpClientOptions
{
    ApiKey = "your-api-key",
    BaseUrl = "https://api.fundraiseup.com",
    Timeout = TimeSpan.FromSeconds(30),
    MaxRetryAttempts = 3,
    RetryDelay = TimeSpan.FromSeconds(1),
    EnableLogging = true,
    LogLevel = LogLevel.Information
});
```

### Dependency Injection Setup

```csharp
// Program.cs or Startup.cs
using FundraiseUp.Client.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add FundraiseUp client to DI container
builder.Services.AddFundraiseUpClient(options =>
{
    options.ApiKey = builder.Configuration["FundraiseUp:ApiKey"];
    options.BaseUrl = builder.Configuration["FundraiseUp:BaseUrl"];
    options.Timeout = TimeSpan.FromSeconds(30);
    options.MaxRetryAttempts = 3;
    options.EnableLogging = true;
});

var app = builder.Build();
```

## Donation Management

### Creating Donations

```csharp
using FundraiseUp.Client.Requests;

// Basic donation creation (following actual FundraiseUp API spec)
var donation = await client.Donations
    .Create(new CreateDonationRequest
    {
        Amount = "100.00",
        Currency = "usd",
        Campaign = "FUNXXXXXXXX",
        Designation = "EXXXXXXX", // Required designation ID
        PaymentMethod = new PaymentMethodRequest
        {
            Stripe = new StripePaymentMethodRequest
            {
                Id = "pm_card_visa" // Test payment method ID
            }
        },
        Supporter = new SupporterRequest
        {
            FirstName = "John",
            LastName = "Donor",
            Email = "donor@example.com"
        }
    })
    .ExecuteAsync();

// Donation with full supporter information and optional fields
var detailedDonation = await client.Donations
    .Create(new CreateDonationRequest
    {
        Amount = "250.00",
        Currency = "usd",
        Campaign = "FUNXXXXXXXX",
        Designation = "EXXXXXXX",
        PaymentMethod = new PaymentMethodRequest
        {
            Stripe = new StripePaymentMethodRequest
            {
                Id = "pm_card_visa"
            }
        },
        Supporter = new SupporterRequest
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            Phone = "+1-555-123-4567",
            Address = new SupporterAddressRequest
            {
                Line1 = "123 Main St",
                City = "New York",
                Region = "NY",
                PostalCode = "10001",
                Country = "us"
            }
        },
        Comment = "Keep up the great work!",
        CustomFields = new List<CustomFieldRequest>
        {
            new() { Name = "Source", Value = "Newsletter" }
        }
    })
    .WithTimeout(TimeSpan.FromSeconds(60))
    .ExecuteAsync();

Console.WriteLine($"Donation created: {donation.Id}");
Console.WriteLine($"Status: {donation.Status}");
Console.WriteLine($"Amount: {donation.Amount} {donation.Currency}");
```

### Retrieving Donations

```csharp
// Get specific donation
var donation = await client.Donations
    .GetById("donation-123")
    .ExecuteAsync();

// Get donations with pagination (FundraiseUp API uses cursor-based pagination)
var recentDonations = await client.Donations
    .List()
    .WithLimit(50)
    .ExecuteAsync();

Console.WriteLine($"Found {recentDonations.Data.Count} donations");
foreach (var d in recentDonations.Data)
{
    Console.WriteLine($"- {d.Id}: ${d.Amount} {d.Currency} from {d.Supporter.Email}");
    Console.WriteLine($"  Status: {d.Status}, Created: {d.CreatedAt}");
}

// Get next page if available
if (recentDonations.HasMore)
{
    var lastDonationId = recentDonations.Data.Last().Id;
    var nextPage = await client.Donations
        .List()
        .WithLimit(50)
        .StartingAfter(lastDonationId)
        .ExecuteAsync();
}
```

### Updating Donations

```csharp
// Update donation details (only API-created donations within 24 hours)
var updatedDonation = await client.Donations
    .Update("DXXXXXXX", new UpdateDonationRequest
    {
        Supporter = new SupporterPutRequest
        {
            FirstName = "Jane",
            LastName = "Updated",
            Email = "updated@example.com"
        },
        Comment = "Updated donation comment",
        Campaign = "FUNXXXXXXXX", // Can change campaign
        Designation = "EXXXXXXX", // Can change designation
        CustomFields = new List<CustomFieldPutRequest>
        {
            new() { Name = "Source", Value = "Updated Source" }
        }
    })
    .ExecuteAsync();

Console.WriteLine($"Updated donation: {updatedDonation.Id}");
```

## Campaign Management

**Note:** The FundraiseUp API does not provide endpoints for creating or updating campaigns. Campaigns must be managed through the FundraiseUp Dashboard. The API only provides campaign information embedded in donation responses.

### Working with Campaign Data

```csharp
// Campaign information is available in donation responses
var donations = await client.Donations
    .List()
    .WithLimit(10)
    .ExecuteAsync();

foreach (var donation in donations.Data)
{
    var campaign = donation.Campaign;
    Console.WriteLine($"Campaign: {campaign.Name} (ID: {campaign.Id})");
    Console.WriteLine($"Campaign Code: {campaign.Code}");
}

// You can filter donations by campaign when creating them
var campaignDonation = await client.Donations
    .Create(new CreateDonationRequest
    {
        Amount = "50.00",
        Currency = "usd",
        Campaign = "FUNXXXXXXXX", // Your specific campaign ID
        Designation = "EXXXXXXX",
        PaymentMethod = new PaymentMethodRequest
        {
            Stripe = new StripePaymentMethodRequest { Id = "pm_card_visa" }
        },
        Supporter = new SupporterRequest
        {
            FirstName = "Campaign",
            LastName = "Supporter",
            Email = "supporter@example.com"
        }
    })
    .ExecuteAsync();

Console.WriteLine($"Donation created for campaign: {campaignDonation.Campaign.Name}");
```

### Campaign Information from API

The FundraiseUp API provides campaign information in the following format:

```csharp
// Campaign data structure (read-only from API responses)
public class Campaign
{
    public string Id { get; set; }        // e.g., "FUNXXXXXXXX"
    public string Code { get; set; }      // Campaign code from Dashboard
    public string Name { get; set; }      // Campaign name
}
```

## Supporter Management

**Note:** The FundraiseUp API calls donors "Supporters" and provides read-only access. Supporter records are automatically created and updated through donations.

### Retrieving Supporter Information

```csharp
// Get a specific supporter by ID
var supporter = await client.Supporters
    .GetById("SXXXXXXXX")
    .ExecuteAsync();

Console.WriteLine($"Supporter: {supporter.FirstName} {supporter.LastName}");
Console.WriteLine($"Email: {supporter.Email}");
Console.WriteLine($"Phone: {supporter.Phone}");
Console.WriteLine($"Created: {supporter.CreatedAt}");
Console.WriteLine($"Language: {supporter.Language}");

if (supporter.Address != null)
{
    Console.WriteLine($"Address: {supporter.Address.Line1}, {supporter.Address.City}");
}

if (supporter.Employer != null)
{
    Console.WriteLine($"Employer: {supporter.Employer.Name}");
}

// List all supporters with pagination
var supporters = await client.Supporters
    .List()
    .WithLimit(50)
    .ExecuteAsync();

Console.WriteLine($"Found {supporters.Data.Count} supporters");
foreach (var s in supporters.Data)
{
    Console.WriteLine($"- {s.FirstName} {s.LastName} ({s.Email})");
}
```

### Working with Supporter Data

```csharp
// Supporter information is automatically updated when donations are made
// The API provides the latest contact details for each supporter

// Get supporter data from donation
var donations = await client.Donations
    .List()
    .WithLimit(10)
    .ExecuteAsync();

foreach (var donation in donations.Data)
{
    var supporter = donation.Supporter;
    Console.WriteLine($"Donation from: {supporter.FirstName} {supporter.LastName}");
    Console.WriteLine($"Supporter ID: {supporter.Id}");
    Console.WriteLine($"Email: {supporter.Email}");
    
    if (supporter.Address != null)
    {
        Console.WriteLine($"Location: {supporter.Address.City}, {supporter.Address.Country}");
    }
}

// Pagination through supporters
var allSupporters = new List<SupporterResponse>();
var currentPage = await client.Supporters.List().WithLimit(100).ExecuteAsync();

do
{
    allSupporters.AddRange(currentPage.Data);
    
    if (currentPage.HasMore)
    {
        var lastId = currentPage.Data.Last().Id;
        currentPage = await client.Supporters
            .List()
            .WithLimit(100)
            .StartingAfter(lastId)
            .ExecuteAsync();
    }
    else
    {
        break;
    }
} while (true);

Console.WriteLine($"Total supporters retrieved: {allSupporters.Count}");
```

### Supporter Data Structure

```csharp
// Read-only supporter information from API
public class SupporterResponse
{
    public string Id { get; set; }              // e.g., "SXXXXXXXX"
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Phone { get; set; }
    public string Title { get; set; }           // e.g., "mr", "mrs", "ms"
    public string Language { get; set; }        // e.g., "en-US"
    public DateTime CreatedAt { get; set; }
    public bool LiveMode { get; set; }
    public AddressResponse Address { get; set; }
    public EmployerResponse Employer { get; set; }
    public AccountResponse Account { get; set; }
}
```

## Error Handling

### Comprehensive Error Handling Example

```csharp
using FundraiseUp.Client.Exceptions;

public async Task<Donation> CreateDonationWithErrorHandling(CreateDonationRequest request)
{
    try
    {
        return await client.Donations
            .Create(request)
            .WithTimeout(TimeSpan.FromSeconds(30))
            .ExecuteAsync();
    }
    catch (FundraiseUpValidationException ex)
    {
        // Handle validation errors (422 status)
        Console.WriteLine("Validation failed:");
        foreach (var error in ex.ValidationErrors)
        {
            Console.WriteLine($"- {error.Key}: {string.Join(", ", error.Value)}");
        }
        
        // Log for monitoring
        _logger.LogWarning("Donation validation failed: {ValidationErrors}", ex.ValidationErrors);
        throw;
    }
    catch (FundraiseUpApiException ex) when (ex.StatusCode == HttpStatusCode.Unauthorized)
    {
        // Handle authentication issues
        Console.WriteLine("Authentication failed - check your API key");
        _logger.LogError("API authentication failed");
        throw;
    }
    catch (FundraiseUpApiException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
    {
        // Handle missing resources
        Console.WriteLine("Campaign or donor not found");
        _logger.LogWarning("Resource not found: {Message}", ex.Message);
        throw;
    }
    catch (FundraiseUpApiException ex)
    {
        // Handle other API errors
        Console.WriteLine($"API error [{ex.StatusCode}]: {ex.Message}");
        _logger.LogError(ex, "FundraiseUp API error: {StatusCode}", ex.StatusCode);
        throw;
    }
    catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
    {
        // Handle timeout
        Console.WriteLine("Request timed out");
        _logger.LogWarning("Request timeout occurred");
        throw;
    }
    catch (HttpRequestException ex)
    {
        // Handle network issues
        Console.WriteLine("Network error occurred");
        _logger.LogError(ex, "Network error during API call");
        throw;
    }
}
```

## Advanced Scenarios

### Batch Operations

```csharp
// Process multiple donations efficiently
public async Task ProcessBatchDonations(List<CreateDonationRequest> requests)
{
    var semaphore = new SemaphoreSlim(3, 3); // Limit concurrent requests
    var tasks = requests.Select(async request =>
    {
        await semaphore.WaitAsync();
        try
        {
            return await client.Donations
                .Create(request)
                .WithRetry(3)
                .ExecuteAsync();
        }
        finally
        {
            semaphore.Release();
        }
    });

    var donations = await Task.WhenAll(tasks);
    Console.WriteLine($"Processed {donations.Length} donations");
}
```

### Custom Retry Logic

```csharp
// Implement custom retry with exponential backoff
public async Task<T> ExecuteWithCustomRetry<T>(Func<Task<T>> operation, int maxAttempts = 3)
{
    var delay = TimeSpan.FromSeconds(1);
    
    for (int attempt = 1; attempt <= maxAttempts; attempt++)
    {
        try
        {
            return await operation();
        }
        catch (FundraiseUpApiException ex) when (attempt < maxAttempts && IsRetryableError(ex))
        {
            Console.WriteLine($"Attempt {attempt} failed, retrying in {delay.TotalSeconds}s...");
            await Task.Delay(delay);
            delay = TimeSpan.FromMilliseconds(delay.TotalMilliseconds * 2); // Exponential backoff
        }
    }
    
    throw new InvalidOperationException($"Operation failed after {maxAttempts} attempts");
}

private bool IsRetryableError(FundraiseUpApiException ex)
{
    return ex.StatusCode == HttpStatusCode.InternalServerError ||
           ex.StatusCode == HttpStatusCode.BadGateway ||
           ex.StatusCode == HttpStatusCode.ServiceUnavailable ||
           ex.StatusCode == HttpStatusCode.GatewayTimeout;
}
```

### Configuration Management

```csharp
// Environment-based configuration
public class FundraiseUpService
{
    private readonly IFundraiseUpClient _client;
    private readonly ILogger<FundraiseUpService> _logger;

    public FundraiseUpService(IConfiguration configuration, ILogger<FundraiseUpService> logger)
    {
        _logger = logger;
        
        var options = new FundraiseUpClientOptions();
        configuration.GetSection("FundraiseUp").Bind(options);
        
        // Override for specific environments
        if (configuration["ASPNETCORE_ENVIRONMENT"] == "Development")
        {
            options.BaseUrl = "https://api-sandbox.fundraiseup.com";
            options.LogLevel = LogLevel.Debug;
        }
        
        _client = new FundraiseUpClient(options, logger);
    }

    public async Task<Donation> CreateDonationAsync(CreateDonationRequest request)
    {
        _logger.LogInformation("Creating donation for campaign {CampaignId}", request.CampaignId);
        
        var donation = await _client.Donations
            .Create(request)
            .ExecuteAsync();
            
        _logger.LogInformation("Created donation {DonationId} for ${Amount}", 
            donation.Id, donation.Amount);
            
        return donation;
    }
}
```

## Testing Examples

### Unit Testing with Mocks

```csharp
using Moq;
using Xunit;
using FundraiseUp.Client.Tests.TestHelpers;

public class DonationServiceTests
{
    [Fact]
    public async Task CreateDonation_WithValidRequest_ShouldReturnDonation()
    {
        // Arrange
        var expectedDonation = MockResponseBuilder.CreateSampleDonation("test-123");
        var mockResponse = MockResponseBuilder.CreateJsonResponse(expectedDonation, HttpStatusCode.Created);
        
        var httpSetup = new HttpClientMockSetup();
        httpSetup.SetupRequest(HttpMethod.Post, "/donations", mockResponse);
        
        var client = new FundraiseUpClient(
            "test-key",
            new FundraiseUpClientOptions { BaseUrl = "https://test.api" },
            httpSetup.CreateHttpClient()
        );

        var request = new CreateDonationRequest
        {
            Amount = "100.00",
            Currency = "usd",
            Campaign = "FUNXXXXXXXX",
            Designation = "EXXXXXXX",
            PaymentMethod = new PaymentMethodRequest
            {
                Stripe = new StripePaymentMethodRequest { Id = "pm_card_visa" }
            },
            Supporter = new SupporterRequest
            {
                FirstName = "Test",
                LastName = "User",
                Email = "test@example.com"
            }
        };

        // Act
        var result = await client.Donations.Create(request).ExecuteAsync();

        // Assert
        Assert.Equal("test-123", result.Id);
        Assert.Equal("100.00", result.Amount);
        
        // Verify HTTP request was made
        httpSetup.VerifyRequest(HttpMethod.Post, "/donations", Times.Once());
    }
}
```

### Integration Testing

```csharp
public class FundraiseUpIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly IFundraiseUpClient _client;

    public FundraiseUpIntegrationTests()
    {
        _client = new FundraiseUpClient(new FundraiseUpClientOptions
        {
            ApiKey = Environment.GetEnvironmentVariable("FUNDRAISEUP_TEST_API_KEY"),
            BaseUrl = "https://api-sandbox.fundraiseup.com",
            Timeout = TimeSpan.FromSeconds(60),
            EnableLogging = true
        });
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task CreateDonation_WithTestData_ShouldSucceed()
    {
        // Skip if no test API key available
        if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("FUNDRAISEUP_TEST_API_KEY")))
        {
            Skip.If(true, "Test API key not available");
        }

        // Arrange
        var request = new CreateDonationRequest
        {
            Amount = "1.00", // Minimal test amount
            Currency = "usd",
            Campaign = "FUNXXXXXXXX", // Use your test campaign ID
            Designation = "EXXXXXXX", // Use your test designation ID
            PaymentMethod = new PaymentMethodRequest
            {
                Stripe = new StripePaymentMethodRequest { Id = "pm_card_visa" }
            },
            Supporter = new SupporterRequest
            {
                FirstName = "Test",
                LastName = "User",
                Email = $"test-{Guid.NewGuid()}@example.com"
            }
        };

        // Act
        var donation = await _client.Donations.Create(request).ExecuteAsync();

        // Assert
        Assert.NotNull(donation);
        Assert.NotEmpty(donation.Id);
        Assert.Equal("1.00", donation.Amount);
        Assert.Equal("succeeded", donation.Status); // FundraiseUp status values
    }
}
```

This comprehensive examples documentation provides real-world usage patterns for the FundraiseUp .NET Client Library, covering everything from basic setup to advanced scenarios and testing strategies.