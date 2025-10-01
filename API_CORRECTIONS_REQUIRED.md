# FundraiseUp API Corrections Required

This document outlines all the corrections needed to align our client library with the actual FundraiseUp API documentation.

## Summary of Issues

The initial implementation was based on assumptions rather than the official FundraiseUp API specification. This led to incorrect field names, data types, and missing required fields throughout the codebase.

## Actual FundraiseUp API Endpoints

Based on the official API documentation at https://api.fundraiseup.com/v1/docs/, the FundraiseUp API provides these endpoints:

### Available Endpoints:
1. **POST /v1/donations** - Create donation
2. **GET /v1/donations** - List donations  
3. **GET /v1/donations/{id}** - Get specific donation
4. **POST /v1/donations/{id}** - Update donation (API-created only, within 24 hours)
5. **GET /v1/supporters** - List supporters
6. **GET /v1/supporters/{id}** - Get specific supporter
7. **GET /v1/recurring_plans** - List recurring plans
8. **GET /v1/recurring_plans/{id}** - Get specific recurring plan
9. **POST /v1/fundraisers** - Create fundraiser
10. **GET /v1/fundraisers** - List fundraisers
11. **GET /v1/fundraisers/{id}** - Get specific fundraiser
12. **POST /v1/fundraisers/{id}** - Update fundraiser
13. **GET /v1/events** - List events (audit log)
14. **POST /v1/donor_portal/access_links/supporters/{id}** - Create supporter access link
15. **POST /v1/donor_portal/access_links/recurring_plans/{id}** - Create recurring plan access link

### NOT Available (contrary to our implementation):
- ❌ Campaign creation/update endpoints - campaigns are managed via Dashboard only
- ❌ Donor/Supporter creation endpoints - supporters are created automatically via donations
- ❌ Direct donor management - supporters are read-only via API

## Required Model Corrections

### 1. CreateDonationRequest ✅ FIXED
**Before (Incorrect):**
```csharp
public class CreateDonationRequest
{
    public decimal Amount { get; set; }
    public string Currency { get; set; }
    public string DonorEmail { get; set; }
    public string CampaignId { get; set; }
    public string DonorName { get; set; }
    public string Message { get; set; }
    public bool IsAnonymous { get; set; }
}
```

**After (Correct):**
```csharp
public class CreateDonationRequest
{
    public string Amount { get; set; }              // String, not decimal
    public string Campaign { get; set; }            // Not "CampaignId"
    public string Currency { get; set; }            // Lowercase ISO codes
    public string Designation { get; set; }         // Required field
    public PaymentMethodRequest PaymentMethod { get; set; } // Required
    public SupporterRequest Supporter { get; set; } // Not individual fields
    public string Comment { get; set; }             // Not "Message"
    public List<CustomFieldRequest> CustomFields { get; set; }
    public List<QuestionRequest> Questions { get; set; }
    public RecurringPlanRequest RecurringPlan { get; set; }
    public bool? SkipThankYouEmail { get; set; }
}
```

### 2. UpdateDonationRequest (Needs Fix)
**Current implementation needs to match API spec:**
```csharp
public class UpdateDonationRequest
{
    public List<QuestionPutRequest> Questions { get; set; }
    public SupporterPutRequest Supporter { get; set; }
    public string Campaign { get; set; }
    public string Comment { get; set; }
    public List<CustomFieldPutRequest> CustomFields { get; set; }
    public string Designation { get; set; }
}
```

### 3. Donation Response Model (Needs Major Updates)
The current `Donation` model needs to match the API response structure:

