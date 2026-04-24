# Query Template

Use these templates when generating a new query. Replace all `{…}` placeholders with actual values.

---

## 1. Query Record — `{OperationName}Query.cs`

```csharp
using BookShelf.Application.{Aggregate}.DTOs;
using BookShelf.Application.Common.Models;
using MediatR;

namespace BookShelf.Application.{Aggregate}.Queries.{OperationName};

public record {OperationName}Query({ParamType} {Param}) : IRequest<Result<{DtoType}>>;
```

> For paginated list queries, add paging/filter parameters directly on the record:
>
> ```csharp
> public record Get{Aggregate}sQuery(
>     int Page,
>     int PageSize,
>     string? FilterParam = null,
>     string SortBy = "name",
>     string SortOrder = "asc") : IRequest<Result<PagedResult<{DtoType}>>>;
> ```

---

## 2. Handler — `{OperationName}Handler.cs`

```csharp
using BookShelf.Application.{Aggregate}.DTOs;
using BookShelf.Application.Common.Interfaces;
using BookShelf.Application.Common.Models;
using MediatR;

namespace BookShelf.Application.{Aggregate}.Queries.{OperationName};

public class {OperationName}Handler : IRequestHandler<{OperationName}Query, Result<{DtoType}>>
{
    private readonly I{Aggregate}Repository _repository;

    public {OperationName}Handler(I{Aggregate}Repository repository)
    {
        _repository = repository;
    }

    public async Task<Result<{DtoType}>> Handle({OperationName}Query query, CancellationToken cancellationToken)
    {
        // 1. Fetch from repository using query parameters
        // 2. Return Result.Failure if not found
        // 3. Map to DTO and return Result.Success
        throw new NotImplementedException();
    }
}
```

### Common query handler patterns

| Scenario | Code |
|---|---|
| Get by ID | `var entity = await _repository.GetByIdAsync(query.Id, ct); if (entity is null) return Result<{DtoType}>.Failure("…not found.");` |
| Get all / paged | `var (items, total) = await _repository.GetAllAsync(query.Page, query.PageSize, ct); return Result<PagedResult<{DtoType}>>.Success(new PagedResult<{DtoType}>(items.Select(MapToDto), total, query.Page, query.PageSize));` |
| Filtered list | Pass filter params through to the repository method |

---

## 3. DTO Response (add to `DTOs/` if new)

```csharp
namespace BookShelf.Application.{Aggregate}.DTOs;

public record {Aggregate}Dto(
    int Id,
    string Field1,
    string Field2,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
```

---

## PagedResult helper (already exists at `Common/Models/PagedResult.cs`)

```csharp
// Already defined — do NOT duplicate
// BookShelf.Application.Common.Models.PagedResult<T>
```

Use `PagedResult<{DtoType}>` as the return type when the query returns a list with pagination metadata.
