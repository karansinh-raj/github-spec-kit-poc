---
name: cqrs-scaffold
description: 'Scaffolds CQRS artifacts for the BookShelf project: commands, queries, handlers, FluentValidation validators, DTOs, unit tests, and minimal API endpoint registrations. Use when adding a new feature, creating a new operation, generating a handler, scaffolding a command or query, wiring up an endpoint, or adding tests for application layer handlers.'
argument-hint: '<Aggregate> <OperationName> <command|query> [--with-dto] [--with-test]'
---

# CQRS Scaffold

Generates all CQRS artifacts for a new operation in the BookShelf project, following the established Clean Architecture + MediatR patterns.

## When to Use

- "Add a new command to …"
- "Create a query for …"
- "Scaffold a handler for …"
- "Generate CQRS files for …"
- "Add an endpoint for …"
- "Write tests for a new handler"

## Project Conventions

| Concern | Convention |
|---|---|
| Namespace root | `BookShelf` |
| Command location | `src/BookShelf.Application/{Aggregate}/Commands/{OperationName}/` |
| Query location | `src/BookShelf.Application/{Aggregate}/Queries/{OperationName}/` |
| DTO location | `src/BookShelf.Application/{Aggregate}/DTOs/` |
| Endpoint file | `src/BookShelf.API/Endpoints/{Aggregate}Endpoints.cs` |
| Test location | `tests/BookShelf.Application.Tests/{Aggregate}/Commands/` or `…/Queries/` |
| Return type | Always `Result<T>` from `BookShelf.Application.Common.Models` |
| Mediator marker | Commands implement `IRequest<Result<T>>`, Queries implement `IRequest<Result<T>>` |
| Validator base | `AbstractValidator<TCommand>` (FluentValidation) |
| Test framework | xUnit + FluentAssertions + NSubstitute |
| Mock pattern | `Substitute.For<IRepository>()` |

## Step-by-Step Procedure

### 1. Collect Parameters

Ask for (or infer from context):
- **Aggregate** — e.g., `Books`, `ReadingLists`
- **OperationName** — e.g., `CreateBook`, `GetReadingListById`
- **Type** — `command` (mutates state) or `query` (reads state)
- **Return DTO** — which DTO the handler returns (e.g., `BookDto`)
- **Request fields** — list of fields and types for the command/query input
- **Validation rules** — business rules to enforce (required fields, length, etc.)
- **Repository methods** — which `IRepository` method(s) will be called

### 2. Generate Application Layer Files

**For a Command**, create three files. See [command template](./references/command-template.md).

**For a Query**, create two files. See [query template](./references/query-template.md).

If a new DTO or Request record is needed, add it to the `DTOs/` folder.

### 3. Wire Up the Minimal API Endpoint

Open `src/BookShelf.API/Endpoints/{Aggregate}Endpoints.cs` and add a new `.Map{Verb}(...)` call inside the existing `group`, following the pattern already present in the file. Use:
- `Results.Created(...)` for POST (201)
- `Results.Ok(...)` for GET / PUT (200)
- `Results.NoContent()` for DELETE (204)
- `Results.NotFound(...)` / `Results.Conflict(...)` for failures
- Wrap all responses in `ApiResponse<T>` from `BookShelf.Application.Common.Models`

### 4. Generate Unit Tests

Create a test class in `tests/BookShelf.Application.Tests/{Aggregate}/{Commands|Queries}/`. See [test template](./references/test-template.md).

Cover at minimum:
- Happy-path (returns `Result.Success`)
- Not-found / already-exists failure path (returns `Result.Failure`)
- Any domain-specific edge cases

### 5. Run Validation

After generating all files:
1. Check for compile errors using the errors tool.
2. Run tests to confirm the new handler tests pass.
3. Confirm no existing tests regress.

## Quick Reference: Key Types

```csharp
// Command record (immutable input)
public record {Name}Command({RequestType} Request) : IRequest<Result<{DtoType}>>;

// Query record (immutable input)
public record {Name}Query({ParamType} {Param}) : IRequest<Result<{DtoType}>>;

// Handler skeleton
public class {Name}Handler : IRequestHandler<{Name}Command, Result<{DtoType}>>
{
    private readonly I{Aggregate}Repository _repository;
    public {Name}Handler(I{Aggregate}Repository repository) => _repository = repository;
    public async Task<Result<{DtoType}>> Handle({Name}Command command, CancellationToken cancellationToken) { … }
}

// Validator skeleton
public class {Name}Validator : AbstractValidator<{Name}Command>
{
    public {Name}Validator()
    {
        RuleFor(x => x.Request.{Field}).NotEmpty().WithMessage("…");
    }
}
```

## Checklist

- [ ] Command or Query record created
- [ ] Handler created and injecting the correct repository interface
- [ ] Validator created (commands only)
- [ ] DTO / Request record added if new fields are needed
- [ ] Endpoint registered in `{Aggregate}Endpoints.cs`
- [ ] Unit test class created with happy-path + failure-path tests
- [ ] No compile errors
- [ ] All new tests pass
