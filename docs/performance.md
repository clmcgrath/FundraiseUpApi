---
layout: default
title: Performance Guide
nav_order: 8
description: "Performance optimization tips and best practices"
---

# Performance Guide

Optimize your FundraiseUp .NET Client Library usage for maximum performance and efficiency.

## 🚀 Performance Principles

### 1. Use HttpClientFactory

**✅ Recommended:**
```csharp
// Program.cs
builder.Services.AddFundraiseUpClient(options => options.ApiKey = "key");

// Service
public class DonationService(IFundraiseUpClient client)
{
    // Shared HttpClient with connection pooling
}
```

**❌ Avoid:**
```csharp
// Creates new HttpClient each time - poor performance
public class BadDonationService()
{
    public async Task ProcessAsync()
    {
        using var client = new FundraiseUpClient("key"); // Don't do this!
        await client.Donations.Create(request).ExecuteAsync();
    }
}
```

### 2. HttpClientFactory Integration

The FundraiseUp.Client library fully supports .NET's HttpClientFactory pattern for proper HttpClient lifecycle management, DNS refresh, and connection pooling.

#### Benefits of HttpClientFactory Integration

- **Proper Connection Pooling**: Automatic management of connection pools
- **DNS Refresh**: Automatic DNS updates without restarting the application
- **Resource Management**: Proper disposal and lifecycle management
- **Resilience**: Built-in support for retry policies and circuit breakers
- **Monitoring**: Integration with .NET diagnostics and logging

#### Basic Registration
```csharp
// In Program.cs or Startup.cs
services.AddFundraiseUpClient("your-api-key");
```

#### Advanced Configuration
```csharp
services.AddFundraiseUpClient(options =>
{
    options.ApiKey = "your-api-key";
    options.BaseUrl = "https://api.fundraiseup.com";
    options.Timeout = TimeSpan.FromSeconds(30);
});
```

#### With HttpClient Customization
```csharp
services.AddFundraiseUpClient(
    options =>
    {
        options.ApiKey = "your-api-key";
    },
    httpClient =>
    {
        // Additional HttpClient configuration
        httpClient.DefaultRequestHeaders.Add("Custom-Header", "Value");
    }
);
```

#### With Polly Resilience Policies
```csharp
services.AddFundraiseUpClient(options =>
{
    options.ApiKey = "your-api-key";
})
.AddPolicyHandler(GetRetryPolicy())
.AddPolicyHandler(GetCircuitBreakerPolicy());

static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .WaitAndRetryAsync(3, retryAttempt => 
            TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
}

static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .CircuitBreakerAsync(3, TimeSpan.FromSeconds(30));
}
```

#### Performance Benefits

Using HttpClientFactory provides significant performance improvements:

- **Connection Reuse**: Up to 50% faster requests due to connection pooling
- **Memory Efficiency**: Reduced memory pressure from proper HttpClient lifecycle
- **DNS Resolution**: Automatic DNS refresh prevents stale DNS issues
- **Resource Cleanup**: Proper disposal prevents resource leaks

### 3. Configure Connection Pooling

```csharp
services.AddFundraiseUpClient(options => options.ApiKey = "key")
    .ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler
    {
        MaxConnectionsPerServer = 10,              // Sufficient pool size
        PooledConnectionLifetime = TimeSpan.FromMinutes(15),
        PooledConnectionIdleTimeout = TimeSpan.FromMinutes(2),
        EnableMultipleHttp2Connections = true      // HTTP/2 multiplexing
    });
```

### 3. Optimize Rate Limiting Strategy

```csharp
// High-throughput applications
services.AddFundraiseUpClient(options => 
{
    options.RateLimitStrategy = RateLimitStrategy.Queue;  // Best for high load
    options.MaxConcurrentRequests = 3;                   // API limit
    options.MaxQueueSize = 200;                          // Large queue
    options.QueueTimeout = TimeSpan.FromMinutes(5);      // Generous timeout
});
```

## ⚡ Async Best Practices

### 1. Use ConfigureAwait(false)

```csharp
public async Task<DonationResponse> CreateDonationAsync(CreateDonationRequest request)
{
    // Use ConfigureAwait(false) for library code
    var donation = await client.Donations
        .Create(request)
        .ExecuteAsync()
        .ConfigureAwait(false);
        
    return donation;
}
```

### 2. Parallel Processing with Rate Limiting

