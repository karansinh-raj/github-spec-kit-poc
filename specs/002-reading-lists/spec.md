# Feature Specification: Reading Lists

**Feature Branch**: `002-reading-lists`  
**Created**: 2026-04-07  
**Status**: Draft  
**Input**: User description: "Build a Reading Lists feature that extends the existing Books API with reading list management, book tracking, and reading statistics"

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Create and Manage Reading Lists (Priority: P1)

As a user, I want to create named reading lists so that I can organize books into collections (e.g., "Summer 2026", "Must Read Classics"). I can also update the list name/description and delete lists I no longer need.

**Why this priority**: Without reading lists, no other feature in this spec works.

**Independent Test**: POST a new reading list, verify it's created. PUT to update name, verify change. DELETE, verify 404 on re-fetch.

**Acceptance Scenarios**:

1. **Given** valid data with a name and optional description, **When** I POST to `/api/reading-lists`, **Then** the reading list is created with a 201 status
2. **Given** a reading list name that already exists, **When** I POST to `/api/reading-lists`, **Then** a 409 Conflict is returned
3. **Given** an existing reading list, **When** I PUT `/api/reading-lists/{id}` with a new name, **Then** the list is updated and returned with 200
4. **Given** an existing reading list, **When** I DELETE `/api/reading-lists/{id}`, **Then** a 204 is returned and the books themselves are NOT deleted
5. **Given** no reading list with that ID, **When** I GET/PUT/DELETE `/api/reading-lists/{id}`, **Then** a 404 is returned

---

### User Story 2 - Browse Reading Lists (Priority: P1)

As a user, I want to view all my reading lists with a summary (book count, progress percentage) so I can see my reading progress at a glance.

**Why this priority**: Core read experience for the feature.

**Independent Test**: Create multiple reading lists, add books, mark some as read, then GET all and verify counts and percentages.

**Acceptance Scenarios**:

1. **Given** reading lists exist, **When** I GET `/api/reading-lists`, **Then** all lists are returned with bookCount and completionPercentage
2. **Given** a reading list with 4 books (2 read), **When** I GET `/api/reading-lists`, **Then** completionPercentage is 50
3. **Given** a specific reading list ID, **When** I GET `/api/reading-lists/{id}`, **Then** the full list is returned including all books with their read/unread status

---

### User Story 3 - Add and Remove Books from Reading Lists (Priority: P1)

As a user, I want to add existing books to my reading lists and remove them when I change my mind.

**Why this priority**: The core interaction between Books and Reading Lists.

**Independent Test**: Create a reading list, add a book, verify it appears. Remove the book, verify it's gone.

**Acceptance Scenarios**:

1. **Given** a reading list and a book both exist, **When** I POST `/api/reading-lists/{listId}/books/{bookId}`, **Then** the book is added with 201 status and defaults to unread
2. **Given** a book is already in the reading list, **When** I POST to add it again, **Then** a 409 Conflict is returned
3. **Given** a book is in a reading list, **When** I DELETE `/api/reading-lists/{listId}/books/{bookId}`, **Then** the book is removed from the list (not from the system) with 204
4. **Given** a non-existent book or reading list, **When** I POST to add, **Then** a 404 is returned

---

### User Story 4 - Track Reading Progress (Priority: P2)

As a user, I want to mark books as read or unread within a reading list, optionally adding notes and a completion date, so I can track my reading journey.

**Why this priority**: Important for the reading experience but the feature works without it initially.

**Independent Test**: Add a book to a list, mark as read with notes, verify the status. Mark as unread again, verify reset.

**Acceptance Scenarios**:

1. **Given** a book is in a reading list, **When** I PUT `/api/reading-lists/{listId}/books/{bookId}/status` with `{ isRead: true, notes: "Loved it!", completedDate: "2026-04-01" }`, **Then** the status is updated with 200
2. **Given** a book is marked as read, **When** I PUT with `{ isRead: false }`, **Then** the book is marked unread, notes and completedDate are cleared
3. **Given** a book is in two reading lists, **When** I mark it read in list A, **Then** it remains unread in list B (status is per-list)

---

### User Story 5 - Reading Statistics (Priority: P2)

As a user, I want to see statistics for a reading list so I can understand my reading habits.

**Why this priority**: Nice-to-have analytics layer, not critical for MVP.

**Independent Test**: Populate a reading list with books in various read states, call stats endpoint, verify all metrics.

**Acceptance Scenarios**:

1. **Given** a reading list with 10 books (6 read), **When** I GET `/api/reading-lists/{id}/stats`, **Then** I receive: totalBooks=10, booksRead=6, booksUnread=4, completionPercentage=60, mostRecentCompletion (the latest completedDate)
2. **Given** a reading list with no books, **When** I GET stats, **Then** totalBooks=0, completionPercentage=0, mostRecentCompletion=null

---

### Edge Cases

- What happens when deleting a reading list that has books? → Books remain in the system, only the reading list and its book associations are deleted
- Can a book be in multiple reading lists? → Yes, with independent read/unread status per list
- What happens when a book is deleted from the Books API that's in a reading list? → The reading list entry should be cascade-deleted (handled at database level)
- What if completedDate is in the future? → Allowed (user might plan when to finish)

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: System MUST allow creating a reading list with a unique name and optional description
- **FR-002**: System MUST allow updating a reading list's name and description
- **FR-003**: System MUST allow deleting a reading list without deleting the books
- **FR-004**: System MUST allow listing all reading lists with book count and completion percentage
- **FR-005**: System MUST allow getting a single reading list with all its books and their read/unread status
- **FR-006**: System MUST allow adding an existing book to a reading list (defaults to unread)
- **FR-007**: System MUST prevent adding the same book to the same reading list twice
- **FR-008**: System MUST allow removing a book from a reading list
- **FR-009**: System MUST allow marking a book as read/unread within a specific reading list
- **FR-010**: System MUST support per-reading-list read status (a book can be read in one list and unread in another)
- **FR-011**: System MUST allow setting optional reading notes and completion date when marking as read
- **FR-012**: System MUST provide reading statistics per reading list (total, read, unread, percentage, most recent completion)
- **FR-013**: System MUST cascade-delete reading list entries when a book is deleted

### Key Entities

- **ReadingList**: Id (int), Name (string, required, unique), Description (string, optional), CreatedAt, UpdatedAt
- **ReadingListBook**: Id (int), ReadingListId (int, FK), BookId (int, FK), IsRead (bool), Notes (string, optional), CompletedDate (DateOnly, optional), AddedAt (DateTime) — composite unique on (ReadingListId, BookId)

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: All reading list CRUD endpoints return correct HTTP status codes
- **SC-002**: Book count and completion percentage are accurate in list overview
- **SC-003**: Read/unread status is correctly isolated per reading list
- **SC-004**: Statistics endpoint returns accurate metrics
- **SC-005**: Unit tests cover all new command/query handlers

## Assumptions

- No authentication — all reading lists are shared/global for POC
- Reading list names are unique system-wide (not per-user)
- No pagination on reading lists or books within a list (small dataset for POC)
- Statistics are calculated on-the-fly (no caching or pre-aggregation)
