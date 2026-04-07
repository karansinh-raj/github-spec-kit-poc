# Implementation Plan: Books Management API

**Branch**: `001-books-api` | **Date**: 2026-04-07 | **Spec**: [spec.md](spec.md)
**Input**: Feature specification from `specs/001-books-api/spec.md`

## Summary

Build a Books Management API using .NET 10 Minimal APIs with Clean Architecture. The API supports full CRUD operations on books, with pagination, filtering, search, sorting, and input validation. Uses EF Core InMemory provider and MediatR for CQRS.

## Technical Context

**Language/Version**: C# 13 / .NET 10  
**Primary Dependencies**: MediatR, FluentValidation, Entity Framework Core (InMemory)  
**Storage**: EF Core InMemory provider  
**Testing**: xUnit, FluentAssertions, NSubstitute, Microsoft.AspNetCore.Mvc.Testing  
**Target Platform**: Cross-platform web API  
**Project Type**: Web API (Minimal APIs)  
**Performance Goals**: N/A (POC)  
**Constraints**: InMemory database, no auth, no caching  
**Scale/Scope**: Single-entity CRUD API for demonstration

## Constitution Check

| Principle | Status |
|-----------|--------|
| I. Clean Architecture | ✅ Four-layer structure planned |
| II. Minimal APIs | ✅ No controllers, MapGroup-based endpoints |
| III. SOLID & Clean Code | ✅ Records for DTOs, interfaces for abstractions |
| IV. Test-First | ✅ xUnit + FluentAssertions + NSubstitute |
| V. Consistent API Design | ✅ Envelope response, FluentValidation, Result pattern |
| VI. Simplicity | ✅ InMemory provider, no over-engineering |

## Project Structure

### Source Code

```text
src/
├── BookShelf.Domain/
│   ├── Entities/
│   │   └── Book.cs
│   └── Enums/
│       └── Genre.cs
├── BookShelf.Application/
│   ├── Common/
│   │   ├── Interfaces/
│   │   │   └── IBookRepository.cs
│   │   ├── Models/
│   │   │   ├── ApiResponse.cs
│   │   │   ├── PagedResult.cs
│   │   │   └── Result.cs
│   │   └── Behaviors/
│   │       └── ValidationBehavior.cs
│   ├── Books/
│   │   ├── Commands/
│   │   │   ├── CreateBook/
│   │   │   │   ├── CreateBookCommand.cs
│   │   │   │   ├── CreateBookHandler.cs
│   │   │   │   └── CreateBookValidator.cs
│   │   │   ├── UpdateBook/
│   │   │   │   ├── UpdateBookCommand.cs
│   │   │   │   ├── UpdateBookHandler.cs
│   │   │   │   └── UpdateBookValidator.cs
│   │   │   └── DeleteBook/
│   │   │       ├── DeleteBookCommand.cs
│   │   │       └── DeleteBookHandler.cs
│   │   ├── Queries/
│   │   │   ├── GetBookById/
│   │   │   │   ├── GetBookByIdQuery.cs
│   │   │   │   └── GetBookByIdHandler.cs
│   │   │   └── GetBooks/
│   │   │       ├── GetBooksQuery.cs
│   │   │       └── GetBooksHandler.cs
│   │   └── DTOs/
│   │       ├── BookDto.cs
│   │       ├── CreateBookRequest.cs
│   │       └── UpdateBookRequest.cs
│   └── DependencyInjection.cs
├── BookShelf.Infrastructure/
│   ├── Persistence/
│   │   ├── BookShelfDbContext.cs
│   │   └── BookRepository.cs
│   └── DependencyInjection.cs
└── BookShelf.API/
    ├── Endpoints/
    │   └── BookEndpoints.cs
    ├── Middleware/
    │   └── GlobalExceptionHandler.cs
    ├── Program.cs
    └── appsettings.json

tests/
└── BookShelf.Application.Tests/
    └── Books/
        ├── Commands/
        │   ├── CreateBookHandlerTests.cs
        │   ├── UpdateBookHandlerTests.cs
        │   └── DeleteBookHandlerTests.cs
        └── Queries/
            ├── GetBookByIdHandlerTests.cs
            └── GetBooksHandlerTests.cs
```

## Data Model

### Book Entity

| Field | Type | Constraints |
|-------|------|------------|
| Id | int | Primary key, auto-generated |
| Title | string | Required, max 200 chars |
| Author | string | Required, max 100 chars |
| ISBN | string | Required, unique, max 17 chars |
| PublishedDate | DateOnly | Required, must not be in the future |
| Genre | Genre (enum) | Required, from predefined list |
| Description | string? | Optional, max 2000 chars |
| CreatedAt | DateTime | Auto-set on creation |
| UpdatedAt | DateTime? | Auto-set on update |

### Genre Enum

Fiction, NonFiction, Science, Technology, History, Biography, Fantasy, Mystery, Romance, Other

## API Contracts

### Endpoints

| Method | Route | Description | Response |
|--------|-------|-------------|----------|
| POST | `/api/books` | Create a book | 201 + BookDto |
| GET | `/api/books/{id}` | Get book by ID | 200 + BookDto |
| GET | `/api/books` | List/search/filter books | 200 + PagedResult<BookDto> |
| PUT | `/api/books/{id}` | Update a book | 200 + BookDto |
| DELETE | `/api/books/{id}` | Delete a book | 204 |

### Response Envelope

```json
{
  "data": { ... },
  "errors": [ { "field": "Title", "message": "Title is required" } ],
  "meta": { "totalCount": 25, "totalPages": 3, "currentPage": 1, "pageSize": 10 }
}
```

### Query Parameters (GET /api/books)

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| page | int | 1 | Page number |
| pageSize | int | 10 | Items per page (max 50) |
| genre | string? | null | Filter by genre name |
| author | string? | null | Filter by author (exact match) |
| search | string? | null | Search by title (partial, case-insensitive) |
| sortBy | string | "title" | Sort field: "title" or "publishedDate" |
| sortOrder | string | "asc" | Sort direction: "asc" or "desc" |

## NuGet Packages

| Package | Project | Purpose |
|---------|---------|---------|
| MediatR | Application | CQRS command/query dispatching |
| FluentValidation | Application | Request validation |
| FluentValidation.DependencyInjectionExtensions | Application | Auto-register validators |
| Microsoft.EntityFrameworkCore | Infrastructure | ORM |
| Microsoft.EntityFrameworkCore.InMemory | Infrastructure | InMemory database |
| Swashbuckle.AspNetCore | API | Swagger/OpenAPI docs |
| xUnit | Tests | Test framework |
| FluentAssertions | Tests | Test assertions |
| NSubstitute | Tests | Mocking framework |
| Microsoft.AspNetCore.Mvc.Testing | Tests | Integration testing |
