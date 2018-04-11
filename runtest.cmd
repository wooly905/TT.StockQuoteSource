@echo off

set "turtleRoot=%~dp0"
set "turtleTools=%turtleRoot%tools"
set "turtleSource=%turtleRoot%src"
set "turtleTest=%turtleRoot%out\test"

rem run test
%turtleTools%\xunit.runner.console\xunit.console.exe %turtleTest%\TT.StockQuoteSource.Tests.dll -verbose