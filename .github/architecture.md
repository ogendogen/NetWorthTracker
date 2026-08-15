# NetWorthTracker Architecture

## Repository Layout

| Area             | Location                                  | Responsibility                                                             |
| ---------------- | ----------------------------------------- | -------------------------------------------------------------------------- |
| API              | `NetWorthTracker.Api/NetWorthTracker.Api` | .NET 10 HTTP API, mock JWT authentication, and mock net-worth data.        |
| SPA              | `NetWorthTracker.Spa`                     | Angular 21 standalone SPA using Angular Material.                          |
| Project guidance | `.github/copilot-instructions.md`         | Required Angular, TypeScript, accessibility, and local architecture rules. |

## Local Development

Start the API yourself from the repository root:

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

JWT configuration lives under the `Jwt` section. Development uses the signing key in `appsettings.Development.json` for local scaffolding. Production must supply `SigningKey` through the `Jwt__SigningKey` environment variable; the API fails at startup when it is missing. For deployed environments, provide the variable through the platform's secure secret configuration and use a stable secret so API restarts do not invalidate existing tokens.

The API uses `StyleCop.Analyzers` as a private development-time analyzer dependency. Its API-local `.editorconfig` preserves the existing modern C# conventions: file-scoped namespaces, underscore-prefixed private fields, no mandatory file headers, no mandatory `this.` prefixes, and no required trailing commas. Keep StyleCop active for all other diagnostics.

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

This is an initial scaffold. The following are intentionally absent:

- Database, user persistence, password hashing, and registration workflow.
- Refresh tokens, password reset, authorization roles, and production secret management.
- Financial CRUD, historical persistence, and real dashboard calculations.
- Complete automated test coverage. `npm test` is available, but focused auth and feature tests were deferred.

## Validation Commands

Run from the indicated directories:

```bash
cd NetWorthTracker.Api/NetWorthTracker.Api
dotnet build
```

```bash
cd NetWorthTracker.Spa
npm run build
```

For browser validation, start both projects yourself, log in with `test` / `test`, verify the Dashboard summary loads, navigate each sidebar feature, and log out. The API package graph currently reports a transitive `Microsoft.OpenApi` vulnerability advisory; assess and update it separately from product changes.
