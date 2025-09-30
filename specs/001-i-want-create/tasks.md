# Tasks: FundraiseUp API .NET Client Library

**Input**: Design documents from `/specs/001-i-want-create/`
**Prerequisites**: plan.md, research.md, data-model.md, contracts/, quickstart.md

## Execution Flow (main)
```
1. Load plan.md from feature directory ✅
   → Tech stack: C# (.NET Standard 2.0 + .NET 6+)
   → Libraries: System.Net.Http, Microsoft.Extensions.*, xUnit, Moq
   → Structure: Single project with src/FundraiseUp.Client/ and tests/
2. Load design documents ✅:
   → data-model.md: Enhanced with comprehensive configuration validation and custom exceptions
   → contracts/: IFundraiseUpClient interface + fluent builder with validation exceptions
   → research.md: HttpClient with IHttpClientFactory, token bucket rate limiting
   → quickstart.md: DI setup scenarios and fluent builder examples
3. Generate tasks by category ✅:
   → Setup: .NET project, NuGet dependencies, multi-targeting
   → Tests: Contract tests + validation tests + integration scenarios
   → Core: Models with validation, configuration with exceptions, services
   → Integration: DI extensions with validation, logging, error handling
   → Polish: Unit tests, performance validation, documentation
4. Task rules applied ✅:
   → Different files = [P] for parallel execution
   → Tests before implementation (TDD workflow)
   → Enhanced validation throughout all components
5. Tasks numbered T001-T038 with clear dependencies and validation focus
```

## Format: `[ID] [P?] Description`
- **[P]**: Can run in parallel (different files, no dependencies)
- All file paths are absolute based on project structure

## Phase 3.1: Setup & Project Structure
- [x] T001 Create .NET solution and project structure with multi-targeting
  - Create `src/FundraiseUp.Client/FundraiseUp.Client.csproj` with .NET Standard 2.0 + .NET 6+ targets
  - Create `tests/FundraiseUp.Client.Tests/FundraiseUp.Client.Tests.csproj`
  - Create `tests/FundraiseUp.Client.TestHelpers/FundraiseUp.Client.TestHelpers.csproj`
  
- [x] T002 Install NuGet dependencies and configure project properties
  - Add System.Net.Http, Microsoft.Extensions.* packages to main project
  - Add xUnit, Moq, Microsoft.AspNetCore.Mvc.Testing to test projects
  - Configure AssemblyInfo, package metadata, and XML documentation

- [ ] T003 [P] Set up development tooling and quality gates
  - Configure EditorConfig for consistent formatting
  - Add Directory.Build.props for shared MSBuild properties
  - Set up analyzers and code quality rules

## Phase 3.2: Tests First (TDD) ⚠️ MUST COMPLETE BEFORE 3.3
**CRITICAL: These tests MUST be written and MUST FAIL before ANY implementation**

### Contract Tests (API Interface Validation)
- [ ] T004 [P] Contract test for donation operations in `tests/FundraiseUp.Client.Tests/Contract/DonationContractTests.cs`
  - Test IFundraiseUpClient.CreateDonationAsync contract
  - Test IFundraiseUpClient.GetDonationAsync contract
  - Test IFundraiseUpClient.GetDonationsAsync contract
  - Test IFundraiseUpClient.UpdateDonationAsync contract

- [ ] T005 [P] Contract test for campaign operations in `tests/FundraiseUp.Client.Tests/Contract/CampaignContractTests.cs`
  - Test IFundraiseUpClient.CreateCampaignAsync contract
  - Test IFundraiseUpClient.GetCampaignAsync contract
  - Test IFundraiseUpClient.GetCampaignsAsync contract
  - Test IFundraiseUpClient.UpdateCampaignAsync contract
  - Test IFundraiseUpClient.DeleteCampaignAsync contract

- [ ] T006 [P] Contract test for donor operations in `tests/FundraiseUp.Client.Tests/Contract/DonorContractTests.cs`
  - Test IFundraiseUpClient.CreateDonorAsync contract
  - Test IFundraiseUpClient.GetDonorAsync contract
  - Test IFundraiseUpClient.GetDonorsAsync contract
  - Test IFundraiseUpClient.UpdateDonorAsync contract
  - Test IFundraiseUpClient.DeleteDonorAsync contract

