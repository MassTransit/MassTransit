@echo off

::Project UppercuT - http://uppercut.googlecode.com
::No edits to this file are required - http://uppercut.pbwiki.com

SET DIR=%~d0%~p0%
::This is gone once teamcity can call multiple files
call "%DIR%build.bat"
if %ERRORLEVEL% NEQ 0 goto errors

SET NANT="%DIR%lib\Nant\nant.exe"
SET build.config.settings="%DIR%settings\UppercuT.config"

%NANT% %1 /f:.\build\zip.build -D:build.config.settings=%build.config.settings%

if %ERRORLEVEL% NEQ 0 goto errors

goto finish

:errors
EXIT /B %ERRORLEVEL%

:finish