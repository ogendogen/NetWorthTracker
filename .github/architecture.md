# NetWorthTracker Architecture

## Repository Layout

| Area              | Location                                                                                | Responsibility                                                                            |
| ----------------- | --------------------------------------------------------------------------------------- | ----------------------------------------------------------------------------------------- |
| API               | `NetWorthTracker.Api/NetWorthTracker.Api`                                               | .NET 10 HTTP API, composition root, controllers, JWT, and mock net-worth data.            |
| Application       | `NetWorthTracker.Api/NetWorthTracker.Application`                                       | CQRS/MediatR commands, handlers, application models, and authentication services.         |
| Domain            | `NetWorthTracker.Api/NetWorthTracker.Domain`                                            | Domain models and repository contracts; no infrastructure dependencies.                   |
| Infrastructure    | `NetWorthTracker.Api/NetWorthTracker.Infrastructure`                                    | EF Core/PostgreSQL persistence, migrations, DbContext, and repository implementations.    |
| Integration tests | `NetWorthTracker.Api/NetWorthTracker.IntegrationTests/NetWorthTracker.IntegrationTests` | Full-stack API tests using TUnit, `WebApplicationFactory`, and PostgreSQL Testcontainers. |
| SPA               | `NetWorthTracker.Spa`                                                                   | Angular 21 standalone SPA using Angular Material.                                         |
| Project guidance  | `.github/copilot-instructions.md`                                                       | Required Angular, TypeScript, accessibility, and local architecture rules.                |

## Local Development

Start PostgreSQL before running the API:

```bash
docker compose -f devops/docker-compose.yml up -d postgres
```

The Compose service exposes PostgreSQL on `localhost:5432` with database, user, and password `networthtracker`. The named `postgres-data` volume keeps data between container restarts.

Before running the API or EF tooling, decrypt the encrypted development secrets file from the repository root:

```powershell
sops decrypt NetWorthTracker.Api/NetWorthTracker.Api/appsettings.secrets.Development.enc.json `
  > NetWorthTracker.Api/NetWorthTracker.Api/appsettings.secrets.Development.json
```

Apply the checked-in EF Core migration from `NetWorthTracker.Api/NetWorthTracker.Infrastructure`:

```powershell
dotnet ef database update
```

`NetWorthTrackerDbContextFactory` is used by EF tooling. It walks up from the current directory to find the API project's configuration files, including the local `appsettings.secrets.Development.json`, then loads environment variables as overrides. Use `dotnet ef migrations add <MigrationName>` from the same directory when adding migrations.

### SOPS Configuration

The API keeps the encrypted source secrets in the committed environment-specific files `appsettings.secrets.Development.enc.json` and `appsettings.secrets.Production.enc.json`. Plaintext files matching `appsettings.secrets.*.json` are ignored by Git, copied only to local build output, and excluded from publish output. Decrypt an encrypted file into its corresponding plaintext file before starting the API:

```powershell
sops decrypt appsettings.secrets.Development.enc.json > appsettings.secrets.Development.json
```

At startup, the API selects and loads `appsettings.secrets.{Environment}.json`. Environment variables and command-line arguments are reapplied afterward and retain higher precedence. Startup fails when the plaintext file is unavailable. SOPS is required only when creating that local plaintext file, not while the API is running.

SOPS on Windows discovers age keys at `%APPDATA%\sops\age\keys.txt`. Keep private age keys outside the repository and make the required key available to every developer or deployment identity that decrypts secrets. Production should use a dedicated recipient and controlled key distribution rather than the development private key, and securely materialize the plaintext file before application startup.

Start the API from the repository root:

```bash
dotnet run --project NetWorthTracker.Api/NetWorthTracker.Api --launch-profile https
```

The profile listens on both `https://localhost:7063` and `http://localhost:5062`.

In Development, Scalar starts with the API and is available at `https://localhost:7063/scalar/v1`. The HTTPS launch profile opens this API reference automatically. Scalar reads the development OpenAPI document at `/openapi/v1.json`.

Start the SPA yourself:

```bash
cd NetWorthTracker.Spa
npm start
```

The SPA defaults to `http://localhost:4200`.

### Integration Tests

The integration suite starts the real API in-process and provisions a disposable PostgreSQL 18 container through Testcontainers. The shared test fixture applies the checked-in EF Core migrations and seeds one BCrypt-backed user before exercising login, JWT validation, and the protected data endpoint. It does not connect to or modify the persistent Compose development database.

Docker must be running. Execute the suite from the repository root:

