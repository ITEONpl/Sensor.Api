#!/bin/bash
MYGET_ENV=""
case "$TRAVIS_BRANCH" in
  "develop")
    MYGET_ENV="-dev"
    ;;
esac

dotnet tool install --global dotnet-sonarscanner
dotnet sonarscanner begin /k:"project-key"
dotnet build -c Release --source "https://api.nuget.org/v3/index.json" --no-cache
dotnet sonarscanner end