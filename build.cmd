@echo off
cls
msbuild src\.nuget\NuGet.targets -Target:RestorePackages
src\.nuget\nuget.exe Install FAKE -OutputDirectory "src\packages" -ExcludeVersion
src\packages\FAKE\tools\fake.exe build.fsx %*
