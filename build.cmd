@echo off

set "turtleRoot=%~dp0"
set "turtleTools=%turtleRoot%tools"
set "turtleSource=%turtleRoot%src"
set "turtleTest=%turtleRoot%out\test"

rem restore package
%turtleTools%\NuGet.exe restore

rem msbuild - .Net Core SDK is required on local machine
dotnet build