FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
ENV ASPNETCORE_URLS=http://+:80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["DWServer/DWServer.csproj", "DWServer/"]
RUN dotnet restore "DWServer/DWServer.csproj"
COPY . .
WORKDIR "/src/DWServer"
RUN dotnet build "DWServer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DWServer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DWServer.dll"]
