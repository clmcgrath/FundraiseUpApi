---
layout: default
title: Rate Limiting & Connection Pooling
nav_order: 4
description: "Advanced guide for rate limiting with connection pooling strategies"
---

# Rate Limiting with Connection Pooling - Advanced Guide

This document provides detailed information about how FundraiseUp .NET client's rate limiting works with various connection pooling strategies and scenarios.

## 🎯 Overview

The FundraiseUp API enforces a **3 concurrent request limit per account**. The .NET client implements intelligent rate limiting that works seamlessly with HttpClientFactory's connection pooling while maintaining thread safety across all scenarios.

## 🔄 Request Flow Architecture

```
┌─────────────┐    ┌──────────────────┐    ┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│   Thread    │───▶│  RateLimitHandler │───▶│ Connection Pool │───▶│  HTTP Request   │───▶│ FundraiseUp API │
│ (Any Thread)│    │ (3 max concurrent)│    │ (Reuse/Create)  │    │   (Network)     │    │ (3 max global)  │
└─────────────┘    └──────────────────┘    └─────────────────┘    └─────────────────┘    └─────────────────┘
```

**Key Points:**
- Rate limiting occurs **before** connection pooling
- All threads compete for the same 3 concurrent slots  
- Connection pooling happens within the rate-limited requests
- API enforcement is the final safety net

## 🧵 Thread Safety Implementation

### Semaphore-Based Concurrency Control

```csharp
public class RateLimitHandler : DelegatingHandler
{
    private readonly SemaphoreSlim _semaphore;         // Thread-safe concurrency control
    private readonly SemaphoreSlim _queueSemaphore;    // Thread-safe queue management
    private long _currentRequests;                     // Interlocked operations only

    public RateLimitHandler(FundraiseUpClientOptions options)
    {
        _semaphore = new SemaphoreSlim(
            options.MaxConcurrentRequests,     // Initial count: 3
            options.MaxConcurrentRequests      // Maximum count: 3
        );
        _queueSemaphore = new SemaphoreSlim(options.MaxQueueSize, options.MaxQueueSize);
    }
}
```

### Atomic Operations for Request Counting

```csharp
// All request counting uses lock-free atomic operations
Interlocked.Increment(ref _currentRequests);  // Thread-safe increment
Interlocked.Decrement(ref _currentRequests);  // Thread-safe decrement  
var current = Interlocked.Read(ref _currentRequests);  // Thread-safe read
```

## 🏊 Connection Pool Strategies

### 1. Default HttpClientHandler with Pooling

```csharp
services.AddFundraiseUpClient(options => options.ApiKey = "key")
    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler()); // Default pooling
```

**Behavior:**
- ✅ Rate limiting: Applied before connection selection
- ✅ Connection reuse: Efficient connection management
- ✅ Thread safety: All threads share same rate limiter
- 📊 Pool size: Managed automatically by HttpClientHandler

### 2. SocketsHttpHandler with Advanced Pooling

```csharp
services.AddFundraiseUpClient(options => options.ApiKey = "key")
    .ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler
    {
        MaxConnectionsPerServer = 10,              // Connection pool size
        PooledConnectionLifetime = TimeSpan.FromMinutes(15),
        PooledConnectionIdleTimeout = TimeSpan.FromMinutes(2),
        EnableMultipleHttp2Connections = true
    });
```

**Behavior:**
- ✅ Rate limiting: Still enforced before connection selection
- ✅ Advanced pooling: Better connection management and HTTP/2 support
- ✅ Performance: Optimal for high-throughput scenarios
- 🎯 Recommendation: Use when pool size > rate limit (10 connections > 3 concurrent)

### 3. Custom Handler Chains

```csharp
services.AddFundraiseUpClient(options => options.ApiKey = "key")
    .AddHttpMessageHandler<AuthenticationHandler>()      // Custom auth
    .AddHttpMessageHandler<RateLimitHandler>()           // Rate limiting (POSITION MATTERS!)
    .AddHttpMessageHandler<RetryPolicyHandler>()         // Additional retry logic
    .AddHttpMessageHandler<LoggingHandler>();            // Request/response logging
```

**Handler Order Considerations:**
- **Before RateLimitHandler**: Authentication, request preparation
- **After RateLimitHandler**: Retry policies, logging, connection-level handlers
- **Impact**: Handlers before rate limiting run for every attempt; handlers after only run for allowed requests

