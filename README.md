# Carton Caps Referral API

This is the implementation for the Referral Feature challenge.
Built with **.NET 10**, using a Clean Architecture approach to keep the core logic isolated and testable.

## Getting Started

> **See [docs/API_SPEC.md](docs/API_SPEC.md) for the detailed API Specification (Deliverable #1).**

### Prerequisites
You'll need **.NET 10 SDK** and **Visual Studio 2026** (or VS Code).

To run the API:
```bash
dotnet run --project CartonCaps.Referrals.API
```
It usually starts on port 5172. You can check the Swagger UI at `/swagger`.

To run the tests:
```bash
dotnet test
```

## Architecture
I went with **Clean Architecture** (Core / Infrastructure / API) because it fits the senior requirement well—it makes it easy to test the business rules without worrying about the database or external APIs right away.

- **Core**: Holds the `Referral` entity and the repository interfaces.
- **Infrastructure**: The implementation details. currently using an In-Memory list for the database and a mock service for Deep Links.
- **API**: Standard Controllers.

## Notes & Assumptions
- **Auth**: The challenge requirements state that Authentication is already handled. I used a `MockUserId` constant ("user_123") to simulate a reliable authenticated context.
- **Rate Limiting**: Implemented a **24-hour rolling window** limit (max 5 referrals). This is more robust than a simple daily counter reset.
- **Validation**: Referral codes are validated against the in-memory store (case-insensitive) to ensure integrity before resolving.
- **Deep Links**: The `DeepLinkService` generates simulated deep links (`cartoncaps.mock.link`) to mimic providers like Branch.io.
- **Persistence**: Using a Singleton `ConcurrentBag` for thread-safe in-memory storage.
- **Testing**: Includes **15 comprehensive tests** covering happy paths, edge cases, rate limiting, and error scenarios.
