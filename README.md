# SafeShift

An **offline-first HSE (Health, Safety & Environment) incident reporting system** for oil and gas and industrial sites.
Workers report hazards in seconds, even without internet. The site PIC is notified straight away, and supervisors make sure every problem gets fixed.

## The problem

Existing HSE apps depend on internet connectivity, have complicated reporting forms, and cause delays in reporting hazards,
so workers on offshore platforms and remote sites often don't report at all. SafeShift is built for those conditions:
fast, simple, and usable with no signal.

## Features

| Version | Features | Status |
|---|---|---|
| **V1** | Login with roles · quick report (photo, GPS, severity) · offline save and auto-sync · notify the site PIC · supervisor review · corrective actions · auto-escalation after 15 min · trend dashboard · title autocomplete with duplicate check | In progress |
| **V2** | Live map · nearest-responder dispatch · offline SOS screen | Planned |

## Tech stack

| Layer | Technology |
|---|---|
| Frontend | React 18 + TypeScript + Vite, React Router, PWA |
| Backend | ASP.NET Core Web API on .NET 10 (C#), ASP.NET Core Identity, JWT bearer auth |
| Database | SQL Server / Azure SQL via Entity Framework Core 10, NetTopologySuite (GPS) |
| Testing | xUnit (API), Vitest + Testing Library (web) |
| DevOps | GitHub Actions CI, Docker |
| Cloud | Azure App Service, Azure SQL, Key Vault |

## Run it locally

**You need:** .NET 10 SDK, Node 22, Docker, and the EF Core tool (`dotnet tool install -g dotnet-ef`).

```bash
# 1. Database: SQL Server in Docker
cp .env.example .env              # then set a strong SQL_SA_PASSWORD in .env
docker compose up -d

# 2. API (http://localhost:5080)
cd SafeShift.Api
dotnet user-secrets set "ConnectionStrings:SafeShift" \
  "Server=localhost,1433;Database=SafeShift;User Id=sa;Password=<your password>;TrustServerCertificate=True"
dotnet ef database update
dotnet run

# 3. Web app (http://localhost:5173), in a second terminal
cd web
npm install
npm run dev
```

**Demo accounts** (password `Demo@123`, created automatically in Development):

| Email | Role |
|---|---|
| `worker@safeshift.demo` | Worker (phone screens) |
| `supervisor@safeshift.demo` | Supervisor, PIC of Platform A |
| `admin@safeshift.demo` | HSE Admin |
| `firstaid@safeshift.demo` | Worker, on-shift first aider |

## Tests

```bash
dotnet test              # API
cd web && npm test       # web
```

## Project structure

```
SafeShift.Api/          ASP.NET Core Web API
  Auth/                 JWT login (TokenService, AuthController)
  Data/                 EF Core DbContext, migrations, demo data
  Domain/               Entities and enums
SafeShift.Api.Tests/    xUnit tests
web/                    React + TypeScript app
.github/workflows/      CI pipeline
```
