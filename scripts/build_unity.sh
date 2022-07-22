#!/bin/bash

OutputPath='./Unity'
mkdir -p $OutputPath

ProjectName='api.config.importer'
dotnet publish ./$ProjectName/$ProjectName/$ProjectName.csproj -c Release -o $OutputPath
