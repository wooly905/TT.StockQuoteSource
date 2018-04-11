@echo off

set "turtleRoot=%~dp0"
set "turtleRoot=%turtleRoot:~0,-7%"
set "turtleTools=%turtleRoot%\tools"
set "turtleSource=%turtleRoot%\src"
set "turtleTest=%turtleRoot%\out\test"

rem restore package
%turtleTools%\xunit.runner.console\xunit.console.exe %turtleTest%\TT.StockQuoteSource.Tests.dll
