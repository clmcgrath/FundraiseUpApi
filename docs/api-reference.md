---
layout: default
title: API Reference
nav_order: 6
description: "Complete API reference for the FundraiseUp .NET Client Library"
---

# API Reference
{: .no_toc }

Complete documentation for all classes, methods, and interfaces in the FundraiseUp .NET Client Library.

## Table of Contents
{: .no_toc .text-delta }

1. TOC
{:toc}

---

## Core Classes

### FundraiseUpClient
The main client class for interacting with the FundraiseUp API.

```csharp
public class FundraiseUpClient : IFundraiseUpClient
{
    // Properties
    public IDonationOperations Donations { get; }
    public ICampaignOperations Campaigns { get; }
    public ISupporterOperations Supporters { get; }
    public IFundraiserOperations Fundraisers { get; }
    public IRecurringPlanOperations RecurringPlans { get; }
    public IEventOperations Events { get; }
    public IDonorPortalOperations DonorPortal { get; }
}
```

**Usage Examples:**
```csharp
// Basic client creation
var client = new FundraiseUpClient("your-api-key");

// With options
var client = new FundraiseUpClient(options => {
    options.ApiKey = "your-api-key";
    options.RateLimitStrategy = RateLimitStrategy.Queue;
    options.Timeout = TimeSpan.FromSeconds(30);
});
```

### FundraiseUpClientOptions
Configuration options for the FundraiseUp client.

```csharp
public class FundraiseUpClientOptions
{
    public string ApiKey { get; set; }
    public string BaseUrl { get; set; } = "https://api.fundraiseup.com";
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);
    public RateLimitStrategy RateLimitStrategy { get; set; } = RateLimitStrategy.Queue;
    public int MaxConcurrentRequests { get; set; } = 3;
    public TimeSpan RetryDelay { get; set; } = TimeSpan.FromSeconds(1);
    public int MaxRetryAttempts { get; set; } = 3;
}
```

---

## Operations

### IDonationOperations
Manages donation-related operations.

**Key Methods:**
- `Task<DonationResponse> GetAsync(string donationId)`
- `Task<DonationListResponse> ListAsync(DonationListRequest request)`
- `Task<DonationResponse> CreateAsync(DonationRequest request)`
- `Task<DonationResponse> UpdateAsync(string donationId, DonationUpdateRequest request)`

**Example:**
```csharp
var donation = await client.Donations
    .Create()
    .WithAmount(100.00m)
    .WithCurrency("USD")
    .WithCampaignId("campaign-123")
    .ExecuteAsync();
```

### ICampaignOperations
Manages campaign-related operations (read-only).

**Key Methods:**
- `Task<CampaignResponse> GetAsync(string campaignId)`
- `Task<CampaignListResponse> ListAsync(CampaignListRequest request)`

### ISupporterOperations
Manages supporter/donor operations.

**Key Methods:**
- `Task<SupporterResponse> GetAsync(string supporterId)`
- `Task<SupporterListResponse> ListAsync(SupporterListRequest request)`
- `Task<SupporterResponse> CreateAsync(SupporterRequest request)`
- `Task<SupporterResponse> UpdateAsync(string supporterId, SupporterUpdateRequest request)`

### IFundraiserOperations
Manages individual fundraiser operations.

### IRecurringPlanOperations
Manages recurring donation plans (read-only).

### IEventOperations
Provides access to FundraiseUp audit logs and system events.

### IDonorPortalOperations
Generates secure access links for supporter self-service.

---

## Rate Limiting

### RateLimitStrategy
Configures how the client handles rate limiting.

```csharp
public enum RateLimitStrategy
{
    Queue,      // Queue requests when limit reached
    Retry,      // Retry with exponential backoff
    Exception   // Throw exception immediately
}
```

