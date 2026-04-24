# Command Template

Use these templates when generating a new command. Replace all `{…}` placeholders with actual values.

---

## 1. Command Record — `{OperationName}Command.cs`

```csharp
using BookShelf.Application.{Aggregate}.DTOs;
using BookShelf.Application.Common.Models;
using MediatR;

namespace BookShelf.Application.{Aggregate}.Commands.{OperationName};

public record {OperationName}Command({RequestType} Request) : IRequest<Result<{DtoType}>>;
```

> If the command has no request body (e.g., a delete by ID), use a primitive parameter directly:
>
> ```csharp
> public record {OperationName}Command(int Id) : IRequest<Result<{DtoType}>>;
> ```

---

## 2. Handler — `{OperationName}Handler.cs`

```csharp
using BookShelf.Application.{Aggregate}.DTOs;
using BookShelf.Application.Common.Interfaces;
using BookShelf.Application.Common.Models;
using BookShelf.Domain.Entities;
using MediatR;

namespace BookShelf.Application.{Aggregate}.Commands.{OperationName};

public class {OperationName}Handler : IRequestHandler<{OperationName}Command, Result<{DtoType}>>
{
    private readonly I{Aggregate}Repository _repository;

    public {OperationName}Handler(I{Aggregate}Repository repository)
    {
        _repository = repository;
    }

    public async Task<Result<{DtoType}>> Handle({OperationName}Command command, CancellationToken cancellationToken)
    {
        // 1. Validate business rules (uniqueness, existence, etc.)
        // 2. Map from request to domain entity
        // 3. Persist via repository
        // 4. Return Result.Success(MapToDto(entity))
        throw new NotImplementedException();
    }

    private static {DtoType} MapToDto({EntityType} entity) => new(/* map properties */);
}
```

### Common handler patterns

| Scenario | Code |
|---|---|
| Check duplicate | `if (await _repository.Exists…Async(…)) return Result<{DtoType}>.Failure("…already exists.");` |
| Check not found | `var entity = await _repository.GetByIdAsync(id, ct); if (entity is null) return Result<{DtoType}>.Failure("…not found.");` |
| Add entity | `var created = await _repository.AddAsync(entity, ct); return Result<{DtoType}>.Success(MapToDto(created));` |
| Update entity | `await _repository.UpdateAsync(entity, ct); return Result<{DtoType}>.Success(MapToDto(entity));` |
| Delete entity | `await _repository.DeleteAsync(entity, ct); return Result<bool>.Success(true);` |

---

## 3. Validator — `{OperationName}Validator.cs`

```csharp
using FluentValidation;

namespace BookShelf.Application.{Aggregate}.Commands.{OperationName};

public class {OperationName}Validator : AbstractValidator<{OperationName}Command>
{
    public {OperationName}Validator()
    {
        RuleFor(x => x.Request.{Field1})
            .NotEmpty().WithMessage("{Field1} is required.")
            .MaximumLength({MaxLength}).WithMessage("{Field1} must not exceed {MaxLength} characters.");

        // Add more rules as needed
    }
}
```

### FluentValidation quick-reference

| Rule | Example |
|---|---|
| Required | `.NotEmpty().WithMessage("…")` |
| Max length | `.MaximumLength(200).WithMessage("…")` |
| Positive number | `.GreaterThan(0).WithMessage("…")` |
| Valid enum | `.Must(v => Enum.TryParse<MyEnum>(v, true, out _)).WithMessage("…")` |
| Future date | `.Must(d => d > DateOnly.FromDateTime(DateTime.UtcNow)).WithMessage("…")` |
| Conditional | `.When(x => x.Request.{Field} != null)` |

---

## 4. Request DTO — `{OperationName}Request.cs` (add to `DTOs/` if new)

```csharp
namespace BookShelf.Application.{Aggregate}.DTOs;

public record {OperationName}Request(
    string Field1,
    int Field2,
    string? OptionalField = null);
```
