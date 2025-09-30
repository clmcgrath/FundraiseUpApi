# OpenAPI Compliance Guide

This document outlines how the FundraiseUp .NET Client Library adheres to OpenAPI 3.0+ standards.

## OpenAPI Specification Alignment

The client library is designed to strictly adhere to the FundraiseUp API OpenAPI specification. All request/response models, HTTP status codes, and error handling patterns are derived from the official OpenAPI documentation.

### Schema Compliance

#### Request Models
All request DTOs are generated to match OpenAPI schema definitions:

```csharp
// Generated from OpenAPI schema: DonationRequest
public class DonationRequest
{
    [Required]
    [JsonPropertyName("amount")]
    public decimal Amount { get; set; }

    [Required]
    [JsonPropertyName("currency")]
    [StringLength(3, MinimumLength = 3)]
    public string Currency { get; set; }

    [Required]  
    [JsonPropertyName("campaign_id")]
    public string CampaignId { get; set; }

    [JsonPropertyName("donor_id")]
    public string? DonorId { get; set; }
}
```

#### Response Models
Response DTOs mirror OpenAPI response schemas exactly:

```csharp
// Generated from OpenAPI schema: DonationResponse
public class DonationResponse : BaseResponse
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("amount")]
    public decimal Amount { get; set; }

    [JsonPropertyName("currency")]
    public string Currency { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; }

    [JsonPropertyName("transaction_id")]
    public string? TransactionId { get; set; }
}
```

### HTTP Status Code Mapping

The client library maps HTTP status codes according to OpenAPI specifications:

```csharp
// HTTP 200/201 - Success responses
Task<DonationResponse> CreateDonationAsync(DonationRequest request);

// HTTP 400 - Bad Request (validation errors)
throw new FundraiseUpValidationException(errorDetails);

// HTTP 401 - Unauthorized (invalid API key)
throw new FundraiseUpAuthenticationException("Invalid API key");

// HTTP 429 - Too Many Requests (rate limiting)
throw new FundraiseUpRateLimitException(retryAfter, concurrentRequests);

// HTTP 500+ - Server errors
throw new FundraiseUpServerException(statusCode, message);
```

### Error Response Handling

Error responses follow OpenAPI error schema:

```json
{
  "error": {
    "code": "validation_failed",
    "message": "Request validation failed",
    "details": {
      "field_errors": [
        {
          "field": "amount",
          "message": "Amount must be positive"
        }
      ]
    }
  }
}
```

Corresponding .NET exception:

```csharp
public class FundraiseUpApiException : FundraiseUpException
{
    public string ErrorCode { get; }
    public Dictionary<string, object> ErrorDetails { get; }
    
    // Populated from OpenAPI error response
}
```

### Authentication Schemes

The client supports OpenAPI-defined authentication:

```yaml
# OpenAPI Security Scheme
security:
  - bearerAuth: []

components:
  securitySchemes:
    bearerAuth:
      type: http
      scheme: bearer
      bearerFormat: JWT
```

Implementation:

```csharp
// Bearer token authentication as per OpenAPI spec
request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
```

### API Versioning

Following OpenAPI versioning patterns:

```yaml
# OpenAPI version specification
openapi: 3.0.3
info:
  title: FundraiseUp API
  version: 1.0.0
servers:
  - url: https://api.fundraiseup.com/v1
```

Client implementation:

```csharp
public class FundraiseUpClientOptions
{
    public string BaseUrl { get; set; } = "https://api.fundraiseup.com/v1";
    public string ApiVersion { get; set; } = "1.0.0";
}
```

## Contract Testing

The library includes contract tests to validate OpenAPI compliance:

```csharp
[Test]
public async Task CreateDonation_RequestMatchesOpenApiSchema()
{
    // Arrange
    var request = new DonationRequest
    {
        Amount = 100.00m,
        Currency = "USD",
        CampaignId = "campaign-123"
    };

    // Act & Assert - Validates against OpenAPI schema
    var response = await client.CreateDonationAsync(request);
    
    // Verify response matches OpenAPI schema
    Assert.That(response.Id, Is.Not.Null);
    Assert.That(response.Amount, Is.EqualTo(100.00m));
    Assert.That(response.Currency, Is.EqualTo("USD"));
}

[Test]
public async Task ApiError_MatchesOpenApiErrorSchema()
{
    // Act & Assert
    var exception = await Assert.ThrowsAsync<FundraiseUpApiException>(() =>
        client.CreateDonationAsync(new DonationRequest { Amount = -100 }));
    
    // Verify exception matches OpenAPI error schema
    Assert.That(exception.ErrorCode, Is.EqualTo("validation_failed"));
    Assert.That(exception.ErrorDetails, Contains.Key("field_errors"));
}
```

## Schema Generation

The library can optionally generate OpenAPI schemas from its models:

```csharp
// Generate OpenAPI schema for validation
public static class SchemaGenerator
{
    public static OpenApiDocument GenerateSchema()
    {
        var document = new OpenApiDocument
        {
            Info = new OpenApiInfo
            {
                Title = "FundraiseUp .NET Client",
                Version = "1.0.0"
            }
        };

        // Add schemas for all request/response models
        document.Components.Schemas.Add(nameof(DonationRequest), 
            GenerateSchemaForType<DonationRequest>());
        
        return document;
    }
}
```

## Validation Tools

The library includes tools to validate OpenAPI compliance:

```bash
# Validate client models against OpenAPI spec
dotnet run --project Tools.SchemaValidator -- validate-models --spec fundraiseup-api.json

# Generate client code from OpenAPI spec  
dotnet run --project Tools.CodeGenerator -- generate --spec fundraiseup-api.json --output Generated/
```

This ensures the client library maintains strict compliance with OpenAPI standards throughout its development lifecycle.