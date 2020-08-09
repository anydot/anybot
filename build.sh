#!/bin/bash

set -eu
cd `dirname $0`

dotnet build
dotnet test
dotnet publish -o ../app
