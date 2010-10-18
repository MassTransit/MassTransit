@echo off

::Project UppercuT - http://uppercut.googlecode.com
::No edits to this file are required - http://uppercut.pbwiki.com

SET DIR=%~d0%~p0%
SET NANT="%DIR%lib\Nant\nant.exe"
SET build.config.settings="%DIR%settings\UppercuT.config"

%NANT% %1 /f:.\build\open.build -D:build.config.settings=%build.config.settings%