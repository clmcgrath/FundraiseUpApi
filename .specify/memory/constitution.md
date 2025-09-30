<!--
Sync Impact Report:
Version change: 1.5.0 → 1.5.1 (Patch: Added MIT license and licensing requirements)
Enhanced sections:
- Quality Standards: Added Licensing Requirements for open source compliance
Artifacts created:
- ✅ LICENSE (MIT License file in repository root)
- ✅ Updated docs/index.md license reference
Templates updated:
- ✅ .specify/templates/plan-template.md (version reference updated to v1.5.1)
Follow-up TODOs: None
-->

# FundraiseUpApi Constitution

## Core Principles

### I. Library-First Architecture
Every feature MUST be implemented as a standalone, reusable library. Libraries MUST be self-contained, independently testable, and thoroughly documented. No organizational-only libraries without clear functional purpose. All libraries MUST support dependency injection and follow .NET Standard conventions for maximum compatibility.

### II. Developer Experience Focus
All APIs MUST provide fluent, discoverable interfaces that read naturally and integrate seamlessly with modern .NET development practices. IntelliSense discoverability is mandatory. Configuration MUST follow standard .NET patterns (IOptions, IConfiguration). Installation MUST be simple via NuGet with minimal setup required.

### III. Microsoft DI Integration (NON-NEGOTIABLE)
All libraries MUST provide extension methods for IServiceCollection registration following Microsoft DI conventions. Extension methods MUST include sensible default configurations while allowing full customization through lambda expressions (Action<TOptions> configure). Configuration MUST support IOptions pattern, IConfiguration binding, and environment-specific overrides. Service lifetimes MUST be appropriate (Scoped for stateful, Singleton for stateless services). HttpClient registration MUST use IHttpClientFactory patterns.

### IV. Test-Driven Development (NON-NEGOTIABLE)
TDD is strictly enforced: Contract tests written → Contracts approved → Tests fail → Implementation created to pass tests. Red-Green-Refactor cycle MUST be followed. All public APIs MUST have contract tests. Integration tests MUST validate actual external service interactions. No implementation without corresponding failing tests first.

### V. Enterprise-Grade Reliability  
All external integrations MUST handle failures gracefully with configurable retry strategies. Rate limiting MUST be respected and configurable. Authentication MUST be secure with proper credential management. All operations MUST support cancellation tokens and timeouts. Error handling MUST provide actionable information with custom exception types.

### VI. Async-First Architecture (NON-NEGOTIABLE)
All I/O-bound operations MUST be asynchronous using Task/Task<T> return types with async/await patterns. Synchronous blocking calls are prohibited for external operations. All async methods MUST accept CancellationToken parameters. ConfigureAwait(false) MUST be used in library code. Async methods MUST follow naming conventions (suffix with "Async"). Deadlock-safe patterns MUST be enforced.

### VII. Comprehensive Observability
All operations MUST integrate with Microsoft.Extensions.Logging for structured logging. Configurable log levels MUST be supported (minimal, standard, verbose). Performance metrics MUST be measurable. All external calls MUST be traceable. Request/response correlation MUST be maintained for debugging.

### VIII. Security-First Design (NON-NEGOTIABLE)
All libraries MUST be secure by default with no sensitive data exposed in logs, exceptions, or serialization. API credentials MUST be handled securely with support for secure storage providers (Azure Key Vault, etc.). Input validation MUST be comprehensive with proper sanitization. HTTPS MUST be enforced for all external communications. Security headers MUST be properly configured. Credential rotation MUST be supported without service restart.

### IX. Performance & Resource Management (NON-NEGOTIABLE)
All libraries MUST implement proper resource disposal patterns with IDisposable/IAsyncDisposable. Memory allocation MUST be minimized for high-throughput scenarios. Connection pooling MUST be utilized through IHttpClientFactory. Large responses MUST support streaming. Proper object lifecycle management MUST prevent memory leaks. Resource limits MUST be configurable and enforced.

### X. API Design & Consistency
All public APIs MUST follow consistent naming conventions, parameter ordering, and return type patterns throughout the library. Method overloads MUST be logically organized from simple to complex. Fluent interfaces MUST maintain consistent chaining patterns. Error handling MUST be uniform across all operations. Parameter validation MUST be consistent and provide clear error messages.

### XI. OpenAPI Standards Compliance (NON-NEGOTIABLE)
All client libraries MUST be generated from or strictly adhere to OpenAPI 3.0+ specifications. Request/response models MUST match OpenAPI schema definitions exactly. HTTP status codes, headers, and error responses MUST align with OpenAPI documentation. Contract testing MUST validate compliance with OpenAPI specifications. API versioning MUST follow OpenAPI versioning patterns. Client libraries MUST support OpenAPI-defined authentication schemes.

