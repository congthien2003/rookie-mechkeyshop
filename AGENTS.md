# AGENTS.md - Rookie MechKeyShop

## Monorepo Layout

```
client/        React 19 + Vite + Tailwind 3 admin SPA  (`npm run dev`)
landingpage/   Next.js 16 + Tailwind 4 landing         (`npm run dev`)
MechKey/       .NET 10 backend microservices            (solution: MechKey.sln)
```

Three projects are independent. Changes to one do not affect others.

## Backend (MechKey/)

### Quick Start

```powershell
# 1. Start infrastructure (Redis + RabbitMQ)
docker compose -f MechKey\docker-compose.yml up -d

# 2. Run all services via Aspire orchestrator
dotnet run --project MechKey\MechKey.AppHost

# 3. Build
dotnet build MechKey\MechKey.sln

# 4. Run tests (xUnit)
dotnet test MechKey\MechKey.sln
```

### Architecture

- **Target framework**: `net10.0` (not .NET 8 as README says - README is stale)
- **Orchestrator**: .NET Aspire 13.0 — `MechKey.AppHost` starts all services. Always use this as the run entrypoint, not individual project F5.
- **Service map** (7 projects launched):
  - `MechkeyShop` — ASP.NET Core MVC admin portal
  - `WebAPI` — REST API with Swagger + API versioning (`/api/v{n}/...`)
  - `Mechkey.Gateway` — YARP reverse proxy
  - `Notifcation.Api` — Email notifications (MongoDB logs)
  - `Order.Api` — Order service (Event Sourcing: Postgres event store + Mongo read DB)
  - `Invoice.Service.API` — Invoice service
  - `GenAI.API` — GenAI service
- **Clean Architecture layers**: `MechKey.Domain` → `MechKey.Application` → `MechKey.Infrastructure`
- **Messaging**: MassTransit over RabbitMQ. `EventBus/` project holds shared config.
- **Caching**: Redis via `IRedisService`. Cache invalidation uses `RemoveByPrefixAsync`.
- **Auth**: JWT read from `accessToken` cookie (both MVC and API), not Authorization header.

### Configuration

`appsettings.json` is **gitignored**. Secrets come from:
- .NET User Secrets (each project has a `UserSecretsId`)
- Environment variables
- `appsettings.Development.json` (checked in)

### Active Refactoring: Catalog → MediatR CQRS

See `rule-cqrs-refactor.md` for full spec. Key rules that differ from defaults:

- **Static validators**, not FluentValidation. Pattern: `XxxValidator.Validate(command)` throws on failure.
- **No repository layer** in handlers. Inject `ApplicationDbContext` directly. Use `_dbContext.Xxx.AsQueryable()` instead of `_repository.GetAllAsync()`.
- **One file per use case** contains request class + validator class + handler class. File name matches the command/query name (e.g. `CreateCategory.cs`).
- **Controller injects `ISender`** only (from MediatR). One-liner endpoints: `await _sender.Send(new CreateCategoryCommand { ... }, ct)`.
- **Do NOT delete old service code**. New MediatR handlers live alongside existing service classes during transition.
- Command naming: `{Action}{Entity}Command`. Query naming: `Get{Entity}{Filter}Query`. Validator: `{Action}{Entity}Validator`. Handler: `{Action}{Entity}Handler` / `Get{Entity}{Filter}Handler`.
- **Result pattern**: All command/query return `Result<T>` or `Result<PagedResult<T>>`.
- Product commands need extra dependencies: `IProductUnitOfWork`, `ISupabaseService`, `IEventBus`, `IRedisService`.

### DI Registration Order

`Program.cs` pattern for all services:
1. `builder.AddServiceDefaults()` (Aspire: health checks, OTel, service discovery, resilience)
2. `builder.Services.AddApplication().AddInfrastructure(config)`
3. `builder.Services.AddEventBus()`
4. `builder.AddRedisCache()` / `builder.AddMassTransit()` (Aspire integration extensions)
5. Concrete services (controllers, auth, CORS, Swagger)

### Testing

- Framework: **xUnit** + **Moq** + **MockQueryable.Moq** (for mocking EF Core `DbSet`)
- Test projects under `MechKey/test/`: `Application.Tests`, `Domain.Test`, `Repository.Test`, `Abstraction`
- Run single test: `dotnet test MechKey\MechKey.sln --filter "FullyQualifiedName~CategoryServiceTest"`
- No CI workflows configured (`.github/workflows/` is empty).

## Client (`client/`)

```
npm run dev      Vite dev server
npm run build    tsc -b && vite build
npm run lint     eslint .
```

- React 19, Vite 6, TypeScript ~5.7, Tailwind CSS 3
- shadcn/ui components (Radix UI primitives) via `components.json`
- State: Zustand. Forms: Formik + Yup. HTTP: Axios.
- Path alias: `@` → `src/`
- Supabase client for auth/storage

## Landing Page (`landingpage/`)

```
npm run dev      next dev
npm run build    next build
npm run lint     eslint
```

- Next.js 16.1, React 19.2, Tailwind CSS 4
- Deployed to **Vercel** (see `vercel.json`). `npm run build` output is `.next/`.
- Images proxied from `bsnnwuphmgsqfpvlqdwn.supabase.co` (configured in `next.config.ts`)
