# Imagem do runner dos testes BDD (Reqnroll + Gherkin) e de contrato (JSON Schema).
# Build a partir da raiz do repositório:
#   docker build -f tests/EcoTracker.BddTests/Dockerfile -t ecotracker-bdd-tests .
FROM mcr.microsoft.com/dotnet/sdk:8.0
WORKDIR /src

COPY ["src/EcoTracker/EcoTracker.csproj", "src/EcoTracker/"]
COPY ["tests/EcoTracker.BddTests/EcoTracker.BddTests.csproj", "tests/EcoTracker.BddTests/"]
RUN dotnet restore "tests/EcoTracker.BddTests/EcoTracker.BddTests.csproj"

COPY src/ src/
COPY tests/EcoTracker.BddTests/ tests/EcoTracker.BddTests/
RUN dotnet build "tests/EcoTracker.BddTests/EcoTracker.BddTests.csproj" -c Release --no-restore

# API_BASE_URL vazio = API em memória; preenchido = API real (ex.: http://api:8080 no compose)
ENV API_BASE_URL=""
WORKDIR /src/tests/EcoTracker.BddTests
ENTRYPOINT ["dotnet", "test", "-c", "Release", "--no-build", \
            "--logger", "trx;LogFileName=bdd-results.trx", \
            "--logger", "console;verbosity=normal", \
            "--results-directory", "/src/tests/EcoTracker.BddTests/TestResults"]
