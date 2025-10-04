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
    /// <summary>
    /// Sets the API key for authentication
    /// </summary>
    /// <param name="apiKey">FundraiseUp API key (required, 10-500 characters, no whitespace)</param>
    /// <exception cref="ConfigurationMissingException">Thrown when apiKey is null or empty</exception>
    /// <exception cref="ConfigurationValidationException">Thrown when apiKey format is invalid</exception>
    IFundraiseUpClientBuilder WithApiKey(string apiKey);
    
    /// <summary>
    /// Sets the base URL for the FundraiseUp API
    /// </summary>
    /// <param name="baseUrl">HTTPS URL (required, HTTPS only for security)</param>
    /// <exception cref="ConfigurationValidationException">Thrown when URL is invalid or not HTTPS</exception>
    IFundraiseUpClientBuilder WithBaseUrl(string baseUrl);
    
    /// <summary>
    /// Sets the HTTP request timeout
    /// </summary>
    /// <param name="timeout">Timeout duration (required, 1-300 seconds)</param>
    /// <exception cref="ConfigurationValidationException">Thrown when timeout is outside valid range</exception>
    IFundraiseUpClientBuilder WithTimeout(TimeSpan timeout);
    
    /// <summary>
    /// Sets the rate limiting strategy
    /// </summary>
    /// <param name="strategy">Rate limiting strategy (required enum value)</param>
    /// <exception cref="ConfigurationValidationException">Thrown when strategy is not a valid enum value</exception>
    IFundraiseUpClientBuilder WithRateLimitStrategy(RateLimitStrategy strategy);
    
    /// <summary>
    /// Sets the retry policy configuration
    /// </summary>
    /// <param name="retryConfig">Retry configuration (validates all nested properties)</param>
    /// <exception cref="ConfigurationValidationException">Thrown when retry config has invalid values</exception>
    /// <exception cref="ConfigurationLogicException">Thrown when retry config has logical conflicts</exception>
    IFundraiseUpClientBuilder WithRetryPolicy(RetryConfiguration retryConfig);
    
    /// <summary>
    /// Sets the logging level
    /// </summary>
    /// <param name="logLevel">Microsoft.Extensions.Logging.LogLevel enum value</param>
    /// <exception cref="ConfigurationValidationException">Thrown when logLevel is not a valid enum value</exception>
    IFundraiseUpClientBuilder WithLogging(LogLevel logLevel);
    
    /// <summary>
    /// Builds the configured FundraiseUp client
    /// </summary>
    /// <returns>Configured IFundraiseUpClient instance</returns>
    /// <exception cref="ConfigurationMissingException">Thrown when required configuration is missing</exception>
    /// <exception cref="ConfigurationLogicException">Thrown when configuration has logical conflicts</exception>
    IFundraiseUpClient Build();
}

