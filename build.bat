@echo off

echo Building for .NET 4.0
call rake unclean %1
IF NOT %ERRORLEVEL% == 0 goto FAILED

echo Creating NuGet package
call rake nuget %1
IF NOT %ERRORLEVEL% == 0 goto FAILED

echo Create ZIP package
call rake package

:FAILED
