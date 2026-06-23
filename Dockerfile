FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/EcoTracker/EcoTracker.csproj", "src/EcoTracker/"]
RUN dotnet restore "src/EcoTracker/EcoTracker.csproj"

COPY . .
WORKDIR "/src/src/EcoTracker"
RUN dotnet build "EcoTracker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "EcoTracker.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Development
ENTRYPOINT ["dotnet", "EcoTracker.dll"]
