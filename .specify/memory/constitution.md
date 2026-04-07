<!--
Sync Impact Report:
- Version change: 0.0.0 → 1.0.0
- Added principles: Clean Architecture, Minimal APIs, SOLID & Clean Code, Test-First, Consistent API Design, Simplicity
- Added sections: Technology Stack, Development Workflow
- Templates requiring updates: ✅ spec-template.md (no changes needed) | ✅ plan-template.md (no changes needed) | ✅ tasks-template.md (no changes needed)
- Follow-up TODOs: none
-->

# BookShelf API Constitution

## Core Principles

### I. Clean Architecture (NON-NEGOTIABLE)

Every feature MUST follow Clean Architecture with four layers:
- **Domain**: Entities, value objects, enumerations — zero external dependencies
- **Application**: Use cases (commands/queries via MediatR), DTOs, interfaces, validators — depends only on Domain
- **Infrastructure**: EF Core DbContext, repository implementations, external services — implements Application interfaces
- **API**: Minimal API endpoint definitions, middleware, DI configuration — references Application and Infrastructure

Layer dependency rule: dependencies MUST point inward only (API → Infrastructure → Application → Domain). No layer may reference a layer above it.

### II. Minimal APIs

All HTTP endpoints MUST use .NET Minimal APIs (not MVC controllers). Endpoints MUST be organized in static extension method classes grouped by feature/entity (e.g., `BookEndpoints.cs`). Each endpoint class MUST use `MapGroup` for route prefixing.

### III. SOLID & Clean Code

- Single Responsibility: each class has one reason to change
- Open/Closed: extend via new implementations, not modifying existing code
- Liskov Substitution: derived types MUST be substitutable for their base types
- Interface Segregation: prefer small, focused interfaces
- Dependency Inversion: depend on abstractions, not concretions
- Use nullable reference types throughout
- Use `record` types for DTOs and value objects where immutability is appropriate

### IV. Test-First

Unit tests MUST exist for all business logic in the Application layer. Testing stack: xUnit + FluentAssertions + NSubstitute. Tests MUST be organized mirroring the source project structure. Integration tests SHOULD cover critical API endpoint flows using `WebApplicationFactory`.

### V. Consistent API Design

- All API responses MUST use a consistent envelope: `{ data, errors, meta }`
- Use FluentValidation for all request validation
- Return appropriate HTTP status codes: 200 (OK), 201 (Created), 204 (No Content), 400 (Bad Request), 404 (Not Found), 409 (Conflict), 422 (Validation Error)
- Use Result pattern for error handling — no exceptions for business logic flow control
- Support pagination via `page` and `pageSize` query parameters with sensible defaults

### VI. Simplicity (YAGNI)

Start simple. Do not add abstractions, patterns, or features until there is a concrete, immediate need. Prefer EF Core InMemory provider for this POC — no external database dependencies. Avoid over-engineering: if a simple method call works, do not introduce a pattern around it.

## Technology Stack

- **Runtime**: .NET 10
- **API Style**: Minimal APIs
- **ORM**: Entity Framework Core (InMemory provider)
- **Validation**: FluentValidation
- **CQRS**: MediatR
- **Testing**: xUnit, FluentAssertions, NSubstitute, Microsoft.AspNetCore.Mvc.Testing
- **Documentation**: Swagger / OpenAPI (Swashbuckle)
- **Language**: C# 13 with nullable reference types enabled

## Development Workflow

- Each feature MUST go through the Spec-Driven Development workflow: specify → plan → tasks → implement
- All code changes MUST compile with zero warnings (TreatWarningsAsErrors)
- All tests MUST pass before a feature is considered complete
- Feature branches follow the naming convention: `###-feature-name`

## Governance

This constitution supersedes all ad-hoc decisions. Any deviation MUST be documented with justification in the relevant plan.md. Amendments require updating version, ratification date, and a sync impact report.

**Version**: 1.0.0 | **Ratified**: 2026-04-07 | **Last Amended**: 2026-04-07
