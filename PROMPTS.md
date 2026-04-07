# Prompts Used — Step-by-Step Guide

This document records **every prompt** used during the Spec-Driven Development workflow for this POC. Follow these steps in order to reproduce the entire project.

---

## Phase 0: Setup

```powershell
# 1. Install Python (if not installed)
winget install Python.Python.3.13

# 2. Install uv package manager
irm https://astral.sh/uv/install.ps1 | iex

# 3. Install Spec Kit CLI (pinned to v0.5.0)
uv tool install specify-cli --from git+https://github.com/github/spec-kit.git@v0.5.0

# 4. Initialize project with GitHub Copilot + PowerShell scripts
specify init . --ai copilot --script ps --force

# 5. Verify setup
specify check
```

---

## Feature 1: Books API

### Step 1 — Constitution (Project Principles)

> **Command:** `/speckit.constitution`

**Prompt used:**
```
/speckit.constitution

Create project principles for a .NET 10 Web API project with the following guidelines:

- Use Clean Architecture (Domain, Application, Infrastructure, API layers)
- Use Minimal APIs (not controllers)
- Use Entity Framework Core with InMemory provider for simplicity
- Follow SOLID principles and clean code practices
- Require unit tests for all business logic (xUnit + FluentAssertions)
- Use record types for DTOs
- Use Result pattern for error handling (no exceptions for flow control)
- API responses should follow a consistent envelope format
- Use FluentValidation for request validation
- All public API endpoints must have XML documentation
- Use nullable reference types throughout
```

### Step 2 — Specify (Define What to Build)

> **Command:** `/speckit.specify`

**Prompt used:**
```
/speckit.specify

Build a Books Management API that allows users to:

1. Create a new book with title, author, ISBN, published date, genre, and description
2. Get a book by its ID
3. Get all books with support for:
   - Pagination (page number + page size)
   - Filtering by genre and/or author
   - Searching by title (partial match, case-insensitive)
   - Sorting by title or published date (ascending/descending)
4. Update an existing book's details
5. Delete a book by its ID

Business rules:
- ISBN must be unique across all books
- Title and Author are required fields
- Genre must be from a predefined list (Fiction, Non-Fiction, Science, Technology, History, Biography, Fantasy, Mystery, Romance, Other)
- Published date cannot be in the future
- Page size for pagination defaults to 10, max 50
```

### Step 3 — Clarify (Optional but Recommended)

> **Command:** `/speckit.clarify`

**Prompt used:**
```
/speckit.clarify
```
*(No additional input needed — the agent reads the spec and asks YOU questions to fill in gaps)*

### Step 4 — Plan (Technical Implementation)

> **Command:** `/speckit.plan`

**Prompt used:**
```
/speckit.plan

Technical choices:
- .NET 10 with Minimal APIs
- Entity Framework Core with InMemory database provider
- Clean Architecture: src/Domain, src/Application, src/Infrastructure, src/API
- FluentValidation for input validation
- xUnit + FluentAssertions + NSubstitute for testing
- Use MediatR for CQRS pattern (commands and queries)
- Swagger/OpenAPI for API documentation
- Global exception handling middleware
- Use record types for all DTOs and value objects where appropriate
```

### Step 5 — Tasks (Generate Task Breakdown)

> **Command:** `/speckit.tasks`

**Prompt used:**
```
/speckit.tasks
```
*(No additional input needed — tasks are generated from the plan)*

### Step 6 — Analyze (Optional Consistency Check)

> **Command:** `/speckit.analyze`

**Prompt used:**
```
/speckit.analyze
```
*(Validates consistency across constitution, spec, plan, and tasks)*

### Step 7 — Implement (Build It!)

> **Command:** `/speckit.implement`

**Prompt used:**
```
/speckit.implement
```
*(Executes all tasks from tasks.md to generate the working implementation)*

---

## Feature 2: Reading Lists

### Step 1 — Specify (New Feature on Existing Codebase)

> **Command:** `/speckit.specify`

**Prompt used:**
```
/speckit.specify

Build a Reading Lists feature that extends the existing Books API:

1. Create a reading list with a name and optional description
2. Get a reading list by ID (includes all books in it with their read/unread status)
3. Get all reading lists for overview (with book count and progress percentage)
4. Update a reading list's name and description
5. Delete a reading list (does not delete the books themselves)
6. Add a book to a reading list
7. Remove a book from a reading list
8. Mark a book as read/unread within a reading list (with optional reading notes and completion date)
9. Get reading statistics for a reading list:
   - Total books
   - Books read vs unread
   - Completion percentage
   - Most recent book completed

Business rules:
- Reading list names must be unique
- A book can appear in multiple reading lists
- A book's read/unread status is per reading list (read in one list, unread in another)
- Cannot add the same book to the same reading list twice
- Deleting a reading list should not affect the books
```

### Step 2 — Plan

> **Command:** `/speckit.plan`

**Prompt used:**
```
/speckit.plan

Continue with the same technical architecture established in Feature 1:
- Add new domain entities (ReadingList, ReadingListBook) to the Domain layer
- Add new commands/queries via MediatR in the Application layer
- Extend the EF Core DbContext in Infrastructure
- Add new Minimal API endpoints in the API layer
- Follow the same patterns (Result, DTOs as records, FluentValidation)
- Add unit tests for all new business logic
```

### Step 3 — Tasks

> **Command:** `/speckit.tasks`

```
/speckit.tasks
```

### Step 4 — Analyze

> **Command:** `/speckit.analyze`

```
/speckit.analyze
```

### Step 5 — Implement

> **Command:** `/speckit.implement`

```
/speckit.implement
```

---

## Summary: The Complete Workflow

```
Feature 1 (Greenfield):
  /speckit.constitution → /speckit.specify → /speckit.clarify → /speckit.plan → /speckit.tasks → /speckit.analyze → /speckit.implement

Feature 2 (Iterative):
  /speckit.specify → /speckit.plan → /speckit.tasks → /speckit.analyze → /speckit.implement
```

> **Note:** The constitution is created ONCE per project. All subsequent features inherit the same principles.

---

## Tips for Using Spec Kit Effectively

1. **Be specific in `/speckit.specify`** — The more detail you provide about requirements and business rules, the better the output
2. **Use `/speckit.clarify`** — Let the AI ask you questions; it often catches edge cases you missed
3. **Review artifacts before proceeding** — Check spec.md, plan.md, and tasks.md before running `/speckit.implement`
4. **Don't skip `/speckit.analyze`** — It catches inconsistencies between your spec, plan, and tasks
5. **Iterate** — If the generated code isn't quite right, update the spec and re-run the workflow