```bash
dotnet run --project NetWorthTracker.Api/NetWorthTracker.IntegrationTests/NetWorthTracker.IntegrationTests/NetWorthTracker.IntegrationTests.csproj --configuration Release --
```

Testcontainers assigns a random host port and removes the database container after the TUnit session. Keep integration tests independent after fixture seeding so they can run in parallel against the session-scoped database.

### API URL and CORS

- Development SPA configuration is in `NetWorthTracker.Spa/src/environments/environment.ts`.
- It intentionally uses `http://localhost:5062`. The local ASP.NET development HTTPS certificate may be untrusted by a browser; a failed TLS handshake appears as a browser CORS failure with a null status and no response headers.
- In Development, the API does not redirect HTTP to HTTPS so CORS preflight requests can reach the CORS middleware.
- Outside Development, the API redirects HTTP to HTTPS.
- The named API CORS policy `SpaDevelopment` permits the Angular development origins on ports `4200` and `4201`, over HTTP and HTTPS. Add an explicit origin when using a different SPA port.
- For local HTTPS, trust the developer certificate with `dotnet dev-certs https --trust`, then change the development API URL only if HTTPS is deliberately required.

## API Contract

All API routes are root-level routes, with no `/api` prefix.

| Endpoint         | Auth       | Current behavior                                                                                                                   |
| ---------------- | ---------- | ---------------------------------------------------------------------------------------------------------------------------------- |
| `POST /login`    | Anonymous  | Accepts `{"username":"test","password":"test"}` only. Returns `accessToken`, `expiresAt`, and `userName`; otherwise returns `401`. |
| `POST /register` | Anonymous  | Accepts the typed registration request and returns `200 OK`. It does not persist a user or issue a session.                        |
| `GET /data`      | Bearer JWT | Returns mock net-worth summary data. Requests without a valid token return `401`.                                                  |

JWT configuration lives under the `Jwt` section. The signing key is loaded from the active environment's local plaintext secrets file and can be overridden through the `Jwt__SigningKey` environment variable. The API fails at startup when the resolved production signing key is empty. Use a stable production secret so API restarts do not invalidate existing tokens.

The API reads `ConnectionStrings:DefaultConnection` from the active local plaintext secrets file or the higher-precedence `ConnectionStrings__DefaultConnection` environment variable. Development points to the Compose PostgreSQL service at `localhost:5432`; deployed environments must provide an appropriate connection string. The API registers `NetWorthTrackerDbContext` with Npgsql, `IUserRepository` with `UserRepository`, and `ITokenService` with `TokenService` in the API composition root.

### Backend Architecture

The backend follows a dependency direction of API -> Infrastructure -> Application -> Domain:

- Domain contains the `User` model and `IUserRepository` contract.
- Application contains the login request/response models, `LoginCommand`, its `IRequestHandler`, JWT settings, and token service. MediatR scans the assembly marked by `ApplicationAssemblyMarker`; new handlers belong in this layer.
- Infrastructure contains EF Core configuration, `NetWorthTrackerDbContext`, the design-time factory, migrations, and the BCrypt-backed `UserRepository` implementation.
- API composes these services, configures authentication/database/MediatR, and exposes controllers.

The initial `users` table has a generated UUID primary key, required `Login` (maximum 32), `PasswordHash` (maximum 256), `Email` (maximum 320), `IsEmailConfirmed`, and `CreatedAt` columns. `Login` and `Email` each have unique indexes; UUIDs use `gen_random_uuid()` and `CreatedAt` uses `CURRENT_TIMESTAMP`.

`POST /login` is now database-backed: the repository looks up the login and verifies the supplied password with BCrypt before the application handler creates a JWT response. `POST /register` remains scaffolding only: it accepts a typed request but does not create a user. Do not describe registration as available until a registration command, validation, password hashing, and persistence are implemented.

The API currently uses the standard ASP.NET Core logging providers and the existing `Logging` configuration in `appsettings.json`. No Serilog, audit log, correlation ID, or dedicated logging table was introduced by this branch; add and document those separately if observability is required.

The backend solution centralizes `StyleCop.Analyzers` in `NetWorthTracker.Api/Directory.Build.props`, and the shared `NetWorthTracker.Api/.editorconfig` preserves the existing modern C# conventions: file-scoped namespaces, underscore-prefixed private fields, no mandatory file headers, no mandatory `this.` prefixes, and no required trailing commas. Keep StyleCop active for all other diagnostics.

The middleware order is intentional:

1. Development OpenAPI mapping.
2. Development Scalar API reference mapping.
3. HTTPS redirection outside Development.
4. CORS.
5. Authentication.
6. Authorization.
7. Controllers.

