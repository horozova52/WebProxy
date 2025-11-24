# Imagine de bază pentru runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

# Imagine pentru build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiem fișierul csproj și facem restore
COPY ["DWServer/DWServer.csproj", "DWServer/"]
RUN dotnet restore "DWServer/DWServer.csproj"

# Copiem restul soluției
COPY . .
WORKDIR "/src/DWServer"

# Build
RUN dotnet build "DWServer.csproj" -c Release -o /app/build

# Publish
FROM build AS publish
RUN dotnet publish "DWServer.csproj" -c Release -o /app/publish

# Imagine finală
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://+:80
ENTRYPOINT ["dotnet", "DWServer.dll"]
