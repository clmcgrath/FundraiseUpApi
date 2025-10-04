---
layout: default
title: Best Practices
nav_order: 12
description: "Best practices and recommendations for production use"
---

# Best Practices Guide
{: .no_toc }

Best practices and recommendations for using the FundraiseUp .NET Client Library in production applications.
{: .fs-6 .fw-300 }

## Table of contents
{: .no_toc .text-delta }

1. TOC
{:toc}

---

## Architecture Patterns

### Dependency Injection (Recommended)

**✅ Best Practice**: Use dependency injection with HttpClientFactory

```csharp
// Program.cs - Configure once
builder.Services.AddFundraiseUpClient(options =>
{
    options.ApiKey = builder.Configuration["FundraiseUp:ApiKey"];
    options.RateLimitStrategy = RateLimitStrategy.Queue;
    options.LogLevel = LogLevel.Information;
});

// Service - Inject and use
public class DonationService(IFundraiseUpClient client, ILogger<DonationService> logger)
{
    public async Task<DonationResponse> ProcessDonationAsync(CreateDonationRequest request)
    {
        try
        {
            return await client.Donations.Create(request).ExecuteAsync();
        }
        catch (FundraiseUpException ex)
        {
            logger.LogError(ex, "Failed to process donation");
            throw;
        }
    }
}
```

**❌ Anti-Pattern**: Creating new clients repeatedly

```csharp
// Don't do this - inefficient and loses connection pooling benefits
public class BadDonationService
{
    public async Task ProcessAsync()
    {
        using var client = new FundraiseUpClient("key"); // Creates new HttpClient each time
        await client.Donations.Create(request).ExecuteAsync();
    }
}
```

### Service Layer Pattern

**✅ Best Practice**: Create service layers that encapsulate business logic

```csharp
public interface IDonationService
{
    Task<DonationResult> ProcessDonationAsync(DonationModel model);
    Task<IEnumerable<DonationSummary>> GetRecentDonationsAsync(int count = 10);
}

public class DonationService : IDonationService
{
    private readonly IFundraiseUpClient _client;
    private readonly IMapper _mapper;
    private readonly ILogger<DonationService> _logger;

    public DonationService(IFundraiseUpClient client, IMapper mapper, ILogger<DonationService> logger)
    {
        _client = client;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<DonationResult> ProcessDonationAsync(DonationModel model)
    {
        // Validate business rules
        if (model.Amount <= 0)
            throw new BusinessException("Donation amount must be positive");

        // Map to API request
        var request = _mapper.Map<CreateDonationRequest>(model);
        
        try
        {
            // Call API
            var response = await _client.Donations.Create(request).ExecuteAsync();
            
            // Map response and return business model
            return _mapper.Map<DonationResult>(response);
        }
        catch (FundraiseUpValidationException ex)
        {
            _logger.LogWarning("Validation failed for donation: {Errors}", ex.ValidationErrors);
            throw new BusinessValidationException("Invalid donation data", ex);
        }
    }
}
```

---

## Configuration Management

### Environment-Specific Configuration

**✅ Best Practice**: Use configuration providers for different environments

```csharp
// appsettings.json
{
  "FundraiseUp": {
    "ApiKey": "", // Empty in source control
    "Environment": "Production",
    "RateLimitStrategy": "Queue",
    "TimeoutSeconds": 30
  }
}

// appsettings.Development.json
{
  "FundraiseUp": {
    "ApiKey": "test_your-development-key",
    "Environment": "Development", 
    "LogLevel": "Debug"
  }
}

// Program.cs
builder.Services.AddFundraiseUpClient(options =>
{
    var config = builder.Configuration.GetSection("FundraiseUp");
    options.ApiKey = config["ApiKey"] ?? throw new InvalidOperationException("FundraiseUp API key not configured");
    options.RateLimitStrategy = Enum.Parse<RateLimitStrategy>(config["RateLimitStrategy"] ?? "Queue");
    options.Timeout = TimeSpan.FromSeconds(config.GetValue<int>("TimeoutSeconds", 30));
    
    if (builder.Environment.IsDevelopment())
    {
        options.LogLevel = LogLevel.Debug;
    }
});
```

