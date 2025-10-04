# Data Model: FundraiseUp API .NET Client Library

## Core Entities

### FundraiseUpClient
**Purpose**: Primary client interface for API interactions  
**Key Properties**:
- Configuration settings (API key, base URL, timeout)
- HTTP client instance
- Rate limiter instance
- Logger instance

**Relationships**:
- Contains AuthenticationProvider
- Contains RateLimitHandler
- Uses RequestModels and ResponseModels

### ClientConfiguration
**Purpose**: Configuration container for client setup  
**Key Properties**:
- ApiKey (string, required)
- BaseUrl (string, default: https://api.fundraiseup.com/v1)
- TimeoutSeconds (int, default: 30)
- RateLimitStrategy (enum: Retry, Exception, Queue)
- RetryOptions (RetryConfiguration, optional)
- LogLevel (LogLevel, default: Information)

**Validation Rules**:
- ApiKey: Required, non-null/empty string, length 10-500 characters, no whitespace allowed
- BaseUrl: Required, valid HTTPS URL only (HTTP rejected for security), max length 2048 characters
- TimeoutSeconds: Required, positive integer, range 1-300 seconds (5 minutes max)
- RateLimitStrategy: Required, must be valid enum value (Retry, Exception, Queue)
- RetryOptions: Required when RateLimitStrategy is Retry, must pass nested validation, null otherwise
- LogLevel: Required, must be valid Microsoft.Extensions.Logging.LogLevel enum value

**Configuration Exception Handling**:
```csharp
// Missing required values - throw ConfigurationMissingException
if (string.IsNullOrEmpty(ApiKey))
    throw new ConfigurationMissingException("ApiKey", "API key is required for FundraiseUp client authentication");

// Malformed input - throw ConfigurationValidationException  
if (!Uri.TryCreate(BaseUrl, UriKind.Absolute, out var uri) || uri.Scheme != "https")
    throw new ConfigurationValidationException("BaseUrl", BaseUrl, "Base URL must be a valid HTTPS URL");

if (TimeoutSeconds < 1 || TimeoutSeconds > 300)
    throw new ConfigurationValidationException("TimeoutSeconds", TimeoutSeconds, "Timeout must be between 1 and 300 seconds");

// Conditional validation - throw ConfigurationLogicException
if (RateLimitStrategy == RateLimitStrategy.Retry && RetryOptions == null)
    throw new ConfigurationLogicException("RetryOptions", "RetryOptions is required when RateLimitStrategy is set to Retry");
```

### AuthenticationProvider
**Purpose**: Handles Bearer token authentication  
**Key Properties**:
- ApiKey (secure string)
- TokenPrefix (constant: "Bearer ")

**Methods**:
- AttachAuthHeader(HttpRequestMessage)
- ValidateCredentials()

### RateLimitHandler
**Purpose**: Manages API rate limiting according to FundraiseUp's 3 concurrent request limit  
**Key Properties**:
- ConcurrentRequestCount (int, max: 3)
- Strategy (RateLimitStrategy enum)
- RequestQueue (for Queue strategy)
- RetryConfiguration (for Retry strategy)

**State Transitions**:
- Idle → Processing (when request starts)
- Processing → Idle (when request completes)
- Processing → Throttled (when limit exceeded)
- Throttled → Processing (when slot available)

### Request Models

#### BaseRequest
**Purpose**: Common properties for all API requests  
**Key Properties**:
- RequestId (Guid, auto-generated)
- Timestamp (DateTimeOffset, auto-generated)

#### DonationRequest : BaseRequest
**Purpose**: Request model for donation operations  
**Key Properties**:
- Amount (decimal, required)
- Currency (string, required, ISO 4217)
- DonorId (string, optional)
- CampaignId (string, required)
- PaymentMethod (PaymentMethodInfo, required)

**Validation Rules**:
- Amount: Must be positive (> 0), maximum 6 decimal places, range 0.01 to 999,999,999.99
- Currency: Must be valid ISO 4217 code (3 uppercase letters), required
- DonorId: If provided, must be non-empty string, max length 100 characters
- CampaignId: Required, non-null/empty string, max length 100 characters
- PaymentMethod: Required, must pass nested validation

#### CampaignRequest : BaseRequest
**Purpose**: Request model for campaign operations  
**Key Properties**:
- Name (string, required)
- Description (string, optional)
- GoalAmount (decimal, required)
- Currency (string, required)
- StartDate (DateTimeOffset, required)
- EndDate (DateTimeOffset, optional)

**Validation Rules**:
- Name: Required, non-null/empty string, length 1-255 characters, no special characters except spaces, hyphens, apostrophes
- Description: Optional, max length 2000 characters if provided
- GoalAmount: Required, positive decimal (> 0), maximum 6 decimal places, range 1.00 to 999,999,999.99
- Currency: Required, valid ISO 4217 code (3 uppercase letters)
- StartDate: Required, must be valid DateTimeOffset, cannot be more than 5 years in the past
- EndDate: Optional, if provided must be after StartDate, cannot be more than 10 years in the future

#### DonorRequest : BaseRequest
**Purpose**: Request model for donor operations  
**Key Properties**:
- Email (string, required)
- FirstName (string, required)
- LastName (string, required)
- Phone (string, optional)
- Address (AddressInfo, optional)

**Validation Rules**:
- Email: Required, valid email format (RFC 5322), max length 320 characters, normalized to lowercase
- FirstName: Required, non-null/empty string, length 1-100 characters, letters and common punctuation only
- LastName: Required, non-null/empty string, length 1-100 characters, letters and common punctuation only
- Phone: Optional, if provided must match international format (+1234567890) or domestic format, max length 20 characters
- Address: Optional, if provided must pass nested AddressInfo validation

### Response Models

#### BaseResponse
**Purpose**: Common properties for all API responses  
**Key Properties**:
- Id (string, from API)
- CreatedAt (DateTimeOffset, from API)
- UpdatedAt (DateTimeOffset, from API)
- Status (string, from API)

#### DonationResponse : BaseResponse
**Purpose**: Response model for donation operations  
**Key Properties**:
- Amount (decimal)
- Currency (string)
- DonorId (string)
- CampaignId (string)
- PaymentStatus (string)
- TransactionId (string)

#### CampaignResponse : BaseResponse
**Purpose**: Response model for campaign operations  
**Key Properties**:
- Name (string)
- Description (string)
- GoalAmount (decimal)
- CurrentAmount (decimal)
- Currency (string)
- DonationCount (int)
- StartDate (DateTimeOffset)
- EndDate (DateTimeOffset)

#### DonorResponse : BaseResponse
**Purpose**: Response model for donor operations  
**Key Properties**:
- Email (string)
- FirstName (string)
- LastName (string)
- Phone (string)
- TotalDonated (decimal)
- DonationCount (int)
- FirstDonationDate (DateTimeOffset)
- LastDonationDate (DateTimeOffset)

### Supporting Models

#### RetryConfiguration
**Purpose**: Configuration for retry behavior  
**Key Properties**:
- MaxRetries (int, default: 3)
- BaseDelay (TimeSpan, default: 1 second)
- MaxDelay (TimeSpan, default: 30 seconds)
- BackoffMultiplier (double, default: 2.0)

**Validation Rules**:
- MaxRetries: Required, non-negative integer, range 0-10 (0 = no retries)
- BaseDelay: Required, positive TimeSpan, range 100ms to 60 seconds
- MaxDelay: Required, positive TimeSpan, must be >= BaseDelay, max 300 seconds (5 minutes)
- BackoffMultiplier: Required, positive decimal, range 1.0-10.0 (1.0 = linear, >1.0 = exponential)

**Configuration Exception Handling**:
```csharp
// Validate MaxRetries range
if (MaxRetries < 0 || MaxRetries > 10)
    throw new ConfigurationValidationException("MaxRetries", MaxRetries, "Range",
        "MaxRetries must be between 0 and 10. Use 0 to disable retries entirely.");

// Validate BaseDelay range
if (BaseDelay < TimeSpan.FromMilliseconds(100) || BaseDelay > TimeSpan.FromSeconds(60))
    throw new ConfigurationValidationException("BaseDelay", BaseDelay, "Range",
        "BaseDelay must be between 100ms and 60 seconds. Consider impact on user experience.");

// Validate MaxDelay range  
if (MaxDelay > TimeSpan.FromMinutes(5))
    throw new ConfigurationValidationException("MaxDelay", MaxDelay, "Range",
        "MaxDelay cannot exceed 5 minutes to maintain reasonable response times.");

// Validate logical relationship between delays
if (MaxDelay < BaseDelay)
    throw new ConfigurationLogicException(
        new[] { "MaxDelay", "BaseDelay" },
        "MaxDelay must be greater than or equal to BaseDelay",
        "Adjust MaxDelay to be at least equal to BaseDelay, or reduce BaseDelay to a smaller value.");

// Validate BackoffMultiplier range
if (BackoffMultiplier < 1.0 || BackoffMultiplier > 10.0)
    throw new ConfigurationValidationException("BackoffMultiplier", BackoffMultiplier, "Range",
        "BackoffMultiplier must be between 1.0 and 10.0. Use 1.0 for linear backoff, higher values for exponential.");
```

#### PaymentMethodInfo
**Purpose**: Payment method details  
**Key Properties**:
- Type (string, e.g., "card", "bank")
- Last4 (string, for display)
- Brand (string, e.g., "visa", "mastercard")

**Validation Rules**:
- Type: Required, non-null/empty string, must be one of: "card", "bank", "paypal", "apple_pay", "google_pay"
- Last4: Required, exactly 4 numeric characters (0-9), used for display purposes only
- Brand: Required for card types, must be one of: "visa", "mastercard", "amex", "discover", "diners", "jcb"

#### AddressInfo
**Purpose**: Address information  
**Key Properties**:
- Street (string)
- City (string)
- State (string)
- PostalCode (string)
- Country (string, ISO 3166-1)

**Validation Rules**:
- Street: Required, non-null/empty string, length 1-200 characters, alphanumeric and common punctuation
- City: Required, non-null/empty string, length 1-100 characters, letters and common punctuation only
- State: Optional for some countries, length 2-100 characters, alphanumeric and spaces
- PostalCode: Required for most countries, length 3-20 characters, format varies by country
- Country: Required, valid ISO 3166-1 alpha-2 code (2 uppercase letters), e.g., "US", "CA", "GB"

#### ApiErrorResponse
**Purpose**: Error response from API  
**Key Properties**:
- ErrorCode (string, from API)
- Message (string, from API)
- Details (Dictionary<string, object>, optional)
- RequestId (string, for tracking)

**Validation Rules** (for deserialization):
- ErrorCode: Required from API, non-null/empty string, max length 100 characters
- Message: Required from API, non-null/empty string, max length 1000 characters
- Details: Optional, if present must be valid dictionary with string keys
- RequestId: Optional, if present must be valid GUID or alphanumeric string, max length 100 characters

### Configuration Exception Models

#### FundraiseUpConfigurationException : FundraiseUpException
**Purpose**: Base class for all configuration-related exceptions  
**Key Properties**:
- PropertyName (string) - Name of the configuration property that failed
- ConfigurationSection (string) - Section of configuration (e.g., "FundraiseUp", "RetryOptions")
- HelpLink (string) - URL to configuration documentation

#### ConfigurationMissingException : FundraiseUpConfigurationException
**Purpose**: Thrown when required configuration values are missing or null  
**Key Properties**:
- RequiredValue (string) - Name of the missing required value
- ConfigurationPath (string) - Full configuration path (e.g., "FundraiseUp:ApiKey")

**Usage Examples**:
```csharp
// Missing API key
throw new ConfigurationMissingException("ApiKey", "FundraiseUp:ApiKey", 
    "API key is required for FundraiseUp client authentication. " +
    "Set via configuration, environment variable, or fluent builder.");

// Missing retry options when required
throw new ConfigurationMissingException("RetryOptions", "FundraiseUp:RetryOptions",
    "RetryOptions is required when RateLimitStrategy is set to Retry. " +
    "Configure retry settings or change to a different rate limit strategy.");
```

#### ConfigurationValidationException : FundraiseUpConfigurationException
**Purpose**: Thrown when configuration values are provided but are malformed or invalid  
**Key Properties**:
- AttemptedValue (object) - The invalid value that was provided
- ValidationType (string) - Type of validation that failed (e.g., "Format", "Range", "Enum")
- ExpectedFormat (string) - Description of the expected format or range

**Usage Examples**:
```csharp
// Invalid URL format
throw new ConfigurationValidationException("BaseUrl", invalidUrl, "Format",
    "Base URL must be a valid HTTPS URL (e.g., https://api.fundraiseup.com/v1). " +
    "HTTP URLs are not allowed for security reasons.");

// Invalid timeout range  
throw new ConfigurationValidationException("TimeoutSeconds", 9999, "Range",
    "Timeout must be between 1 and 300 seconds. " +
    "Consider whether such a long timeout is necessary for your use case.");

// Invalid enum value
throw new ConfigurationValidationException("RateLimitStrategy", "InvalidStrategy", "Enum",
    "RateLimitStrategy must be one of: Retry, Exception, Queue. " +
    "Check the spelling and capitalization of the strategy name.");
```

#### ConfigurationLogicException : FundraiseUpConfigurationException
**Purpose**: Thrown when configuration values are individually valid but create logical conflicts  
**Key Properties**:
- ConflictingProperties (string[]) - Names of properties involved in the conflict
- ConflictDescription (string) - Description of the logical conflict

**Usage Examples**:
```csharp
// Retry strategy without retry options
throw new ConfigurationLogicException(
    new[] { "RateLimitStrategy", "RetryOptions" },
    "RetryOptions must be configured when RateLimitStrategy is set to Retry",
    "Either provide RetryOptions configuration or change RateLimitStrategy to Exception or Queue");

// Invalid retry configuration logic
throw new ConfigurationLogicException(
    new[] { "MaxDelay", "BaseDelay" },
    "MaxDelay must be greater than or equal to BaseDelay",
    "Adjust MaxDelay to be at least equal to BaseDelay, or reduce BaseDelay");
```

## Entity Relationships

```
FundraiseUpClient
├── ClientConfiguration
├── AuthenticationProvider
├── RateLimitHandler
└── HttpClient

Request Flow:
BaseRequest → [Validation] → HTTP Request → [Rate Limiting] → [Authentication] → API

Response Flow:
API → HTTP Response → [Error Handling] → BaseResponse → Typed Response

Error Flow:
API Error → ApiErrorResponse → Custom Exception → Consumer
```

### Query Options Models

#### DonationQueryOptions
**Purpose**: Parameters for filtering and pagination of donation queries  
**Validation Rules**:
- CampaignId: Optional, if provided must be non-empty string, max length 100 characters
- DonorId: Optional, if provided must be non-empty string, max length 100 characters
- MinAmount/MaxAmount: Optional, positive decimals with MaxAmount >= MinAmount
- StartDate/EndDate: Optional, EndDate >= StartDate, reasonable date ranges
- Page: Required, positive integer, range 1-10000
- PageSize: Required, positive integer, range 1-1000

#### CampaignQueryOptions  
**Purpose**: Parameters for filtering and pagination of campaign queries
**Validation Rules**:
- Name: Optional, length 1-255 characters for partial matching
- Status: Optional, must be "active", "inactive", "completed", or "cancelled"
- MinGoal/MaxGoal: Optional, positive decimals with MaxGoal >= MinGoal
- CreatedAfter/CreatedBefore: Optional, CreatedBefore >= CreatedAfter
- Page/PageSize: Same rules as DonationQueryOptions

#### DonorQueryOptions
**Purpose**: Parameters for filtering and pagination of donor queries
**Validation Rules**:
- Email/FirstName/LastName: Optional, valid formats for partial matching
- MinTotalDonated/MaxTotalDonated: Optional, non-negative decimals with MaxTotalDonated >= MinTotalDonated  
- FirstDonationAfter/FirstDonationBefore: Optional, FirstDonationBefore >= FirstDonationAfter
- Page/PageSize: Same rules as DonationQueryOptions

## Comprehensive Validation Implementation

### Data Annotation Strategy
```csharp
// Monetary validation
[Required(ErrorMessage = "Amount is required")]
[Range(0.01, 999999999.99, ErrorMessage = "Amount must be between 0.01 and 999,999,999.99")]
[RegularExpression(@"^\d+(\.\d{1,6})?$", ErrorMessage = "Amount cannot have more than 6 decimal places")]
public decimal Amount { get; set; }

// Currency validation
[Required(ErrorMessage = "Currency is required")]
[CurrencyCode(ErrorMessage = "Currency must be a valid ISO 4217 code")]
public string Currency { get; set; }

// Email validation
[Required(ErrorMessage = "Email is required")]
[EmailAddress(ErrorMessage = "Invalid email format")]
[MaxLength(320, ErrorMessage = "Email cannot exceed 320 characters")]
public string Email { get; set; }

// String length and format validation
[Required(ErrorMessage = "Campaign name is required")]
[StringLength(255, MinimumLength = 1, ErrorMessage = "Campaign name must be 1-255 characters")]
[RegularExpression(@"^[a-zA-Z0-9\s\-']+$", ErrorMessage = "Campaign name contains invalid characters")]
public string Name { get; set; }
```

### Custom Validation Attributes
- **[CurrencyCode]**: Validates against ISO 4217 currency codes (USD, EUR, GBP, etc.)
- **[CountryCode]**: Validates against ISO 3166-1 alpha-2 country codes (US, CA, GB, etc.)
- **[PhoneNumber]**: Validates international (+1234567890) and domestic phone formats
- **[PositiveDecimal]**: Validates positive decimal with configurable max decimal places
- **[DateRange]**: Validates date ranges with configurable past/future limits
- **[HttpsUrl]**: Validates HTTPS URLs only (rejects HTTP for security)

### Multi-Layer Validation Flow
1. **Property Level**: Data annotations validate individual property constraints
2. **Object Level**: IValidatableObject validates cross-property relationships and business rules
3. **Business Logic**: Custom validators for complex domain-specific validation
4. **Pre-Request**: Client-side validation before HTTP requests to prevent unnecessary API calls
5. **Post-Response**: Validation of API responses and error handling

### Validation Error Handling
```csharp
public class ValidationException : FundraiseUpApiException
{
    public List<ValidationError> ValidationErrors { get; set; }
    public string PropertyName { get; set; }
    public object AttemptedValue { get; set; }
}

public class ValidationError
{
    public string PropertyName { get; set; }
    public string ErrorMessage { get; set; }
    public object AttemptedValue { get; set; }
    public string ErrorCode { get; set; }
}
```

## Validation Rules Summary

### Data Type Constraints
- **Decimals**: Positive values, max 6 decimal places, reasonable ranges
- **Strings**: Non-null/empty when required, length limits, format validation
- **DateTimeOffset**: Reasonable past/future ranges, logical date relationships
- **Integers**: Positive ranges, reasonable limits for pagination and timeouts
- **Enums**: Must be valid enum values, case-insensitive parsing supported

### Business Rules Validation
- Currency codes must be ISO 4217 compliant (USD, EUR, GBP, etc.)
- URLs must be HTTPS format for security compliance
- Email addresses must be RFC 5322 compliant
- Phone numbers support international and domestic formats
- Amounts must be positive and within reasonable donation ranges
- Date ranges must be logical (EndDate >= StartDate)
- Pagination parameters must be within reasonable limits

### Security Validation
- API keys validated for format and length constraints
- No sensitive data exposure in validation error messages
- HTTPS enforcement for all URLs
- Input sanitization for display purposes
- Rate limiting parameters validated to prevent abuse

### Performance Validation
- Page sizes limited to prevent excessive data transfer
- Query date ranges limited to prevent expensive database queries
- Timeout values constrained to reasonable limits
- Retry configurations validated to prevent infinite loops

This comprehensive validation strategy ensures data integrity, security, and optimal performance while providing clear, actionable error messages to developers using the library.