- [ ] T007 [P] Contract test for fluent configuration validation in `tests/FundraiseUp.Client.Tests/Contract/FluentBuilderContractTests.cs`
  - Test IFundraiseUpClientBuilder interface methods and exception specifications
  - Test FundraiseUpClient.Create() fluent chain with all configuration methods
  - Test ConfigurationMissingException scenarios (null/empty API key)
  - Test ConfigurationValidationException scenarios (invalid URLs, out-of-range values, malformed input)
  - Test ConfigurationLogicException scenarios (Retry strategy without RetryOptions)
  - Test successful validation and Build() method

### Integration Tests (End-to-End Scenarios)
- [ ] T008 [P] Integration test for configuration validation in `tests/FundraiseUp.Client.Tests/Integration/ConfigurationValidationTests.cs`
  - Test all configuration validation scenarios (missing, malformed, logical conflicts)
  - Test startup-time validation with dependency injection
  - Test runtime validation with fluent builder
  - Test configuration exception types and error messages

- [ ] T009 [P] Integration test for simple donation flow in `tests/FundraiseUp.Client.Tests/Integration/DonationFlowTests.cs`
  - Test complete donation creation workflow from quickstart guide
  - Test request/response validation and error handling
  - Test authentication and HTTPS enforcement

- [ ] T010 [P] Integration test for dependency injection setup in `tests/FundraiseUp.Client.Tests/Integration/DependencyInjectionTests.cs`
  - Test AddFundraiseUpClient extension method with validation
  - Test IOptions pattern configuration with validation
  - Test service resolution and lifecycle management

- [ ] T011 [P] Integration test for rate limiting strategies in `tests/FundraiseUp.Client.Tests/Integration/RateLimitingTests.cs`
  - Test 3 concurrent request limit enforcement
  - Test retry, queue, and exception strategies
  - Test backoff and timeout behaviors

## Phase 3.3: Core Implementation (ONLY after tests are failing)

### Data Models and DTOs
- [ ] T012 [P] Create donation models with comprehensive validation in `src/FundraiseUp.Client/Models/Donations/`
  - Implement DonationRequest, DonationResponse, DonationUpdateRequest
  - Implement DonationQueryOptions with filtering and pagination
  - Add comprehensive data annotations and custom validation attributes
  - Add JSON serialization attributes and format validation

- [ ] T013 [P] Create campaign models with comprehensive validation in `src/FundraiseUp.Client/Models/Campaigns/`
  - Implement CreateCampaignRequest, CampaignResponse, UpdateCampaignRequest
  - Implement CampaignQueryOptions with filtering and pagination
  - Add comprehensive data annotations and custom validation attributes
  - Add JSON serialization attributes and format validation

- [ ] T014 [P] Create donor models with comprehensive validation in `src/FundraiseUp.Client/Models/Donors/`
  - Implement CreateDonorRequest, DonorResponse, UpdateDonorRequest
  - Implement DonorQueryOptions with filtering and pagination
  - Add comprehensive data annotations and custom validation attributes
  - Add JSON serialization attributes and format validation

### Configuration and Authentication
- [ ] T015 [P] Implement client configuration with comprehensive validation in `src/FundraiseUp.Client/Configuration/`
  - Implement ClientConfiguration class with data annotations and IValidatableObject
  - Implement RetryConfiguration with nested validation and logical constraints
  - Create custom configuration exception types (ConfigurationMissingException, ConfigurationValidationException, ConfigurationLogicException)
  - Add validation for API key format, HTTPS URL enforcement, timeout ranges, enum values
  - Implement cross-property validation (RetryOptions required when RateLimitStrategy is Retry)
  - Add IOptions pattern support with validation on registration

- [ ] T016 [P] Implement authentication provider in `src/FundraiseUp.Client/Authentication/AuthenticationProvider.cs`
  - Implement Bearer token attachment to requests
  - Add secure credential management
  - Implement credential validation methods

### Rate Limiting and Utilities
- [ ] T017 [P] Implement rate limiting handler in `src/FundraiseUp.Client/Utilities/RateLimitHandler.cs`
  - Implement token bucket algorithm for 3 concurrent requests
  - Support retry, queue, and exception strategies
  - Add configurable backoff and timeout logic

- [ ] T018 [P] Create custom exceptions in `src/FundraiseUp.Client/Exceptions/`
  - Implement FundraiseUpException base class hierarchy
  - Implement API exceptions (FundraiseUpApiException, RateLimitException, ValidationException, AuthenticationException)
  - Implement configuration exceptions (FundraiseUpConfigurationException, ConfigurationMissingException, ConfigurationValidationException, ConfigurationLogicException)
  - Add HTTP status code and error context properties for API exceptions
  - Add property name, attempted value, and validation context for configuration exceptions
  - Include helpful error messages and documentation links for troubleshooting

