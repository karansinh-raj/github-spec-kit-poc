# GitHub Spec Kit POC — Spec-Driven Development with .NET 10

A hands-on proof-of-concept demonstrating **GitHub Spec Kit** and **Spec-Driven Development (SDD)** using a .NET 10 Web API project.

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

See [PROMPTS.md](PROMPTS.md) for the exact prompts used at each step.

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

---

## License

MIT