```csharp
public class DonationResponse
{
    public string Id { get; set; }                    // e.g., "DXXXXXXX"
    public string Amount { get; set; }                // String, not decimal
    public string AmountInDefaultCurrency { get; set; }
    public string Currency { get; set; }
    public bool Anonymous { get; set; }               // Not "IsAnonymous"
    public string Status { get; set; }                // "succeeded", "failed", etc.
    public DateTime CreatedAt { get; set; }
    public DateTime? SucceededAt { get; set; }
    public DateTime? FailedAt { get; set; }
    public string Comment { get; set; }
    public bool LiveMode { get; set; }
    public string Source { get; set; }                // "api", "website", etc.
    
    // Nested objects
    public AccountResponse Account { get; set; }
    public CampaignResponse Campaign { get; set; }
    public EmbeddedSupporterResponse Supporter { get; set; }
    public DesignationResponse Designation { get; set; }
    public ChargePaymentResponse Payment { get; set; }
    public List<CustomFieldResponse> CustomFields { get; set; }
    public List<QuestionResponse> Questions { get; set; }
    public DonationRecurringPlanResponse RecurringPlan { get; set; }
    // ... many more fields
}
```

### 4. Supporter/Donor Models (Major Changes Required)
**Current "Donor" should be "Supporter" and read-only:**

```csharp
public class SupporterResponse
{
    public string Id { get; set; }              // "SXXXXXXXX"
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Phone { get; set; }
    public string Title { get; set; }           // "mr", "mrs", "ms"
    public string Language { get; set; }        // "en-US", "fr-FR"
    public DateTime CreatedAt { get; set; }
    public bool LiveMode { get; set; }
    public AddressResponse Address { get; set; }
    public EmployerResponse Employer { get; set; }
    public AccountResponse Account { get; set; }
}
```

### 5. Campaign Model (Read-Only)
```csharp
public class CampaignResponse
{
    public string Id { get; set; }      // "FUNXXXXXXXX"
    public string Code { get; set; }    // Custom campaign code
    public string Name { get; set; }    // Campaign name
}
```

### 6. Recurring Plan Models (New)
```csharp
public class RecurringPlanResponse
{
    public string Id { get; set; }              // "RXXXXXXX"
    public string Status { get; set; }          // "active", "paused", etc.
    public string Frequency { get; set; }       // "monthly", "weekly", etc.
    public DateTime CreatedAt { get; set; }
    public DateTime? EndedAt { get; set; }
    public DateTime? NextInstallmentAt { get; set; }
    // ... many more fields matching API
}
```

### 7. Events Model (New)
```csharp
public class EventResponse
{
    public string Id { get; set; }
    public string Type { get; set; }        // "donation.created", etc.
    public DateTime CreatedAt { get; set; }
    public bool LiveMode { get; set; }
    public string Donation { get; set; }    // Donation ID if applicable
    public string RecurringPlan { get; set; } // Recurring plan ID if applicable
    public string Supporter { get; set; }   // Supporter ID if applicable
    public AccountResponse Account { get; set; }
}
```

## Required Client Operation Updates

### 1. Donation Operations ✅ COMPLETED
- ✅ Create donation - Updated request model with proper validation
- ✅ Update donation - Added UpdateDonationRequest with comprehensive validation
- ✅ List donations - Fixed API endpoint and pagination model  
- ✅ Get donation - Fixed API endpoint and response model
- ✅ All endpoints now use correct /v1/donations path
- ✅ Added comprehensive request validation and error handling

### 2. Supporter Operations (Major Changes) ✅ COMPLETED
- ✅ Remove Create/Update operations - not supported by API
- ✅ Add List supporters with cursor pagination
- ✅ Add Get supporter by ID
- ✅ Update all references from "Donor" to "Supporter"
- ✅ Use SupporterResponse model for consistency
- ✅ Fixed API endpoints to use correct /v1/supporters path

### 3. Campaign Operations (Remove Most) ✅ COMPLETED
- ✅ Remove Create/Update operations - not supported by API  
- ✅ Campaign data only available embedded in other responses
- ✅ Updated interface and implementation to reflect API limitations

