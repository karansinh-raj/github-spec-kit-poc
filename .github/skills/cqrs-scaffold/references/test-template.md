# Test Template

Use these templates when generating xUnit unit tests for a new handler.  
All tests use **xUnit + FluentAssertions + NSubstitute** — the same stack as the rest of `BookShelf.Application.Tests`.

---

## 1. Command Handler Tests — `{OperationName}HandlerTests.cs`

```csharp
using BookShelf.Application.{Aggregate}.Commands.{OperationName};
using BookShelf.Application.{Aggregate}.DTOs;
using BookShelf.Application.Common.Interfaces;
using BookShelf.Domain.Entities;
using FluentAssertions;
using NSubstitute;

namespace BookShelf.Application.Tests.{Aggregate}.Commands;

public class {OperationName}HandlerTests
{
    private readonly I{Aggregate}Repository _repository;
    private readonly {OperationName}Handler _handler;

    public {OperationName}HandlerTests()
    {
        _repository = Substitute.For<I{Aggregate}Repository>();
        _handler = new {OperationName}Handler(_repository);
    }

    [Fact]
    public async Task Handle_ValidRequest_ReturnsSuccess()
    {
        // Arrange
        var request = new {RequestType}(/* fill required fields */);
        var command = new {OperationName}Command(request);

        _repository.{RelevantCheckMethod}(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(false);                                     // no duplicate / pre-condition passes

        _repository.AddAsync(Arg.Any<{EntityType}>(), Arg.Any<CancellationToken>())
            .Returns(ci =>
            {
                var entity = ci.Arg<{EntityType}>();
                entity.Id = 1;
                return entity;
            });

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Id.Should().Be(1);
    }

    [Fact]
    public async Task Handle_PreConditionFails_ReturnsFailure()
    {
        // Arrange — e.g., duplicate already exists
        var request = new {RequestType}(/* fill required fields */);
        var command = new {OperationName}Command(request);

        _repository.{RelevantCheckMethod}(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(true);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle()
            .Which.Should().Contain("/* expected error substring */");
    }
}
```

---

## 2. Query Handler Tests — `{OperationName}HandlerTests.cs`

```csharp
using BookShelf.Application.{Aggregate}.Queries.{OperationName};
using BookShelf.Application.{Aggregate}.DTOs;
using BookShelf.Application.Common.Interfaces;
using BookShelf.Domain.Entities;
using FluentAssertions;
using NSubstitute;

namespace BookShelf.Application.Tests.{Aggregate}.Queries;

public class {OperationName}HandlerTests
{
    private readonly I{Aggregate}Repository _repository;
    private readonly {OperationName}Handler _handler;

    public {OperationName}HandlerTests()
    {
        _repository = Substitute.For<I{Aggregate}Repository>();
        _handler = new {OperationName}Handler(_repository);
    }

    [Fact]
    public async Task Handle_ExistingId_ReturnsSuccessWithDto()
    {
        // Arrange
        var entity = new {EntityType} { Id = 42, /* fill key properties */ };
        _repository.GetByIdAsync(42, Arg.Any<CancellationToken>()).Returns(entity);

        var query = new {OperationName}Query(42);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.Id.Should().Be(42);
    }

    [Fact]
    public async Task Handle_NonExistentId_ReturnsFailure()
    {
        // Arrange
        _repository.GetByIdAsync(99, Arg.Any<CancellationToken>()).Returns((({EntityType}?)null));

        var query = new {OperationName}Query(99);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle()
            .Which.Should().Contain("not found");
    }
}
```

---

## Assertion Quick-Reference

| Intent | FluentAssertions code |
|---|---|
| Success result | `result.IsSuccess.Should().BeTrue()` |
| Failure result | `result.IsSuccess.Should().BeFalse()` |
| Specific error message | `result.Errors.Should().ContainSingle().Which.Should().Contain("…")` |
| DTO property value | `result.Value!.Title.Should().Be("…")` |
| Null check | `result.Value.Should().BeNull()` / `.NotBeNull()` |
| Repository called once | `await _repository.Received(1).AddAsync(Arg.Any<Book>(), Arg.Any<CancellationToken>())` |
| Repository NOT called | `await _repository.DidNotReceive().AddAsync(…)` |

---

## NSubstitute Return Patterns

```csharp
// Return a simple value
_repository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(myEntity);

// Return null (use explicit cast to avoid ambiguity)
_repository.GetByIdAsync(99, Arg.Any<CancellationToken>()).Returns((Book?)null);

// Mutate the argument and return it (simulate DB assigning an ID)
_repository.AddAsync(Arg.Any<Book>(), Arg.Any<CancellationToken>())
    .Returns(ci => { var b = ci.Arg<Book>(); b.Id = 1; return b; });
```
