---
layout: default
title: FAQ
nav_order: 11
description: "Frequently asked questions and answers"
---

# Frequently Asked Questions
{: .no_toc }

Common questions and answers about the FundraiseUp .NET Client Library.
{: .fs-6 .fw-300 }

## Table of contents
{: .no_toc .text-delta }

1. TOC
{:toc}

---

## General Questions

### What is the FundraiseUp .NET Client Library?

The FundraiseUp .NET Client Library is a modern, fluent .NET client for the FundraiseUp API. It provides comprehensive support for donations, supporters, fundraisers, recurring plans, events, and donor portal access with enterprise-grade reliability.

### Which .NET versions are supported?

The library targets:
- **.NET Standard 2.0** (compatible with .NET Framework 4.6.1+, .NET Core 2.0+)  
- **.NET 6.0+** (for modern .NET applications)

This ensures maximum compatibility across different .NET implementations.

### Is this library officially supported by FundraiseUp?

This is a community-maintained library that implements the official FundraiseUp API specification. While not officially maintained by FundraiseUp, it follows their API documentation precisely.

---

## Setup and Configuration

### How do I get a FundraiseUp API key?

1. Log into your FundraiseUp dashboard
2. Navigate to Settings → API Keys
3. Generate a new API key with the required permissions
4. Use `test_` prefixed keys for development and `live_` prefixed keys for production

### What permissions does my API key need?

Your API key needs these permissions:
- **Retrieve donation data** - To access donation information
- **Create new donations** - To process donations
- **Retrieve fundraiser data** - To access fundraiser information
- **Create new fundraisers** - To create fundraiser records
- **Generate Donor Portal access links** - To create supporter login links

### Can I use this library in both test and production environments?

Yes! The library automatically detects test vs. production API keys:
- Test keys start with `test_` 
- Production keys start with `live_`

```csharp
// Automatically uses appropriate environment
services.AddFundraiseUpClient(options =>
{
    options.ApiKey = configuration["FundraiseUp:ApiKey"]; // test_ or live_
});
```

---

## Rate Limiting

### What are FundraiseUp's rate limits?

FundraiseUp enforces a maximum of **3 concurrent requests** per account. The library automatically handles this with configurable strategies.

### Which rate limiting strategy should I use?

**For most applications**: Use `RateLimitStrategy.Queue`
```csharp
options.RateLimitStrategy = RateLimitStrategy.Queue; // Automatically queues excess requests
```

**For high-performance applications**: Use `RateLimitStrategy.Exception` with manual handling
```csharp
options.RateLimitStrategy = RateLimitStrategy.Exception; // Fastest, requires error handling
```

**For unreliable networks**: Use `RateLimitStrategy.Retry`
```csharp
options.RateLimitStrategy = RateLimitStrategy.Retry; // Automatic retry with backoff
```

### Does rate limiting work across multiple threads?

Yes! The rate limiting is thread-safe and works across:
- Multiple threads
- HttpClientFactory instances
- Connection pooling strategies
- Dependency injection scopes

---

## API Operations

### Can I create campaigns through the API?

No, campaigns are read-only through the API. They must be created and managed through the FundraiseUp dashboard. Campaign data is available embedded in donation and fundraiser responses.

### Can I create supporters/donors directly?

No, supporters are created automatically when donations are made. The API provides read-only access to supporter information.

### What donation updates are allowed?

Donation updates are only available for:
- API-created donations (not dashboard-created)
- Within 24 hours of creation
- Limited fields: questions, supporters, campaign, comment, custom fields, designation

### How do I handle payment methods?

The library supports Stripe payment methods. Use test payment methods for development:

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

## Performance

### Should I create a new client for each request?

**No!** Use dependency injection with HttpClientFactory:

```csharp
// ✅ Correct - Single client instance with connection pooling
services.AddFundraiseUpClient(options => options.ApiKey = "key");

// ❌ Wrong - Creates new client each time
public async Task ProcessAsync()
{
    using var client = new FundraiseUpClient("key"); // Don't do this!
}
```

### How does connection pooling work?

The library uses HttpClientFactory for automatic connection pooling:
- Connections are reused across requests
- DNS is automatically refreshed
- Resource cleanup is handled automatically
- Rate limiting works across all pooled connections

### What's the recommended timeout setting?

Default timeout is 30 seconds, which works for most scenarios:

```csharp
// For standard operations
options.Timeout = TimeSpan.FromSeconds(30); // Default

// For batch operations or slow networks
options.Timeout = TimeSpan.FromMinutes(2);
```

---

## Error Handling

### What exceptions should I expect?

The library provides specific exception types:

