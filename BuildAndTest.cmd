@echo off

dotnet restore --packages .\packages

dotnet build -c Release --no-restore

dotnet test .\test\TT.StockQuoteSource.TestsInNetCore\TT.StockQuoteSource.TestsInNetCore.csproj -v n --no-build --no-restore
