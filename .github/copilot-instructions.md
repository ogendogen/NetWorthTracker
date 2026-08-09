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

- The repository contains an ASP.NET Core API in `NetWorthTracker.Api/NetWorthTracker.Api` and an Angular SPA in `NetWorthTracker.Spa`.
- Treat the SPA feature registry at `src/app/features/functionality.registry.ts` as the single source of truth for protected feature routes and left navigation entries. Do not duplicate a feature route separately in the side navigation.
- Implement each business capability as one lazy standalone page under `src/app/features/<feature-name>/`. Keep the application shell and layout components free of feature-specific business logic.
- `AppShellComponent` owns only the full-width top bar, left navigation/drawer, and routed content outlet. `TopBarComponent` emits logout and `SideNavigationComponent` renders registry entries.
- Keep API URLs centralized through `API_BASE_URL` and the environment files; never hard-code API URLs in a feature or service.
- Use `AuthService` for session lifecycle. It stores a validated access-token session in `sessionStorage` under `net-worth-tracker.session`; guards and the interceptor depend on this behavior.
- The functional `authInterceptor` attaches bearer tokens only to requests for `API_BASE_URL` and redirects to login after an API `401`.
- The current API contract is mock-only. Do not present the `test` / `test` credentials, no-op registration, committed development signing key, or mock `/data` values as production-ready behavior.
- Refer to `.github/architecture.md` before changing authentication, local development URLs, API routes, or the feature-shell structure.
