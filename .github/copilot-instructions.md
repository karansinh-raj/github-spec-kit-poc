# BookShelf — Copilot Instructions

## Project Overview
BookShelf is a .NET Web API for managing books and reading lists, built with Clean Architecture and CQRS.

## Folder Structure

### Solution: `BookShelf.slnx`

### Source (`src/`)
- **BookShelf.API** — ASP.NET Core Web API entry point
  - `Endpoints/` — Minimal API endpoint definitions (`BookEndpoints.cs`, `ReadingListEndpoints.cs`)
  - `Middleware/` — Global exception handler
  - `Properties/` — Launch settings
- **BookShelf.Application** — Application layer (CQRS with MediatR)
  - `Books/Commands/` — Book write operations
  - `Books/Queries/` — Book read operations
  - `Books/DTOs/` — Book data transfer objects
  - `ReadingLists/Commands/` — Reading list write operations
  - `ReadingLists/Queries/` — Reading list read operations
  - `ReadingLists/DTOs/` — Reading list data transfer objects
  - `Common/Behaviors/` — MediatR pipeline behaviors (e.g., validation)
  - `Common/Interfaces/` — Repository and service abstractions
  - `Common/Models/` — Shared models
- **BookShelf.Domain** — Core domain layer (no dependencies)
  - `Entities/` — `Book`, `ReadingList`, `ReadingListBook`
  - `Enums/` — `Genre`
- **BookShelf.Infrastructure** — Infrastructure layer (EF Core)
  - `Persistence/` — `BookShelfDbContext`, `BookRepository`, `ReadingListRepository`

### Tests (`tests/`)
- **BookShelf.Application.Tests** — Unit tests for the application layer
  - `Books/Commands/` — Tests for book commands
  - `Books/Queries/` — Tests for book queries
  - `ReadingLists/Commands/` — Tests for reading list commands
  - `ReadingLists/Queries/` — Tests for reading list queries

### Specs (`specs/`)
- `001-books-api/` — `spec.md`, `plan.md`, `tasks.md` for Books API feature
- `002-reading-lists/` — `spec.md`, `plan.md`, `tasks.md` for Reading Lists feature

## Architecture
- **Pattern**: Clean Architecture + CQRS
- **API style**: Minimal APIs (ASP.NET Core)
- **ORM**: Entity Framework Core
- **Mediator**: MediatR