## SPA Structure

```text
src/app/
  core/
    auth/       Session service, guards, interceptor, auth contracts
    data/       Typed API service and net-worth data contract
  features/     Lazy business pages and feature registry
  layout/       App shell, top bar, side navigation
```

### Authentication Flow

1. A user submits the reactive login form.
2. `AuthService` calls `POST /login` and writes the successful response to `sessionStorage` as `net-worth-tracker.session`.
3. `AuthService` validates both `expiresAt` and the JWT `exp` claim when restoring or using a session.
4. `authGuard` protects the application shell and all child functionality routes; guests are sent to `/login` with a return URL.
5. `guestGuard` sends an authenticated user away from `/login` to `/dashboard`.
6. `authInterceptor` adds `Authorization: Bearer <access token>` only to requests beginning with `API_BASE_URL`.
7. An API `401` clears the session and sends the user to login.

Session storage is intentional for this scaffold: closing the browser session signs the user out. There are no refresh tokens or persistent login behavior yet.

### Feature and Layout Model

`features/functionality.registry.ts` defines every authenticated business feature with:

- `path`
- `label`
- Material icon name
- lazy standalone component loader

`app.routes.ts` derives the protected child routes from that registry. `SideNavigationComponent` receives the same registry and derives the left navigation. This prevents route and navigation drift.

To add a functionality:

1. Create one lazy standalone page component under `src/app/features/<feature-name>/`.
2. Add one typed entry to `functionality.registry.ts` with its path, label, icon, and lazy loader.
3. Add a dedicated typed API service or extend the relevant service only when the feature needs API data.

Do not add a feature directly to the app shell or manually duplicate it in `app.routes.ts` and side navigation.

The app shell follows the product draft: a full-width top bar above a fixed left navigation panel and routed content area. At widths below `760px`, the navigation becomes an overlaid Material drawer.

## UI and Accessibility

- Angular Material is the selected component library.
- Material Icons are loaded globally in `src/styles.scss`. Use named Material icons through `mat-icon`; do not leave icon ligatures unconfigured.
- The root app is only a `router-outlet`; feature content belongs in lazy pages.
- Components use `OnPush`, signals for local state, native Angular control flow, and reactive forms.
- Preserve accessible names for icon-only commands through `aria-label` and tooltips.

## Current Scope and Deliberate Gaps

This is an initial scaffold. The following are intentionally absent or incomplete:

- Registration workflow and user creation validation. Login persistence, BCrypt password verification, and the initial `users` migration are present.
- Structured application logging, audit logging, and correlation IDs.
- Refresh tokens, password reset, authorization roles, and production secret management.
- Managed production key distribution and rotation; deployment identities and production recipients still require operational configuration to decrypt and materialize secrets before startup.
- Financial CRUD, historical persistence, and real dashboard calculations.
- Complete automated test coverage. `npm test` is available, but focused auth and feature tests were deferred.

## GitHub Actions and Merge Protection

Pull requests run four separate jobs from `.github/workflows/dotnet-build.yml`: `Build .NET solution`, `Run .NET unit tests`, `Run .NET integration tests`, and `Build Angular SPA`. The unit and integration jobs depend on a successful backend build and then run in parallel; the SPA build remains independent. The integration job uses Testcontainers with the GitHub-hosted runner's Docker daemon. Keeping builds and test suites in separate jobs makes each result visible as its own pull-request check.

To require this check before merging, create a branch protection rule or repository ruleset for the default branch in GitHub and enable:

- Require a pull request before merging.
- Require status checks to pass before merging, then select `Build .NET solution`, `Run .NET unit tests`, `Run .NET integration tests`, and `Build Angular SPA`.
- Require branches to be up to date before merging.
- Include administrators when bypassing branch rules should not be allowed.

## Validation Commands

Run from the indicated directories:

```bash
cd NetWorthTracker.Api/NetWorthTracker.Api
dotnet build
```

```bash
dotnet run --project NetWorthTracker.Api/NetWorthTracker.UnitTests/NetWorthTracker.UnitTests.csproj --configuration Release --
dotnet run --project NetWorthTracker.Api/NetWorthTracker.IntegrationTests/NetWorthTracker.IntegrationTests/NetWorthTracker.IntegrationTests.csproj --configuration Release --
```

```bash
cd NetWorthTracker.Spa
npm run build
```

For browser validation, start both projects yourself, log in with `test` / `test`, verify the Dashboard summary loads, navigate each sidebar feature, and log out. The API package graph currently reports a transitive `Microsoft.OpenApi` vulnerability advisory; assess and update it separately from product changes.
