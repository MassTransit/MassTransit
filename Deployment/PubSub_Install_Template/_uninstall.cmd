@echo off
SET DIR=%~d0%~p0%
SET INSTANCE=(local)

NET STOP "MassTransit Subscription Manager"

echo .

osql -S %INSTANCE% -E -i "%DIR%SQLScripts\UninstallMassTransitDatabaseDDL.sql"

echo .

"%DIR%PubSub.exe" -uninstall

PAUSE