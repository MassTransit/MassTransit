public class BuildVersion
{
    public string Prefix { get; set; }
    public string Suffix { get; set; }

    public static BuildVersion Calculate(ICakeContext context, BuildParameters buildParameters)
    {
        if (context == null)
        {
            throw new ArgumentNullException("context");
        }

        var buildSystem = context.BuildSystem();

        var prefix = context.XmlPeek(buildParameters.Paths.Files.VersionProperties, "/Project/PropertyGroup/VersionPrefix/text()");

        string suffix = "alpha.9999";

        if(!buildParameters.IsLocalBuild)
        {
            var commitHash = buildSystem.AppVeyor.Environment.Repository.Commit.Id;

            suffix =
                buildParameters.IsMasterBranch ? null
                : buildParameters.IsDevelopBranch ? "develop"
                : "beta";

            if(suffix != null)
                suffix += "." + buildSystem.AppVeyor.Environment.Build.Number + "+sha." + commitHash.Substring(0,Math.Min(commitHash.Length,7));
        }


        return new BuildVersion
        {
            Prefix = prefix,
            Suffix = suffix
        };
    }
}
