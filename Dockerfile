# =========================
# Build
# =========================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

# Copy project files first so Docker can cache restore
COPY ["Authentication.Api/Authentication.Api.csproj", "Authentication.Api/"]
COPY ["Authentication.Application/Authentication.Application.csproj", "Authentication.Application/"]
COPY ["Authentication.Domain/Authentication.Domain.csproj", "Authentication.Domain/"]
COPY ["Authentication.Infrastructure/Authentication.Infrastructure.csproj", "Authentication.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "Authentication.Api/Authentication.Api.csproj"

# Copy the remaining source code
COPY . .

# Publish
WORKDIR "/src/Authentication.Api"

RUN dotnet publish "Authentication.Api.csproj" \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false


# =========================
# Runtime
# =========================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "Authentication.Api.dll"]