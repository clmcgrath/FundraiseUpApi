---
layout: default
title: Error Handling
nav_order: 7
description: "Exception types and error handling strategies"
---

# Error Handling

The FundraiseUp .NET Client Library provides comprehensive error handling with specific exception types for different scenarios.

## Exception Hierarchy

```
FundraiseUpException (Base)
├── FundraiseUpConfigurationException
├── FundraiseUpApiException
│   ├── FundraiseUpValidationException
│   └── RateLimitExceededException
└── FundraiseUpNetworkException
```

## Exception Types

### FundraiseUpException

Base exception for all FundraiseUp client exceptions.

```csharp
try
{
    var donation = await client.Donations.Create(request).ExecuteAsync();
}
catch (FundraiseUpException ex)
{
    // Handle any FundraiseUp-related exception
    logger.LogError(ex, "FundraiseUp operation failed: {Message}", ex.Message);
}
```

### FundraiseUpConfigurationException

Thrown when client configuration is invalid.

```csharp
try
{
    var client = new FundraiseUpClient(new FundraiseUpClientOptions
    {
        ApiKey = "", // Invalid - empty API key
        BaseUrl = "invalid-url" // Invalid URL format
    });
}
catch (FundraiseUpConfigurationException ex)
{
    logger.LogError("Configuration error: {Message}", ex.Message);
    // Handle configuration issues
}
```

### FundraiseUpApiException

Thrown for API-level errors (HTTP 4xx, 5xx responses).

```csharp
try
{
    var donation = await client.Donations.Get("invalid-id").ExecuteAsync();
}
catch (FundraiseUpApiException ex)
{
    logger.LogError("API error: {StatusCode} - {Message}", ex.StatusCode, ex.Message);
    
    switch (ex.StatusCode)
    {
        case 401:
            // Handle authentication error
            break;
        case 404:
            // Handle not found
            break;
        case 500:
            // Handle server error
            break;
    }
}
```

### FundraiseUpValidationException

Thrown for validation errors (HTTP 422).

```csharp
try
{
    var donation = await client.Donations.Create(invalidRequest).ExecuteAsync();
}
catch (FundraiseUpValidationException ex)
{
    logger.LogWarning("Validation failed: {Message}", ex.Message);
    
    foreach (var error in ex.ValidationErrors)
    {
        logger.LogWarning("Field {Field}: {Error}", error.Key, error.Value);
    }
}
```

### RateLimitExceededException

Thrown when rate limits are exceeded (with Exception strategy).

```csharp
try
{
    var donation = await client.Donations.Create(request).ExecuteAsync();
}
catch (RateLimitExceededException ex)
{
    logger.LogWarning("Rate limit exceeded: {Current}/{Max} requests. Retry after {RetryAfter}s", 
        ex.CurrentConcurrentRequests, ex.MaxConcurrentRequests, ex.RetryAfterSeconds);
    
    // Implement backoff strategy
    if (ex.RetryAfterSeconds.HasValue)
    {
        await Task.Delay(TimeSpan.FromSeconds(ex.RetryAfterSeconds.Value));
        // Retry the operation
    }
}
```

## Error Handling Strategies

### 1. Specific Exception Handling

```csharp
public async Task<DonationResponse?> CreateDonationSafely(CreateDonationRequest request)
{
    try
    {
        return await client.Donations.Create(request).ExecuteAsync();
    }
    catch (FundraiseUpValidationException ex)
    {
        // Log validation errors but don't crash
        logger.LogWarning("Donation validation failed: {Errors}", 
            string.Join(", ", ex.ValidationErrors.Select(e => $"{e.Key}: {e.Value}")));
        return null;
    }
    catch (RateLimitExceededException ex)
    {
        // Implement exponential backoff
        var delay = ex.RetryAfterSeconds?.Seconds ?? TimeSpan.FromSeconds(Math.Pow(2, retryCount));
        await Task.Delay(delay);
        
        if (retryCount < maxRetries)
        {
            return await CreateDonationSafely(request); // Recursive retry
        }
        
        throw; // Re-throw after max retries
    }
    catch (FundraiseUpApiException ex) when (ex.StatusCode >= 500)
    {
        // Server errors - retry with backoff
        logger.LogError("Server error {StatusCode}: {Message}", ex.StatusCode, ex.Message);
        throw; // Let higher-level retry policy handle
    }
    catch (FundraiseUpApiException ex)
    {
        // Client errors - don't retry
        logger.LogError("Client error {StatusCode}: {Message}", ex.StatusCode, ex.Message);
        return null;
    }
}
```