### Secure Secret Management

**✅ Best Practice**: Use secure secret storage for API keys

```csharp
// Azure Key Vault
builder.Configuration.AddAzureKeyVault(keyVaultEndpoint, credential);

// AWS Secrets Manager
builder.Configuration.AddSystemsManager("/myapp/fundraiseup");

// Environment Variables (containers/deployment)
services.AddFundraiseUpClient(options =>
{
    options.ApiKey = Environment.GetEnvironmentVariable("FUNDRAISEUP_API_KEY") 
        ?? throw new InvalidOperationException("FundraiseUp API key not found");
});

// User Secrets (development only)
// dotnet user-secrets set "FundraiseUp:ApiKey" "test_your-dev-key"
```

**❌ Anti-Pattern**: Hardcoding API keys

```csharp
// Never do this - security risk
services.AddFundraiseUpClient(options =>
{
    options.ApiKey = "live_actual-api-key-in-source-code"; // Security vulnerability!
});
```

---

## Error Handling Strategies

### Comprehensive Error Handling

**✅ Best Practice**: Handle different exception types appropriately

```csharp
public async Task<DonationResult> ProcessDonationAsync(CreateDonationRequest request)
{
    try
    {
        var response = await _client.Donations.Create(request).ExecuteAsync();
        return new DonationResult { Success = true, Donation = response };
    }
    catch (FundraiseUpValidationException ex)
    {
        // Validation errors - return user-friendly messages
        _logger.LogWarning("Donation validation failed: {Errors}", ex.ValidationErrors);
        return new DonationResult 
        { 
            Success = false, 
            ErrorMessage = "Please check your donation details",
            ValidationErrors = ex.ValidationErrors
        };
    }
    catch (FundraiseUpAuthenticationException ex)
    {
        // Authentication issues - log and alert administrators
        _logger.LogError(ex, "FundraiseUp authentication failed");
        await _alertService.NotifyAdministratorsAsync("FundraiseUp API authentication issue");
        return new DonationResult { Success = false, ErrorMessage = "Service temporarily unavailable" };
    }
    catch (FundraiseUpRateLimitException ex)
    {
        // Rate limiting - should be rare with Queue strategy
        _logger.LogWarning("Rate limit hit despite queue strategy");
        return new DonationResult { Success = false, ErrorMessage = "Please try again in a moment" };
    }
    catch (FundraiseUpApiException ex) when (ex.StatusCode >= 500)
    {
        // Server errors - retry may help
        _logger.LogError(ex, "FundraiseUp server error: {StatusCode}", ex.StatusCode);
        return new DonationResult { Success = false, ErrorMessage = "Service temporarily unavailable" };
    }
    catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
    {
        // Timeout - may indicate network issues
        _logger.LogWarning(ex, "FundraiseUp request timed out");
        return new DonationResult { Success = false, ErrorMessage = "Request timed out, please try again" };
    }
    catch (Exception ex)
    {
        // Unexpected errors
        _logger.LogError(ex, "Unexpected error processing donation");
        return new DonationResult { Success = false, ErrorMessage = "An unexpected error occurred" };
    }
}
```

### Circuit Breaker Pattern

**✅ Best Practice**: Use circuit breaker for external service resilience

```csharp
services.AddFundraiseUpClient(options => options.ApiKey = "key")
    .AddPolicyHandler(Policy
        .Handle<FundraiseUpApiException>(ex => ex.StatusCode >= 500)
        .Or<TaskCanceledException>()
        .CircuitBreakerAsync(
            handledEventsAllowedBeforeBreaking: 3,
            durationOfBreak: TimeSpan.FromSeconds(30),
            onBreak: (exception, duration) =>
            {
                // Log circuit breaker opening
            },
            onReset: () =>
            {
                // Log circuit breaker closing
            }));
```

---

## Performance Optimization

