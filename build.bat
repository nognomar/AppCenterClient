@echo off

pushd src
dotnet publish --configuration Release --output ../.AppCenter
popd