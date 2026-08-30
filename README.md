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

SOPS can obtain an age X25519 private identity in these ways:

- **Default Windows key file (recommended for local development):** `%APPDATA%\sops\age\keys.txt`.
- **Explicit key file:** set `SOPS_AGE_KEY_FILE` to a file path. This is useful for CI or a separately managed production identity.
- **Process environment variable:** set `SOPS_AGE_KEY` to the complete identity. Avoid persisting this variable because process environments can be exposed through diagnostics and child processes.
- **Command output:** set `SOPS_AGE_KEY_CMD` to a command that writes the identity to standard output. The command can use `SOPS_AGE_RECIPIENT` to determine which recipient SOPS needs. Use this with a secret manager rather than storing a key in a file.

For files encrypted to SSH recipients, SOPS can instead read an SSH private key from `SOPS_AGE_SSH_PRIVATE_KEY_FILE` or `SOPS_AGE_SSH_PRIVATE_KEY_CMD`; it otherwise tries `~/.ssh/id_ed25519` and `~/.ssh/id_rsa`. This repository uses age X25519 recipients, so the SSH options do not apply to its current encrypted files.

For this development setup, create the default key directory and save the administrator-provided development identity as `%APPDATA%\sops\age\keys.txt`:

```powershell
New-Item -ItemType Directory -Force -Path "$env:APPDATA\sops\age"
notepad "$env:APPDATA\sops\age\keys.txt"
```

The key file may contain multiple identities, one `AGE-SECRET-KEY-...` line each; lines beginning with `#` are comments. Paste the complete administrator-provided identity, including its `# public key:` comment when supplied, save the file, and close the editor. Verify SOPS can use it:

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
