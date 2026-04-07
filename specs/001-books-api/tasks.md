# Tasks: Books Management API

**Input**: Design documents from `specs/001-books-api/`
**Prerequisites**: plan.md (required), spec.md (required)

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to (e.g., US1, US2, US3)

## Phase 1: Setup (Project Infrastructure)

**Purpose**: Create the .NET solution structure with all projects and dependencies

- [x] T001 Create .NET 10 solution file `BookShelf.sln` at repository root
- [x] T002 Create `src/BookShelf.Domain/` class library project targeting net10.0
- [x] T003 [P] Create `src/BookShelf.Application/` class library project targeting net10.0
- [x] T004 [P] Create `src/BookShelf.Infrastructure/` class library project targeting net10.0
- [x] T005 Create `src/BookShelf.API/` web project targeting net10.0
- [x] T006 Create `tests/BookShelf.Application.Tests/` xUnit test project targeting net10.0
- [x] T007 Add project references: API → Infrastructure → Application → Domain; Tests → Application
- [x] T008 Install NuGet packages per plan.md (MediatR, FluentValidation, EF Core InMemory, Swashbuckle, xUnit, FluentAssertions, NSubstitute)

---

## Phase 2: Domain Layer (Foundational)

**Purpose**: Define core entities and enums — no external dependencies

- [x] T009 [P] Create `Genre` enum in `src/BookShelf.Domain/Enums/Genre.cs` with values: Fiction, NonFiction, Science, Technology, History, Biography, Fantasy, Mystery, Romance, Other
- [x] T010 Create `Book` entity in `src/BookShelf.Domain/Entities/Book.cs` with properties: Id, Title, Author, ISBN, PublishedDate, Genre, Description, CreatedAt, UpdatedAt

---

## Phase 3: Application Layer (Core Logic)

**Purpose**: Commands, queries, DTOs, interfaces, validators, and the Result pattern

### Common Infrastructure

- [x] T011 Create `Result<T>` class in `src/BookShelf.Application/Common/Models/Result.cs` with Success/Failure factory methods and Error list
- [x] T012 [P] Create `ApiResponse<T>` record in `src/BookShelf.Application/Common/Models/ApiResponse.cs` with Data, Errors, and Meta properties
- [x] T013 [P] Create `PagedResult<T>` record in `src/BookShelf.Application/Common/Models/PagedResult.cs` with Items, TotalCount, TotalPages, CurrentPage, PageSize
- [x] T014 Create `IBookRepository` interface in `src/BookShelf.Application/Common/Interfaces/IBookRepository.cs` with GetByIdAsync, GetAllAsync (paged/filtered), AddAsync, UpdateAsync, DeleteAsync, ExistsByIsbnAsync
- [x] T015 Create `ValidationBehavior<TRequest, TResponse>` MediatR pipeline behavior in `src/BookShelf.Application/Common/Behaviors/ValidationBehavior.cs`

### DTOs

- [x] T016 [P] Create `BookDto` record in `src/BookShelf.Application/Books/DTOs/BookDto.cs`
- [x] T017 [P] Create `CreateBookRequest` record in `src/BookShelf.Application/Books/DTOs/CreateBookRequest.cs`
- [x] T018 [P] Create `UpdateBookRequest` record in `src/BookShelf.Application/Books/DTOs/UpdateBookRequest.cs`

### US1 — Create Book Command

- [x] T019 Create `CreateBookCommand` record in `src/BookShelf.Application/Books/Commands/CreateBook/CreateBookCommand.cs`
- [x] T020 Create `CreateBookValidator` in `src/BookShelf.Application/Books/Commands/CreateBook/CreateBookValidator.cs` (title required, author required, future date check, valid genre, ISBN format)
- [x] T021 Create `CreateBookHandler` in `src/BookShelf.Application/Books/Commands/CreateBook/CreateBookHandler.cs` (check duplicate ISBN, map to entity, save, return BookDto)

### US2 — Get Book By ID Query

- [x] T022 Create `GetBookByIdQuery` record in `src/BookShelf.Application/Books/Queries/GetBookById/GetBookByIdQuery.cs`
- [x] T023 Create `GetBookByIdHandler` in `src/BookShelf.Application/Books/Queries/GetBookById/GetBookByIdHandler.cs`

