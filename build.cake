//////////////////////////////////////////////////////////////////////
// TOOLS
//////////////////////////////////////////////////////////////////////

// Load other scripts.
#load "./build/parameters.cake"

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");

//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

Setup<BuildParameters>(setupContext =>
{
    var buildParams = BuildParameters.GetParameters(setupContext);
    buildParams.Initialize(setupContext);
    return buildParams;
});

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does<BuildParameters>(data =>
{
    CleanDirectories($"./src/**/obj/{data.Configuration}");
    CleanDirectories($"./src/**/bin/{data.Configuration}");
    CleanDirectory(data.Paths.Directories.Artifacts);
});

Task("CleanAll")
    .Does<BuildParameters>(data =>
{
    CleanDirectories($"./src/**/obj");
    CleanDirectories($"./src/**/bin");
    CleanDirectory(data.Paths.Directories.Artifacts);
});

Task("Restore-NuGet")
    .Does<BuildParameters>(data =>
{
    DotNetCoreRestore(data.Paths.Directories.Solution.FullPath);
});

Task("Build")
    .IsDependentOn("Clean")
    .IsDependentOn("Restore-NuGet")
    .Does<BuildParameters>(data =>
{
    var settings = new DotNetCoreBuildSettings{
        NoRestore = true,
        Configuration = data.Configuration,
        MSBuildSettings = new DotNetCoreMSBuildSettings().WithProperty("Version", data.Version.Version)
    };

    DotNetCoreBuild(data.Paths.Directories.Solution.FullPath, settings);
});

Task("Test")
    .IsDependentOn("Build")
    .Does<BuildParameters>(data =>
{
    var hasFailures = false;
    foreach(var project in GetFiles("./src/**/*.Tests.csproj"))
    {
        var filename = project.GetFilenameWithoutExtension().FullPath;
        if (!data.Tests.Criteria.ContainsKey(filename)) continue;

        var settings = new DotNetCoreTestSettings
        {
            NoRestore = true,
            NoBuild = true,
            Configuration = data.Configuration,
            DiagnosticOutput = true,
            Filter = data.Tests.Criteria[filename]
        };

        if(settings.Filter == null)
            settings.Filter = data.Tests.SharedFilter;
        else
            settings.Filter += "&" + data.Tests.SharedFilter;

        // Skip running tests for csproj's that only target full framework. The Windows Job will run those tests
        var targetFrameworks = XmlPeek(File(project.FullPath), "/Project/PropertyGroup/TargetFrameworks/text()") ?? XmlPeek(File(project.FullPath), "/Project/PropertyGroup/TargetFramework/text()");
        var coreFrameworks = targetFrameworks.Split(new[]{";"}, StringSplitOptions.RemoveEmptyEntries).Where(x=>!x.StartsWith("net4"));
        if(data.IsRunningOnUnix)
        {
            if (!coreFrameworks.Any())
                continue; // Skip, there's no netcore tests to run for linux/osx
            else
                settings.Framework = coreFrameworks.First(); // only test netcore
        }

        if(data.IsRunningOnAppVeyor) settings.ArgumentCustomization = args => args.Append($"--test-adapter-path:.").Append("--logger:Appveyor");

        try
        {
            DotNetCoreTest(project.FullPath, settings);
        }
        catch
        {
            hasFailures = true;
        }
    }

    if(hasFailures) throw new Exception("Some Tests Failed. Please review the logs for details.");
});

Task("Pack")
    .IsDependentOn("Build")
    .WithCriteria<BuildParameters>((context,data) => data.ShouldPublish)
    .Does<BuildParameters>(data =>
{
    var settings = new DotNetCorePackSettings{
        NoRestore = true,
        NoBuild = true,
        OutputDirectory = data.Paths.Directories.Artifacts,
        Configuration = data.Configuration,
        MSBuildSettings = new DotNetCoreMSBuildSettings().WithProperty("Version", data.Version.Version)
    };
    DotNetCorePack(data.Paths.Directories.Solution.FullPath, settings);
});



//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("Clean")
    .IsDependentOn("Restore-NuGet")
    .IsDependentOn("Build")
    .IsDependentOn("Test")
    .IsDependentOn("Pack");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
