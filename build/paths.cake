public class BuildPaths
{
    public BuildDirectories Directories { get; private set; }

    public static BuildPaths GetPaths(
        ICakeContext context
        )
    {
        if (context == null)
        {
            throw new ArgumentNullException("context");
        }


        var rootDir = context.MakeAbsolute(context.Directory("./"));
        var solutionDir = rootDir.Combine("src");
        var artifactsDir = rootDir.Combine("artifacts");

        // Directories
        var buildDirectories = new BuildDirectories(
            artifactsDir,
            rootDir,
            solutionDir);

        return new BuildPaths
        {
            Directories = buildDirectories
        };
    }
}

public class BuildFiles
{
    public FilePath VersionProperties { get; private set; }

    public BuildFiles(
        FilePath versionProperties
        )
    {
        VersionProperties = versionProperties;
    }
}

public class BuildDirectories
{
    public DirectoryPath Artifacts { get; }
    public DirectoryPath Root { get; }
    public DirectoryPath Solution { get; }
    public ICollection<DirectoryPath> ToClean { get; }

    public BuildDirectories(
        DirectoryPath artifactsDir,
        DirectoryPath rootDir,
        DirectoryPath solutionDir
        )
    {
        Artifacts = artifactsDir;
        Root = rootDir;
        Solution = solutionDir;
        ToClean = new[] {
            Artifacts
        };
    }
}
