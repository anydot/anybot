#!/bin/bash

set -eu
cd `dirname $0`

test -e ../app && mv -f ../app ../app-$(date --iso-8601=minutes)

dotnet build
dotnet test
dotnet publish -o ../app
