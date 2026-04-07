# Tasks: Reading Lists Feature

**Branch**: `002-reading-lists` | **Date**: 2026-04-07  
**Plan**: [plan.md](plan.md) | **Spec**: [spec.md](spec.md)

## Phase 1: Domain Layer

- [ ] **Task 1.1**: Create `ReadingList` entity in `src/BookShelf.Domain/Entities/ReadingList.cs`
- [ ] **Task 1.2**: Create `ReadingListBook` entity in `src/BookShelf.Domain/Entities/ReadingListBook.cs`
- [ ] **Task 1.3**: Add navigation property `ReadingListBooks` to `Book` entity

## Phase 2: Application Layer — Interfaces & DTOs

- [ ] **Task 2.1**: Create `IReadingListRepository` interface in `Common/Interfaces/`
- [ ] **Task 2.2**: Create `ReadingListDto` record
- [ ] **Task 2.3**: Create `ReadingListDetailDto` record
- [ ] **Task 2.4**: Create `ReadingListBookDto` record
- [ ] **Task 2.5**: Create `ReadingListStatsDto` record
- [ ] **Task 2.6**: Create `CreateReadingListRequest` record
- [ ] **Task 2.7**: Create `UpdateReadingListRequest` record
- [ ] **Task 2.8**: Create `UpdateReadStatusRequest` record

## Phase 3: Application Layer — Commands

- [ ] **Task 3.1**: Create `CreateReadingListCommand`, `CreateReadingListValidator`, `CreateReadingListHandler`
- [ ] **Task 3.2**: Create `UpdateReadingListCommand`, `UpdateReadingListValidator`, `UpdateReadingListHandler`
- [ ] **Task 3.3**: Create `DeleteReadingListCommand`, `DeleteReadingListHandler`
- [ ] **Task 3.4**: Create `AddBookToListCommand`, `AddBookToListHandler`
- [ ] **Task 3.5**: Create `RemoveBookFromListCommand`, `RemoveBookFromListHandler`
- [ ] **Task 3.6**: Create `UpdateReadStatusCommand`, `UpdateReadStatusHandler`

## Phase 4: Application Layer — Queries

- [ ] **Task 4.1**: Create `GetReadingListsQuery`, `GetReadingListsHandler`
- [ ] **Task 4.2**: Create `GetReadingListByIdQuery`, `GetReadingListByIdHandler`
- [ ] **Task 4.3**: Create `GetReadingListStatsQuery`, `GetReadingListStatsHandler`

## Phase 5: Infrastructure Layer

- [ ] **Task 5.1**: Add `ReadingLists` and `ReadingListBooks` DbSets + entity config to `BookShelfDbContext`
- [ ] **Task 5.2**: Create `ReadingListRepository` implementing `IReadingListRepository`
- [ ] **Task 5.3**: Register `IReadingListRepository` in `DependencyInjection.cs`

## Phase 6: API Layer

- [ ] **Task 6.1**: Create `ReadingListEndpoints.cs` with all 9 endpoint mappings
- [ ] **Task 6.2**: Register `MapReadingListEndpoints()` in `Program.cs`

## Phase 7: Unit Tests

- [ ] **Task 7.1**: Create `CreateReadingListHandlerTests`
- [ ] **Task 7.2**: Create `UpdateReadingListHandlerTests`
- [ ] **Task 7.3**: Create `DeleteReadingListHandlerTests`
- [ ] **Task 7.4**: Create `AddBookToListHandlerTests`
- [ ] **Task 7.5**: Create `RemoveBookFromListHandlerTests`
- [ ] **Task 7.6**: Create `UpdateReadStatusHandlerTests`
- [ ] **Task 7.7**: Create `GetReadingListsHandlerTests`
- [ ] **Task 7.8**: Create `GetReadingListByIdHandlerTests`
- [ ] **Task 7.9**: Create `GetReadingListStatsHandlerTests`

## Phase 8: Verify & Commit

- [ ] **Task 8.1**: Build entire solution
- [ ] **Task 8.2**: Run all tests (Feature 1 + Feature 2)
- [ ] **Task 8.3**: Commit all changes
