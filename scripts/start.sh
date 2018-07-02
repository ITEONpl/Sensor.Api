#!/bin/bash
export ASPNETCORE_ENVIRONMENT=local
cd src/Sensor.Api
dotnet run --no-restore