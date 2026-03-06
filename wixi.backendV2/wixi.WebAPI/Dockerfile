# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files
COPY ["wixi.backendV2/wixi.WebAPI/wixi.WebAPI.csproj", "wixi.backendV2/wixi.WebAPI/"]
COPY ["wixi.backendV2/wixi.Core/wixi.Core.csproj", "wixi.backendV2/wixi.Core/"]
COPY ["wixi.backendV2/wixi.DataAccess/wixi.DataAccess.csproj", "wixi.backendV2/wixi.DataAccess/"]
COPY ["wixi.backendV2/wixi.Identity/wixi.Identity.csproj", "wixi.backendV2/wixi.Identity/"]
COPY ["wixi.backendV2/wixi.Forms/wixi.Forms.csproj", "wixi.backendV2/wixi.Forms/"]
COPY ["wixi.backendV2/wixi.Appointments/wixi.Appointments.csproj", "wixi.backendV2/wixi.Appointments/"]
COPY ["wixi.backendV2/wixi.Payments/wixi.Payments.csproj", "wixi.backendV2/wixi.Payments/"]
COPY ["wixi.backendV2/wixi.Content/wixi.Content.csproj", "wixi.backendV2/wixi.Content/"]
COPY ["wixi.backendV2/wixi.Clients/wixi.Clients.csproj", "wixi.backendV2/wixi.Clients/"]
COPY ["wixi.backendV2/wixi.Applications/wixi.Applications.csproj", "wixi.backendV2/wixi.Applications/"]
COPY ["wixi.backendV2/wixi.Documents/wixi.Documents.csproj", "wixi.backendV2/wixi.Documents/"]
COPY ["wixi.backendV2/wixi.Email/wixi.Email.csproj", "wixi.backendV2/wixi.Email/"]
COPY ["wixi.backendV2/wixi.Support/wixi.Support.csproj", "wixi.backendV2/wixi.Support/"]

# Copy Iyzipay project files
COPY ["iyzipay-dotnet-master/Iyzipay/Iyzipay.csproj", "iyzipay-dotnet-master/Iyzipay/"]

# Restore dependencies
RUN dotnet restore "wixi.backendV2/wixi.WebAPI/wixi.WebAPI.csproj"

# Copy all source files
COPY wixi.backendV2/ ./wixi.backendV2/
COPY iyzipay-dotnet-master/Iyzipay/ ./iyzipay-dotnet-master/Iyzipay/

# Build project
WORKDIR "/src/wixi.backendV2/wixi.WebAPI"
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
EXPOSE 5115

ENTRYPOINT ["dotnet", "wixi.WebAPI.dll"]

