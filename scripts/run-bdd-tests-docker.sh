#!/usr/bin/env bash
# Sobe API + SQL Server com Docker Compose e executa os testes BDD/API contra a stack real.
set -euo pipefail
cd "$(dirname "$0")/.."

[ -f .env ] || cp .env.example .env
COMPOSE="docker compose -f docker-compose.yml -f docker-compose.tests.yml"

mkdir -p TestResults
status=0
$COMPOSE up --build --abort-on-container-exit --exit-code-from bdd-tests db api bdd-tests || status=$?
$COMPOSE down -v
echo
echo "Relatórios em ./TestResults (reqnroll_report.html, bdd-results.trx) — exit code: $status"
exit $status
