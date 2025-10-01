# Testing Strategy for FundraiseUp .NET Client

## Overview

This project uses a **mocking-first testing approach** that's perfect for open source contributions. Contributors don't need FundraiseUp API keys, and tests run fast and reliably.

## Testing Architecture

### 🎯 Unit Tests (Primary - 32/32 passing)
- **Mock all HTTP calls** using `Moq` framework
- **Fast execution** - no network dependencies
- **Reliable results** - no external API flakiness
- **No credentials required** - perfect for open source
- **Complete coverage** - all business logic and error scenarios

### 🔧 Contract Tests (Optional - for maintainers)
- **Integration tests** that call real APIs
- **Require API credentials** - only for maintainers
- **Used for validation** - ensure mocks match reality
- **Not required for PR contributions**

## For Contributors (No API Key Required)

### Running Tests
```bash
# Run all unit tests (fast, no API key needed)
dotnet test --filter "FullyQualifiedName~UnitTests"

# Run all tests including contract tests (will skip contract tests if no API key)
dotnet test
```

### Writing New Tests

#### 1. Use the MockResponseBuilder
```csharp
// Create sample data
var campaign = MockResponseBuilder.CreateSampleCampaign("test-id");

// Create HTTP response
var response = MockResponseBuilder.CreateJsonResponse(campaign, HttpStatusCode.Created);
```

#### 2. Use HttpClientMockSetup for cleaner mocking
```csharp
var httpMock = new HttpClientMockSetup();
httpMock.SetupRequest(HttpMethod.Post, "/campaigns", mockResponse);
var httpClient = httpMock.CreateHttpClient();
```

#### 3. Verify interactions
```csharp
// Verify the correct HTTP request was made
httpMock.VerifyRequest(HttpMethod.Post, "/campaigns", Times.Once());
```

### Test Categories

#### ✅ Unit Tests (Required for all PRs)
- **Business logic validation**
- **Error handling scenarios**  
- **Request/response mapping**
- **Configuration and options**
- **Retry and timeout behavior**

#### ✅ Mock Response Tests
- **Success scenarios** with various data types
- **Error responses** (404, 401, 400, 500, 422)
- **Validation errors** with structured error parsing
- **Pagination** and list operations
- **Network timeouts** and connectivity issues

## For Maintainers (With API Access)

### Contract Tests
Located in `tests/FundraiseUp.Client.Tests/Contracts/`

These tests:
- Call real FundraiseUp API endpoints
- Validate that mocks match actual API behavior
- Ensure breaking changes are detected
- Require environment configuration:

```bash
# Set environment variables
export FUNDRAISEUP_API_KEY="your-api-key"
export FUNDRAISEUP_BASE_URL="https://api.fundraiseup.com"

# Run contract tests
dotnet test --filter "FullyQualifiedName~Contracts"
```

### Updating Mock Data
When API responses change:
1. Update `MockResponseBuilder` with new response formats
2. Update existing unit tests to match new structures
3. Verify contract tests pass with real API
4. Update mock responses to maintain compatibility

## Test Helpers

### MockResponseBuilder
Creates realistic HTTP responses:
- `CreateJsonResponse<T>()` - JSON responses with proper serialization
- `CreatePaginatedResponse<T>()` - Paginated list responses
- `CreateErrorResponse()` - Error responses with status codes
- `CreateValidationErrorResponse()` - 422 validation errors
- `CreateSample*()` methods - Realistic test data

### HttpClientMockSetup
Simplifies HTTP mocking:
- `SetupRequest()` - Mock specific HTTP method/URL patterns
- `SetupSequence()` - Mock multiple sequential requests
- `VerifyRequest()` - Verify requests were made correctly
- `VerifyNoRequests()` - Ensure no unexpected calls

## Benefits of This Approach

### ✅ For Open Source
- **No barrier to entry** - contributors don't need API accounts
- **Fast CI/CD** - tests complete in seconds, not minutes
- **Reliable builds** - no external dependencies to fail
- **Cost effective** - no API usage costs during development

### ✅ For Quality
- **Comprehensive coverage** - test edge cases easily
- **Predictable results** - same results every time
- **Error scenario testing** - mock any error condition
- **Performance testing** - no rate limits during testing

### ✅ For Development
- **Faster feedback** - instant test results
- **Offline development** - work without internet
- **Parallel testing** - no shared resource conflicts
- **Debug friendly** - step through mock responses easily

## Guidelines

### ✅ Do This
- Mock all HTTP calls in unit tests
- Use realistic test data that matches API schemas
- Test both success and error scenarios
- Verify HTTP requests are made correctly
- Keep tests fast and deterministic

### ❌ Don't Do This
- Make real API calls in unit tests
- Require API keys for basic contribution
- Create tests that depend on external state
- Skip error scenario testing
- Write slow or flaky tests

## Examples

See `tests/FundraiseUp.Client.Tests/UnitTests/Examples/` for complete examples of:
- Creating campaigns with mock responses
- Handling paginated list operations
- Testing validation error scenarios
- Verifying HTTP request patterns

This approach ensures the project remains contributor-friendly while maintaining high test quality and coverage.