### Core Client Services
- [ ] T019 Implement donation service in `src/FundraiseUp.Client/Services/DonationService.cs`
  - Implement all donation CRUD operations
  - Add request/response mapping and validation
  - Integrate rate limiting and error handling

- [ ] T020 Implement campaign service in `src/FundraiseUp.Client/Services/CampaignService.cs`
  - Implement all campaign CRUD operations
  - Add request/response mapping and validation
  - Integrate rate limiting and error handling

- [ ] T021 Implement donor service in `src/FundraiseUp.Client/Services/DonorService.cs`
  - Implement all donor CRUD operations
  - Add request/response mapping and validation
  - Integrate rate limiting and error handling

### Main Client Implementation
- [ ] T022 Implement fluent client builder with comprehensive validation in `src/FundraiseUp.Client/FundraiseUpClientBuilder.cs`
  - Implement IFundraiseUpClientBuilder interface with XML documentation and exception specifications
  - Add immediate validation in each fluent method (WithApiKey, WithBaseUrl, WithTimeout, etc.)
  - Throw ConfigurationMissingException for required null/empty values
  - Throw ConfigurationValidationException for malformed input (invalid URLs, out-of-range values, invalid enums)
  - Implement Build() method with final validation and dependency setup
  - Throw ConfigurationLogicException for logical conflicts (Retry strategy without RetryOptions)
  - Include detailed error messages with corrective guidance

- [ ] T023 Implement main client class in `src/FundraiseUp.Client/FundraiseUpClient.cs`
  - Implement IFundraiseUpClient interface
  - Integrate all service dependencies
  - Add proper IDisposable implementation and lifecycle management

## Phase 3.4: Integration & Extensions
- [ ] T024 [P] Implement dependency injection extensions with validation in `src/FundraiseUp.Client/Extensions/ServiceCollectionExtensions.cs`
  - Create AddFundraiseUpClient extension method with comprehensive configuration validation
  - Implement ValidateConfiguration method with all configuration checks
  - Throw ConfigurationMissingException for missing required values (ApiKey)
  - Throw ConfigurationValidationException for malformed values (invalid URLs, out-of-range timeouts, invalid enums)
  - Throw ConfigurationLogicException for logical conflicts (Retry strategy without RetryOptions)
  - Configure IHttpClientFactory integration with validated timeout and base address
  - Add IOptions pattern support with validation on service registration
  - Include XML documentation with exception specifications for troubleshooting

- [ ] T025 [P] Implement logging integration in `src/FundraiseUp.Client/Utilities/LoggingUtilities.cs`
  - Add structured logging with Microsoft.Extensions.Logging
  - Implement configurable log levels
  - Add request/response logging without sensitive data

- [ ] T026 [P] Add XML documentation and IntelliSense support
  - Document all public APIs with XML comments
  - Add usage examples in documentation
  - Configure project to generate XML documentation files

## Phase 3.5: Testing & Quality Assurance
- [ ] T027 [P] Unit tests for configuration validation in `tests/FundraiseUp.Client.Tests/Unit/Configuration/`
  - Test ClientConfiguration comprehensive validation rules and exception scenarios
  - Test RetryConfiguration cross-property validation and logical constraints
  - Test fluent builder validation behavior and edge cases
  - Test dependency injection registration with validation
  - Test all configuration exception types (Missing, Validation, Logic)

- [ ] T028 [P] Unit tests for authentication in `tests/FundraiseUp.Client.Tests/Unit/Authentication/`
  - Test Bearer token attachment logic
  - Test credential validation methods
  - Test secure credential handling

- [ ] T029 [P] Unit tests for rate limiting in `tests/FundraiseUp.Client.Tests/Unit/Utilities/`
  - Test token bucket algorithm implementation
  - Test different rate limiting strategies
  - Test concurrent request scenarios

- [ ] T030 [P] Unit tests for exception handling in `tests/FundraiseUp.Client.Tests/Unit/Exceptions/`
  - Test custom exception types and inheritance hierarchy
  - Test configuration exception context and error messages
  - Test API exception error context preservation
  - Test HTTP status code mapping

- [ ] T031 [P] Unit tests for model validation in `tests/FundraiseUp.Client.Tests/Unit/Models/`
  - Test request/response model comprehensive validation
  - Test data annotation attributes and custom validation rules
  - Test query options validation and filtering
  - Test cross-property validation scenarios

## Phase 3.6: Performance & Polish
- [ ] T032 [P] Performance tests in `tests/FundraiseUp.Client.Tests/Performance/`
  - Test API response times meet ≤5 second requirement
  - Test concurrent request handling and memory usage
  - Test connection pooling and resource management

