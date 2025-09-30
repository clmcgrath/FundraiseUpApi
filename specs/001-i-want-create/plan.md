
# Implementation Plan: FundraiseUp API .NET Client Library

**Branch**: `001-i-want-create` | **Date**: September 29, 2025 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/001-i-want-create/spec.md`

## Execution Flow (/plan command scope)
```
1. Load feature spec from Input path
   → If not found: ERROR "No feature spec at {path}"
2. Fill Technical Context (scan for NEEDS CLARIFICATION)
   → Detect Project Type from file system structure or context (web=frontend+backend, mobile=app+api)
   → Set Structure Decision based on project type
3. Fill the Constitution Check section based on the content of the constitution document.
4. Evaluate Constitution Check section below
   → If violations exist: Document in Complexity Tracking
   → If no justification possible: ERROR "Simplify approach first"
   → Update Progress Tracking: Initial Constitution Check
5. Execute Phase 0 → research.md
   → If NEEDS CLARIFICATION remain: ERROR "Resolve unknowns"
6. Execute Phase 1 → contracts, data-model.md, quickstart.md, agent-specific template file (e.g., `CLAUDE.md` for Claude Code, `.github/copilot-instructions.md` for GitHub Copilot, `GEMINI.md` for Gemini CLI, `QWEN.md` for Qwen Code or `AGENTS.md` for opencode).
7. Re-evaluate Constitution Check section
   → If new violations: Refactor design, return to Phase 1
   → Update Progress Tracking: Post-Design Constitution Check