### Connection Pool Configuration

**✅ Best Practice**: Configure connection pooling for your load

```csharp
services.AddFundraiseUpClient(options => options.ApiKey = "key")
    .ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler
    {
        MaxConnectionsPerServer = 10,              // Adjust based on concurrent load
        PooledConnectionLifetime = TimeSpan.FromMinutes(15), // DNS refresh interval
        PooledConnectionIdleTimeout = TimeSpan.FromMinutes(2), // Close idle connections
        EnableMultipleHttp2Connections = true      // HTTP/2 multiplexing
    });
```

### Async Best Practices

**✅ Best Practice**: Use proper async patterns

```csharp
// ✅ Correct async usage
public async Task<IEnumerable<DonationResponse>> GetDonationsAsync(string campaignId)
{
    var donations = await _client.Donations
        .List()
        .ByCampaign(campaignId)
        .Take(50)
        .ExecuteAsync();
    
    return donations.Items;
}

// ✅ Correct parallel processing
public async Task<IEnumerable<DonationResponse>> GetMultipleCampaignDonationsAsync(string[] campaignIds)
{
    var tasks = campaignIds.Select(async campaignId =>
        await _client.Donations
            .List()
            .ByCampaign(campaignId)
            .Take(10)
            .ExecuteAsync());
    
    var results = await Task.WhenAll(tasks);
    return results.SelectMany(r => r.Items);
}
```

**❌ Anti-Pattern**: Blocking async calls

```csharp
// Don't do this - can cause deadlocks
public DonationResponse GetDonation(string id)
{
    return _client.Donations.GetById(id).ExecuteAsync().Result; // Blocking!
}

// Don't do this - not truly parallel
public async Task<IEnumerable<DonationResponse>> GetDonationsSequentially(string[] ids)
{
    var results = new List<DonationResponse>();
    foreach (var id in ids)
    {
        var donation = await _client.Donations.GetById(id).ExecuteAsync(); // Sequential!
        results.Add(donation);
    }
    return results;
}
```

### Caching Strategies

**✅ Best Practice**: Cache appropriate data with proper invalidation

```csharp
public class CachedFundraiseUpService
{
    private readonly IFundraiseUpClient _client;
    private readonly IMemoryCache _cache;
    private readonly ILogger<CachedFundraiseUpService> _logger;

    public async Task<SupporterResponse> GetSupporterAsync(string supporterId)
    {
        var cacheKey = $"supporter:{supporterId}";
        
        if (_cache.TryGetValue(cacheKey, out SupporterResponse cachedSupporter))
        {
            return cachedSupporter;
        }

        var supporter = await _client.Supporters.GetById(supporterId).ExecuteAsync();
        
        // Cache for 5 minutes - supporters don't change frequently
        _cache.Set(cacheKey, supporter, TimeSpan.FromMinutes(5));
        
        return supporter;
    }

    public async Task<DonationResponse> CreateDonationAsync(CreateDonationRequest request)
    {
        var donation = await _client.Donations.Create(request).ExecuteAsync();
        
        // Invalidate related caches
        if (!string.IsNullOrEmpty(request.Supporter?.Email))
        {
            var supporterCacheKey = $"supporter-by-email:{request.Supporter.Email}";
            _cache.Remove(supporterCacheKey);
        }
        
        return donation;
    }
}
```

---

## Monitoring and Observability

### Structured Logging

**✅ Best Practice**: Use structured logging with correlation IDs

```csharp
public class DonationService
{
    private readonly IFundraiseUpClient _client;
    private readonly ILogger<DonationService> _logger;

    public async Task<DonationResponse> ProcessDonationAsync(CreateDonationRequest request, string correlationId)
    {
        using var scope = _logger.BeginScope(new Dictionary<string, object>
        {
            ["CorrelationId"] = correlationId,
            ["Operation"] = "ProcessDonation",
            ["Amount"] = request.Amount,
            ["Currency"] = request.Currency
        });

        _logger.LogInformation("Starting donation processing");

        try
        {
            var response = await _client.Donations.Create(request).ExecuteAsync();
            
            _logger.LogInformation("Donation processed successfully: {DonationId}", response.Id);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process donation");
            throw;
        }
    }
}
```