### 4. Recurring Plan Operations (Add New) ✅ COMPLETED
- ✅ Add List recurring plans with cursor pagination
- ✅ Add Get recurring plan by ID
- ✅ Add recurring plan creation via donations (handled through donation operations)

### 5. Events Operations (Add New) ✅ COMPLETED
- ✅ Add List events with filtering by event types
- ✅ Add proper event type enums
- ✅ Add Get event by ID
- ✅ Add comprehensive filtering options (date range, entity types, event types)

### 6. Fundraiser Operations (Add New) ✅ COMPLETED
- ✅ Add Create fundraiser
- ✅ Add Update fundraiser
- ✅ Add List fundraisers
- ✅ Add Get fundraiser by ID
- ✅ Add comprehensive filtering and search options

### 7. Donor Portal Operations (Add New) ✅ COMPLETED  
- ✅ Add Create supporter access link
- ✅ Add Create recurring plan access link

## Pagination Model Corrections

FundraiseUp uses cursor-based pagination, not offset-based:

```csharp
public class PaginatedResponse<T>
{
    public List<T> Data { get; set; }    // Not "Items"
    public bool HasMore { get; set; }    // Not "TotalCount"
}

// Pagination parameters
public class PaginationOptions
{
    public int? Limit { get; set; }           // 1-100, default 10
    public string StartingAfter { get; set; } // Object ID for next page
    public string EndingBefore { get; set; }  // Object ID for previous page
}
```

## Files That Need Updates

### Request/Response Models:
- ✅ `src/FundraiseUp.Client/Requests/ApiRequests.cs` - Partially fixed
- ❌ `src/FundraiseUp.Client/Models/` - All model files need updates
- ❌ Add missing models: Events, Fundraisers
- ✅ RecurringPlans - COMPLETED

### Operations:
- ✅ `src/FundraiseUp.Client/Operations/DonationOperations.cs` - COMPLETED
- ✅ `src/FundraiseUp.Client/Operations/SupporterOperations.cs` (rename from DonorOperations) - COMPLETED
- ✅ Remove: `CampaignOperations.cs` (most operations) - COMPLETED
- ✅ Add: `RecurringPlanOperations.cs` - COMPLETED
- ✅ Add: `EventOperations.cs` - COMPLETED
- ✅ Add: `FundraiserOperations.cs` - COMPLETED
- ✅ Add: `DonorPortalOperations.cs` - COMPLETED

### Tests:
- ❌ All unit tests need request/response model updates
- ❌ All contract tests need API structure updates
- ❌ All mock helpers need correct field mappings

### Documentation:
- ✅ `docs/EXAMPLES.md` - Partially fixed
- ❌ `README.md` - Needs updates
- ❌ `docs/GETTING-STARTED.md` - Needs updates

## Next Steps Priority

1. **HIGH**: ✅ Fix all response models to match API specification - **COMPLETED**
2. **HIGH**: ✅ Update all operations to match available endpoints - **PARTIALLY COMPLETED** (Supporters, Campaigns done)
3. **HIGH**: ✅ Fix pagination throughout the codebase - **PARTIALLY COMPLETED** (Supporters, new operations done)
4. **MEDIUM**: ✅ Add missing operations (Events, Fundraisers, RecurringPlans, DonorPortal) - **COMPLETED**
5. **MEDIUM**: Update all tests with correct API structures
6. **LOW**: Update remaining documentation

## API Key Requirements

The API requires specific permissions:
- **Retrieve donation data**: To access donation information
- **Create new donations**: To process donations
- **Retrieve fundraiser data**: To access fundraiser information  
- **Create new fundraisers**: To create fundraiser records
- **Generate Donor Portal access links**: To create login links

## Testing Considerations

- Test mode API keys have `test_` prefix
- Use Stripe test payment methods like `pm_card_visa`
- Sandbox environment: May be different base URL
- Rate limiting: Max 3 concurrent requests per account

This represents a significant amount of work to correct, but it's essential for a production-ready client library that actually works with the FundraiseUp API.