// Fluent usage example with validation:
// var client = FundraiseUpClient.Create()
//     .WithApiKey("your-api-key")                    // Validates: non-null, length, format
//     .WithBaseUrl("https://api.fundraiseup.com/v1")   // Validates: HTTPS, valid URL
//     .WithTimeout(TimeSpan.FromSeconds(30))        // Validates: 1-300 seconds range  
//     .WithRateLimitStrategy(RateLimitStrategy.Retry) // Validates: valid enum value
//     .WithRetryPolicy(new RetryConfiguration       // Validates: all nested properties
//     {
//         MaxRetries = 3,                           // Validates: 0-10 range
//         BaseDelay = TimeSpan.FromSeconds(1),      // Validates: 100ms-60s range
//         MaxDelay = TimeSpan.FromSeconds(30),      // Validates: >= BaseDelay, <= 5min
//         BackoffMultiplier = 2.0                   // Validates: 1.0-10.0 range
//     })
//     .Build();                                     // Validates: all required config present
```

## Dependency Injection Contract

```csharp
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds FundraiseUp client services to the dependency injection container
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="configure">Configuration action with validation</param>
    /// <exception cref="ConfigurationMissingException">Thrown when required configuration is missing</exception>
    /// <exception cref="ConfigurationValidationException">Thrown when configuration is invalid</exception>
    /// <exception cref="ConfigurationLogicException">Thrown when configuration has logical conflicts</exception>
    public static IServiceCollection AddFundraiseUpClient(this IServiceCollection services, Action<FundraiseUpClientOptions> configure)
    {
        // Configure and validate options
        services.Configure<FundraiseUpClientOptions>(options =>
        {
            configure(options);
            ValidateConfiguration(options);
        });
        
        // Register HTTP client with timeout configuration
        services.AddHttpClient<IFundraiseUpClient, FundraiseUpClient>((serviceProvider, httpClient) =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<FundraiseUpClientOptions>>().Value;
            httpClient.BaseAddress = new Uri(options.BaseUrl);
            httpClient.Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds);
        });
        
        // Register supporting services
        services.AddScoped<IAuthenticationProvider, BearerTokenAuthenticationProvider>();
        services.AddScoped<IRateLimitHandler, ConcurrencyRateLimitHandler>();
        
        return services;
    }
    
    /// <summary>
    /// Validates FundraiseUp client configuration
    /// </summary>
    /// <param name="options">Configuration options to validate</param>
    /// <exception cref="ConfigurationMissingException">Required values missing</exception>
    /// <exception cref="ConfigurationValidationException">Invalid values provided</exception>
    /// <exception cref="ConfigurationLogicException">Logical configuration conflicts</exception>
    private static void ValidateConfiguration(FundraiseUpClientOptions options)
    {
        // Validate required API key
        if (string.IsNullOrWhiteSpace(options.ApiKey))
            throw new ConfigurationMissingException("ApiKey", "FundraiseUp:ApiKey", 
                "API key is required. Configure via appsettings.json, environment variables, or options action.");
                
        // Validate API key format
        if (options.ApiKey.Length < 10 || options.ApiKey.Length > 500 || options.ApiKey.Any(char.IsWhiteSpace))
            throw new ConfigurationValidationException("ApiKey", options.ApiKey, "Format",
                "API key must be 10-500 characters with no whitespace characters.");
                
        // Validate base URL
        if (!Uri.TryCreate(options.BaseUrl, UriKind.Absolute, out var uri) || uri.Scheme != "https")
            throw new ConfigurationValidationException("BaseUrl", options.BaseUrl, "Format",
                "Base URL must be a valid HTTPS URL. HTTP URLs are not allowed for security reasons.");
                
        // Validate timeout range
        if (options.TimeoutSeconds < 1 || options.TimeoutSeconds > 300)
            throw new ConfigurationValidationException("TimeoutSeconds", options.TimeoutSeconds, "Range",
                "Timeout must be between 1 and 300 seconds for reasonable response times.");
                
        // Validate rate limit strategy enum
        if (!Enum.IsDefined(typeof(RateLimitStrategy), options.RateLimitStrategy))
            throw new ConfigurationValidationException("RateLimitStrategy", options.RateLimitStrategy, "Enum",
                "RateLimitStrategy must be one of: Retry, Exception, Queue.");
                
        // Validate retry options when required
        if (options.RateLimitStrategy == RateLimitStrategy.Retry)
        {
            if (options.RetryOptions == null)
                throw new ConfigurationLogicException(
                    new[] { "RateLimitStrategy", "RetryOptions" },
                    "RetryOptions is required when RateLimitStrategy is set to Retry",
                    "Either provide RetryOptions configuration or change RateLimitStrategy to Exception or Queue.");
                    
            ValidateRetryOptions(options.RetryOptions);
        }
        
        // Validate log level enum
        if (!Enum.IsDefined(typeof(LogLevel), options.LogLevel))
            throw new ConfigurationValidationException("LogLevel", options.LogLevel, "Enum",
                "LogLevel must be a valid Microsoft.Extensions.Logging.LogLevel value.");
    }
    
    /// <summary>
    /// Validates retry configuration options
    /// </summary>
    private static void ValidateRetryOptions(RetryConfiguration retryOptions)
    {
        if (retryOptions.MaxRetries < 0 || retryOptions.MaxRetries > 10)
            throw new ConfigurationValidationException("RetryOptions.MaxRetries", retryOptions.MaxRetries, "Range",
                "MaxRetries must be between 0 and 10. Use 0 to disable retries.");
                
        if (retryOptions.BaseDelay < TimeSpan.FromMilliseconds(100) || retryOptions.BaseDelay > TimeSpan.FromSeconds(60))
            throw new ConfigurationValidationException("RetryOptions.BaseDelay", retryOptions.BaseDelay, "Range",
                "BaseDelay must be between 100ms and 60 seconds.");
                
        if (retryOptions.MaxDelay > TimeSpan.FromMinutes(5))
            throw new ConfigurationValidationException("RetryOptions.MaxDelay", retryOptions.MaxDelay, "Range",
                "MaxDelay cannot exceed 5 minutes.");
                
        if (retryOptions.MaxDelay < retryOptions.BaseDelay)
            throw new ConfigurationLogicException(
                new[] { "RetryOptions.MaxDelay", "RetryOptions.BaseDelay" },
                "MaxDelay must be greater than or equal to BaseDelay",
                "Adjust MaxDelay to be at least equal to BaseDelay.");
                
        if (retryOptions.BackoffMultiplier < 1.0 || retryOptions.BackoffMultiplier > 10.0)
            throw new ConfigurationValidationException("RetryOptions.BackoffMultiplier", retryOptions.BackoffMultiplier, "Range",
                "BackoffMultiplier must be between 1.0 and 10.0.");
    }
}

// Usage examples with validation:

// Valid configuration
services.AddFundraiseUpClient(options =>
{
    options.ApiKey = configuration["FundraiseUp:ApiKey"];           // Validates: required, format
    options.BaseUrl = "https://api.fundraiseup.com/v1";               // Validates: HTTPS, valid URL
    options.TimeoutSeconds = 30;                                   // Validates: 1-300 range
    options.RateLimitStrategy = RateLimitStrategy.Queue;           // Validates: valid enum
    options.LogLevel = LogLevel.Information;                       // Validates: valid enum
});

// Configuration with retry policy validation
services.AddFundraiseUpClient(options =>
{
    options.ApiKey = configuration["FundraiseUp:ApiKey"];
    options.RateLimitStrategy = RateLimitStrategy.Retry;           // Requires RetryOptions
    options.RetryOptions = new RetryConfiguration                 // Validates all nested properties
    {
        MaxRetries = 3,                                           // Validates: 0-10 range
        BaseDelay = TimeSpan.FromSeconds(1),                      // Validates: 100ms-60s
        MaxDelay = TimeSpan.FromSeconds(30),                      // Validates: >= BaseDelay
        BackoffMultiplier = 2.0                                   // Validates: 1.0-10.0 range
    };
});

// Configuration exceptions thrown at startup:
// - ConfigurationMissingException: When ApiKey is missing
// - ConfigurationValidationException: When values are malformed  
// - ConfigurationLogicException: When Retry strategy lacks RetryOptions
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
    public string BaseUrl { get; set; } = "https://api.fundraiseup.com/v1";
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