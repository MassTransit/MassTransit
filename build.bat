@echo off
cls
If Not Exist src\.nuget\nuget.exe msbuild src\.nuget\NuGet.targets -Target:RestorePackages
If Not Exist tools\Cake\Cake.exe src\.nuget\nuget.exe Install Cake -Source "https://www.nuget.org/api/v2/" -OutputDirectory "tools" -ExcludeVersion

SET NUGET_EXE=%cd%\src\.nuget\nuget.exe

tools\Cake\Cake.exe build.cake -Experimental