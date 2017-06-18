#tool "nuget:?package=NUnit.ConsoleRunner"

#addin nuget:?package=Cake.AppVeyor

var target = Argument("target", "Default");

var solutionPath = "./src/MassTransit.sln";
var buildArtifactPath = "./build_artifacts";

var baseVersion = "3.6.0.0";

var NuGetVersion = $"{baseVersion}-local";
var FileVersion = string.Empty;
var InfoVersion = string.Empty;

var testResultsFileName = "nunit-testresults.xml";
var configuration = Argument("configuration", "Release");

Task("Clean")
    .Does(() => {
        if(FileExists(testResultsFileName))
        {
            DeleteFile(testResultsFileName);
        }

        if(DirectoryExists(buildArtifactPath))
        {
            DeleteDirectory(buildArtifactPath);
        }
    });

Task("Restore")
    .Does(() => {
        NuGetRestore(solutionPath);
    });

Task("Version")
    .Does(() => {

        if(!AppVeyor.IsRunningOnAppVeyor)
        {
           return;
        }

        var branchName = AppVeyor.Environment.Repository.Branch;
        var commitSha = AppVeyor.Environment.Repository.Commit.Id.Substring(0, 7);

        FileVersion = (AppVeyor.Environment.Build.Version ?? baseVersion);

        // Remove the build number
        var buildVersion = FileVersion.Substring(0, FileVersion.LastIndexOf('.'));

        var infoVersionLabel = string.Empty;
        var nugetVersionLabel = string.Empty;
        var nugetBaseVersion = buildVersion;

        if(branchName != "master")
        {
            infoVersionLabel = $" ({branchName}/{commitSha})";
            nugetVersionLabel = $"-{branchName}";
            nugetBaseVersion = FileVersion;
        }

        InfoVersion = $"{FileVersion}{infoVersionLabel}";

        NuGetVersion = $"{nugetBaseVersion}{nugetVersionLabel}";
    });

Task("Build")
    .IsDependentOn("Clean")
    .IsDependentOn("Restore")
    .IsDependentOn("Version")
    .Does(() => {
        var settings = new DotNetCoreBuildSettings {
            Configuration = configuration,
            ArgumentCustomization = args => args
                .Append($"/p:Version=\"{NuGetVersion}\"")
                .Append($"/p:AssemblyVersion=\"{FileVersion}\"")
                .Append($"/p:FileVersion=\"{FileVersion}\"")
                .Append($"/p:InformationalVersion=\"{InfoVersion}\"")
        };

        DotNetCoreBuild(solutionPath, settings);
    });

Task("Test")
    .IsDependentOn("Build")
    .Does(() => {
        // Test will not run this way on appveyor for now since the appveyor format is not available via nuget
        if(AppVeyor.IsRunningOnAppVeyor)
        {
            return;
        }

        var nunitTestSettings = new NUnit3Settings {
                    Results = new FilePath(testResultsFileName),
                    //ResultFormat = "AppVeyor",
                    NoResults = true,
                    Where = "cat != Flakey"
                };
        var testAssemblyFilePattern = $"./src/**/bin/{configuration}/**/*Tests.dll";

        NUnit3(testAssemblyFilePattern, nunitTestSettings);
    })
    .OnError(exception => {
        // When tests fail task will return 1 and this will stop the whole build.
        if(!AppVeyor.IsRunningOnAppVeyor)
        {
            throw exception;
        }
    });

Task("Package")
    .IsDependentOn("Test")
    .Does(() => {
        var packSettings = new DotNetCorePackSettings {
            Configuration = configuration,
            OutputDirectory = buildArtifactPath,
            ArgumentCustomization = args => args
                .Append($"/p:Version=\"{NuGetVersion}\"")
                .Append($"/p:AssemblyVersion=\"{FileVersion}\"")
                .Append($"/p:FileVersion=\"{FileVersion}\"")
                .Append($"/p:InformationalVersion=\"{InfoVersion}\"")
                .Append("--include-symbols")
                .Append("--include-source")
        };

        DotNetCorePack(solutionPath, packSettings);
    });

Task("Default")
    .IsDependentOn("Package");

RunTarget(target);