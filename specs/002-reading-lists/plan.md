# Implementation Plan: Reading Lists Feature

**Branch**: `002-reading-lists` | **Date**: 2026-04-07 | **Spec**: [spec.md](spec.md)
**Input**: Feature specification from `specs/002-reading-lists/spec.md`

## Summary

Extend the existing BookShelf API with Reading Lists management. Users can create named reading lists, add/remove books, track read/unread status per list, and view reading statistics. Builds on the existing Clean Architecture foundation from Feature 1.

## Technical Context

**Language/Version**: C# 13 / .NET 10
**Existing Foundation**: Books API (Feature 001) with Clean Architecture, MediatR, FluentValidation, EF Core InMemory
**New Dependencies**: None — all required packages already installed
**Storage**: EF Core InMemory provider (extends existing BookShelfDbContext)
**Testing**: xUnit, FluentAssertions, NSubstitute

## Constitution Check

| Principle | Status |
|-----------|--------|
| I. Clean Architecture | ✅ Extends existing four-layer structure |
| II. Minimal APIs | ✅ New MapGroup for reading-lists endpoints |
| III. SOLID & Clean Code | ✅ New repository interface, records for DTOs |
| IV. Test-First | ✅ Unit tests for all handlers |
| V. Consistent API Design | ✅ Same ApiResponse envelope, FluentValidation, Result pattern |
| VI. Simplicity | ✅ Follows existing patterns, no over-engineering |

## Project Structure (New/Modified Files)

```text
src/
├── BookShelf.Domain/
│   └── Entities/
│       ├── ReadingList.cs                    [NEW]
│       └── ReadingListBook.cs                [NEW]
├── BookShelf.Application/
│   ├── Common/
│   │   └── Interfaces/
│   │       └── IReadingListRepository.cs     [NEW]
│   └── ReadingLists/
│       ├── DTOs/
│       │   ├── ReadingListDto.cs             [NEW]
│       │   ├── ReadingListDetailDto.cs       [NEW]
│       │   ├── ReadingListBookDto.cs         [NEW]
│       │   ├── ReadingListStatsDto.cs        [NEW]
│       │   ├── CreateReadingListRequest.cs   [NEW]
│       │   ├── UpdateReadingListRequest.cs   [NEW]
│       │   └── UpdateReadStatusRequest.cs    [NEW]
│       ├── Commands/
│       │   ├── CreateReadingList/
│       │   │   ├── CreateReadingListCommand.cs     [NEW]
│       │   │   ├── CreateReadingListHandler.cs     [NEW]
│       │   │   └── CreateReadingListValidator.cs   [NEW]
│       │   ├── UpdateReadingList/
│       │   │   ├── UpdateReadingListCommand.cs     [NEW]
│       │   │   ├── UpdateReadingListHandler.cs     [NEW]
│       │   │   └── UpdateReadingListValidator.cs   [NEW]
│       │   ├── DeleteReadingList/
│       │   │   ├── DeleteReadingListCommand.cs     [NEW]
│       │   │   └── DeleteReadingListHandler.cs     [NEW]
│       │   ├── AddBookToList/
│       │   │   ├── AddBookToListCommand.cs         [NEW]
│       │   │   └── AddBookToListHandler.cs         [NEW]
│       │   ├── RemoveBookFromList/
│       │   │   ├── RemoveBookFromListCommand.cs    [NEW]
│       │   │   └── RemoveBookFromListHandler.cs    [NEW]
│       │   └── UpdateReadStatus/
│       │   │   ├── UpdateReadStatusCommand.cs      [NEW]
│       │   │   └── UpdateReadStatusHandler.cs      [NEW]
│       └── Queries/
│           ├── GetReadingLists/
│           │   ├── GetReadingListsQuery.cs         [NEW]
│           │   └── GetReadingListsHandler.cs       [NEW]
│           ├── GetReadingListById/
│           │   ├── GetReadingListByIdQuery.cs      [NEW]
│           │   └── GetReadingListByIdHandler.cs    [NEW]
│           └── GetReadingListStats/
│               ├── GetReadingListStatsQuery.cs     [NEW]
│               └── GetReadingListStatsHandler.cs   [NEW]
├── BookShelf.Infrastructure/
│   ├── Persistence/
│   │   ├── BookShelfDbContext.cs              [MODIFY - add new DbSets]
│   │   └── ReadingListRepository.cs          [NEW]
│   └── DependencyInjection.cs                [MODIFY - register new repo]
└── BookShelf.API/
    ├── Endpoints/
    │   └── ReadingListEndpoints.cs            [NEW]
    └── Program.cs                             [MODIFY - map new endpoints]

tests/
└── BookShelf.Application.Tests/
    └── ReadingLists/
        ├── Commands/
        │   ├── CreateReadingListHandlerTests.cs    [NEW]
        │   ├── UpdateReadingListHandlerTests.cs    [NEW]
        │   ├── DeleteReadingListHandlerTests.cs    [NEW]
        │   ├── AddBookToListHandlerTests.cs        [NEW]
        │   ├── RemoveBookFromListHandlerTests.cs   [NEW]
        │   └── UpdateReadStatusHandlerTests.cs     [NEW]
        └── Queries/
            ├── GetReadingListsHandlerTests.cs      [NEW]
            ├── GetReadingListByIdHandlerTests.cs   [NEW]
            └── GetReadingListStatsHandlerTests.cs  [NEW]
```

