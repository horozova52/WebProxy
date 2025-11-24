# Use .NET 8 SDK to build the project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj
COPY ["ProxyServer/ProxyServer.csproj", "ProxyServer/"]
COPY ["Shared/Shared.csproj", "Shared/"]
COPY ["DWServer/DWServer.csproj", "DWServer/"]

# Restore dependencies
RUN dotnet restore "ProxyServer/ProxyServer.csproj"

# Copy restul soluției
COPY . .

# Build
WORKDIR "/src/ProxyServer"
RUN dotnet publish -c Release -o /app/publish

# Final runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "ProxyServer.dll"]
