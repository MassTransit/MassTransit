@echo off

SET DIR=%~d0%~p0%

SET database.name="MassTransitRunTimeServices"
SET sql.files.directory="%DIR%..\MassTransit.RunTimeServices.Database\MassTransitRunTimeServices"
SET repository.path="http://masstransit.googlecode.com/svn"
SET version.file="_BuildInfo.xml"
SET version.xpath="//buildInfo/version"
SET environment=LOCAL

echo "What server instance would you like to have MassTransitRunTimeServices on? Leave empty to deploy to (local)" 
set /p server.database=
if '%server.database%' == '' SET server.database="(local)"

"%DIR%tools\rh.exe" /d=%database.name% /f=%sql.files.directory% /s=%server.database% /vf=%version.file% /vx=%version.xpath% /r=%repository.path% /env=%environment% /simple 
