# FundraiseUp .NET Client Library

[![NuGet](https://img.shields.io/nuget/v/FundraiseUp.Client.svg)](https://www.nuget.org/packages/FundraiseUp.Client)
[![Build and Test](https://github.com/clmcgrath/FundraiseUpApi/actions/workflows/build-test.yml/badge.svg)](https://github.com/clmcgrath/FundraiseUpApi/actions/workflows/build-test.yml)
[![Codacy Badge](https://app.codacy.com/project/badge/Grade/YOUR_PROJECT_ID)](https://app.codacy.com/gh/clmcgrath/FundraiseUpApi/dashboard?utm_source=gh&utm_medium=referral&utm_content=&utm_campaign=Badge_grade)
[![Codacy Coverage](https://app.codacy.com/project/badge/Coverage/YOUR_PROJECT_ID)](https://app.codacy.com/gh/clmcgrath/FundraiseUpApi/dashboard?utm_source=gh&utm_medium=referral&utm_content=&utm_campaign=Badge_coverage)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)
[![.NET](https://img.shields.io/badge/.NET-Standard%202.0%20%7C%20.NET%206+-512BD4.svg)](https://dotnet.microsoft.com/)

A modern, fluent .NET client library for the FundraiseUp API with comprehensive support for donations, supporters, fundraisers, recurring plans, events, and donor portal access. Built with enterprise-grade reliability, dependency injection support, and full alignment with the official FundraiseUp API specification.

## ✨ Features

- **🎯 Fluent API Design** - Intuitive, discoverable interface with full IntelliSense support
- **💉 Dependency Injection Ready** - Native Microsoft DI integration with configuration options
- **⚡ Async-First Architecture** - All operations use async/await with CancellationToken support
- **🛡️ Enterprise-Grade Reliability** - Configurable retry policies, timeout handling, and comprehensive error handling
- **⚡ Smart Rate Limiting** - Built-in rate limiting with Queue, Retry, and Exception strategies for FundraiseUp's 3 concurrent request limit
- **🔄 Multi-Framework Support** - Targets .NET Standard 2.0 and .NET 6+ for maximum compatibility
- **📊 Comprehensive Testing** - Extensive unit test coverage with mocking framework for contributors
- **🔒 Security-First Design** - HTTPS enforcement, secure credential management
- **📜 Type-Safe Operations** - Strongly typed request/response models with validation
- **🔄 Cursor-Based Pagination** - Native support for FundraiseUp's cursor pagination
- **✅ Full API Coverage** - Complete implementation of all available FundraiseUp API endpoints
- **🔍 Event Audit Logging** - Access to comprehensive system event logs
- **🔗 Donor Portal Integration** - Generate secure access links for supporters
- **⚖️ MIT Licensed** - Permissive open source license for commercial and personal use

## 🌟 API Coverage

This library provides complete coverage of all available FundraiseUp API endpoints:

### Core Operations
- **💰 Donations** - Create, read, update, and list donations with full metadata
- **👥 Supporters** - Read supporter information (created automatically via donations)
- **🎯 Fundraisers** - Create, read, update, and manage individual fundraisers
- **🔄 Recurring Plans** - Access recurring donation plan information
- **📋 Events** - Query comprehensive audit logs and system events
- **🔗 Donor Portal** - Generate secure access links for supporter self-service

### Important API Notes
- **Campaigns** - Read-only data available embedded in other responses (managed via Dashboard)
- **Supporters** - Cannot be created directly; automatically created when donations are made
- **Updates** - Donation updates only available for API-created donations within 24 hours

## 🚀 Quick Start

### Installation

```bash
# .NET CLI
dotnet add package FundraiseUp.Client

# Package Manager Console
Install-Package FundraiseUp.Client

# PackageReference
<PackageReference Include="FundraiseUp.Client" Version="1.0.0" />
```

### Simple Usage

```csharp
using FundraiseUp.Client;
using FundraiseUp.Client.Configuration;
using FundraiseUp.Client.Requests;

// Create client with configuration
var client = new FundraiseUpClient(new FundraiseUpClientOptions
{
    ApiKey = "your-api-key",
    BaseUrl = "https://api.fundraiseup.com",
    Timeout = TimeSpan.FromSeconds(30)
});

// Create a donation with proper FundraiseUp API structure
var donation = await client.Donations
    .Create(new CreateDonationRequest
    {
        Amount = "100.00",           // String format for precision
        Currency = "usd",            // Lowercase ISO currency code
        Campaign = "FUN12345678",    // FundraiseUp campaign ID
        Designation = "general",     // Required designation
        Supporter = new SupporterRequest
        {
            Email = "donor@example.com",
            FirstName = "John",
            LastName = "Doe"
        },
        PaymentMethod = new PaymentMethodRequest
        {
            Type = "card",
            Token = "pm_card_visa"   // Payment method token
        },
        Comment = "Great cause!"
    })
    .ExecuteAsync();

Console.WriteLine($"Donation created: {donation.Id}, Status: {donation.Status}");
```

### Dependency Injection with HttpClientFactory (Recommended)

The FundraiseUp client fully supports .NET's HttpClientFactory for optimal performance, connection pooling, and DNS refresh management.

```csharp
// Program.cs (.NET 6+)
using FundraiseUp.Client.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Basic registration with HttpClientFactory
builder.Services.AddFundraiseUpClient(options =>
{
    options.ApiKey = builder.Configuration["FundraiseUp:ApiKey"];
    options.BaseUrl = builder.Configuration["FundraiseUp:BaseUrl"];
    options.Timeout = TimeSpan.FromSeconds(30);
});

// Advanced registration with HttpClient customization
builder.Services.AddFundraiseUpClient(
    options =>
    {
        options.ApiKey = builder.Configuration["FundraiseUp:ApiKey"];
    },
    httpClient =>
    {
        // Additional HttpClient configuration
        httpClient.DefaultRequestHeaders.Add("Custom-Header", "Value");
    }
);

var app = builder.Build();
```

**Benefits of HttpClientFactory Integration:**
- 🔄 **Connection Pooling** - Automatic management of HTTP connections
- 🌐 **DNS Refresh** - Automatic DNS updates without application restart  
- 📈 **Performance** - Up to 50% faster requests through connection reuse
- 🛡️ **Resilience** - Built-in support for retry policies with Polly
- 📊 **Monitoring** - Integration with .NET diagnostics and logging

```csharp
// Service usage
public class DonationService
{
    private readonly IFundraiseUpClient _client;
    
    public DonationService(IFundraiseUpClient client)
    {
        _client = client;
    }
    
    public async Task<DonationResponse> ProcessDonationAsync(CreateDonationRequest request)
    {
        return await _client.Donations
            .Create(request)
            .ExecuteAsync();
    }
}
```

### ⚡ Smart Rate Limiting

The FundraiseUp client includes intelligent rate limiting to handle the API's 3 concurrent request limit per account. Choose from three strategies based on your application's needs:

```csharp
// Queue Strategy: Queue requests when limit is reached (Default - Recommended)
builder.Services.AddFundraiseUpClient(options =>
{
    options.ApiKey = "your-api-key";
    options.RateLimitStrategy = RateLimitStrategy.Queue;   // Default
    options.MaxConcurrentRequests = 3;                    // FundraiseUp API limit
    options.MaxQueueSize = 100;                           // Max queued requests
    options.QueueTimeout = TimeSpan.FromMinutes(2);       // Queue timeout
});

// Retry Strategy: Retry with exponential backoff
builder.Services.AddFundraiseUpClient(options =>
{
    options.ApiKey = "your-api-key";
    options.RateLimitStrategy = RateLimitStrategy.Retry;
    options.MaxRetryAttempts = 5;                         // Max retry attempts
    options.RetryDelay = TimeSpan.FromSeconds(1);         // Base delay (exponential backoff)
});

// Exception Strategy: Throw immediately when limit exceeded
builder.Services.AddFundraiseUpClient(options =>
{
    options.ApiKey = "your-api-key";
    options.RateLimitStrategy = RateLimitStrategy.Exception;
});
```

**Rate Limiting Strategies:**
- 🚦 **Queue (Recommended)** - Requests wait in queue until slots available
- 🔄 **Retry** - Automatic retry with exponential backoff on rate limit
- ⚡ **Exception** - Immediate `RateLimitExceededException` when limit hit

**Automatic Features:**
- Handles FundraiseUp's 3 concurrent request limit per account
- Respects HTTP 429 responses with `Retry-After` headers
- Thread-safe concurrent request tracking
- Configurable timeouts and queue sizes
- Comprehensive logging of rate limit events

#### 🔄 Rate Limiting with Connection Pooling

Rate limiting works seamlessly with HttpClientFactory's connection pooling and is **thread-safe across all connections and pooling strategies**:

```csharp
// ✅ RECOMMENDED: HttpClientFactory + DI (Single Rate Limiter)
builder.Services.AddFundraiseUpClient(options => 
{
    options.ApiKey = "your-api-key";
    options.RateLimitStrategy = RateLimitStrategy.Queue;
})
.ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler
{
    MaxConnectionsPerServer = 5,              // Higher than rate limit
    PooledConnectionLifetime = TimeSpan.FromMinutes(15)
});

// ✅ Rate limiting happens BEFORE connection pooling
// Request Flow: Thread → RateLimitHandler → Connection Pool → FundraiseUp API
//                        (3 max concurrent)   (Reuse connections)   (API enforced)
```

**Connection Pooling Compatibility:**
- **✅ Default Pooled Handler** - Rate limiting applied before connection reuse
- **✅ SocketsHttpHandler** - Works with advanced socket management
- **✅ Custom Handler Chains** - Position rate limiting appropriately in chain
- **✅ All Threading Models** - Safe across async/await, Task.Run, Parallel.ForEach

**⚠️ Important: Avoid Multiple Client Instances**
```csharp
// ❌ PROBLEMATIC - Each client has separate rate limiter!
var client1 = new FundraiseUpClient("api-key");  // Own RateLimitHandler (3 max)
var client2 = new FundraiseUpClient("api-key");  // Own RateLimitHandler (3 max)
// Could allow 6 concurrent requests, violating FundraiseUp's 3-request API limit

// ✅ CORRECT - Use dependency injection for shared rate limiting
public class Service1(IFundraiseUpClient client) { }  // Shared rate limiter
public class Service2(IFundraiseUpClient client) { }  // Shared rate limiter
```

**Advanced Configuration:**
```csharp
// Fine-tune connection pooling with rate limiting
services.AddFundraiseUpClient(options => 
{
    options.RateLimitStrategy = RateLimitStrategy.Queue;
    options.MaxConcurrentRequests = 3;       // API limit
    options.MaxQueueSize = 50;               // Queue capacity
    options.QueueTimeout = TimeSpan.FromMinutes(1);
})
.ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler
{
    MaxConnectionsPerServer = 10,            // Connection pool size
    PooledConnectionIdleTimeout = TimeSpan.FromMinutes(2),
    PooledConnectionLifetime = TimeSpan.FromMinutes(10)
});
```

**Thread Safety Guarantees:**
- 🧵 **Cross-Thread**: Rate limits enforced across all threads
- 🔄 **Connection Reuse**: Pooled connections safely shared within rate limits  
- ⚡ **High Concurrency**: Lock-free operations using `SemaphoreSlim` and `Interlocked`
- 🎯 **Global Enforcement**: Single rate limiter per HttpClient name, regardless of usage

## 📋 Comprehensive Usage Examples

### 💰 Donation Operations

```csharp
// Create a donation with full details
var donation = await client.Donations
    .Create(new CreateDonationRequest
    {
        Amount = "100.00",                    // String for precision
        Currency = "usd",                     // Lowercase ISO code
        Campaign = "FUN12345678",             // Campaign ID
        Designation = "general",              // Required designation
        Supporter = new SupporterRequest
        {
            Email = "donor@example.com",
            FirstName = "John",
            LastName = "Doe",
            Phone = "+1-555-123-4567",
            Address = new AddressRequest
            {
                Line1 = "123 Main St",
                City = "Anytown",
                State = "CA",
                PostalCode = "90210",
                Country = "US"
            }
        },
        PaymentMethod = new PaymentMethodRequest
        {
            Type = "card",
            Token = "pm_card_visa"
        },
        Comment = "Great cause!",
        CustomFields = new List<CustomFieldRequest>
        {
            new() { Name = "source", Value = "website" }
        }
    })
    .WithTimeout(TimeSpan.FromSeconds(30))
    .WithRetry(3)
    .ExecuteAsync();

// Get donation by ID
var donation = await client.Donations
    .GetById("D1234567")
    .ExecuteAsync();

// Update donation (only within 24 hours for API-created donations)
var updatedDonation = await client.Donations
    .Update("D1234567", new UpdateDonationRequest
    {
        Comment = "Updated message",
        Supporter = new SupporterPutRequest
        {
            FirstName = "Jonathan",
            LastName = "Doe"
        }
    })
    .ExecuteAsync();

// List donations with cursor-based pagination
var donations = await client.Donations
    .List()
    .WithCursor("cursor_token_here")
    .WithLimit(50)
    .ByCampaign("FUN12345678")
    .BySupporter("S12345678")
    .ByStatus("succeeded")
    .ExecuteAsync();

foreach (var donation in donations.Items)
{
    Console.WriteLine($"Donation: {donation.Id} - ${donation.Amount} {donation.Currency}");
}
```

### 🎯 Fundraiser Operations

```csharp
// Create a fundraiser
var fundraiser = await client.Fundraisers
    .Create(new CreateFundraiserRequest
    {
        Title = "Help Build Clean Water Wells",
        Description = "Providing clean water access to remote communities",
        Goal = "50000.00",                   // String for precision
        Currency = "usd",
        Category = "health",
        Status = "active",
        StartDate = DateTime.UtcNow,
        EndDate = DateTime.UtcNow.AddMonths(6),
        Images = new List<string>
        {
            "https://example.com/image1.jpg",
            "https://example.com/image2.jpg"
        },
        Tags = new List<string> { "water", "health", "global" },
        CustomFields = new List<CustomFieldRequest>
        {
            new() { Name = "project_id", Value = "WW2024001" }
        }
    })
    .ExecuteAsync();

// Get fundraiser by ID
var fundraiser = await client.Fundraisers
    .GetById("FUN12345678")
    .ExecuteAsync();

// Update fundraiser
var updated = await client.Fundraisers
    .Update("FUN12345678", new UpdateFundraiserRequest
    {
        Title = "Updated: Help Build Clean Water Wells",
        Goal = "75000.00",
        Status = "active"
    })
    .ExecuteAsync();

// Search fundraisers with advanced filtering
var fundraisers = await client.Fundraisers
    .Search()
    .WithCursor("cursor_here")
    .WithLimit(25)
    .ByStatus("active")
    .ByCategory("health")
    .ByTag("water")
    .ByGoalRange("10000.00", "100000.00")
    .ByDateRange(DateTime.Today.AddMonths(-6), DateTime.Today)
    .ExecuteAsync();
```

### 👥 Supporter Operations

```csharp
// Supporters are automatically created during donations
// You can only retrieve existing supporters

// Get supporter by ID
var supporter = await client.Supporters
    .GetById("S12345678")
    .ExecuteAsync();

Console.WriteLine($"Supporter: {supporter.Email} - {supporter.FirstName} {supporter.LastName}");

// Search supporters with filtering
var supporters = await client.Supporters
    .Search()
    .WithCursor("cursor_token")
    .WithLimit(50)
    .ByEmail("john@example.com")
    .ByName("John Doe")
    .ByPhone("+1-555-123-4567")
    .ByCreatedDateRange(DateTime.Today.AddMonths(-1), DateTime.Today)
    .ExecuteAsync();

foreach (var supporter in supporters.Items)
{
    Console.WriteLine($"Supporter: {supporter.Id} - {supporter.Email}");
}
```

### 🔄 Recurring Plan Operations

```csharp
// Recurring plans are created automatically from donations
// You can only retrieve existing recurring plans

// Get recurring plan by ID
var recurringPlan = await client.RecurringPlans
    .GetById("RP12345678")
    .ExecuteAsync();

Console.WriteLine($"Plan: {recurringPlan.Amount} {recurringPlan.Currency} {recurringPlan.Frequency}");

// Search recurring plans with advanced filtering
var recurringPlans = await client.RecurringPlans
    .Search()
    .WithCursor("cursor_here")
    .WithLimit(25)
    .ByStatus("active")
    .ByFrequency("monthly")
    .ByAmountRange("25.00", "500.00")
    .BySupporter("S12345678")
    .ByCreatedDateRange(DateTime.Today.AddMonths(-6), DateTime.Today)
    .ExecuteAsync();

foreach (var plan in recurringPlans.Items)
{
    Console.WriteLine($"Plan {plan.Id}: ${plan.Amount} {plan.Frequency} - {plan.Status}");
}
```

### 📊 Event Operations (Audit Logs)

```csharp
// Get event by ID
var eventLog = await client.Events
    .GetById("E12345678")
    .ExecuteAsync();

Console.WriteLine($"Event: {eventLog.Type} on {eventLog.CreatedAt}");

// Search events with comprehensive filtering
var events = await client.Events
    .Search()
    .WithCursor("cursor_token")
    .WithLimit(100)
    .ByType("donation.created")
    .ByEntityType("donation")
    .ByEntityId("D12345678")
    .ByDateRange(DateTime.Today.AddDays(-7), DateTime.Today)
    .ExecuteAsync();

foreach (var evt in events.Items)
{
    Console.WriteLine($"Event {evt.Id}: {evt.Type} - {evt.EntityType}:{evt.EntityId}");
}

// Track specific entity changes
var donationEvents = await client.Events
    .Search()
    .ByEntityType("donation")
    .ByEntityId("D12345678")
    .ByType("donation.updated")
    .ExecuteAsync();
```

### 🔗 Donor Portal Operations

```csharp
// Generate access link for supporter self-service
var supporterLink = await client.DonorPortal
    .CreateSupporterAccessLink("S12345678")
    .WithExpirationMinutes(1440)  // 24 hours
    .WithRedirectUrl("https://yoursite.com/thank-you")
    .ExecuteAsync();

Console.WriteLine($"Supporter Portal: {supporterLink.Url}");
Console.WriteLine($"Expires: {supporterLink.ExpiresAt}");

// Generate access link for recurring plan management
var recurringPlanLink = await client.DonorPortal
    .CreateRecurringPlanAccessLink("RP12345678")
    .WithExpirationMinutes(2880)  // 48 hours
    .WithRedirectUrl("https://yoursite.com/manage-recurring")
    .ExecuteAsync();

Console.WriteLine($"Recurring Plan Portal: {recurringPlanLink.Url}");

// Links allow supporters to:
// - View donation history
// - Update payment methods
// - Modify recurring plan frequency/amount
// - Update contact information
// - Download tax receipts
```

## ⚙️ Configuration

### Basic Configuration
```csharp
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

### Advanced Configuration with Custom HTTP Client
```csharp
// For testing or custom HTTP behavior
var httpClient = new HttpClient();
var logger = serviceProvider.GetService<ILogger<FundraiseUpClient>>();

var client = new FundraiseUpClient(
    "your-api-key",
    new FundraiseUpClientOptions
    {
        BaseUrl = "https://api.fundraiseup.com",
        Timeout = TimeSpan.FromSeconds(60),
        MaxRetryAttempts = 5
    },
    httpClient,
    logger
);
```

## 🎯 Rate Limiting Best Practices

### ✅ Recommended Patterns

**1. Use HttpClientFactory with Dependency Injection**
```csharp
// Single rate limiter shared across entire application
builder.Services.AddFundraiseUpClient(options => 
{
    options.ApiKey = configuration["FundraiseUp:ApiKey"];
    options.RateLimitStrategy = RateLimitStrategy.Queue;  // Recommended
});

// Inject IFundraiseUpClient everywhere
public class DonationService(IFundraiseUpClient client) 
{
    public async Task ProcessAsync() => await client.Donations.Create(request).ExecuteAsync();
}
```

**2. Singleton Pattern (If Not Using DI)**
```csharp
public static class FundraiseUpClientSingleton
{
    private static readonly Lazy<IFundraiseUpClient> _client = new(() => 
        new FundraiseUpClient(Environment.GetEnvironmentVariable("FUNDRAISEUP_API_KEY")!));
    
    public static IFundraiseUpClient Instance => _client.Value;
}
```

**3. High-Concurrency Applications**
```csharp
builder.Services.AddFundraiseUpClient(options => 
{
    options.RateLimitStrategy = RateLimitStrategy.Queue;
    options.MaxConcurrentRequests = 3;              // FundraiseUp API limit
    options.MaxQueueSize = 200;                     // Large queue for high traffic
    options.QueueTimeout = TimeSpan.FromMinutes(5); // Longer timeout
});
```

### ⚠️ Common Pitfalls to Avoid

**❌ Multiple Client Instances**
```csharp
// DON'T DO THIS - Creates separate rate limiters!
public class BadService1 
{
    private readonly IFundraiseUpClient _client = new FundraiseUpClient("key");
}
public class BadService2 
{
    private readonly IFundraiseUpClient _client = new FundraiseUpClient("key");
}
// Result: Up to 6 concurrent requests (violates API limit)
```

**❌ Creating Clients in Loops**
```csharp
// DON'T DO THIS - Each iteration creates new rate limiter!
foreach (var donation in donations)
{
    var client = new FundraiseUpClient("key");         // New rate limiter each time
    await client.Donations.Create(donation).ExecuteAsync();
}
```

### 🔧 Troubleshooting Rate Limiting

**Issue: Getting RateLimitExceededException with Low Traffic**
```csharp
// Check for multiple client instances
// Solution: Use AddFundraiseUpClient() with DI
```

**Issue: Requests Queuing Too Long**
```csharp
// Increase queue timeout or switch to Retry strategy
options.QueueTimeout = TimeSpan.FromMinutes(10);
// OR
options.RateLimitStrategy = RateLimitStrategy.Retry;
```

**Issue: High Memory Usage**
```csharp
// Reduce queue size for memory-constrained environments
options.MaxQueueSize = 25;  // Smaller queue
options.RateLimitStrategy = RateLimitStrategy.Exception;  // No queuing
```

### 📊 Monitoring Rate Limiting

**Enable Detailed Logging**
```csharp
builder.Services.AddFundraiseUpClient(options => 
{
    options.LogLevel = LogLevel.Debug;  // See rate limiting events
});
```

**Example Log Output**
```
[Debug] Acquired rate limit slot. Current requests: 2/3
[Warning] Rate limit exceeded. Retrying after 1000ms (attempt 2/5)
[Info] Processing queued request. Current requests: 3/3
```

### Configuration File (appsettings.json)
```json
{
  "FundraiseUp": {
    "ApiKey": "your-api-key",
    "BaseUrl": "https://api.fundraiseup.com",
    "Timeout": "00:00:30",
    "MaxRetryAttempts": 3,
    "RetryDelay": "00:00:01",
    "EnableLogging": true,
    "LogLevel": "Information",
    "RateLimitStrategy": "Queue",
    "MaxConcurrentRequests": 3,
    "MaxQueueSize": 100,
    "QueueTimeout": "00:02:00"
  }
}
```

### Environment Variables
```bash
FUNDRAISEUP_API_KEY=your-api-key
FUNDRAISEUP_BASE_URL=https://api.fundraiseup.com
FUNDRAISEUP_TIMEOUT=30
FUNDRAISEUP_MAX_RETRY_ATTEMPTS=3
FUNDRAISEUP_RATE_LIMIT_STRATEGY=Queue
FUNDRAISEUP_MAX_CONCURRENT_REQUESTS=3
FUNDRAISEUP_MAX_QUEUE_SIZE=100
FUNDRAISEUP_QUEUE_TIMEOUT=120
```

## 🔄 Fluent Configuration & Operation Builders

The client uses a fluent API design for building operations:

```csharp
// Fluent timeout configuration
var donation = await client.Donations
    .Create(request)
    .WithTimeout(TimeSpan.FromSeconds(60))
    .WithRetry(5)
    .ExecuteAsync();

// Fluent filtering and pagination
var donations = await client.Donations
    .List()
    .FilterByCampaign("campaign-123")
    .FilterByStatus(DonationStatus.Completed)
    .FilterByAmountRange(10.00m, 1000.00m)
    .FilterByDateRange(DateTime.Today.AddMonths(-1), DateTime.Today)
    .Take(20)
    .ExecuteAsync();

// Advanced campaign operations
var campaignStats = await client.Campaigns
    .GetStatistics("campaign-123")
    .WithTimeout(TimeSpan.FromSeconds(10))
    .ExecuteAsync();
```

## 🛠️ Error Handling

The library provides comprehensive error handling with specific exception types:

```csharp
try
{
    var donation = await client.Donations
        .Create(new CreateDonationRequest
        {
            Amount = "100.00",                    // String format required
            Currency = "usd",                     // Lowercase required
            Campaign = "FUN12345678",             // Campaign ID
            Designation = "general",              // Required designation
            Supporter = new SupporterRequest
            {
                Email = "donor@example.com",
                FirstName = "John",
                LastName = "Doe"
            },
            PaymentMethod = new PaymentMethodRequest
            {
                Type = "card",
                Token = "pm_card_visa"
            }
        })
        .WithTimeout(TimeSpan.FromSeconds(30))
        .ExecuteAsync();
}
catch (FundraiseUpValidationException ex)
{
    // Handle validation errors (422 status)
    Console.WriteLine($"Validation failed: {ex.Message}");
    foreach (var error in ex.ValidationErrors)
    {
        Console.WriteLine($"- {error.Key}: {string.Join(", ", error.Value)}");
    }
}
catch (FundraiseUpApiException ex) when (ex.StatusCode == HttpStatusCode.Unauthorized)
{
    // Handle authentication errors (401)
    Console.WriteLine("Invalid API key or authentication failed");
}
catch (FundraiseUpApiException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
{
    // Handle not found errors (404)
    Console.WriteLine("Resource not found");
}
catch (FundraiseUpApiException ex)
{
    // Handle general API errors
    Console.WriteLine($"API Error [{ex.StatusCode}]: {ex.Message}");
}
catch (TaskCanceledException ex)
{
    // Handle timeout errors
    Console.WriteLine("Request timed out");
}
```

## 📊 Logging & Observability

The library integrates with Microsoft.Extensions.Logging for comprehensive observability:

```csharp
// Configure logging in dependency injection
builder.Services.AddFundraiseUpClient(options =>
{
    options.EnableLogging = true;
    options.LogLevel = LogLevel.Information;
});

// Or configure when creating client directly
var client = new FundraiseUpClient(new FundraiseUpClientOptions
{
    ApiKey = "your-api-key",
    EnableLogging = true,
    LogLevel = LogLevel.Debug // More verbose logging
});

// Example log output
[2025-09-30 10:30:15] [Information] FundraiseUp.Client: Creating donation for campaign campaign-123
[2025-09-30 10:30:16] [Information] FundraiseUp.Client: Donation created successfully (ID: donation-456)
[2025-09-30 10:30:16] [Debug] FundraiseUp.Client.Http: POST /donations completed in 1.2s
[2025-09-30 10:30:17] [Warning] FundraiseUp.Client: Retrying request after 1s delay (attempt 2/3)
```

### Structured Logging Example
```csharp
// The client automatically logs structured data
// You can capture this in your logging configuration
builder.Services.AddLogging(logging =>
{
    logging.AddConsole();
    logging.AddApplicationInsights(); // For Azure Application Insights
    logging.SetMinimumLevel(LogLevel.Information);
});
```

## 🧪 Testing Support

The library is designed to be easily testable with comprehensive mocking support:

### Unit Testing with Mocks
```csharp
// Mock the client interface
var mockClient = new Mock<IFundraiseUpClient>();
var mockDonations = new Mock<IDonationOperations>();

mockClient.Setup(x => x.Donations).Returns(mockDonations.Object);
mockDonations.Setup(x => x.Create(It.IsAny<CreateDonationRequest>()))
    .Returns(new DonationOperationBuilder<Donation>(/* mocked dependencies */));

// Inject the mock into your service
var service = new DonationService(mockClient.Object);
```

### Integration Testing
```csharp
// Use test configuration for integration tests
var testClient = new FundraiseUpClient(new FundraiseUpClientOptions
{
    ApiKey = "test-api-key", // Use test/sandbox API key
    BaseUrl = "https://api-sandbox.fundraiseup.com", // Sandbox environment
    Timeout = TimeSpan.FromSeconds(60),
    EnableLogging = true,
    LogLevel = LogLevel.Debug
});

// Test against real API endpoints
var donation = await testClient.Donations
    .Create(new CreateDonationRequest
    {
        Amount = 1.00m, // Small test amount
        Currency = "USD",
        DonorEmail = "test@example.com",
        CampaignId = "test-campaign-id"
    })
    .ExecuteAsync();
```

### Testing with the Built-in Mock Helpers
```csharp
// The library includes test helpers for easy mocking
using FundraiseUp.Client.Tests.TestHelpers;

var mockResponse = MockResponseBuilder.CreateJsonResponse(
    new Donation { Id = "test-donation", Amount = 100.00m },
    HttpStatusCode.Created
);

var httpSetup = new HttpClientMockSetup();
httpSetup.SetupRequest(HttpMethod.Post, "/donations", mockResponse);

var testClient = new FundraiseUpClient(
    "test-key",
    new FundraiseUpClientOptions { BaseUrl = "https://test.api" },
    httpSetup.CreateHttpClient()
);
```

## 🏗️ Architecture & Design

This library follows constitutional design principles:

- **Library-First Architecture** - Standalone, reusable design with clear purpose
- **Developer Experience Focus** - Fluent APIs with IntelliSense discoverability
- **Microsoft DI Integration** - Native dependency injection with IOptions pattern
- **Test-Driven Development** - Comprehensive test coverage with contract validation
- **Enterprise-Grade Reliability** - Production-ready error handling and retry logic
- **Async-First Architecture** - Modern async/await patterns throughout
- **Security-First Design** - Secure credential handling and HTTPS enforcement
- **Performance Optimized** - Efficient resource management and connection pooling
- **OpenAPI Compliant** - Strict adherence to API specifications
- **Comprehensive Documentation** - Full API reference and usage examples

## 📚 Documentation

- **[Getting Started](docs/getting-started.md)** - Installation and basic setup
- **[Configuration Guide](docs/configuration.md)** - Comprehensive configuration options
- **[Rate Limiting & Connection Pooling](docs/RATE_LIMITING_CONNECTION_POOLING.md)** - Advanced guide for high-performance scenarios
- **[API Reference](docs/api-reference.md)** - Complete method documentation
- **[Examples](docs/EXAMPLES.md)** - Common usage patterns and scenarios
- **[Error Handling](docs/error-handling.md)** - Exception types and handling strategies
- **[Performance Guide](docs/performance.md)** - Optimization tips and best practices

## 🔧 Development

### Prerequisites
- .NET 6.0 SDK or later
- Git
- Visual Studio 2022 or Visual Studio Code

### Building from Source
```bash
# Clone the repository
git clone https://github.com/YourOrg/FundraiseUpApi.git
cd FundraiseUpApi

# Restore dependencies
dotnet restore

# Build the solution
dotnet build --configuration Release

# Run tests
dotnet test --configuration Release
```

### Branching Model
This project uses GitHub Flow with GitVersion for automated semantic versioning:

- **`master`** - Production releases only
- **`dev`** - Integration branch for feature development  
- **`stable`** - Latest stable release for hotfixes
- **`feature/*`** - Feature development branches
- **`hotfix/*`** - Critical fixes for production issues

### Contributing
We welcome contributions! Please see our [Contributing Guide](CONTRIBUTING.md) for details on:
- Code of conduct and community guidelines
- Development workflow and branching strategy
- Testing requirements and quality gates
- Pull request process and review guidelines

## 📈 Roadmap

- [ ] **v1.1** - Advanced filtering and search capabilities
- [ ] **v1.2** - Enhanced caching and performance optimizations
- [ ] **v1.3** - Improved error handling and retry strategies
- [ ] **v2.0** - Modern .NET features and performance enhancements

## 🤝 Support & Community

- **📖 Documentation**: [https://yourorgg.github.io/FundraiseUpApi/](https://yourorgg.github.io/FundraiseUpApi/)
- **🐛 Bug Reports**: [GitHub Issues](https://github.com/YourOrg/FundraiseUpApi/issues)
- **💡 Feature Requests**: [GitHub Discussions](https://github.com/YourOrg/FundraiseUpApi/discussions)
- **❓ Questions**: [Stack Overflow](https://stackoverflow.com/questions/tagged/fundraiseup-dotnet) (tag: `fundraiseup-dotnet`)

## 📜 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🙏 Acknowledgments

- [FundraiseUp](https://fundraiseup.com/) for providing the API and documentation
- [Microsoft](https://microsoft.com/) for the .NET ecosystem and tooling
- All [contributors](https://github.com/YourOrg/FundraiseUpApi/contributors) who help make this library better

---

<div align="center">
  <sub>Built with ❤️ by the FundraiseUpApi team</sub>
</div>