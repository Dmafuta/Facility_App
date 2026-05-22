# ── Build stage ──────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY FacilityApp.csproj ./
RUN dotnet restore

COPY . ./
RUN dotnet publish -c Release -o /app/publish --no-restore

# .NET 10 fingerprints blazor.web.js (e.g. blazor.web.abc123.js).
# Create a plain-URL copy so UseStaticFiles can serve /_framework/blazor.web.js
# when @Assets doesn't resolve the fingerprint correctly (e.g. manifest not loaded).
RUN set -e; \
    fp=$(find /app/publish -path "*/_framework/blazor.web.*.js" ! -name "*.map" | head -1); \
    if [ -n "$fp" ]; then \
        cp "$fp" "$(dirname "$fp")/blazor.web.js"; \
        echo "Created plain copy from: $fp"; \
    else \
        echo "WARNING: fingerprinted blazor.web.js not found — skipping copy"; \
    fi

# ── Runtime stage ─────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# Create non-root user (Debian-based image)
RUN groupadd --system --gid 1001 appgroup \
 && useradd  --system --uid 1001 --gid appgroup appuser

# Copy published output
COPY --from=build /app/publish .

# Writable directories for uploads/documents/dataprotection keys
RUN mkdir -p wwwroot/uploads wwwroot/documents keys \
 && chown -R appuser:appgroup wwwroot/uploads wwwroot/documents keys

USER appuser

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "FacilityApp.dll"]
