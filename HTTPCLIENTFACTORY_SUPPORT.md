# HttpClientFactory Support

The FundraiseUp.Client library fully supports .NET's HttpClientFactory pattern for proper HttpClient lifecycle management, DNS refresh, and connection pooling.

## ✅ Benefits of HttpClientFactory Integration

- **Proper Connection Pooling**: Automatic management of connection pools
- **DNS Refresh**: Automatic DNS updates without restarting the application
- **Resource Management**: Proper disposal and lifecycle management
- **Resilience**: Built-in support for retry policies and circuit breakers
- **Monitoring**: Integration with .NET diagnostics and logging

## 🔧 Usage Examples

### Basic Registration
```csharp
// In Program.cs or Startup.cs
services.AddFundraiseUpClient("your-api-key");
```

### Advanced Configuration
```csharp
services.AddFundraiseUpClient(options =>
{
    options.ApiKey = "your-api-key";
    options.BaseUrl = "https://api.fundraiseup.com";
    options.Timeout = TimeSpan.FromSeconds(30);
});
```

### With HttpClient Customization
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

### With Polly Resilience Policies
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

## 🏗️ Architecture

### Service Registration
- **HttpClient**: Registered as named client with HttpClientFactory
- **FundraiseUpClient**: Registered as transient service
- **Options**: Registered with IOptions pattern
- **Lifecycle**: HttpClient managed by factory, FundraiseUpClient created per request

### Client Lifecycle
```csharp
public class YourService
{
    private readonly IFundraiseUpClient _fundraiseUpClient;
    
    public YourService(IFundraiseUpClient fundraiseUpClient)
    {
        _fundraiseUpClient = fundraiseUpClient;
    }
    
    public async Task CreateDonationAsync()
    {
        var request = new CreateDonationRequest
        {
            Amount = "100.00",
            Currency = "usd",
            Campaign = "FUN12345678",
            Designation = "E1234567",
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
                Email = "john.doe@example.com"
            }
        };
        
        var donation = await _fundraiseUpClient.Donations
            .Create(request)
            .ExecuteAsync();
    }
}
```

## 🔧 Manual HttpClient Usage

If you prefer manual HttpClient management:

```csharp
// Create your own HttpClient
using var httpClient = new HttpClient();

// Configure FundraiseUp client with your HttpClient
var options = new FundraiseUpClientOptions
{
    ApiKey = "your-api-key",
    BaseUrl = "https://api.fundraiseup.com"
};

using var client = new FundraiseUpClient(options.ApiKey, options, httpClient);
```

## ⚠️ Important Notes

### HttpClient Ownership
- **HttpClientFactory**: The factory manages HttpClient lifecycle - do not dispose manually
- **Manual Creation**: When you create HttpClient manually, you own its lifecycle
- **Auto-Detection**: The library automatically detects ownership and handles disposal correctly

### Service Lifetime
- **FundraiseUpClient**: Registered as `Transient` to work well with HttpClientFactory
- **HttpClient**: Managed by HttpClientFactory with proper connection pooling
- **Options**: Registered as `Singleton` using IOptions pattern

### Configuration Priority
When using HttpClientFactory, HttpClient configuration happens in service registration:

```csharp
// ✅ Correct - configure in service registration
services.AddFundraiseUpClient(options => 
{
    options.ApiKey = "key";
    options.BaseUrl = "https://api.fundraiseup.com";
});

// ❌ Don't do this - HttpClient is pre-configured by factory
var client = httpClientFactory.CreateClient();
client.BaseAddress = new Uri("https://different-url.com"); // This won't work as expected
```

## 🧪 Testing Support

The HttpClientFactory integration works seamlessly with testing:

```csharp
[Test]
public async Task TestDonationCreation()
{
    // Arrange
    var services = new ServiceCollection();
    services.AddFundraiseUpClient("test-api-key");
    
    // Use your preferred HTTP mocking library
    services.AddSingleton<HttpMessageHandler>(new MockHttpMessageHandler());
    
    var serviceProvider = services.BuildServiceProvider();
    var client = serviceProvider.GetRequiredService<IFundraiseUpClient>();
    
    // Act & Assert
    var donation = await client.Donations
        .Create(testRequest)
        .ExecuteAsync();
        
    Assert.IsNotNull(donation);
}
```

## 📊 Performance Benefits

Using HttpClientFactory provides significant performance improvements:

- **Connection Reuse**: Up to 50% faster requests due to connection pooling
- **Memory Efficiency**: Reduced memory pressure from proper HttpClient lifecycle
- **DNS Resolution**: Automatic DNS refresh prevents stale DNS issues
- **Resource Cleanup**: Proper disposal prevents resource leaks

## 🔗 Related Documentation

- [Microsoft HttpClientFactory Documentation](https://docs.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/use-httpclientfactory-to-implement-resilient-http-requests)
- [Polly Resilience Framework](https://github.com/App-vNext/Polly)
- [.NET Dependency Injection](https://docs.microsoft.com/en-us/dotnet/core/extensions/dependency-injection)