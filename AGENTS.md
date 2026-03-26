# AGENTS.md

## Cursor Cloud specific instructions

### Overview

envSafe is a stateless developer utility (no database, no external services) with two components:

- **Backend API**: .NET 8 Minimal API at `src/envSafe.Api` (port 5012)
- **Frontend**: React + Vite + Tailwind CSS SPA at `src/envSafe.Web` (port 5173)
- **Tests**: xUnit at `tests/envSafe.Api.Tests`

### Prerequisites

- **.NET 8 SDK** — installed at `$HOME/.dotnet`. The `DOTNET_ROOT` and `PATH` are configured in `~/.bashrc`.
- **Node.js 22** — pre-installed via nvm.

### Common commands

See `README.md` for full local run and test instructions. Quick reference:

| Task | Command | Working directory |
|---|---|---|
| Restore .NET deps | `dotnet restore envSafe.sln` | `/workspace` |
| Install frontend deps | `npm install` | `src/envSafe.Web` |
| Run backend tests | `dotnet test envSafe.sln` | `/workspace` |
| Frontend lint | `npm run lint` | `src/envSafe.Web` |
| Frontend build | `npm run build` | `src/envSafe.Web` |
| Start API (dev) | `dotnet run --project src/envSafe.Api/envSafe.Api.csproj --urls "http://0.0.0.0:5012"` | `/workspace` |
| Start frontend (dev) | `npm run dev -- --host 0.0.0.0` | `src/envSafe.Web` |

### Non-obvious notes

- The Vite dev server proxies `/api` requests to `http://localhost:5012` (configured in `vite.config.js`), so the API must be running before testing frontend-to-backend flows.
- The API has no database or external service dependencies — it is fully stateless and self-contained.
- Health check: `GET http://localhost:5012/health` returns `{"status":"ok","service":"envSafe.Api"}`.
- Core endpoint: `POST http://localhost:5012/api/generate` accepts JSON with `variableName`, `valueType`, `mode`, and `length`.