## ⚠️ Anti-Patterns and Solutions

### Problem 1: Multiple Client Instances

```csharp
// ❌ ANTI-PATTERN: Each service creates its own client
public class OrderService 
{
    private readonly IFundraiseUpClient _client = new FundraiseUpClient("api-key");
}

public class PaymentService 
{
    private readonly IFundraiseUpClient _client = new FundraiseUpClient("api-key");
}

public class ReportingService 
{
    private readonly IFundraiseUpClient _client = new FundraiseUpClient("api-key");
}

// RESULT: 3 separate rate limiters = up to 9 concurrent requests (violates API limit!)
```

**✅ Solution: Shared Rate Limiter via DI**

```csharp
// Program.cs
builder.Services.AddFundraiseUpClient(options => options.ApiKey = "api-key");

// Services use shared client
public class OrderService(IFundraiseUpClient client) 
{
    private readonly IFundraiseUpClient _client = client;  // Shared rate limiter
}

public class PaymentService(IFundraiseUpClient client) 
{
    private readonly IFundraiseUpClient _client = client;  // Shared rate limiter
}

public class ReportingService(IFundraiseUpClient client) 
{
    private readonly IFundraiseUpClient _client = client;  // Shared rate limiter
}

// RESULT: Single rate limiter = exactly 3 concurrent requests (respects API limit)
```

### Problem 2: Client Creation in Loops

```csharp
// ❌ ANTI-PATTERN: Creating clients in loops
public async Task ProcessDonations(List<DonationRequest> donations)
{
    foreach (var donation in donations)
    {
        var client = new FundraiseUpClient("api-key");    // New rate limiter each iteration!
        var result = await client.Donations.Create(donation).ExecuteAsync();
        client.Dispose();
    }
}
```

**✅ Solution: Single Client Instance**

```csharp
public class DonationProcessor(IFundraiseUpClient client)
{
    public async Task ProcessDonations(List<DonationRequest> donations)
    {
        // Process with different strategies based on volume
        
        if (donations.Count <= 10)
        {
            // Sequential processing for small batches
            foreach (var donation in donations)
            {
                await client.Donations.Create(donation).ExecuteAsync();
            }
        }
        else
        {
            // Parallel processing with rate limiting for large batches
            await Parallel.ForEachAsync(donations, 
                new ParallelOptions { MaxDegreeOfParallelism = 10 }, 
                async (donation, ct) =>
                {
                    await client.Donations.Create(donation).ExecuteAsync();
                    // Rate limiter automatically handles concurrency
                });
        }
    }
}
```

## 🏎️ High-Performance Scenarios

### Scenario 1: High-Throughput Web API

```csharp
// Configure for high-throughput scenarios
builder.Services.AddFundraiseUpClient(options => 
{
    options.ApiKey = configuration["FundraiseUp:ApiKey"];
    options.RateLimitStrategy = RateLimitStrategy.Queue;  // Queue excess requests
    options.MaxConcurrentRequests = 3;                   // API limit
    options.MaxQueueSize = 500;                          // Large queue for burst traffic
    options.QueueTimeout = TimeSpan.FromMinutes(10);     // Generous timeout
})
.ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler
{
    MaxConnectionsPerServer = 20,                        // Large connection pool
    PooledConnectionLifetime = TimeSpan.FromMinutes(30),
    PooledConnectionIdleTimeout = TimeSpan.FromMinutes(5),
    EnableMultipleHttp2Connections = true
});

// Controller handles burst traffic gracefully
[ApiController]
public class DonationController(IFundraiseUpClient client) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateDonation(CreateDonationRequest request)
    {
        try
        {
            var donation = await client.Donations.Create(request).ExecuteAsync();
            return Ok(donation);
        }
        catch (RateLimitExceededException ex)
        {
            // This should rarely happen with Queue strategy and large queue
            return StatusCode(429, "Rate limit exceeded. Please retry later.");
        }
    }
}
```

### Scenario 2: Background Processing with Retries