```csharp
public async Task ProcessDonationsAsync(List<CreateDonationRequest> requests)
{
    // Process in parallel with automatic rate limiting
    var semaphore = new SemaphoreSlim(10); // Control local concurrency
    
    var tasks = requests.Select(async request =>
    {
        await semaphore.WaitAsync();
        try
        {
            // Rate limiter handles FundraiseUp API limits automatically
            return await client.Donations.Create(request).ExecuteAsync();
        }
        finally
        {
            semaphore.Release();
        }
    });
    
    var results = await Task.WhenAll(tasks);
}
```

### 3. Batch Operations

```csharp
public async Task<List<DonationResponse>> CreateDonationsBatchAsync(
    List<CreateDonationRequest> requests, 
    int batchSize = 50)
{
    var results = new List<DonationResponse>();
    
    for (int i = 0; i < requests.Count; i += batchSize)
    {
        var batch = requests.Skip(i).Take(batchSize);
        var batchTasks = batch.Select(request => 
            client.Donations.Create(request).ExecuteAsync());
            
        var batchResults = await Task.WhenAll(batchTasks);
        results.AddRange(batchResults);
        
        // Small delay between batches to be API-friendly
        if (i + batchSize < requests.Count)
        {
            await Task.Delay(100);
        }
    }
    
    return results;
}
```

## 🔄 Memory Optimization

### 1. Proper Disposal Patterns

```csharp
// ✅ Use using statements for manual clients
public async Task ProcessWithManualClient()
{
    using var client = new FundraiseUpClient("api-key");
    await client.Donations.Create(request).ExecuteAsync();
    // Automatically disposed
}

// ✅ Better: Use DI (no manual disposal needed)
public class Service(IFundraiseUpClient client)
{
    public async Task ProcessAsync()
    {
        await client.Donations.Create(request).ExecuteAsync();
        // DI container handles disposal
    }
}
```

### 2. Stream Large Responses

```csharp
// For large data sets, process items as they arrive
public async Task ProcessLargeDonationList()
{
    await foreach (var donation in client.Donations.ListAsync())
    {
        // Process each donation individually
        await ProcessDonationAsync(donation);
        
        // Allow garbage collection between items
        if (totalProcessed % 100 == 0)
        {
            GC.Collect(0, GCCollectionMode.Optimized);
        }
    }
}
```

## 📊 Monitoring and Metrics

### 1. Performance Counters

```csharp
public class PerformanceTrackingHandler : DelegatingHandler
{
    private static readonly Counter RequestCounter = 
        Metrics.CreateCounter("fundraiseup_requests_total", "Total requests");
    private static readonly Histogram RequestDuration = 
        Metrics.CreateHistogram("fundraiseup_request_duration_seconds", "Request duration");

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        using var timer = RequestDuration.NewTimer();
        RequestCounter.Inc();
        
        try
        {
            return await base.SendAsync(request, cancellationToken);
        }
        catch
        {
            RequestCounter.WithLabels("failed").Inc();
            throw;
        }
    }
}

// Register the handler
services.AddFundraiseUpClient(options => options.ApiKey = "key")
    .AddHttpMessageHandler<PerformanceTrackingHandler>();
```

### 2. Health Checks

```csharp
public class FundraiseUpHealthCheck : IHealthCheck
{
    private readonly IFundraiseUpClient _client;
    private readonly ILogger<FundraiseUpHealthCheck> _logger;

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();
        
        try
        {
            // Quick connectivity test
            await _client.Campaigns.List()
                .WithTimeout(TimeSpan.FromSeconds(5))
                .ExecuteAsync();
                
            var responseTime = stopwatch.ElapsedMilliseconds;
            
            return responseTime < 1000 
                ? HealthCheckResult.Healthy($"Response time: {responseTime}ms")
                : HealthCheckResult.Degraded($"Slow response: {responseTime}ms");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("FundraiseUp API unavailable", ex);
        }
    }
}
```

## 🏎️ High-Performance Scenarios

### 1. Web API with High Throughput