- **`FundraiseUpApiException`** - General API errors (4xx, 5xx responses)
- **`FundraiseUpValidationException`** - Validation errors (422 responses)
- **`FundraiseUpNotFoundException`** - Resource not found (404 responses)
- **`FundraiseUpAuthenticationException`** - Authentication failures (401, 403)
- **`FundraiseUpRateLimitException`** - Rate limiting (429 responses)
- **`FundraiseUpConfigurationException`** - Client configuration errors

### How should I handle rate limit exceptions?

Depends on your strategy:

```csharp
// Queue strategy - no manual handling needed
options.RateLimitStrategy = RateLimitStrategy.Queue;

// Exception strategy - handle manually
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

---

## Testing

### How do I test code that uses this library?

The library provides comprehensive testing support:

1. **Use built-in mock helpers**:
```csharp
var mockSetup = new HttpClientMockSetup();
mockSetup.SetupSuccessResponse("/v1/donations", mockDonation);
```

2. **Inject IFundraiseUpClient for easy mocking**:
```csharp
public class MyService(IFundraiseUpClient client) // Easy to mock
```

3. **Use test API keys for integration tests**:
```csharp
var client = new FundraiseUpClient("test_your-test-key");
```

### Should I test against the real API?

- **Unit tests**: Use mocks (no real API calls)
- **Integration tests**: Use test API keys with real API calls
- **Production deployment**: Use live API keys

---

## Troubleshooting

### Why am I getting authentication errors?

1. Verify your API key is correct and active
2. Check if you're using the right environment (test vs. live)
3. Ensure your API key has the required permissions
4. Verify the API key format (`test_` or `live_` prefix)

### Why are my requests slow?

1. **Check your rate limiting strategy** - Queue strategy adds some latency
2. **Enable connection pooling** - Use HttpClientFactory (automatic)
3. **Check your timeout settings** - May be too conservative
4. **Review your error handling** - Retries can add latency

### How do I debug API issues?

Enable detailed logging:

```csharp
services.AddFundraiseUpClient(options =>
{
    options.LogLevel = LogLevel.Debug; // Shows HTTP requests/responses
});
```

This will log:
- HTTP request/response details
- Rate limiting events
- Error details with API responses

---

## Integration

### Can I use this with ASP.NET Core?

Yes! The library is designed for ASP.NET Core with full dependency injection support:

```csharp
// Program.cs
builder.Services.AddFundraiseUpClient(options =>
{
    options.ApiKey = builder.Configuration["FundraiseUp:ApiKey"];
});

// Controller
public class DonationController(IFundraiseUpClient client) : ControllerBase
{
    // Use client here
}
```

### Does it work with Azure Functions?

Yes, use the dependency injection container:

```csharp
// Startup.cs or Program.cs
builder.Services.AddFundraiseUpClient(options =>
{
    options.ApiKey = Environment.GetEnvironmentVariable("FUNDRAISEUP_API_KEY");
});

// Function
public class DonationFunction(IFundraiseUpClient client)
{
    [FunctionName("ProcessDonation")]
    public async Task<IActionResult> Run([HttpTrigger] HttpRequest req)
    {
        // Use client here
    }
}
```

### Can I use it in a console application?

Absolutely! Set up a basic service container:

```csharp
var services = new ServiceCollection();
services.AddFundraiseUpClient(options => options.ApiKey = "your-key");
services.AddLogging(builder => builder.AddConsole());

var serviceProvider = services.BuildServiceProvider();
var client = serviceProvider.GetRequiredService<IFundraiseUpClient>();
```

---

## Advanced Scenarios

### How do I implement custom retry logic?

You can implement custom retry policies with Polly:

```csharp
services.AddFundraiseUpClient(options => options.ApiKey = "key")
    .AddPolicyHandler(Policy
        .Handle<HttpRequestException>()
        .WaitAndRetryAsync(3, attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt))));
```

### Can I customize the HTTP client?

Yes, through HttpClientFactory:

```csharp
services.AddFundraiseUpClient(options => options.ApiKey = "key")
    .ConfigureHttpClient(client =>
    {
        client.DefaultRequestHeaders.Add("User-Agent", "MyApp/1.0");
        client.Timeout = TimeSpan.FromMinutes(5);
    })
    .ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler
    {
        MaxConnectionsPerServer = 20
    });
```

### How do I handle webhooks from FundraiseUp?

Webhook handling is not currently included in this library. You'll need to implement webhook endpoints separately in your application to receive FundraiseUp webhook events.

---

## Support

### Where can I get help?

1. **Documentation**: Check our comprehensive guides
2. **GitHub Issues**: Search existing issues or create a new one
3. **Community**: GitHub Discussions for questions and community support

### How do I report bugs?

Create a GitHub issue with:
- Your .NET version
- Library version  
- Complete error message
- Minimal code example
- Steps to reproduce

### How do I request features?

Create a GitHub issue with the "enhancement" label and describe:
- The use case
- Expected behavior
- Why it would be valuable
- Any alternative approaches you've considered