# envSafe

envSafe is a practical developer utility for generating environment variable values that are less likely to break common configuration formats and shell workflows.

It is **not** a secrets manager, vault, password manager, or account-based platform.

## What problem envSafe solves

Generated secrets often fail in real setups because of shell-sensitive symbols, YAML/JSON parsing quirks, or copy/paste hazards. envSafe focuses on safer-by-default value generation, clear formatting snippets, and transparent safety metadata.

## What envSafe does

- Generates 3 candidate values per request.
- Supports value types:
  - Password
  - API Key
  - Token
  - URL-safe string
  - Connection-string-safe value
- Supports generation modes:
  - Strict Safe
  - Balanced
  - Max Entropy
- Formats each candidate for:
  - Raw value
  - `.env`
  - Bash export
  - PowerShell
  - JSON property
  - YAML key/value
  - Docker compose env example
- Returns safety metadata:
  - Whether regeneration occurred
  - Which output formats required escaping
  - Brief explanation flags

## What envSafe does not do

- No account system
- No secret storage
- No database
- No telemetry
- No paid API dependencies

## Stack

- Backend: .NET 8 minimal API (`src/envSafe.Api`)
- Frontend: React + Vite + Tailwind CSS (`src/envSafe.Web`)
- Tests: xUnit + ASP.NET Core test host (`tests/envSafe.Api.Tests`)

## Repository layout

- `src/envSafe.Api`
- `src/envSafe.Web`
- `tests/envSafe.Api.Tests`
- `.github/workflows/ci.yml`
- `docker-compose.yml`

## Local run instructions

### 1) API

From repository root:

- `dotnet restore envSafe.sln`
- `dotnet run --project src/envSafe.Api/envSafe.Api.csproj --urls "http://0.0.0.0:5012"`

API endpoints:

- `GET /health`
- `POST /api/generate`

### 2) Web

In a second terminal:

- `cd src/envSafe.Web`
- `npm install`
- `npm run dev -- --host 0.0.0.0`

Vite defaults to port `5173`.

Set API URL if needed:

- `VITE_API_URL=http://localhost:5012`

## Docker run instructions

Run both services with Docker Compose:

- `docker compose up --build`

Then open:

- Web: `http://localhost:5173`
- API health: `http://localhost:5012/health`

## Test instructions

From repository root:

- `dotnet test envSafe.sln`
- `cd src/envSafe.Web && npm run lint && npm run build`

## CI

GitHub Actions workflow at `.github/workflows/ci.yml`:

- Restores/builds backend
- Runs backend tests
- Installs/builds frontend

## Screenshots

_Placeholder: add screenshots of the generator UI and candidate output cards._

## Future ideas

- Paste broken `.env` content and auto-fix risky lines
- Bulk variable generation for multiple keys
- Preset templates by use case (web app, worker, local dev)
- CLI version for terminal workflows

