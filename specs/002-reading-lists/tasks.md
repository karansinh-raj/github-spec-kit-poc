# Tasks: Reading Lists Feature

**Branch**: `002-reading-lists` | **Date**: 2026-04-07  
**Plan**: [plan.md](plan.md) | **Spec**: [spec.md](spec.md)

## Phase 1: Domain Layer

- [x] **Task 1.1**: Create `ReadingList` entity in `src/BookShelf.Domain/Entities/ReadingList.cs`
- [x] **Task 1.2**: Create `ReadingListBook` entity in `src/BookShelf.Domain/Entities/ReadingListBook.cs`
- [x] **Task 1.3**: Add navigation property `ReadingListBooks` to `Book` entity

## Phase 2: Application Layer — Interfaces & DTOs

- [x] **Task 2.1**: Create `IReadingListRepository` interface in `Common/Interfaces/`
- [x] **Task 2.2**: Create `ReadingListDto` record
- [x] **Task 2.3**: Create `ReadingListDetailDto` record
- [x] **Task 2.4**: Create `ReadingListBookDto` record
- [x] **Task 2.5**: Create `ReadingListStatsDto` record
- [x] **Task 2.6**: Create `CreateReadingListRequest` record
- [x] **Task 2.7**: Create `UpdateReadingListRequest` record
- [x] **Task 2.8**: Create `UpdateReadStatusRequest` record

## Phase 3: Application Layer — Commands

- [x] **Task 3.1**: Create `CreateReadingListCommand`, `CreateReadingListValidator`, `CreateReadingListHandler`
- [x] **Task 3.2**: Create `UpdateReadingListCommand`, `UpdateReadingListValidator`, `UpdateReadingListHandler`
- [x] **Task 3.3**: Create `DeleteReadingListCommand`, `DeleteReadingListHandler`
- [x] **Task 3.4**: Create `AddBookToListCommand`, `AddBookToListHandler`
- [x] **Task 3.5**: Create `RemoveBookFromListCommand`, `RemoveBookFromListHandler`
- [x] **Task 3.6**: Create `UpdateReadStatusCommand`, `UpdateReadStatusHandler`

## Phase 4: Application Layer — Queries

- [x] **Task 4.1**: Create `GetReadingListsQuery`, `GetReadingListsHandler`
- [x] **Task 4.2**: Create `GetReadingListByIdQuery`, `GetReadingListByIdHandler`
- [x] **Task 4.3**: Create `GetReadingListStatsQuery`, `GetReadingListStatsHandler`

## Phase 5: Infrastructure Layer

- [x] **Task 5.1**: Add `ReadingLists` and `ReadingListBooks` DbSets + entity config to `BookShelfDbContext`
- [x] **Task 5.2**: Create `ReadingListRepository` implementing `IReadingListRepository`
- [x] **Task 5.3**: Register `IReadingListRepository` in `DependencyInjection.cs`

## Phase 6: API Layer

- [x] **Task 6.1**: Create `ReadingListEndpoints.cs` with all 9 endpoint mappings
- [x] **Task 6.2**: Register `MapReadingListEndpoints()` in `Program.cs`

## Phase 7: Unit Tests

- [x] **Task 7.1**: Create `CreateReadingListHandlerTests`
- [x] **Task 7.2**: Create `UpdateReadingListHandlerTests`
- [x] **Task 7.3**: Create `DeleteReadingListHandlerTests`
- [x] **Task 7.4**: Create `AddBookToListHandlerTests`
- [x] **Task 7.5**: Create `RemoveBookFromListHandlerTests`
- [x] **Task 7.6**: Create `UpdateReadStatusHandlerTests`
- [x] **Task 7.7**: Create `GetReadingListsHandlerTests`
- [x] **Task 7.8**: Create `GetReadingListByIdHandlerTests`
- [x] **Task 7.9**: Create `GetReadingListStatsHandlerTests`

## Phase 8: Verify & Commit

- [x] **Task 8.1**: Build entire solution
- [x] **Task 8.2**: Run all tests (Feature 1 + Feature 2)
- [x] **Task 8.3**: Commit all changes