## Data Model

### ReadingList Entity

| Field | Type | Constraints |
|-------|------|------------|
| Id | int | Primary key, auto-generated |
| Name | string | Required, max 100 chars, unique |
| Description | string? | Optional, max 500 chars |
| CreatedAt | DateTime | Auto-set on creation |
| UpdatedAt | DateTime? | Auto-set on update |
| Books | ICollection\<ReadingListBook\> | Navigation property |

### ReadingListBook Entity (Join Table with Payload)

| Field | Type | Constraints |
|-------|------|------------|
| Id | int | Primary key, auto-generated |
| ReadingListId | int | FK → ReadingList, required |
| BookId | int | FK → Book, required |
| IsRead | bool | Default false |
| Notes | string? | Optional, max 2000 chars |
| CompletedDate | DateOnly? | Optional |
| AddedAt | DateTime | Auto-set on creation |
| ReadingList | ReadingList | Navigation property |
| Book | Book | Navigation property |

**Unique constraint**: (ReadingListId, BookId) — prevents duplicate entries.
**Cascade delete**: When a ReadingList is deleted, its ReadingListBook entries are cascade-deleted. When a Book is deleted, its ReadingListBook entries are cascade-deleted.

## API Contracts

### Endpoints

| Method | Route | Description | Response |
|--------|-------|-------------|----------|
| POST | `/api/reading-lists` | Create a reading list | 201 + ReadingListDto |
| GET | `/api/reading-lists` | Get all reading lists (with counts) | 200 + List\<ReadingListDto\> |
| GET | `/api/reading-lists/{id}` | Get reading list with books | 200 + ReadingListDetailDto |
| PUT | `/api/reading-lists/{id}` | Update reading list | 200 + ReadingListDto |
| DELETE | `/api/reading-lists/{id}` | Delete reading list | 204 |
| POST | `/api/reading-lists/{listId}/books/{bookId}` | Add book to list | 201 + ReadingListBookDto |
| DELETE | `/api/reading-lists/{listId}/books/{bookId}` | Remove book from list | 204 |
| PUT | `/api/reading-lists/{listId}/books/{bookId}/status` | Update read status | 200 + ReadingListBookDto |
| GET | `/api/reading-lists/{id}/stats` | Get reading statistics | 200 + ReadingListStatsDto |

### DTO Definitions

**ReadingListDto** (list overview):
```json
{ "id": 1, "name": "Summer 2026", "description": "...", "bookCount": 5, "completionPercentage": 60, "createdAt": "...", "updatedAt": null }
```

**ReadingListDetailDto** (single list with books):
```json
{ "id": 1, "name": "Summer 2026", "description": "...", "books": [ { "bookId": 1, "title": "...", "author": "...", "isRead": true, "notes": "Great!", "completedDate": "2026-04-01", "addedAt": "..." } ], "createdAt": "...", "updatedAt": null }
```

**ReadingListStatsDto** (statistics):
```json
{ "totalBooks": 10, "booksRead": 6, "booksUnread": 4, "completionPercentage": 60, "mostRecentCompletion": "2026-04-01" }
```

## Repository Interface

```csharp
public interface IReadingListRepository
{
    Task<ReadingList?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<ReadingList?> GetByIdWithBooksAsync(int id, CancellationToken ct = default);
    Task<List<ReadingList>> GetAllWithBooksAsync(CancellationToken ct = default);
    Task<ReadingList> AddAsync(ReadingList list, CancellationToken ct = default);
    Task UpdateAsync(ReadingList list, CancellationToken ct = default);
    Task DeleteAsync(ReadingList list, CancellationToken ct = default);
    Task<bool> ExistsByNameAsync(string name, int? excludeId = null, CancellationToken ct = default);
    Task<ReadingListBook?> GetReadingListBookAsync(int listId, int bookId, CancellationToken ct = default);
    Task<ReadingListBook> AddBookToListAsync(ReadingListBook entry, CancellationToken ct = default);
    Task RemoveBookFromListAsync(ReadingListBook entry, CancellationToken ct = default);
    Task UpdateReadingListBookAsync(ReadingListBook entry, CancellationToken ct = default);
}
```