```csharp
// Configure for background processing with resilience
builder.Services.AddFundraiseUpClient(options => 
{
    options.ApiKey = configuration["FundraiseUp:ApiKey"];
    options.RateLimitStrategy = RateLimitStrategy.Retry;  // Retry on rate limit
    options.MaxRetryAttempts = 10;                       // Many retries for background tasks
    options.RetryDelay = TimeSpan.FromSeconds(2);        // Base delay for exponential backoff
})
.ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler
{
    MaxConnectionsPerServer = 5,                         // Moderate pool size
    PooledConnectionLifetime = TimeSpan.FromHours(1),   // Long-lived connections
});

// Background service processes donations with automatic retries
public class DonationBackgroundService(IFundraiseUpClient client, ILogger<DonationBackgroundService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var donation in GetPendingDonations(stoppingToken))
        {
            try
            {
                await client.Donations.Create(donation).ExecuteAsync();
                logger.LogInformation("Processed donation {DonationId}", donation.Id);
            }
            catch (RateLimitExceededException ex)
            {
                // With Retry strategy, this should be rare
                logger.LogWarning("Rate limit exceeded after retries: {Message}", ex.Message);
                // Could implement additional backoff or dead letter queue
            }
        }
    }
}
```

## 📊 Monitoring and Observability

### Custom Logging for Rate Limiting Events

```csharp
// Enable detailed rate limiting logs
builder.Services.AddFundraiseUpClient(options => 
{
    options.LogLevel = LogLevel.Debug;  // See all rate limiting events
});

// Custom logging handler to track rate limiting metrics
public class RateLimitingMetricsHandler : DelegatingHandler
{
    private readonly ILogger<RateLimitingMetricsHandler> _logger;
    private readonly IMetrics _metrics;

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();
        
        try
        {
            var response = await base.SendAsync(request, cancellationToken);
            
            _metrics.Counter("fundraiseup_requests_total")
                .WithTag("status", response.StatusCode.ToString())
                .Increment();
                
            _metrics.Histogram("fundraiseup_request_duration_ms")
                .Record(stopwatch.ElapsedMilliseconds);
                
            return response;
        }
        catch (RateLimitExceededException ex)
        {
            _metrics.Counter("fundraiseup_rate_limit_exceeded_total").Increment();
            _logger.LogWarning("Rate limit exceeded: {CurrentRequests}/{MaxRequests}", 
                ex.CurrentConcurrentRequests, ex.MaxConcurrentRequests);
            throw;
        }
    }
}

// Add to handler chain
services.AddFundraiseUpClient(options => options.ApiKey = "key")
    .AddHttpMessageHandler<RateLimitingMetricsHandler>();
```

### Health Checks with Rate Limiting Awareness

```csharp
public class FundraiseUpHealthCheck(IFundraiseUpClient client) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            // Use a simple operation to test connectivity and rate limiting
            await client.Campaigns.List().ExecuteAsync();
            return HealthCheckResult.Healthy("FundraiseUp API is accessible");
        }
        catch (RateLimitExceededException)
        {
            // Rate limiting working but at capacity
            return HealthCheckResult.Degraded("FundraiseUp API rate limit active");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("FundraiseUp API is not accessible", ex);
        }
    }
}
```

## 🎯 Best Practices Summary

### ✅ Do This

1. **Use HttpClientFactory with DI** - Single rate limiter shared across application
2. **Configure appropriate queue sizes** - Match your traffic patterns
3. **Monitor rate limiting events** - Track queue depths and timeouts
4. **Use Queue strategy for web apps** - Better user experience
5. **Use Retry strategy for background tasks** - Resilient processing
6. **Size connection pools appropriately** - Pool size ≥ rate limit

### ❌ Avoid This

1. **Multiple client instances** - Creates separate rate limiters
2. **Creating clients in loops** - Defeats rate limiting purpose
3. **Ignoring queue timeouts** - Can cause unexpected delays
4. **Exception strategy in high-load scenarios** - Poor user experience
5. **Undersized connection pools** - Can create artificial bottlenecks

### 🔧 Troubleshooting Checklist

**Problem: Getting rate limit exceptions with low traffic**
- ✅ Check for multiple client instances
- ✅ Verify using AddFundraiseUpClient() registration
- ✅ Ensure singleton client pattern if not using DI

**Problem: Requests taking too long**
- ✅ Check QueueTimeout setting
- ✅ Consider switching to Retry strategy
- ✅ Monitor queue depth metrics

**Problem: High memory usage**
- ✅ Reduce MaxQueueSize
- ✅ Consider Exception strategy for memory-constrained environments
- ✅ Monitor queue utilization

**Problem: Poor performance with connection pooling**
- ✅ Ensure connection pool size > rate limit
- ✅ Check PooledConnectionLifetime settings
- ✅ Consider using SocketsHttpHandler for better performance