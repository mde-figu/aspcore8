#!/usr/bin/env bash
# Executa os testes BDD/API localmente.
#   ./scripts/run-bdd-tests.sh                         -> API em memória (não precisa de banco)
#   API_BASE_URL=http://localhost:8080 ./scripts/run-bdd-tests.sh -> contra uma API já em execução
#   ./scripts/run-bdd-tests.sh --filter "Category=negativo"       -> apenas cenários com a tag @negativo
set -euo pipefail
cd "$(dirname "$0")/.."

dotnet test tests/EcoTracker.BddTests/EcoTracker.BddTests.csproj -c Release \
  --logger "trx;LogFileName=bdd-results.trx" \
  --logger "console;verbosity=normal" \
  --results-directory tests/EcoTracker.BddTests/TestResults "$@"

echo
echo "Relatório HTML: tests/EcoTracker.BddTests/TestResults/reqnroll_report.html"
