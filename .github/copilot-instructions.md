# file: .github/copilot-instructions.md
# Repository-wide Copilot Custom Instructions

## Stack & conventions
- C#: .NET 8 Minimal APIs, EF Core (Npgsql), FluentValidation-like patterns without heavy deps, MediatR optional. Favor DI, typed HttpClient, async/await, `CancellationToken`.
- Python: FastAPI, Pydantic v2, `async` endpoints, avoid global state. Use dependency injection patterns via FastAPI `Depends`.
- Frontend: Angular standalone components, Reactive Forms, DRY via shared `ApiClient` and `Ui` components.

## Security defaults
- Validate all inputs (length, characters, enums). Reject or sanitize dangerous payloads.
- Parameterized queries only (EF Core default). No string concatenation for SQL.
- Secrets come from env vars or Key Vault; never hardcode.
- Enforce CORS allowlist; enable HTTPS redirection and HSTS in prod.
- Log with context but **never** secrets/PII.
- Add rate limiting on public routes.

## Code-quality defaults
- Small, pure functions; early returns; guard clauses.
- Strong typing; DTOs separate from entities; mapping kept explicit and minimal.
- Errors via Problem Details (RFC 7807) from middleware.
- Unit-testable composition; avoid static singletons.

## Python secure coding checklist
- Use `Annotated[str, MinLen(…)]` and Pydantic validators.
- `httpx.AsyncClient` with timeouts, retries; validate external responses.
- Content filtering: strip HTML, control length, basic PII redaction.

## Angular DRY guidance
- Centralize endpoints and HTTP in `ApiClient`.
- Shared `models.ts` for DTOs.
- Presentational components (dumb) + containers (smart) when complexity grows.
- `environment.ts` carries base URLs; never inline URLs.

## C# Design Pattern Review Prompt (use in Copilot Chat)
> You are a senior C# architect. Given this repo, review the API layer for appropriate use of patterns (Repository, Specification, Strategy for AI provider, Adapter for Python client, Decorator for validation/rate limiting). Identify over-engineering. Propose refactorings with code snippets that:
> 1) reduce coupling, 2) improve testability, 3) protect boundaries. Flag anti-patterns and threading pitfalls. Output a prioritized checklist.