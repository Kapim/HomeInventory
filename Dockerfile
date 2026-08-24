# --- Build stage ---
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy source. Only the API project graph (Api -> Application/Contracts/Infrastructure/Domain)
# is restored/published, so the Windows-only WPF/MAUI projects are never touched on Linux.
COPY HomeInventory/ ./HomeInventory/

RUN dotnet restore HomeInventory/HomeInventory.Api/HomeInventory.Api.csproj
RUN dotnet publish HomeInventory/HomeInventory.Api/HomeInventory.Api.csproj \
    -c Release -o /app/publish --no-restore

# --- Runtime stage ---
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# Kestrel listens on 8080 inside the container (compose maps it to the host).
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "HomeInventory.Api.dll"]
