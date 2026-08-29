# NetWorthTracker

## Development Setup

These instructions are for Windows and assume a fresh clone of the repository.

### Prerequisites

Install the following tools:

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Node.js LTS](https://nodejs.org/)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/), running with Linux containers enabled
- [SOPS](https://github.com/getsops/sops/releases) and [age](https://github.com/FiloSottile/age/releases)

For SOPS and age, download the current Windows AMD64 release archives, extract the executables, and add their directory to your `PATH`. Confirm both tools are available in a new PowerShell window:

```powershell
sops --version
age --version
```

### Obtain the Development Key

Request the development age private key from an administrator through the approved secure channel. Do not commit, email, or copy the private key into this repository.

Create the SOPS age key directory and save the administrator-provided key as `%APPDATA%\sops\age\keys.txt`:

```powershell
New-Item -ItemType Directory -Force -Path "$env:APPDATA\sops\age"
notepad "$env:APPDATA\sops\age\keys.txt"
```

Paste the complete key contents, including the `# public key:` comment and `AGE-SECRET-KEY-...` line, save the file, and close the editor. Verify SOPS can use it:

```powershell
sops decrypt --output $null devops/postgres.enc.env
```

### Restore Local Configuration

From the repository root, decrypt the development secrets used by the API and PostgreSQL Compose service:

```powershell
sops decrypt --output NetWorthTracker.Api/NetWorthTracker.Api/appsettings.secrets.Development.json NetWorthTracker.Api/NetWorthTracker.Api/appsettings.secrets.Development.enc.json
sops decrypt --output devops/postgres.env devops/postgres.enc.env
```

The generated plaintext files are ignored by Git. Do not add them to commits.

### Install Dependencies and Start Services

Install the SPA dependencies:

```powershell
cd NetWorthTracker.Spa
npm install
cd ..
```

Start PostgreSQL and wait until Docker reports it healthy:

```powershell
docker compose --env-file devops/postgres.env -f devops/docker-compose.yml up -d postgres
docker compose --env-file devops/postgres.env -f devops/docker-compose.yml ps
```

Apply the database migration:

```powershell
cd NetWorthTracker.Api/NetWorthTracker.Infrastructure
dotnet ef database update
cd ../..
```

Start the API from the repository root:

```powershell
dotnet run --project NetWorthTracker.Api/NetWorthTracker.Api --launch-profile https
```

The API listens at `http://localhost:5062` and `https://localhost:7063`. In Development, the API reference is available at `https://localhost:7063/scalar/v1`.

In a separate terminal, start the Angular SPA:

```powershell
cd NetWorthTracker.Spa
npm start
```

Open `http://localhost:4200`.

### Updating Encrypted Development Files

After intentionally changing a local plaintext configuration file, re-encrypt it before committing the encrypted counterpart:

```powershell
sops encrypt --output NetWorthTracker.Api/NetWorthTracker.Api/appsettings.secrets.Development.enc.json NetWorthTracker.Api/NetWorthTracker.Api/appsettings.secrets.Development.json
sops encrypt --output devops/postgres.enc.env devops/postgres.env
```

Only the `.enc.json` and `.enc.env` files belong in source control. Production uses a separate age key supplied and managed outside this development workflow.
