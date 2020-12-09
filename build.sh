#!/bin/bash

set -eo pipefail

pushd src
dotnet publish --configuration Release --output ../.AppCenter
popd

chmod +x .AppCenter/AppCenterClient.dll