# GitHub Spec Kit POC — Spec-Driven Development with .NET 10

A hands-on proof-of-concept demonstrating **GitHub Spec Kit** and **Spec-Driven Development (SDD)** using a .NET 10 Web API project.

---

## Tech Stack

- .NET 10 (C#)
- ASP.NET Core Minimal APIs
- GitHub Spec Kit (Spec-Driven Development workflow)
- Entity Framework Core (InMemory provider)
- xUnit + FluentAssertions + NSubstitute (testing)

---

## What is GitHub Spec Kit?

[GitHub Spec Kit](https://github.com/github/spec-kit) is an open-source toolkit that enables **Spec-Driven Development** — a structured process where specifications become the primary artifact, not throwaway documents. Instead of jumping straight into code ("vibe coding"), you define *what* you want to build through specifications, and AI coding agents generate working implementations from them.

> **Core Idea:** Specifications are executable — they directly generate working implementations rather than just guiding them.

---

## How Does Spec-Driven Development Work?

SDD follows a **6-phase workflow**, each with a dedicated slash command:

```
Constitution → Specify → Plan → Tasks → Implement
     ↓             ↓        ↓       ↓         ↓
  Principles    What to   How to  Actionable  Build
  & guidelines  build     build   task list   it!
```

### Phase-by-Phase Breakdown

| # | Phase | Command | What It Does |
|---|-------|---------|-------------|
| 1 | **Constitution** | `/speckit.constitution` | Establishes project governing principles — coding standards, testing requirements, architecture patterns, and guidelines that apply to ALL features |
| 2 | **Specify** | `/speckit.specify` | Defines *what* to build — user stories, acceptance criteria, requirements. Focus on the *what* and *why*, NOT the tech stack |
| 3 | **Plan** | `/speckit.plan` | Creates a technical implementation plan — architecture decisions, tech stack choices, data models, API contracts |
| 4 | **Tasks** | `/speckit.tasks` | Breaks the plan into actionable, ordered tasks with dependencies and acceptance criteria |
| 5 | **Implement** | `/speckit.implement` | Executes ALL tasks sequentially to build the feature according to the plan |

### Optional Enhancement Commands

| Command | When to Use | Purpose |
|---------|-------------|---------|
| `/speckit.clarify` | After `/speckit.specify`, before `/speckit.plan` | Agent asks YOU clarifying questions to fill gaps in the spec |
| `/speckit.analyze` | After `/speckit.tasks`, before `/speckit.implement` | Cross-artifact consistency & coverage analysis |
| `/speckit.checklist` | After `/speckit.plan` | Generate quality checklists to validate requirements |

---

## What This POC Demonstrates

### Feature 1: Books API (Greenfield)
Full SDD lifecycle from scratch — Clean Architecture, CRUD operations, search/filter, pagination, validation on a .NET 10 Minimal API.

### Feature 2: Reading Lists (Iterative/Brownfield)
Adding a second feature on top of existing code — demonstrates how SDD works for iterative development. Users can create reading lists, add/remove books, track read/unread status, and view stats.

---

## Prerequisites

| Tool | Version | Install Command |
|------|---------|----------------|
| Git | 2.x+ | [git-scm.com](https://git-scm.com/) |
| Python | 3.11+ | `winget install Python.Python.3.13` |
| uv | Latest | `irm https://astral.sh/uv/install.ps1 \| iex` |
| .NET SDK | 10.0 | [dot.net](https://dot.net/) |
| VS Code | Latest | With GitHub Copilot extension |

---

## Setup Steps

### 1. Install Spec Kit CLI

```powershell
# Install (pinned to stable release)
uv tool install specify-cli --from git+https://github.com/github/spec-kit.git@v0.5.0

# Verify
specify check
```

### 2. Initialize Project

```powershell
# Initialize Spec Kit in your project with GitHub Copilot + PowerShell
specify init . --ai copilot --script ps --force
```

This creates:
- `.github/prompts/` — Slash command prompt files for Copilot
- `.specify/templates/` — Spec artifact templates
- `.specify/scripts/` — Helper scripts
- `.specify/memory/` — Project memory files

### 3. Run the SDD Workflow

Use the prompts in the section below to reproduce the full workflow.

---

## Step-by-Step Prompts Guide

Every prompt used during this POC — follow these in order to reproduce the entire project from scratch.

### Phase 0: Setup

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

### Feature 1: Books API

#### Step 1 — Constitution (Project Principles)

> **Command:** `/speckit.constitution`

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

#### Step 2 — Specify (Define What to Build)

> **Command:** `/speckit.specify`

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

#### Step 3 — Clarify (Optional but Recommended)

> **Command:** `/speckit.clarify`

```
/speckit.clarify
```

*(No additional input needed — the agent reads the spec and asks YOU questions to fill in gaps)*

#### Step 4 — Plan (Technical Implementation)

> **Command:** `/speckit.plan`

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

#### Step 5 — Tasks (Generate Task Breakdown)

> **Command:** `/speckit.tasks`

```
/speckit.tasks
```

*(No additional input needed — tasks are generated from the plan)*

#### Step 6 — Analyze (Optional Consistency Check)

> **Command:** `/speckit.analyze`

```
/speckit.analyze
```

*(Validates consistency across constitution, spec, plan, and tasks)*

#### Step 7 — Implement (Build It!)

> **Command:** `/speckit.implement`

```
/speckit.implement
```

*(Executes all tasks from tasks.md to generate the working implementation)*

---

### Feature 2: Reading Lists

#### Step 1 — Specify (New Feature on Existing Codebase)

> **Command:** `/speckit.specify`

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

#### Step 2 — Plan

> **Command:** `/speckit.plan`

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

#### Step 3 — Tasks

> **Command:** `/speckit.tasks`

```
/speckit.tasks
```

#### Step 4 — Analyze

> **Command:** `/speckit.analyze`

```
/speckit.analyze
```

#### Step 5 — Implement

> **Command:** `/speckit.implement`

```
/speckit.implement
```

---

### Complete Workflow Summary

```
Feature 1 (Greenfield):
  /speckit.constitution → /speckit.specify → /speckit.clarify
    → /speckit.plan → /speckit.tasks → /speckit.analyze → /speckit.implement

Feature 2 (Iterative):
  /speckit.specify → /speckit.plan → /speckit.tasks → /speckit.analyze → /speckit.implement
```

> **Note:** The constitution is created **once per project**. All subsequent features inherit the same principles automatically.

### Tips for Using Spec Kit Effectively

1. **Be specific in `/speckit.specify`** — The more detail you provide about requirements and business rules, the better the output
2. **Use `/speckit.clarify`** — Let the AI ask you questions; it often catches edge cases you missed
3. **Review artifacts before proceeding** — Check `spec.md`, `plan.md`, and `tasks.md` before running `/speckit.implement`
4. **Don't skip `/speckit.analyze`** — It catches inconsistencies between your spec, plan, and tasks
5. **Iterate** — If the generated code isn't quite right, update the spec and re-run the workflow

---

## Project Structure (After Implementation)

```
github-spec-kit-poc/
├── .github/prompts/            # Spec Kit slash commands for Copilot
├── .specify/
│   ├── features/               # Generated spec artifacts per feature
│   │   ├── 001-books-api/
│   │   │   ├── spec.md         # What to build
│   │   │   ├── plan.md         # How to build it
│   │   │   └── tasks.md        # Actionable task list
│   │   └── 002-reading-lists/
│   │       ├── spec.md
│   │       ├── plan.md
│   │       └── tasks.md
│   ├── constitution.md         # Project principles
│   ├── templates/              # Artifact templates
│   └── scripts/                # Helper scripts
├── src/                        # .NET 10 API source code
├── tests/                      # Unit & integration tests
├── README.md                   # This file
└── PROMPTS.md                  # All prompts used (step-by-step guide)
```

---

## Key Takeaways for Developers

1. **Specs before code** — Define what you're building before writing a single line
2. **AI as implementer, not just assistant** — The AI doesn't just suggest code snippets; it implements entire features from specs
3. **Reproducible** — Anyone can follow the same specs and get consistent results
4. **Iterative** — Adding features follows the same disciplined workflow
5. **Auditable** — Every decision is documented in spec artifacts (spec.md, plan.md, tasks.md)

---

## Useful Links

- [Spec Kit Repository](https://github.com/github/spec-kit)
- [Spec-Driven Development Methodology](https://github.com/github/spec-kit/blob/main/spec-driven.md)
- [Community Walkthroughs](https://github.com/github/spec-kit#-community-walkthroughs)
- [Greenfield .NET CLI Demo](https://github.com/mnriem/spec-kit-dotnet-cli-demo)
