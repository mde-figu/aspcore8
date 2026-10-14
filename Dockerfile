FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["EcoTracker.sln", "./"]
COPY ["src/EcoTracker/EcoTracker.csproj", "src/EcoTracker/"]
COPY ["tests/EcoTracker.Tests/EcoTracker.Tests.csproj", "tests/EcoTracker.Tests/"]
RUN dotnet restore "src/EcoTracker/EcoTracker.csproj"

COPY . .
WORKDIR "/src/src/EcoTracker"
RUN dotnet build "EcoTracker.csproj" -c Release -o /app/build --no-restore

FROM build AS test
WORKDIR /src
RUN dotnet test "tests/EcoTracker.Tests/EcoTracker.Tests.csproj" -c Release --logger "trx;LogFileName=test-results.trx"

FROM build AS publish
RUN dotnet publish "EcoTracker.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

RUN apt-get update \
    && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/* \
    && useradd --uid 1001 --create-home ecotracker

COPY --from=publish /app/publish .
USER ecotracker

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080 \
    DOTNET_RUNNING_IN_CONTAINER=true \
    DisableHttpsRedirection=true

HEALTHCHECK --interval=15s --timeout=5s --start-period=40s --retries=5 \
    CMD curl --fail http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "EcoTracker.dll"]
