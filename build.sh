#!/bin/bash

set -eu
cd `dirname $0`

bkp_folder=../bkp/$(date --iso-8601=minutes)/
mkdir -p $bkp_folder

test -e ../app && mv -f ../app $bkp_folder
test -e ../appsettings.json && cp ../appsettings.json $bkp_folder

dotnet build
dotnet test
dotnet publish -o ../app
