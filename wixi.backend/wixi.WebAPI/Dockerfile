# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files
COPY ["wixi.WebAPI/wixi.WebAPI.csproj", "wixi.WebAPI/"]
COPY ["wixi.Business/wixi.Business.csproj", "wixi.Business/"]
COPY ["wixi.DataAccess/wixi.DataAccess.csproj", "wixi.DataAccess/"]
COPY ["wixi.Entities/wixi.Entities.csproj", "wixi.Entities/"]
COPY ["wixi.Core/wixi.Core.csproj", "wixi.Core/"]

# Restore dependencies
RUN dotnet restore "wixi.WebAPI/wixi.WebAPI.csproj"

# Copy all source files
COPY . .

# Build project
WORKDIR "/src/wixi.WebAPI"
RUN dotnet build "wixi.WebAPI.csproj" -c Release -o /app/build

# Publish Stage
FROM build AS publish
RUN dotnet publish "wixi.WebAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Install curl for healthcheck
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

COPY --from=publish /app/publish .

# Create Logs directory
RUN mkdir -p /app/Logs && chmod 777 /app/Logs

# Expose ports (will be configured via environment)
EXPOSE 5045
EXPOSE 5048

ENTRYPOINT ["dotnet", "wixi.WebAPI.dll"]

