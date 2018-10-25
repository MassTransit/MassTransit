#!/bin/bash

nuget="src/.nuget/nuget.exe"
if [ ! -f "$nuget" ]
then
    msbuild src/.nuget/NuGet.targets -Target:RestorePackages
fi

gitlink="src/packages/gitlink/lib/net45/GitLink.exe"
if [ ! -f "$gitlink" ]
then
    mono "$nuget" Install gitlink -Source "https://www.nuget.org/api/v2/" -OutputDirectory "./src/packages" -ExcludeVersion
fi

fake="src/packages/FAKE/tools/fake.exe"
if [ ! -f "$fake" ]
then
    mono "$nuget" Install FAKE -Source "https://www.nuget.org/api/v2/" -OutputDirectory "./src/packages" -ExcludeVersion
fi

mono "$fake" build.fsx $1
