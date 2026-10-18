# NetWorthTrackerSpa

This project was generated using [Angular CLI](https://github.com/angular/angular-cli) version 21.2.20.

## Development server

Run the following commands from `NetWorthTracker.Spa`.

### First-time HTTPS setup

Install [mkcert](https://github.com/FiloSottile/mkcert#installation) and make it available on your PATH. Each developer generates their own certificate; do not share certificates or private keys.

In Git Bash, install the local certificate authority and generate a localhost certificate:

```bash
mkcert -install
mkdir -p ssl/dev
mkcert -cert-file ssl/dev/localhost.pem -key-file ssl/dev/localhost-key.pem localhost 127.0.0.1 ::1
```

The development configuration in `angular.json` uses these certificate paths. Both generated files are ignored by Git. Never commit or share mkcert's `rootCA-key.pem`.

If Firefox reports `SEC_ERROR_UNKNOWN_ISSUER`, set `security.enterprise_roots.enabled` to `true` in `about:config`, then fully close and reopen Firefox. If needed, run `mkcert -CAROOT` and import that directory's `rootCA.pem` through Firefox's Settings > Privacy & Security > Certificates > View Certificates > Authorities > Import, enabling trust for identifying websites. Do not import `rootCA-key.pem`.

### Start the SPA

Install dependencies on first checkout, then start the development server:

```bash
npm install
npm start
```

Open `https://localhost:4200/`. The application automatically reloads when source files change. Restart the development server after changing its certificate configuration.

The API is started separately; see [local development instructions](../.github/architecture.md#local-development). If the development API URL uses HTTPS, also trust the separate ASP.NET Core certificate with `dotnet dev-certs https --trust`; mkcert setup does not trust that certificate.

## Code scaffolding

Angular CLI includes powerful code scaffolding tools. To generate a new component, run:

```bash
ng generate component component-name
```

For a complete list of available schematics (such as `components`, `directives`, or `pipes`), run:

```bash
ng generate --help
```

## Building

To build the project run:

```bash
ng build
```

This will compile your project and store the build artifacts in the `dist/` directory. By default, the production build optimizes your application for performance and speed.

## Running unit tests

To execute unit tests with the [Vitest](https://vitest.dev/) test runner, use the following command:

```bash
ng test
```

## Running end-to-end tests

For end-to-end (e2e) testing, run:

```bash
ng e2e
```

Angular CLI does not come with an end-to-end testing framework by default. You can choose one that suits your needs.

## Additional Resources

For more information on using the Angular CLI, including detailed command references, visit the [Angular CLI Overview and Command Reference](https://angular.dev/tools/cli) page.
