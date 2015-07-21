@echo off
cls
If Not Exist src\.nuget\nuget.exe msbuild src\.nuget\NuGet.targets -Target:RestorePackages
If Not Exist src\packages\FAKE\tools\fake.exe src\.nuget\nuget.exe Install FAKE -OutputDirectory "src\packages" -ExcludeVersion
src\packages\FAKE\tools\fake.exe build.fsx %*