### 2. Global Exception Handling

```csharp
// ASP.NET Core middleware
public class FundraiseUpExceptionMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (FundraiseUpValidationException ex)
        {
            context.Response.StatusCode = 400;
            await context.Response.WriteAsync(JsonSerializer.Serialize(new
            {
                error = "Validation failed",
                details = ex.ValidationErrors
            }));
        }
        catch (RateLimitExceededException ex)
        {
            context.Response.StatusCode = 429;
            if (ex.RetryAfterSeconds.HasValue)
            {
                context.Response.Headers.Add("Retry-After", ex.RetryAfterSeconds.ToString());
            }
            await context.Response.WriteAsync("Rate limit exceeded");
        }
        catch (FundraiseUpApiException ex)
        {
            context.Response.StatusCode = ex.StatusCode >= 500 ? 502 : 400;
            await context.Response.WriteAsync($"API error: {ex.Message}");
        }
    }
}
```

### 3. Polly Integration for Resilience

```csharp
// Configure retry policies with Polly
services.AddFundraiseUpClient(options => options.ApiKey = "key")
    .AddPolicyHandler(Policy
        .Handle<FundraiseUpApiException>(ex => ex.StatusCode >= 500) // Retry server errors
        .Or<HttpRequestException>()
        .WaitAndRetryAsync(
            retryCount: 3,
            sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
            onRetry: (outcome, timespan, retryCount, context) =>
            {
                logger.LogWarning("Retrying in {Delay}s (attempt {RetryCount})", 
                    timespan.TotalSeconds, retryCount);
            }));
```

## Best Practices

### ✅ Do This

1. **Handle specific exceptions** - Catch specific exception types for targeted handling
2. **Log with context** - Include relevant details in log messages
3. **Implement retries carefully** - Only retry appropriate errors (server errors, timeouts)
4. **Use structured logging** - Include exception properties in logs
5. **Validate early** - Use request validation before API calls

### ❌ Avoid This

1. **Catching all exceptions** - Don't use `catch (Exception)` for FundraiseUp operations
2. **Retrying client errors** - Don't retry 400-level errors
3. **Ignoring validation errors** - Always handle validation exceptions appropriately
4. **Swallowing exceptions** - Log and handle, don't just ignore
5. **Infinite retries** - Always limit retry attempts

## Logging Integration

```csharp
// Configure structured logging
builder.Services.AddFundraiseUpClient(options => 
{
    options.ApiKey = configuration["FundraiseUp:ApiKey"];
    options.EnableLogging = true;
    options.LogLevel = LogLevel.Information;
});

// Custom logging
public class DonationService(IFundraiseUpClient client, ILogger<DonationService> logger)
{
    public async Task<DonationResponse> CreateDonationAsync(CreateDonationRequest request)
    {
        using var scope = logger.BeginScope("Creating donation for {Email}", request.Supporter.Email);
        
        try
        {
            var donation = await client.Donations.Create(request).ExecuteAsync();
            logger.LogInformation("Successfully created donation {DonationId}", donation.Id);
            return donation;
        }
        catch (FundraiseUpException ex)
        {
            logger.LogError(ex, "Failed to create donation for {Email}: {Message}", 
                request.Supporter.Email, ex.Message);
            throw;
        }
    }
}
```