### Rate Limiting Behavior
- **Concurrent Limit**: 3 requests (respects FundraiseUp's limit)
- **Thread-Safe**: Works across all threads and connection pools
- **Configurable**: Choose between Queue, Retry, or Exception strategies

---

## Models

### Request Models

**DonationRequest**
```csharp
public class DonationRequest
{
    public decimal Amount { get; set; }
    public string Currency { get; set; }
    public string CampaignId { get; set; }
    public string SupporterId { get; set; }
    // ... additional properties
}
```

**SupporterRequest**
```csharp
public class SupporterRequest
{
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public AddressRequest Address { get; set; }
    // ... additional properties
}
```

### Response Models

**DonationResponse**
```csharp
public class DonationResponse
{
    public string Id { get; set; }
    public string Amount { get; set; }
    public string Currency { get; set; }
    public string Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public SupporterResponse Supporter { get; set; }
    public CampaignResponse Campaign { get; set; }
    // ... additional properties
}
```

### List Responses
All list operations return paginated responses with cursor-based pagination:

```csharp
public class DonationListResponse
{
    public List<DonationResponse> Data { get; set; }
    public PaginationInfo Pagination { get; set; }
}

public class PaginationInfo
{
    public string NextCursor { get; set; }
    public string PreviousCursor { get; set; }
    public bool HasMore { get; set; }
}
```

---

## Exception Handling

### Exception Types

**FundraiseUpException**
Base exception for all FundraiseUp-related errors.

**FundraiseUpApiException**
Thrown for API-specific errors (4xx, 5xx responses).

**FundraiseUpRateLimitException**
Thrown when rate limits are exceeded (only with Exception strategy).

**FundraiseUpValidationException**
Thrown for request validation errors.

### Error Handling Example
```csharp
try
{
    var donation = await client.Donations.GetAsync("donation-id");
}
catch (FundraiseUpApiException ex)
{
    Console.WriteLine($"API Error: {ex.StatusCode} - {ex.Message}");
}
catch (FundraiseUpRateLimitException ex)
{
    Console.WriteLine($"Rate limit exceeded: {ex.Message}");
}
```

---

## Fluent API Patterns

### Method Chaining
All operations support fluent configuration:

```csharp
// Donation creation with fluent API
var donation = await client.Donations
    .Create()
    .WithAmount(100.00m)
    .WithCurrency("USD")
    .WithCampaignId("campaign-123")
    .WithSupporterEmail("donor@example.com")
    .ExecuteAsync();

// List operations with pagination
var donations = await client.Donations
    .List()
    .WithLimit(50)
    .WithCursor("next-page-cursor")
    .ExecuteAsync();
```

### Builder Patterns
Complex requests use builder patterns for clarity:

```csharp
var supporter = await client.Supporters
    .Create()
    .WithEmail("supporter@example.com")
    .WithName("John", "Doe")
    .WithAddress(address => address
        .WithLine1("123 Main St")
        .WithCity("Anytown")
        .WithPostalCode("12345")
        .WithCountry("US"))
    .ExecuteAsync();
```

---

## Dependency Injection

### Service Registration
```csharp
// In Program.cs or Startup.cs
services.AddFundraiseUpClient(options =>
{
    options.ApiKey = configuration["FundraiseUp:ApiKey"];
    options.RateLimitStrategy = RateLimitStrategy.Queue;
    options.MaxRetryAttempts = 3;
});
```

### Service Usage
```csharp
public class DonationService
{
    private readonly IFundraiseUpClient _client;
    
    public DonationService(IFundraiseUpClient client)
    {
        _client = client;
    }
    
    public async Task<DonationResponse> ProcessDonationAsync(decimal amount)
    {
        return await _client.Donations
            .Create()
            .WithAmount(amount)
            .WithCurrency("USD")
            .ExecuteAsync();
    }
}
```

---

## Source Code Reference

For complete implementation details, see the source code:
- **GitHub Repository**: [https://github.com/clmcgrath/FundraiseUpApi](https://github.com/clmcgrath/FundraiseUpApi)
- **Core Client**: `src/FundraiseUp.Client/FundraiseUpClient.cs`
- **Operations**: `src/FundraiseUp.Client/Operations/`
- **Models**: `src/FundraiseUp.Client/Models/`
- **Interfaces**: `src/FundraiseUp.Client/Interfaces/`

For detailed examples and usage patterns, see our [Examples Guide](EXAMPLES.md) and [Getting Started](getting-started.md) documentation.