### XII. Backward Compatibility & Versioning
All libraries MUST follow semantic versioning (MAJOR.MINOR.PATCH) strictly using GitVersion with GitHub Flow branching model. Breaking changes MUST be avoided within major versions. Deprecation warnings MUST be provided before removal in next major version. Migration guides MUST be provided for breaking changes. API evolution MUST maintain compatibility with existing consumer code wherever possible. Automated versioning MUST be driven by conventional commits and branch patterns.

### XIII. Documentation & Discoverability
All public APIs MUST include comprehensive XML documentation with examples. IntelliSense MUST provide clear guidance on usage patterns. Code examples MUST be included for common scenarios. API reference documentation MUST be auto-generated and kept current. Quick-start guides MUST enable immediate productivity for new users.

## Quality Standards

### Performance Requirements
- API operations MUST complete within documented SLA timeouts (≤5 seconds standard)
- Connection pooling MUST be utilized through IHttpClientFactory patterns  
- Memory usage MUST be optimized for high-throughput scenarios
- Rate limits MUST be respected with appropriate backoff strategies

### Security Requirements
- HTTPS MUST be enforced for all external communications
- API credentials MUST be handled securely without logging sensitive data
- Bearer token authentication MUST follow OAuth 2.0 standards
- Configuration MUST support secure storage integration (Azure Key Vault, etc.)

### Compatibility Requirements
- Multi-targeting MUST support both .NET Standard 2.0 and .NET 6+ when applicable
- Breaking changes MUST follow semantic versioning (MAJOR.MINOR.PATCH)
- Backward compatibility MUST be maintained within major versions
- Cross-platform compatibility MUST be verified

### Documentation Requirements
- GitHub Pages documentation MUST be maintained in `/docs` directory
- API reference documentation MUST be auto-generated from XML comments
- Getting started guides, tutorials, and examples MUST be provided
- Documentation MUST be versioned alongside releases
- Search functionality and responsive design MUST be supported

### API Standards Requirements
- OpenAPI 3.0+ specification compliance MUST be maintained
- Contract-first development approach MUST be followed
- Request/response schemas MUST match OpenAPI definitions exactly
- HTTP status codes and error responses MUST align with OpenAPI documentation
- API versioning MUST follow OpenAPI semantic versioning patterns

### Licensing Requirements
- All projects MUST include a clear open source license (MIT recommended)
- License file MUST be present in repository root as `LICENSE`
- Copyright notices MUST be included in all source files
- Third-party dependencies MUST have compatible licenses
- License compatibility MUST be verified before adding dependencies

## Development Workflow

### Branching Standards (NON-NEGOTIABLE)
- **master** branch MUST contain production-ready code only
- **dev** branch MUST be the integration branch for all feature development
- **stable** branch MUST track the latest stable release for hotfixes
- Feature branches MUST be created from **dev** and merged back via pull request
- Hotfix branches MUST be created from **stable** and merged to both **master** and **dev**
- Direct commits to **master** are prohibited except for automated releases
- Branch protection rules MUST be enforced on **master**, **dev**, and **stable**

### Versioning & Release Management
- GitVersion MUST be used for automated semantic versioning following GitHub Flow
- Conventional commits MUST be used to drive version increments
- Version numbers MUST be automatically calculated from commit history and branch patterns
- Pre-release versions MUST be generated for **dev** branch builds
- Stable releases MUST only be created from **master** branch
- Release tags MUST be automatically applied by GitVersion

### Code Review Process
- All code changes MUST pass constitutional compliance review
- Test coverage MUST be maintained above 90% for core functionality
- Performance impact MUST be assessed for external API integrations
- Documentation MUST be updated for all public API changes

### Quality Gates
- All tests MUST pass before merge (unit, integration, contract)
- Static analysis MUST pass without violations
- Package vulnerabilities MUST be resolved before release
- API documentation MUST be generated and current
- GitVersion validation MUST pass for all branches

### Release Process
- Semantic versioning MUST be automated through GitVersion
- Release notes MUST be auto-generated from conventional commits
- NuGet packages MUST include proper metadata and symbols
- Compatibility matrices MUST be maintained and published
- GitHub releases MUST be created automatically with proper versioning

## Governance

This constitution supersedes all other development practices and standards. All pull requests and code reviews MUST verify compliance with these principles. Any complexity that violates these principles MUST be explicitly justified with documented rationale and simpler alternatives considered.

Amendments require documentation of impact, stakeholder approval, and migration plan for existing code. Use `.specify/memory/constitution.md` for constitutional guidance and `.github/copilot-instructions.md` for current runtime development context.

**Version**: 1.5.1 | **Ratified**: 2025-09-29 | **Last Amended**: 2025-09-29