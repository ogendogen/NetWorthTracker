You are an expert in TypeScript, Angular, and scalable web application development. You write functional, maintainable, performant, and accessible code following Angular and TypeScript best practices.

## TypeScript Best Practices

- Use strict type checking
- Prefer type inference when the type is obvious
- Avoid the `any` type; use `unknown` when type is uncertain

## Angular Best Practices

- Always use standalone components over NgModules
- Must NOT set `standalone: true` inside Angular decorators. It's the default in Angular v20+.
- Use signals for state management
- Implement lazy loading for feature routes
- Do NOT use the `@HostBinding` and `@HostListener` decorators. Put host bindings inside the `host` object of the `@Component` or `@Directive` decorator instead
- Use `NgOptimizedImage` for all static images.
  - `NgOptimizedImage` does not work for inline base64 images.

## Accessibility Requirements

- It MUST pass all AXE checks.
- It MUST follow all WCAG AA minimums, including focus management, color contrast, and ARIA attributes.

### Components

- Keep components small and focused on a single responsibility
- Use `input()` and `output()` functions instead of decorators
- Use `computed()` for derived state
- Set `changeDetection: ChangeDetectionStrategy.OnPush` in `@Component` decorator
- Prefer inline templates for small components
- Prefer Reactive forms instead of Template-driven ones
- Do NOT use `ngClass`, use `class` bindings instead
- Do NOT use `ngStyle`, use `style` bindings instead
- When using external templates/styles, use paths relative to the component TS file.

## State Management

- Use signals for local component state
- Use `computed()` for derived state
- Keep state transformations pure and predictable
- Do NOT use `mutate` on signals, use `update` or `set` instead

## Templates

- Keep templates simple and avoid complex logic
- Use native control flow (`@if`, `@for`, `@switch`) instead of `*ngIf`, `*ngFor`, `*ngSwitch`
- Use the async pipe to handle observables
- Do not assume globals like (`new Date()`) are available.

## Services

- Design services around a single responsibility
- Use the `providedIn: 'root'` option for singleton services
- Use the `inject()` function instead of constructor injection

## NetWorthTracker Architecture

- The repository contains an ASP.NET Core API in `NetWorthTracker.Api/NetWorthTracker.Api`, a Domain project, an Application project, an Infrastructure project, and an Angular SPA in `NetWorthTracker.Spa`.
- Preserve the backend dependency direction: API composes Infrastructure and Application; Infrastructure implements Application/Domain contracts; Application depends on Domain; Domain remains framework- and infrastructure-independent.
- Keep domain models and repository interfaces in `NetWorthTracker.Api/NetWorthTracker.Domain`, CQRS requests/handlers and authentication services in `NetWorthTracker.Api/NetWorthTracker.Application`, and EF Core/PostgreSQL code, migrations, and repository implementations in `NetWorthTracker.Api/NetWorthTracker.Infrastructure`.
- Use MediatR for application use cases. Register handlers from `ApplicationAssemblyMarker` and place new commands, queries, and handlers in the Application project rather than controllers or Infrastructure.
- Use `IUserRepository` for user persistence and `ITokenService` for JWT creation; keep BCrypt/password persistence details inside Infrastructure's repository implementation.
- Run PostgreSQL with `docker compose -f devops/docker-compose.yml up -d postgres` before database-backed API work. Before starting the API, decrypt `appsettings.secrets.{Environment}.enc.json` into the corresponding local plaintext file. The API reads that file, then reapplies environment variables and command-line arguments as higher-precedence sources. Keep SOPS on `PATH`, age private keys outside the repository, and plaintext `appsettings.secrets.*.json` files uncommitted.
- Use `dotnet ef database update` from the Infrastructure project. `NetWorthTrackerDbContextFactory` reads the local `appsettings.secrets.Development.json` file and allows `ConnectionStrings__DefaultConnection` to override it, as documented in `.github/architecture.md`.
- Treat the SPA feature registry at `src/app/features/functionality.registry.ts` as the single source of truth for protected feature routes and left navigation entries. Do not duplicate a feature route separately in the side navigation.
- Implement each business capability as one lazy standalone page under `src/app/features/<feature-name>/`. Keep the application shell and layout components free of feature-specific business logic.
- `AppShellComponent` owns only the full-width top bar, left navigation/drawer, and routed content outlet. `TopBarComponent` emits logout and `SideNavigationComponent` renders registry entries.
- Keep API URLs centralized through `API_BASE_URL` and the environment files; never hard-code API URLs in a feature or service.
- Use `AuthService` for session lifecycle. It stores a validated access-token session in `sessionStorage` under `net-worth-tracker.session`; guards and the interceptor depend on this behavior.
- The functional `authInterceptor` attaches bearer tokens only to requests for `API_BASE_URL` and redirects to login after an API `401`.
- Login is database-backed through the initial PostgreSQL `users` table and BCrypt password verification, but registration is still a no-op scaffold. Do not present registration, development JWT settings, SOPS key distribution, or mock `/data` values as production-ready behavior.
- The API uses built-in ASP.NET Core logging only. Do not claim that this branch introduced structured, audit, or database logging.
- Refer to `.github/architecture.md` before changing backend layer boundaries, EF migrations, authentication, local development URLs, API routes, or the feature-shell structure.

## Workflow Rules

- Do not start, stop, or restart the API or SPA unless the user explicitly asks.
- Execute unit tests from the repository root using `dotnet run --project NetWorthTracker.Api/NetWorthTracker.UnitTests/NetWorthTracker.UnitTests.csproj --configuration Release --`.
- With Docker running, execute integration tests from the repository root using `dotnet run --project NetWorthTracker.Api/NetWorthTracker.IntegrationTests/NetWorthTracker.IntegrationTests/NetWorthTracker.IntegrationTests.csproj --configuration Release --`.
- Keep backend build, unit tests, integration tests, and the SPA build as separate GitHub Actions jobs so pull requests expose four required checks. Unit and integration jobs must depend on a successful backend build.
- Update `.github/copilot-instructions.md` and `.github/architecture.md` whenever a change affects project architecture, API contracts, local development, authentication, or established workflows.
