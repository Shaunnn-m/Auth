# Authentication API

A learning project for understanding how authentication is built in a modern ASP.NET Core application.

The goal of this repository is to explore the pieces involved in authentication: users, password policies, email confirmation, JWT access tokens, refresh tokens, sessions, authorization, persistence, validation, rate limiting, logging, and integration testing.

This is educational code and is not presented as a production-ready identity provider.

## What This Project Includes

- User registration and account activation
- Email confirmation and confirmation-token handling
- Password hashing and configurable password rules
- JWT access tokens
- Refresh-token rotation and reuse detection
- Login, logout, and session management
- Password change and forgot-password request handling
- Role and permission foundations
- SQL Server persistence with Entity Framework Core
- Swagger/OpenAPI documentation
- Structured request logging with Serilog
- Global error handling with Problem Details
- Health checks
- Authentication rate limiting
- Integration tests

## Technology

- .NET 8
- ASP.NET Core Web API
- Entity Framework Core with SQL Server
- MediatR for application requests
- FluentValidation
- JWT bearer authentication
- Serilog
- xUnit integration tests
- Docker Compose

## Project Structure

The solution is split into four application projects and one test project:

| Project | Responsibility |
| --- | --- |
| `Authentication.Api` | HTTP controllers, configuration, middleware, Swagger, authentication, and application startup |
| `Authentication.Application` | Use cases, commands, queries, handlers, validation, and application interfaces |
| `Authentication.Domain` | Core entities, enums, account states, and domain rules |
| `Authentication.Infrastructure` | EF Core, SQL Server, repositories, unit of work, tokens, password hashing, email, and health checks |
| `Authentication.IntegrationTests` | End-to-end tests using the API host and configured test database |

## Prerequisites

Install the following before running the project:

- .NET 8 SDK
- Docker Desktop with Docker Compose
- SQL Server, if running the API or tests directly on the host
- Mailpit or another SMTP server listening on `localhost:1025` for email-related flows

Docker Compose starts SQL Server for the application, but it does not start an SMTP server. Mailpit can be used locally to inspect messages without sending real email.

## Run With Docker Compose

Docker Compose is the recommended way to start the complete application because it starts SQL Server, waits for the database health check, applies EF Core migrations, and then starts the API.

From the repository root:

```powershell
docker compose --env-file .env up --build
```

Compose uses these environment variables:

- `MSSQL_SA_PASSWORD` - SQL Server administrator password
- `JWT_SIGNING_KEY` - signing key used by the API to create and validate JWTs

The `.env` file contains local-development values. Replace them with your own values outside of local experimentation, and do not use development secrets in a deployed environment.

The services are available at:

- API: `http://localhost:8080`
- SQL Server: `localhost:1433`

The migration container runs the equivalent of:

```text
dotnet ef database update
  --project Authentication.Infrastructure/Authentication.Infrastructure.csproj
  --startup-project Authentication.Api/Authentication.Api.csproj
```

Stop the containers with:

```powershell
docker compose down
```

The SQL Server data is stored in the `sqlserver-data` Docker volume. To remove the database volume as well:

```powershell
docker compose down --volumes
```

## Run Locally

The solution file is located at `Authentication.Api/Authentication.Api.sln`.

From the repository root:

```powershell
dotnet restore Authentication.Api\Authentication.Api.sln
dotnet build Authentication.Api\Authentication.Api.sln
```

The default `appsettings.json` intentionally leaves the database connection string and JWT signing key empty. Configure them through environment variables or development configuration before starting the API:

```powershell
$env:ConnectionStrings__DefaultConnection = "Server=localhost,1433;Database=Authentication;User Id=sa;Password=<your-password>;TrustServerCertificate=True;"
$env:Jwt__Key = "<your-development-signing-key>"
```

The JWT issuer and audience default to `Authentication.Api`. The signing key must be provided because it is validated when the application starts.

Run the API using one of the launch profiles:

```powershell
cd Authentication.Api
dotnet run --launch-profile https
```

Configured local URLs are:

- HTTP: `http://localhost:5020`
- HTTPS: `https://localhost:7204`
- Swagger: `/swagger`
- Health check: `/health`

The configured application base URL defaults to `https://localhost:7001`. Override `Application__BaseUrl` if generated email links need to point to a different local URL.

When running locally, apply migrations if the database has not been created yet:

```powershell
dotnet ef database update \
  --project Authentication.Infrastructure\Authentication.Infrastructure.csproj \
  --startup-project Authentication.Api\Authentication.Api.csproj
```

The existing migrations are in [Authentication.Infrastructure/Migrations](Authentication.Infrastructure/Migrations).

## Authentication Flow

The main controller route is `/api/authentication`.

A typical local flow is:

1. Register a user with a password that is at least eight characters long and contains an uppercase letter, lowercase letter, and digit.
2. Activate the user with `POST /api/authentication/activate/{userId}`, or confirm the email token when using the email flow.
3. Log in with `POST /api/authentication/login`.
4. Store the returned access token and refresh token.
5. Send the access token as `Authorization: Bearer <access-token>` to protected endpoints.
6. Refresh the access token with `POST /api/authentication/refresh` when necessary.
7. Revoke individual sessions, revoke other sessions, or log out when a session should no longer be valid.

Refresh tokens are rotated. Reusing an old refresh token is treated as suspicious and invalidates the related token family.

### Endpoint Summary

