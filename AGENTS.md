# AGENTS.md

## Cursor Cloud specific instructions

### Project overview

SafeEnv is a two-service application for environment variable management:

| Service | Path | Tech | Dev command | Default port |
|---|---|---|---|---|
| **safeenv-ui** (frontend) | `safeenv-ui/` | React 19 + Vite 8 | `npm run dev` | 5173 |
| **safeEnv.Api** (backend) | `safeEnv.Api/` | ASP.NET Core 8 (.NET 8) | `dotnet run` | 5012 (HTTP) |

No databases, Docker, or external services are required.

### Running services

- **Frontend**: `cd safeenv-ui && npm run dev`
- **Backend**: `cd safeEnv.Api && dotnet run` (listens on `http://localhost:5012` by default per `launchSettings.json`)
- **Lint**: `cd safeenv-ui && npm run lint` (ESLint; no lint tooling exists for the .NET project yet)
- **Build frontend**: `cd safeenv-ui && npm run build`
- **Build backend**: `cd safeEnv.Api && dotnet build`

### Environment notes

- .NET 8 SDK is installed at `$HOME/.dotnet`. The `DOTNET_ROOT` and `PATH` exports are persisted in `~/.bashrc`.
- Node.js v22 and npm are pre-installed via the system.
- The two projects are independent sibling directories with no shared workspace configuration.
- There are no automated tests configured in either project yet.
