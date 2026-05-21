# ── Build stage ──────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY FacilityApp.csproj ./
RUN dotnet restore

COPY . ./
RUN dotnet publish -c Release -o /app/publish --no-restore

# ── Runtime stage ─────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# Create non-root user (Debian-based image)
RUN groupadd --system --gid 1001 appgroup \
 && useradd  --system --uid 1001 --gid appgroup appuser

# Copy published output
COPY --from=build /app/publish .

# .NET 10 fingerprints blazor.web.js → blazor.web.<hash>.js during publish.
# Ensure a plain blazor.web.js copy always exists so UseStaticFiles can serve it.
RUN set -e; \
    src=$(find wwwroot/_framework -maxdepth 1 -name "blazor.web.*.js" -type f 2>/dev/null | head -1); \
    if [ -n "$src" ]; then cp "$src" wwwroot/_framework/blazor.web.js; fi

# Writable directories for uploads/documents/dataprotection keys
RUN mkdir -p wwwroot/uploads wwwroot/documents keys \
 && chown -R appuser:appgroup wwwroot/uploads wwwroot/documents keys

USER appuser

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "FacilityApp.dll"]