| Method | Route | Authentication | Purpose |
| --- | --- | --- | --- |
| `POST` | `/api/authentication/register` | Anonymous | Create a pending-verification account |
| `POST` | `/api/authentication/activate/{userId}` | Anonymous | Activate an account for local/testing flows |
| `POST` | `/api/authentication/confirm-email` | Anonymous | Confirm an email token |
| `POST` | `/api/authentication/resend-email-confirmation` | Anonymous | Send another confirmation email |
| `POST` | `/api/authentication/login` | Anonymous | Authenticate and issue access/refresh tokens |
| `POST` | `/api/authentication/refresh` | Anonymous | Rotate a refresh token and issue new tokens |
| `POST` | `/api/authentication/logout` | Anonymous | Revoke a refresh-token session |
| `POST` | `/api/authentication/forgot-password` | Anonymous | Request a password-reset email |
| `GET` | `/api/authentication/getCurrentUser` | Bearer token | Read the current user |
| `GET` | `/api/authentication/sessions` | Bearer token | List active sessions |
| `DELETE` | `/api/authentication/sessions/{sessionId}` | Bearer token | Revoke one session |
| `POST` | `/api/authentication/sessions/revoke-all` | Bearer token | Revoke other sessions |
| `POST` | `/api/authentication/change-password` | Bearer token | Change the current password |

Swagger provides the request models and response shapes when the API is running in Development.

## Email Development

The default SMTP configuration points to:

- Host: `localhost`
- Port: `1025`
- TLS: disabled for local development

Start Mailpit separately, then inspect messages in its web interface. The project uses SMTP for registration confirmation and password-reset emails.

## Run Tests

Run the integration test project from the repository root:

```powershell
dotnet test Authentication.IntegrationTests\Authentication.IntegrationTests.csproj
```

The test host uses the `Test` environment and loads `Authentication.Api/appsettings.Test.json`. The current test configuration points to a developer-specific SQL Server Express instance and database:

```text
SHAUNNN_M\SQLEXPRESS / Authentication_Test
```

The test factory does not replace the database or automatically apply migrations. Before running tests, make sure the configured database exists, the schema is current, and an SMTP listener is available on `localhost:1025` for tests that send email.

The tests cover health checks, registration validation, duplicate registration, login failures, authentication rate limiting, refresh-token rotation and reuse detection, and session management.

## Architectural Decisions

### Clean Architecture Boundaries

The code is divided into API, Application, Domain, and Infrastructure projects so that business use cases are not tied directly to HTTP, SQL Server, or email providers.

The API composes the application and infrastructure layers. The Application layer depends on abstractions. Infrastructure implements those abstractions. The Domain layer contains the core entities and account state.

### CQRS With MediatR

Each authentication action is represented as a command or query with its own handler. MediatR keeps controllers thin and gives each use case a focused place for its behavior.

This makes the authentication flows easier to study independently and leaves room for separate read and write behavior later if the project grows.

### Validation Pipeline

FluentValidation validators run through a MediatR pipeline behavior. Validation is therefore applied consistently before handlers execute instead of being repeated inside every controller action.

### Domain-Owned Account State

The `User` entity owns account-related state such as roles and account status. Token entities are modeled separately so refresh tokens, email confirmation tokens, and password-reset tokens can have their own expiry and revocation behavior.

### Persistence Abstractions

EF Core and SQL Server are kept in Infrastructure. Repositories and a unit-of-work abstraction allow Application code to work with interfaces instead of directly depending on EF Core details.

### JWT Access Tokens and Refresh Sessions

Short-lived JWT access tokens are used for API authorization. Refresh tokens are persisted and associated with sessions so they can be rotated, revoked, listed, and invalidated after reuse.

This demonstrates the difference between stateless access-token validation and stateful session control.

### Password and Authentication Protection

Passwords are hashed rather than stored directly. Password requirements are configuration-driven. Authentication endpoints use a fixed-window rate limiter allowing ten requests per IP address per minute, reducing the impact of repeated login or registration attempts.

### Operational Middleware

The API includes Swagger for exploration, Serilog request logging for diagnostics, health checks for dependency visibility, global Problem Details error responses, and basic security headers. These choices make the learning project closer to the operational shape of a real service.

### Dockerized Database Migration

Compose separates database startup, migration, and API startup. The API waits for the migration container to complete successfully, so a fresh local database is prepared before requests are accepted.

## Current Limitations and Learning Notes

This repository intentionally reflects an ongoing learning project. Some areas still need further work before production use:

- The forgot-password handler creates a reset link, but the reset-password endpoint and feature are not currently implemented.
- The generated email confirmation link and the current confirmation API contract do not yet form a complete browser-based flow. The activation endpoint is useful for local testing.
- Integration tests use a machine-specific SQL Express connection and require local database setup.
- Compose does not start Mailpit or another SMTP server.
- Development secrets and the checked-in `.env` values must not be reused in production.
- There is no frontend application in this repository; Swagger or another HTTP client is used to exercise the API.

The unfinished areas are documented deliberately so the repository shows both what was learned and what would be the next engineering steps.

## Useful Files

- [Docker Compose configuration](docker-compose.yml)
- [Dockerfile](Dockerfile)
- [API startup and middleware](Authentication.Api/Program.cs)
- [Application configuration](Authentication.Api/appsettings.json)
- [Test configuration](Authentication.Api/appsettings.Test.json)
- [Authentication controller](Authentication.Api/Controllers/AuthenticationController.cs)
- [Application dependency injection](Authentication.Application/DependencyInjection.cs)
- [Infrastructure dependency injection](Authentication.Infrastructure/DependencyInjection.cs)
- [Domain user entity](Authentication.Domain/Entities/User.cs)
- [Integration tests](Authentication.IntegrationTests)
