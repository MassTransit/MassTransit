@echo off

echo Building for .NET 3.5
call rake BUILD_CONFIG_KEY=NET35
IF NOT %ERRORLEVEL% == 0 goto FAILED

echo Building for .NET 4.0
call rake unclean
IF NOT %ERRORLEVEL% == 0 goto FAILED

echo Creating NU package
lib\nuget pack MassTransit.nuspec /OutputDirectory build_artifacts 
IF NOT %ERRORLEVEL% == 0 goto FAILED

echo Create ZIP package
call rake package

:FAILED
