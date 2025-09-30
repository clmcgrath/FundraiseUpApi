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
- BaseUrl (string, default: https://api.fundraiseup.com)
- TimeoutSeconds (int, default: 30)
- RateLimitStrategy (enum: Retry, Exception, Queue)
- RetryOptions (RetryConfiguration, optional)
- LogLevel (LogLevel, default: Information)

**Validation Rules**:
- ApiKey must not be null or empty
- BaseUrl must be valid HTTPS URL
- TimeoutSeconds must be positive
- RetryOptions required when RateLimitStrategy is Retry

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
- Amount must be positive
- Currency must be valid ISO 4217 code
- CampaignId must not be null or empty

#### CampaignRequest : BaseRequest
**Purpose**: Request model for campaign operations  
**Key Properties**:
- Name (string, required)
- Description (string, optional)
- GoalAmount (decimal, required)
- Currency (string, required)
- StartDate (DateTimeOffset, required)
- EndDate (DateTimeOffset, optional)

#### DonorRequest : BaseRequest
**Purpose**: Request model for donor operations  
**Key Properties**:
- Email (string, required)
- FirstName (string, required)
- LastName (string, required)
- Phone (string, optional)
- Address (AddressInfo, optional)

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

#### PaymentMethodInfo
**Purpose**: Payment method details  
**Key Properties**:
- Type (string, e.g., "card", "bank")
- Last4 (string, for display)
- Brand (string, e.g., "visa", "mastercard")

#### AddressInfo
**Purpose**: Address information  
**Key Properties**:
- Street (string)
- City (string)
- State (string)
- PostalCode (string)
- Country (string, ISO 3166-1)

#### ApiErrorResponse
**Purpose**: Error response from API  
**Key Properties**:
- ErrorCode (string, from API)
- Message (string, from API)
- Details (Dictionary<string, object>, optional)
- RequestId (string, for tracking)

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

## Validation Rules Summary

### Required Fields
- ClientConfiguration.ApiKey
- All Request models inherit validation from base
- Currency codes must be ISO 4217 compliant
- URLs must be HTTPS format

### Business Rules
- Maximum 3 concurrent requests per account
- All monetary amounts must be positive
- Retry attempts must not exceed configured maximum
- Request timeouts must be reasonable (5-300 seconds)

### State Consistency
- RateLimitHandler concurrent count accurate
- Authentication headers present on all requests
- Request/Response correlation maintained through RequestId