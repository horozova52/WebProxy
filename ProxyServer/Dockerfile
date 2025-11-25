FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

# INSTALĂM UN SHELL NORMAL + UTILITĂȚI
RUN apt-get update && apt-get install -y \
    iputils-ping \
    netcat-openbsd \
    dnsutils \
    curl \
    vim \
    && apt-get clean

EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["ProxyServer/ProxyServer.csproj", "ProxyServer/"]
RUN dotnet restore "ProxyServer/ProxyServer.csproj"
COPY . .
WORKDIR "/src/ProxyServer"
RUN dotnet build "ProxyServer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ProxyServer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ProxyServer.dll"]
