# Feature Specification: Books Management API

**Feature Branch**: `001-books-api`  
**Created**: 2026-04-07  
**Status**: Draft  
**Input**: User description: "Build a Books Management API with CRUD operations, search/filter, pagination, and validation"

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Create a New Book (Priority: P1)

As a user, I want to add a new book to the system so that I can track my book collection. I provide the book's title, author, ISBN, published date, genre, and an optional description.

**Why this priority**: Creating books is the foundational operation — without it, no other feature works.

**Independent Test**: Can be tested by sending a POST request with valid book data and verifying the book is persisted and returned with an ID.

**Acceptance Scenarios**:

1. **Given** valid book data with all required fields, **When** I POST to `/api/books`, **Then** the book is created and returned with a 201 status and a generated ID
2. **Given** book data with a duplicate ISBN, **When** I POST to `/api/books`, **Then** a 409 Conflict is returned with an error message
3. **Given** book data missing required fields (title or author), **When** I POST to `/api/books`, **Then** a 422 Validation Error is returned with field-level errors
4. **Given** book data with a future published date, **When** I POST to `/api/books`, **Then** a 422 Validation Error is returned
5. **Given** book data with an invalid genre, **When** I POST to `/api/books`, **Then** a 422 Validation Error is returned

---

### User Story 2 - Get a Book by ID (Priority: P1)

As a user, I want to retrieve a specific book by its ID so that I can view its full details.

**Why this priority**: Retrieving individual books is essential for any read operation and ties into all other features.

**Independent Test**: Create a book, then GET by its ID and verify all fields match.

**Acceptance Scenarios**:

1. **Given** a book exists with ID 1, **When** I GET `/api/books/1`, **Then** the book details are returned with a 200 status
2. **Given** no book exists with ID 999, **When** I GET `/api/books/999`, **Then** a 404 Not Found is returned

---

### User Story 3 - List Books with Pagination, Filtering, and Search (Priority: P1)

As a user, I want to browse all books with pagination, filter by genre or author, and search by title so that I can find books easily.

**Why this priority**: Listing and discovering books is the core read experience.

**Independent Test**: Seed multiple books, then verify pagination, filtering, and search return correct results.

**Acceptance Scenarios**:

1. **Given** 25 books exist, **When** I GET `/api/books?page=1&pageSize=10`, **Then** 10 books are returned with pagination metadata (totalCount=25, totalPages=3, currentPage=1)
2. **Given** books exist with genres Fiction and Science, **When** I GET `/api/books?genre=Fiction`, **Then** only Fiction books are returned
3. **Given** books exist by author "Asimov", **When** I GET `/api/books?author=Asimov`, **Then** only books by Asimov are returned
4. **Given** a book titled "Foundation", **When** I GET `/api/books?search=found`, **Then** the book is returned (case-insensitive partial match)
5. **Given** books exist, **When** I GET `/api/books?sortBy=title&sortOrder=desc`, **Then** books are returned sorted by title descending
6. **Given** no page parameters, **When** I GET `/api/books`, **Then** defaults to page=1, pageSize=10
7. **Given** pageSize=100 requested, **When** I GET `/api/books?pageSize=100`, **Then** pageSize is capped at 50

---

### User Story 4 - Update a Book (Priority: P2)

As a user, I want to update an existing book's details so that I can correct or enrich book information.

**Why this priority**: Important for data accuracy but not needed for initial browsing.

**Independent Test**: Create a book, update its title, then GET it and verify the title changed.

**Acceptance Scenarios**:

1. **Given** a book exists with ID 1, **When** I PUT `/api/books/1` with updated data, **Then** the book is updated and returned with a 200 status
2. **Given** no book exists with ID 999, **When** I PUT `/api/books/999`, **Then** a 404 Not Found is returned
3. **Given** a book exists, **When** I PUT with an ISBN that belongs to another book, **Then** a 409 Conflict is returned

---

### User Story 5 - Delete a Book (Priority: P2)

As a user, I want to delete a book so that I can remove books I no longer want to track.

**Why this priority**: Needed for complete CRUD but lower priority than read/create.

**Independent Test**: Create a book, delete it, then verify GET returns 404.

**Acceptance Scenarios**:

1. **Given** a book exists with ID 1, **When** I DELETE `/api/books/1`, **Then** a 204 No Content is returned and the book is removed
2. **Given** no book exists with ID 999, **When** I DELETE `/api/books/999`, **Then** a 404 Not Found is returned

---

### Edge Cases

- What happens when an empty string is provided for title or author? → 422 Validation Error
- What happens when ISBN format is invalid? → 422 with specific validation message
- How does the system handle concurrent updates to the same book? → Last write wins (no optimistic concurrency for POC)
- What happens when filtering by multiple criteria simultaneously (genre + author + search)? → All filters are combined with AND logic

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: System MUST allow creating a book with title, author, ISBN, published date, genre, and optional description
- **FR-002**: System MUST enforce unique ISBN constraint across all books
- **FR-003**: System MUST validate that title and author are non-empty strings
- **FR-004**: System MUST validate genre against a predefined list: Fiction, Non-Fiction, Science, Technology, History, Biography, Fantasy, Mystery, Romance, Other
- **FR-005**: System MUST validate that published date is not in the future
- **FR-006**: System MUST support retrieving a single book by its ID
- **FR-007**: System MUST support listing books with pagination (default page=1, pageSize=10, max pageSize=50)
- **FR-008**: System MUST support filtering books by genre and/or author
- **FR-009**: System MUST support searching books by title (case-insensitive partial match)
- **FR-010**: System MUST support sorting books by title or published date in ascending or descending order
- **FR-011**: System MUST allow updating all mutable fields of an existing book
- **FR-012**: System MUST allow deleting a book by its ID
- **FR-013**: System MUST return pagination metadata (totalCount, totalPages, currentPage, pageSize) with list responses

### Key Entities

- **Book**: Represents a book in the system. Key attributes: Id (int, auto-generated), Title (string, required), Author (string, required), ISBN (string, unique), PublishedDate (DateOnly), Genre (enum), Description (string, optional)

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: All 5 CRUD endpoints return correct HTTP status codes and response bodies
- **SC-002**: Pagination returns correct page metadata for any dataset size
- **SC-003**: Search finds books by partial title match regardless of case
- **SC-004**: All validation rules reject invalid input with descriptive error messages
- **SC-005**: Unit tests achieve >90% code coverage on Application layer logic

## Assumptions

- This is a single-user API (no authentication/authorization for POC)
- InMemory database is acceptable (data does not persist across restarts)
- No file uploads (book covers, etc.) — text data only
- No rate limiting or caching for POC
- API versioning is out of scope for v1