### Health Checks

**✅ Best Practice**: Implement health checks for FundraiseUp connectivity

```csharp
public class FundraiseUpHealthCheck : IHealthCheck
{
    private readonly IFundraiseUpClient _client;
    private readonly ILogger<FundraiseUpHealthCheck> _logger;

    public FundraiseUpHealthCheck(IFundraiseUpClient client, ILogger<FundraiseUpHealthCheck> logger)
    {
        _client = client;
        _logger = logger;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            // Simple connectivity test - list first donation
            var donations = await _client.Donations.List().Take(1).ExecuteAsync();
            
            return HealthCheckResult.Healthy("FundraiseUp API is accessible");
        }
        catch (FundraiseUpAuthenticationException ex)
        {
            _logger.LogError(ex, "FundraiseUp authentication failed");
            return HealthCheckResult.Unhealthy("FundraiseUp authentication failed", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "FundraiseUp health check failed");
            return HealthCheckResult.Degraded("FundraiseUp API issues detected", ex);
        }
    }
}

// Registration
services.AddHealthChecks()
    .AddCheck<FundraiseUpHealthCheck>("fundraiseup");
```

### Metrics Collection

**✅ Best Practice**: Collect custom metrics for monitoring

```csharp
public class InstrumentedDonationService
{
    private readonly IFundraiseUpClient _client;
    private readonly Counter<int> _donationCounter;
    private readonly Histogram<double> _donationAmountHistogram;
    private readonly Counter<int> _errorCounter;

    public InstrumentedDonationService(IFundraiseUpClient client, IMeterFactory meterFactory)
    {
        _client = client;
        
        var meter = meterFactory.Create("FundraiseUp.Client");
        _donationCounter = meter.CreateCounter<int>("donations_created_total", "count", "Number of donations created");
        _donationAmountHistogram = meter.CreateHistogram<double>("donation_amount", "dollars", "Donation amounts");
        _errorCounter = meter.CreateCounter<int>("donation_errors_total", "count", "Number of donation errors");
    }

    public async Task<DonationResponse> ProcessDonationAsync(CreateDonationRequest request)
    {
        try
        {
            var response = await _client.Donations.Create(request).ExecuteAsync();
            
            // Record successful metrics
            _donationCounter.Add(1, new KeyValuePair<string, object?>("status", "success"));
            
            if (decimal.TryParse(request.Amount, out var amount))
            {
                _donationAmountHistogram.Record((double)amount, 
                    new KeyValuePair<string, object?>("currency", request.Currency));
            }
            
            return response;
        }
        catch (Exception ex)
        {
            // Record error metrics
            _errorCounter.Add(1, 
                new KeyValuePair<string, object?>("error_type", ex.GetType().Name),
                new KeyValuePair<string, object?>("operation", "create_donation"));
            
            throw;
        }
    }
}
```

---

## Testing Best Practices

### Unit Testing

**✅ Best Practice**: Use dependency injection for easy testing

```csharp
[Test]
public async Task ProcessDonation_WithValidRequest_ShouldReturnSuccess()
{
    // Arrange
    var mockClient = new Mock<IFundraiseUpClient>();
    var mockDonations = new Mock<IDonationOperations>();
    var mockBuilder = new Mock<IDonationOperationBuilder<DonationResponse>>();
    
    var expectedResponse = new DonationResponse
    {
        Id = "DTEST123",
        Amount = "100.00",
        Status = "succeeded"
    };

    mockClient.Setup(c => c.Donations).Returns(mockDonations.Object);
    mockDonations.Setup(d => d.Create(It.IsAny<CreateDonationRequest>())).Returns(mockBuilder.Object);
    mockBuilder.Setup(b => b.ExecuteAsync()).ReturnsAsync(expectedResponse);

    var service = new DonationService(mockClient.Object, Mock.Of<ILogger<DonationService>>());

    // Act
    var result = await service.ProcessDonationAsync(new CreateDonationRequest
    {
        Amount = "100.00",
        Currency = "usd"
    });

    // Assert
    Assert.That(result.Success, Is.True);
    Assert.That(result.Donation.Id, Is.EqualTo("DTEST123"));
}
```