```csharp
// Optimized for high-throughput web applications
builder.Services.AddFundraiseUpClient(options => 
{
    options.ApiKey = configuration["FundraiseUp:ApiKey"];
    options.RateLimitStrategy = RateLimitStrategy.Queue;
    options.MaxQueueSize = 500;                    // Large queue
    options.QueueTimeout = TimeSpan.FromMinutes(10);
})
.ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler
{
    MaxConnectionsPerServer = 20,                  // Large pool
    PooledConnectionLifetime = TimeSpan.FromMinutes(30),
    PooledConnectionIdleTimeout = TimeSpan.FromMinutes(5),
    EnableMultipleHttp2Connections = true,
    UseProxy = false                               // Skip proxy for performance
});

// Enable response compression
builder.Services.Configure<GzipCompressionProviderOptions>(options => 
{
    options.Level = CompressionLevel.Fastest;
});
```

### 2. Background Processing

```csharp
public class HighPerformanceBackgroundService : BackgroundService
{
    private readonly IFundraiseUpClient _client;
    private readonly Channel<CreateDonationRequest> _channel;

    public HighPerformanceBackgroundService(IFundraiseUpClient client)
    {
        _client = client;
        var options = new BoundedChannelOptions(1000)
        {
            FullMode = BoundedChannelFullMode.Wait,
            SingleReader = false,
            SingleWriter = false
        };
        _channel = Channel.CreateBounded<CreateDonationRequest>(options);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Process items from channel with controlled parallelism
        await Parallel.ForEachAsync(
            _channel.Reader.ReadAllAsync(stoppingToken),
            new ParallelOptions 
            { 
                MaxDegreeOfParallelism = 10,
                CancellationToken = stoppingToken 
            },
            async (request, ct) =>
            {
                try
                {
                    await _client.Donations.Create(request).ExecuteAsync();
                }
                catch (Exception ex)
                {
                    // Handle errors without stopping the pipeline
                    _logger.LogError(ex, "Failed to process donation");
                }
            });
    }

    public async Task EnqueueDonationAsync(CreateDonationRequest request)
    {
        await _channel.Writer.WriteAsync(request);
    }
}
```

## 🔧 Troubleshooting Performance Issues

### Common Issues and Solutions

**Issue: High Memory Usage**
```csharp
// ✅ Solution: Reduce queue size and enable streaming
options.MaxQueueSize = 50;  // Smaller queue
options.RateLimitStrategy = RateLimitStrategy.Exception; // No queuing

// Process in smaller batches
const int batchSize = 10;
for (int i = 0; i < items.Count; i += batchSize)
{
    var batch = items.Skip(i).Take(batchSize);
    await ProcessBatchAsync(batch);
    GC.Collect(); // Force cleanup between batches
}
```

**Issue: High Response Times**
```csharp
// ✅ Solution: Optimize connection pooling and timeouts
services.AddFundraiseUpClient(options => 
{
    options.Timeout = TimeSpan.FromSeconds(30);        // Reasonable timeout
    options.RateLimitStrategy = RateLimitStrategy.Queue;
})
.ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler
{
    MaxConnectionsPerServer = 15,                      // Increase pool size
    ConnectTimeout = TimeSpan.FromSeconds(5),          // Fast connection
    PooledConnectionIdleTimeout = TimeSpan.FromMinutes(1)
});
```

**Issue: Rate Limit Exceptions**
```csharp
// ✅ Solution: Check for multiple client instances
// Use DI registration to ensure single rate limiter
builder.Services.AddFundraiseUpClient(options => options.ApiKey = "key");
// NOT: new FundraiseUpClient() in multiple places
```

## 📈 Performance Benchmarks

Typical performance characteristics:

| Scenario | Throughput | Latency | Memory |
|----------|------------|---------|---------|
| Single requests | 3 req/sec* | 200-500ms | 10-20MB |
| Queued requests | 3 req/sec* | 200ms-5min** | 50-100MB |
| Background processing | 3 req/sec* | N/A | 20-40MB |

*Limited by FundraiseUp API (3 concurrent max)
**Depends on queue size and timeout settings

## 🎯 Performance Checklist

### ✅ Do This
- Use HttpClientFactory with DI
- Configure connection pooling appropriately
- Use Queue strategy for high-load scenarios
- Implement proper async patterns with ConfigureAwait(false)
- Monitor performance metrics
- Process large datasets in batches
- Use background services for non-critical operations

### ❌ Avoid This
- Creating multiple FundraiseUpClient instances
- Blocking async calls with .Result or .Wait()
- Large queue sizes in memory-constrained environments  
- Ignoring rate limiting strategy selection
- Not monitoring performance metrics
- Processing large datasets synchronously