### US3 — Get Books Query (Paginated, Filtered, Searchable)

- [x] T024 Create `GetBooksQuery` record in `src/BookShelf.Application/Books/Queries/GetBooks/GetBooksQuery.cs` with Page, PageSize, Genre, Author, Search, SortBy, SortOrder
- [x] T025 Create `GetBooksHandler` in `src/BookShelf.Application/Books/Queries/GetBooks/GetBooksHandler.cs`

### US4 — Update Book Command

- [x] T026 Create `UpdateBookCommand` record in `src/BookShelf.Application/Books/Commands/UpdateBook/UpdateBookCommand.cs`
- [x] T027 Create `UpdateBookValidator` in `src/BookShelf.Application/Books/Commands/UpdateBook/UpdateBookValidator.cs`
- [x] T028 Create `UpdateBookHandler` in `src/BookShelf.Application/Books/Commands/UpdateBook/UpdateBookHandler.cs`

### US5 — Delete Book Command

- [x] T029 Create `DeleteBookCommand` record in `src/BookShelf.Application/Books/Commands/DeleteBook/DeleteBookCommand.cs`
- [x] T030 Create `DeleteBookHandler` in `src/BookShelf.Application/Books/Commands/DeleteBook/DeleteBookHandler.cs`

### DI Registration

- [x] T031 Create `DependencyInjection.cs` in `src/BookShelf.Application/` to register MediatR, FluentValidation validators, and ValidationBehavior

---

## Phase 4: Infrastructure Layer

**Purpose**: EF Core DbContext and repository implementation

- [x] T032 Create `BookShelfDbContext` in `src/BookShelf.Infrastructure/Persistence/BookShelfDbContext.cs` with DbSet<Book>, entity configuration (unique ISBN index, field constraints)
- [x] T033 Create `BookRepository` in `src/BookShelf.Infrastructure/Persistence/BookRepository.cs` implementing IBookRepository with EF Core queries (filtering, search, pagination, sorting)
- [x] T034 Create `DependencyInjection.cs` in `src/BookShelf.Infrastructure/` to register DbContext (InMemory) and BookRepository

---

## Phase 5: API Layer

**Purpose**: Minimal API endpoints, middleware, and program configuration

- [x] T035 Create `GlobalExceptionHandler` in `src/BookShelf.API/Middleware/GlobalExceptionHandler.cs` returning ApiResponse with error details
- [x] T036 Create `BookEndpoints` in `src/BookShelf.API/Endpoints/BookEndpoints.cs` with MapGroup("/api/books") and all 5 endpoints (POST, GET by ID, GET list, PUT, DELETE)
- [x] T037 Configure `Program.cs` in `src/BookShelf.API/` with DI registration, Swagger, global exception handler, and endpoint mapping

---

## Phase 6: Unit Tests

**Purpose**: Test all command/query handlers

- [x] T038 [P] Create `CreateBookHandlerTests` in `tests/BookShelf.Application.Tests/Books/Commands/CreateBookHandlerTests.cs`
- [x] T039 [P] Create `UpdateBookHandlerTests` in `tests/BookShelf.Application.Tests/Books/Commands/UpdateBookHandlerTests.cs`
- [x] T040 [P] Create `DeleteBookHandlerTests` in `tests/BookShelf.Application.Tests/Books/Commands/DeleteBookHandlerTests.cs`
- [x] T041 [P] Create `GetBookByIdHandlerTests` in `tests/BookShelf.Application.Tests/Books/Queries/GetBookByIdHandlerTests.cs`
- [x] T042 [P] Create `GetBooksHandlerTests` in `tests/BookShelf.Application.Tests/Books/Queries/GetBooksHandlerTests.cs`

---

## Dependencies & Execution Order

- **Phase 1** (Setup): No dependencies — start immediately
- **Phase 2** (Domain): Depends on Phase 1
- **Phase 3** (Application): Depends on Phase 2
- **Phase 4** (Infrastructure): Depends on Phase 3
- **Phase 5** (API): Depends on Phase 3 + Phase 4
- **Phase 6** (Tests): Depends on Phase 3 (Application layer only)