8. Plan Phase 2 → Describe task generation approach (DO NOT create tasks.md)
9. STOP - Ready for /tasks command
```

**IMPORTANT**: The /plan command STOPS at step 7. Phases 2-4 are executed by other commands:
- Phase 2: /tasks command creates tasks.md
- Phase 3-4: Implementation execution (manual or via tools)

## Summary
Create a .NET client library for consuming the FundraiseUp API that provides a fluent, developer-friendly interface with dependency injection support. The library must support all FundraiseUp API endpoints, handle Bearer token authentication, enforce HTTPS, and be easily installable via NuGet. Key features include configurable rate limiting strategies, optional retry logic, multi-framework targeting (.NET Standard 2.0 + .NET 6+), and comprehensive logging integration.

## Technical Context
**Language/Version**: C# (.NET Standard 2.0 + .NET 6+)  
**Primary Dependencies**: System.Net.Http, Microsoft.Extensions.DependencyInjection, Microsoft.Extensions.Logging, Microsoft.Extensions.Configuration  
**Storage**: N/A (HTTP client library)  
**Testing**: xUnit, Moq, Microsoft.AspNetCore.Mvc.Testing  
**Target Platform**: Cross-platform (.NET Standard 2.0 compatible + .NET 6+ specific optimizations)
**Project Type**: single (NuGet library package)  
**Performance Goals**: API response times ≤5 seconds, respect 3 concurrent request limit  
**Constraints**: HTTPS only, Bearer token auth, configurable rate limiting, optional retry logic  
**Scale/Scope**: Support all FundraiseUp API endpoints, designed for enterprise-grade applications

## Constitution Check
*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

**Status**: PASS - All constitutional principles satisfied:
- I. Library-First Architecture: ✅ Standalone NuGet package with clear functional purpose
- II. Developer Experience Focus: ✅ Fluent API with IntelliSense discoverability and standard .NET patterns
- III. Microsoft DI Integration: ✅ AddFundraiseUpClient extension method with IOptions pattern and default configurations
- IV. Test-Driven Development: ✅ Contract tests before implementation, TDD workflow planned
- V. Enterprise-Grade Reliability: ✅ Configurable retry strategies, rate limiting, proper error handling
- VI. Async-First Architecture: ✅ All API methods async with Task<T> returns, CancellationToken support, ConfigureAwait(false)
- VII. Comprehensive Observability: ✅ Microsoft.Extensions.Logging integration with configurable levels
- VIII. Security-First Design: ✅ HTTPS enforcement, secure credential handling, no sensitive data in logs
- IX. Performance & Resource Management: ✅ IDisposable patterns, IHttpClientFactory usage, memory-efficient design
- X. API Design & Consistency: ✅ Consistent naming conventions, fluent patterns, uniform error handling
- XI. OpenAPI Standards Compliance: ✅ Request/response models align with FundraiseUp OpenAPI spec, contract testing planned
- XII. Backward Compatibility & Versioning: ✅ GitVersion with GitHub Flow planned, conventional commits, automated versioning
- XIII. Documentation & Discoverability: ✅ XML docs, comprehensive examples, quickstart guide planned

## Project Structure

### Documentation (this feature)
```
specs/[###-feature]/
├── plan.md              # This file (/plan command output)
├── research.md          # Phase 0 output (/plan command)
├── data-model.md        # Phase 1 output (/plan command)
├── quickstart.md        # Phase 1 output (/plan command)
├── contracts/           # Phase 1 output (/plan command)
└── tasks.md             # Phase 2 output (/tasks command - NOT created by /plan)
```

### Source Code (repository root)
<!--
  ACTION REQUIRED: Replace the placeholder tree below with the concrete layout
  for this feature. Delete unused options and expand the chosen structure with
  real paths (e.g., apps/admin, packages/something). The delivered plan must
  not include Option labels.
-->
```
src/FundraiseUp.Client/
├── Models/              # Request/Response DTOs
├── Services/            # Core API client services
├── Configuration/       # Client configuration and options
├── Authentication/      # Bearer token handling
├── Extensions/          # DI registration extensions
├── Contracts/          # Interface definitions
├── Exceptions/         # Custom exception types
└── Utilities/          # Rate limiting, retry logic, logging

tests/
├── FundraiseUp.Client.Tests/
│   ├── Unit/           # Unit tests for individual components
│   ├── Integration/    # Tests against actual API
│   └── Contract/       # API contract validation tests
└── FundraiseUp.Client.TestHelpers/
    └── Fixtures/       # Test data and mock setups
```

**Structure Decision**: Single project structure selected for NuGet library. The main library project (`FundraiseUp.Client`) contains all core functionality organized by concern (Models, Services, Configuration, etc.), with comprehensive test coverage in separate test projects. This structure supports multi-targeting and follows .NET library conventions.

## Phase 0: Outline & Research
1. **Extract unknowns from Technical Context** above:
   - For each NEEDS CLARIFICATION → research task
   - For each dependency → best practices task
   - For each integration → patterns task

2. **Generate and dispatch research agents**:
   ```
   For each unknown in Technical Context:
     Task: "Research {unknown} for {feature context}"
   For each technology choice:
     Task: "Find best practices for {tech} in {domain}"
   ```

3. **Consolidate findings** in `research.md` using format:
   - Decision: [what was chosen]
   - Rationale: [why chosen]
   - Alternatives considered: [what else evaluated]

**Output**: research.md with all NEEDS CLARIFICATION resolved

## Phase 1: Design & Contracts
*Prerequisites: research.md complete*

1. **Extract entities from feature spec** → `data-model.md`:
   - Entity name, fields, relationships
   - Validation rules from requirements
   - State transitions if applicable

2. **Generate API contracts** from functional requirements:
   - For each user action → endpoint
   - Use standard REST/GraphQL patterns
   - Output OpenAPI/GraphQL schema to `/contracts/`

3. **Generate contract tests** from contracts:
   - One test file per endpoint
   - Assert request/response schemas
   - Tests must fail (no implementation yet)

4. **Extract test scenarios** from user stories:
   - Each story → integration test scenario
   - Quickstart test = story validation steps

5. **Update agent file incrementally** (O(1) operation):
   - Run `.specify/scripts/powershell/update-agent-context.ps1 -AgentType copilot`
     **IMPORTANT**: Execute it exactly as specified above. Do not add or remove any arguments.
   - If exists: Add only NEW tech from current plan
   - Preserve manual additions between markers
   - Update recent changes (keep last 3)
   - Keep under 150 lines for token efficiency
   - Output to repository root

**Output**: data-model.md, /contracts/*, failing tests, quickstart.md, agent-specific file

## Phase 2: Task Planning Approach
*This section describes what the /tasks command will do - DO NOT execute during /plan*

**Task Generation Strategy**:
- Load `.specify/templates/tasks-template.md` as base
- Generate tasks from Phase 1 design docs (contracts, data model, quickstart)
- Core models (Request/Response DTOs) → creation tasks [P]
- Interface contracts → contract test tasks [P]
- Rate limiting and authentication → service implementation tasks
- Configuration and DI → extension method tasks [P]
- Error handling → exception type tasks [P]
- Integration scenarios from quickstart → integration test tasks
- NuGet packaging and documentation tasks

**Ordering Strategy**:
- TDD order: Contract tests → Models → Services → Integration tests
- Dependency order: Core models → Authentication → Rate limiting → Client → Extensions
- Mark [P] for parallel execution where dependencies allow
- Group related tasks for logical implementation flow

**Specific Task Categories**:
1. **Foundation Tasks**: Project setup, multi-targeting configuration, NuGet packaging
2. **Model Tasks**: Request/Response DTOs, configuration classes, supporting models [P]
3. **Contract Tasks**: Interface definitions and contract tests [P]
4. **Core Service Tasks**: Authentication provider, rate limit handler, HTTP client wrapper
5. **Integration Tasks**: DI extensions, configuration binding, fluent builder
6. **Testing Tasks**: Unit tests, integration tests, contract validation tests
7. **Documentation Tasks**: README, API docs, quickstart examples

**Estimated Output**: 35-40 numbered, ordered tasks in tasks.md with clear dependencies and parallelization markers

**IMPORTANT**: This phase is executed by the /tasks command, NOT by /plan

## Phase 3+: Future Implementation
*These phases are beyond the scope of the /plan command*

**Phase 3**: Task execution (/tasks command creates tasks.md)  
**Phase 4**: Implementation (execute tasks.md following constitutional principles)  
**Phase 5**: Validation (run tests, execute quickstart.md, performance validation)

## Complexity Tracking
*Fill ONLY if Constitution Check has violations that must be justified*

| Violation | Why Needed | Simpler Alternative Rejected Because |
|-----------|------------|-------------------------------------|
| [e.g., 4th project] | [current need] | [why 3 projects insufficient] |
| [e.g., Repository pattern] | [specific problem] | [why direct DB access insufficient] |


## Progress Tracking
*This checklist is updated during execution flow*

**Phase Status**:
- [x] Phase 0: Research complete (/plan command)
- [x] Phase 1: Design complete (/plan command)
- [x] Phase 2: Task planning complete (/plan command - describe approach only)
- [ ] Phase 3: Tasks generated (/tasks command)
- [ ] Phase 4: Implementation complete
- [ ] Phase 5: Validation passed

**Gate Status**:
- [x] Initial Constitution Check: PASS
- [x] Post-Design Constitution Check: PASS
- [x] All NEEDS CLARIFICATION resolved
- [x] Complexity deviations documented (none required)

---
*Based on Constitution v2.1.1 - See `/memory/constitution.md`*