- [ ] T033 [P] Create test fixtures and helpers in `tests/FundraiseUp.Client.TestHelpers/`
  - Create mock data generators for all entity types
  - Add HTTP client test fixtures and mocking utilities
  - Create integration test base classes

- [ ] T034 [P] Update documentation in `docs/`
  - Update API reference documentation
  - Add comprehensive usage examples
  - Update configuration guide with all validation options

- [ ] T035 Remove code duplication and refactor
  - Extract common patterns into base classes
  - Optimize HTTP client usage and disposal
  - Clean up and optimize service implementations

- [ ] T036 [P] Package and distribution preparation
  - Configure NuGet package metadata and description
  - Add package icon, license, and project URL
  - Test package creation and installation

- [ ] T037 [P] Run manual testing scenarios from quickstart.md
  - Test all quickstart guide examples
  - Validate dependency injection setup scenarios
  - Test configuration validation and error handling

- [ ] T038 Final integration and smoke tests
  - Run complete test suite against sandbox environment
  - Validate all constitutional principles are met
  - Perform final code review and cleanup

## Dependencies
- **Setup before everything**: T001-T003 before all other tasks
- **Tests before implementation**: T004-T011 must complete and FAIL before T012-T023
- **Models before services**: T012-T014 before T019-T021
- **Services before client**: T019-T021 before T022-T023
- **Configuration dependencies**: T015-T016 before T022-T023
- **Exceptions before implementation**: T018 before T019-T023
- **Core before integration**: T012-T023 before T024-T026
- **Implementation before polish**: T012-T026 before T027-T038

## Parallel Execution Examples

### Phase 3.2 (Contract Tests)
```bash
# All contract tests can run in parallel (different files):
Task: "Contract test for donation operations in tests/FundraiseUp.Client.Tests/Contract/DonationContractTests.cs"
Task: "Contract test for campaign operations in tests/FundraiseUp.Client.Tests/Contract/CampaignContractTests.cs"
Task: "Contract test for donor operations in tests/FundraiseUp.Client.Tests/Contract/DonorContractTests.cs"
Task: "Contract test for fluent configuration in tests/FundraiseUp.Client.Tests/Contract/FluentBuilderContractTests.cs"
```

### Phase 3.3 (Model Implementation)
```bash
# All model creation can run in parallel (different directories):
Task: "Create donation models with comprehensive validation in src/FundraiseUp.Client/Models/Donations/"
Task: "Create campaign models with comprehensive validation in src/FundraiseUp.Client/Models/Campaigns/"
Task: "Create donor models with comprehensive validation in src/FundraiseUp.Client/Models/Donors/"
```

### Phase 3.5 (Unit Tests)
```bash
# All unit tests can run in parallel (different test files):
Task: "Unit tests for configuration validation in tests/FundraiseUp.Client.Tests/Unit/Configuration/"
Task: "Unit tests for authentication in tests/FundraiseUp.Client.Tests/Unit/Authentication/"
Task: "Unit tests for rate limiting in tests/FundraiseUp.Client.Tests/Unit/Utilities/"
Task: "Unit tests for exception handling in tests/FundraiseUp.Client.Tests/Unit/Exceptions/"
Task: "Unit tests for model validation in tests/FundraiseUp.Client.Tests/Unit/Models/"
```

## Validation Checklist
- ✅ All contracts from `contracts/api-contracts.md` have corresponding tests with validation scenarios (T004-T007)
- ✅ All entities from `data-model.md` have model implementations with comprehensive validation (T012-T018)
- ✅ All quickstart scenarios from `quickstart.md` have integration tests including configuration validation (T008-T011)
- ✅ Enhanced configuration validation with custom exception types throughout
- ✅ All constitutional principles addressed in implementation tasks
- ✅ TDD workflow enforced with tests before implementation
- ✅ Parallel execution maximized for independent tasks
- ✅ Clear dependency chain with no circular dependencies

## Notes
- **[P] tasks**: Different files with no dependencies - can run simultaneously
- **File conflicts**: Services (T019-T021) are sequential as they may share utilities
- **Test-first approach**: All contract and integration tests MUST fail before implementation
- **Validation focus**: Enhanced configuration validation with custom exceptions throughout
- **Commit strategy**: Commit after each completed task for atomic changes
- **Multi-targeting**: Ensure compatibility across .NET Standard 2.0 and .NET 6+ throughout
- **Configuration validation**: All validation scenarios must throw appropriate exception types with detailed messages