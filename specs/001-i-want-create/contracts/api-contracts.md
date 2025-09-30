# API Contracts: FundraiseUp .NET Client Library

## Client Interface Contract

```csharp
public interface IFundraiseUpClient : IDisposable
{
    // Donations
    Task<DonationResponse> CreateDonationAsync(DonationRequest request, CancellationToken cancellationToken = default);
    Task<DonationResponse> GetDonationAsync(string donationId, CancellationToken cancellationToken = default);
    Task<IEnumerable<DonationResponse>> GetDonationsAsync(DonationQueryOptions options = null, CancellationToken cancellationToken = default);
    Task<DonationResponse> UpdateDonationAsync(string donationId, DonationUpdateRequest request, CancellationToken cancellationToken = default);
    
    // Campaigns
    Task<CampaignResponse> CreateCampaignAsync(CreateCampaignRequest request, CancellationToken cancellationToken = default);
    Task<CampaignResponse> GetCampaignAsync(string campaignId, CancellationToken cancellationToken = default);
    Task<IEnumerable<CampaignResponse>> GetCampaignsAsync(CampaignQueryOptions options = null, CancellationToken cancellationToken = default);
    Task<CampaignResponse> UpdateCampaignAsync(string campaignId, UpdateCampaignRequest request, CancellationToken cancellationToken = default);
    Task DeleteCampaignAsync(string campaignId, CancellationToken cancellationToken = default);
    
    // Donors
    Task<DonorResponse> CreateDonorAsync(CreateDonorRequest request, CancellationToken cancellationToken = default);
    Task<DonorResponse> GetDonorAsync(string donorId, CancellationToken cancellationToken = default);
    Task<IEnumerable<DonorResponse>> GetDonorsAsync(DonorQueryOptions options = null, CancellationToken cancellationToken = default);
    Task<DonorResponse> UpdateDonorAsync(string donorId, UpdateDonorRequest request, CancellationToken cancellationToken = default);
    Task DeleteDonorAsync(string donorId, CancellationToken cancellationToken = default);
}
```

## Fluent Configuration Contract

```csharp
public interface IFundraiseUpClientBuilder
{
    IFundraiseUpClientBuilder WithApiKey(string apiKey);
    IFundraiseUpClientBuilder WithBaseUrl(string baseUrl);
    IFundraiseUpClientBuilder WithTimeout(TimeSpan timeout);
    IFundraiseUpClientBuilder WithRateLimitStrategy(RateLimitStrategy strategy);
    IFundraiseUpClientBuilder WithRetryPolicy(RetryConfiguration retryConfig);
    IFundraiseUpClientBuilder WithLogging(LogLevel logLevel);
    IFundraiseUpClient Build();
}

// Fluent usage example:
// var client = FundraiseUpClient.Create()
//     .WithApiKey("your-api-key")
//     .WithTimeout(TimeSpan.FromSeconds(30))
//     .WithRateLimitStrategy(RateLimitStrategy.Retry)
//     .Build();
```

## Dependency Injection Contract

```csharp
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFundraiseUpClient(this IServiceCollection services, Action<FundraiseUpClientOptions> configure)
    {
        // Configure options
        services.Configure(configure);
        
        // Register HTTP client
        services.AddHttpClient<IFundraiseUpClient, FundraiseUpClient>();
        
        // Register supporting services
        services.AddScoped<IAuthenticationProvider, BearerTokenAuthenticationProvider>();
        services.AddScoped<IRateLimitHandler, ConcurrencyRateLimitHandler>();
        
        return services;
    }
}

// Usage example:
// services.AddFundraiseUpClient(options =>
// {
//     options.ApiKey = configuration["FundraiseUp:ApiKey"];
//     options.RateLimitStrategy = RateLimitStrategy.Queue;
// });
```

## Error Handling Contract

```csharp
public abstract class FundraiseUpException : Exception
{
    public string RequestId { get; }
    public int? HttpStatusCode { get; }
    
    protected FundraiseUpException(string message, string requestId, int? httpStatusCode = null, Exception innerException = null)
        : base(message, innerException) { }
}

public class FundraiseUpApiException : FundraiseUpException
{
    public string ErrorCode { get; }
    public Dictionary<string, object> ErrorDetails { get; }
}

public class FundraiseUpRateLimitException : FundraiseUpException
{
    public TimeSpan RetryAfter { get; }
    public int ConcurrentRequestCount { get; }
}

public class FundraiseUpAuthenticationException : FundraiseUpException
{
    // Thrown when API key is invalid or missing
}

public class FundraiseUpConfigurationException : FundraiseUpException
{
    // Thrown when client configuration is invalid
}
```

## Rate Limiting Contract

```csharp
public interface IRateLimitHandler
{
    Task<T> ExecuteAsync<T>(Func<Task<T>> operation, CancellationToken cancellationToken = default);
    bool CanExecuteImmediately { get; }
    int CurrentConcurrentRequests { get; }
    RateLimitStrategy Strategy { get; }
}

public enum RateLimitStrategy
{
    Retry,      // Retry with exponential backoff
    Exception,  // Throw exception immediately
    Queue       // Queue requests until slot available
}
```

## Configuration Contract

```csharp
public class FundraiseUpClientOptions
{
    public string ApiKey { get; set; }
    public string BaseUrl { get; set; } = "https://api.fundraiseup.com";
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);
    public RateLimitStrategy RateLimitStrategy { get; set; } = RateLimitStrategy.Retry;
    public RetryConfiguration RetryOptions { get; set; } = new RetryConfiguration();
    public LogLevel LogLevel { get; set; } = LogLevel.Information;
}

public class RetryConfiguration
{
    public int MaxRetries { get; set; } = 3;
    public TimeSpan BaseDelay { get; set; } = TimeSpan.FromSeconds(1);
    public TimeSpan MaxDelay { get; set; } = TimeSpan.FromSeconds(30);
    public double BackoffMultiplier { get; set; } = 2.0;
}
```

## HTTP Contract Requirements

### Request Headers
- `Authorization: Bearer {api-key}` (required on all requests)
- `Content-Type: application/json` (for POST/PUT requests)
- `Accept: application/json` (all requests)
- `User-Agent: FundraiseUp-DotNet-Client/{version}` (identification)

### Response Handling
- Success: HTTP 200/201 with JSON response body
- Client Error: HTTP 400-499 with error details in response body
- Server Error: HTTP 500-599 with optional error details
- Rate Limit: HTTP 429 with `concurrent_requests_limit_exceeded` error code

### URL Patterns
- Base URL: `https://api.fundraiseup.com/v1/`
- Donations: `/donations`, `/donations/{id}`
- Campaigns: `/campaigns`, `/campaigns/{id}`
- Donors: `/donors`, `/donors/{id}`

### Query Parameters
- Pagination: `page`, `limit`
- Filtering: `status`, `created_after`, `created_before`
- Sorting: `sort`, `order` (asc/desc)

This contract specification ensures all client implementations follow consistent patterns and meet the functional requirements specified in the feature specification.