### Integration Testing

**✅ Best Practice**: Use test API keys for integration tests

```csharp
[Test]
[Category("Integration")]
public async Task CreateDonation_WithTestApiKey_ShouldSucceed()
{
    // Arrange - requires test API key and configuration
    var configuration = new ConfigurationBuilder()
        .AddUserSecrets<IntegrationTests>()
        .Build();
    
    var testApiKey = configuration["FundraiseUp:TestApiKey"];
    Assume.That(testApiKey, Is.Not.Null.And.Not.Empty, "Test API key required for integration tests");

    var services = new ServiceCollection();
    services.AddFundraiseUpClient(options => options.ApiKey = testApiKey);
    services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Debug));
    
    var serviceProvider = services.BuildServiceProvider();
    var client = serviceProvider.GetRequiredService<IFundraiseUpClient>();

    var request = new CreateDonationRequest
    {
        Amount = "10.00",
        Currency = "usd",
        Campaign = "FUNXXXXXXXX", // Test campaign ID
        Designation = "EXXXXXXX",  // Test designation ID
        PaymentMethod = new PaymentMethodRequest
        {
            Stripe = new StripePaymentMethodRequest
            {
                Id = "pm_card_visa" // Stripe test payment method
            }
        },
        Supporter = new SupporterRequest
        {
            FirstName = "Integration",
            LastName = "Test",
            Email = $"test-{Guid.NewGuid()}@example.com"
        }
    };

    // Act
    var response = await client.Donations.Create(request).ExecuteAsync();

    // Assert
    Assert.That(response.Id, Is.Not.Null.And.Not.Empty);
    Assert.That(response.Amount, Is.EqualTo("10.00"));
    Assert.That(response.Status, Is.EqualTo("succeeded"));
}
```

---

## Security Best Practices

### API Key Security

**✅ Best Practice**: Secure API key management

```csharp
// Environment variables (production)
services.AddFundraiseUpClient(options =>
{
    options.ApiKey = Environment.GetEnvironmentVariable("FUNDRAISEUP_API_KEY") 
        ?? throw new InvalidOperationException("FundraiseUp API key not configured");
});

// Key Vault (Azure)
services.AddFundraiseUpClient(options =>
{
    options.ApiKey = configuration["KeyVault:FundraiseUpApiKey"];
});

// Validate key format
services.Configure<FundraiseUpClientOptions>(options =>
{
    if (!string.IsNullOrEmpty(options.ApiKey))
    {
        var isValidFormat = options.ApiKey.StartsWith("test_") || options.ApiKey.StartsWith("live_");
        if (!isValidFormat)
        {
            throw new FundraiseUpConfigurationException("Invalid API key format");
        }
    }
});
```

### Sensitive Data Handling

**✅ Best Practice**: Protect sensitive data in logs and memory

```csharp
// Custom log formatter to redact sensitive data
public class SensitiveDataLogFormatter : ILoggerProvider
{
    public ILogger CreateLogger(string categoryName)
    {
        return new SensitiveDataLogger(categoryName);
    }

    private class SensitiveDataLogger : ILogger
    {
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            var message = formatter(state, exception);
            
            // Redact sensitive patterns
            message = Regex.Replace(message, @"\b[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Z|a-z]{2,}\b", "[EMAIL_REDACTED]");
            message = Regex.Replace(message, @"\b(?:test_|live_)[A-Za-z0-9]{20,}\b", "[API_KEY_REDACTED]");
            
            // Log the sanitized message
            Console.WriteLine($"[{logLevel}] {message}");
        }
    }
}
```

By following these best practices, you'll have a robust, maintainable, and secure integration with the FundraiseUp API that performs well in production environments.