#!/bin/bash
set -e

echo "===> Aplicando migrations..."
dotnet enova-academy.dll --migrate

echo "===> Iniciando aplicação..."
dotnet enova-academy.dll
