@echo off
SET DIR=%~d0%~p0%
SET INSTANCE=(local)

osql -S %INSTANCE% -E -i "%DIR%SQLScripts\InstallMassTransitDatabaseDDL.sql"
osql -S %INSTANCE% -E -i "%DIR%SQLScripts\CreateMassTransitTableDDL.sql"
osql -S %INSTANCE% -E -i "%DIR%SQLScripts\CreateUserAccountLoginDDL.sql"

echo .

"%DIR%PubSub.exe" -install

echo .

NET START "MassTransit Subscription Manager"

PAUSE