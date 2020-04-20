#!/usr/bin/env bash

cp /app/mount-config/appsettings.json /app/appsettings.json
chmod -R 777 /app/
dotnet SmartEnergy.dll
