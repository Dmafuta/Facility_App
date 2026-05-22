# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

All commands run from the repo root unless stated otherwise.

```bash
# Run the app (dev)
cd FacilityApp/FacilityApp
dotnet run

# Build
dotnet build FacilityApp/FacilityApp/FacilityApp.csproj

# Run all tests
dotnet test FacilityApp.Tests/FacilityApp.Tests.csproj

# Run a single test class
dotnet test FacilityApp.Tests/FacilityApp.Tests.csproj --filter "FullyQualifiedName~VisitorServiceTests"

# Add a new EF Core migration (run from FacilityApp/FacilityApp/)
dotnet ef migrations add <MigrationName>

# Apply migrations manually (also runs automatically on startup)
dotnet ef database update
```

The app requires `ConnectionStrings__DefaultConnection` environment variable (PostgreSQL). Migrations run automatically on startup via `MigrateAsync`. Identity roles are seeded automatically too.

**Dev seed endpoints** (only available in `Development` environment):
- `GET /dev/seed-admin?tenantSlug=x&email=x&password=x` — creates a tenant + Admin user
- `GET /dev/seed-superadmin` — creates the platform tenant + SuperAdmin user
- `GET /dev/reset-password?email=x&newPassword=x`

## Architecture

### Multi-tenancy
Every tenant has a `Slug` (e.g. `acme`) and optionally a `CustomDomain`. All requests resolve the tenant before auth via `TenantDomainMiddleware`, which sets `TenantContext` (Scoped). Pages have dual `@page` routes — `/{tenantSlug}/xxx` and `/xxx` — and use `TenantCtx.RouteBase` for all links so custom-domain tenants get slug-free URLs.

All EF Core entities carry a `TenantId` column. `AppDbContext` applies global query filters on every entity using a `private Guid CurrentTenantId => _tenantContext.TenantId` property (NOT the field directly — this is required for EF Core 9 `ExpressionTreeFuncletizer` compatibility).

### TenantService uses raw Npgsql — never EF Core
`TenantService` resolves tenants using `NpgsqlDataSource` (Singleton), not `AppDbContext`. This prevents concurrent-circuit crashes where `ResidentLayout.OnInitializedAsync` and the page's `OnInitializedAsync` both call the service simultaneously. Do not change this to use EF Core.

### Concurrent init — IDbContextFactory pattern
Services called during page `OnInitializedAsync` while `MainLayout` is also loading (gate context) must use `IDbContextFactory<AppDbContext>` to create isolated contexts per call. Services affected: `DashboardService`, `ReportService`, `ParcelService`. Register with `ServiceLifetime.Scoped` alongside the standard `AddDbContext`.

### Page component structure
Pages live in `Components/Modules/<Feature>/`. Blazor scans subdirectories automatically, so `@page` routes work regardless of folder depth. Layouts: `MainLayout` (staff), `ResidentLayout` (residents), `AuthLayout` (login pages).

### SignalR
`NotificationHub` at `/hubs/notifications` powers real-time dashboard updates. CORS policy `"BlazorHub"` must allow the app's origins with credentials; configure `AllowedOrigins` in appsettings for production.

### Auth flow
- Staff login: `/{slug}/login` — requires a staff `UserType`; post-login check enforces this
- Resident login: `/{slug}/resident/login` — `UserType.HomeOwner` or `Resident`
- SuperAdmin: `/platform/login` → `/superadmin/tenants` (cross-tenant, no query filters apply because SuperAdmin queries use `IgnoreQueryFilters()`)
- Cookie redirect logic in `Program.cs` is tenant-aware and handles custom domains

### SaaS plans
`TenantPlan` enum: `Starter` (default) / `Professional`. Custom domain is gated behind Professional. SuperAdmin upgrades/downgrades tenants from `/superadmin/tenants`.

### File uploads
- Visitor photos → `wwwroot/uploads/{slug}/`
- Documents → `wwwroot/documents/{slug}/`
- Both directories are created with correct ownership in the Dockerfile and must be writable at runtime.

### Docker / production notes
- Published under `/app`; DataProtection keys persisted to `/app/keys` (mount as a volume)
- .NET 10 fingerprints `blazor.web.js` — the Dockerfile copies it to the plain path, and `Program.cs` has a runtime fallback that serves the fingerprinted copy at `/_framework/blazor.web.js`
- Runs behind Caddy; `ForwardedHeaders` middleware trusts `X-Forwarded-For` and `X-Forwarded-Proto` from any proxy IP
