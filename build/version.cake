public class BuildVersion
{
    public string Prefix { get; set; }
    public string Suffix { get; set; }
    public string Metadata { get; set; }

    public string Version => (Prefix + "-" + Suffix).Trim('-') + Metadata;

    public static BuildVersion Calculate(ICakeContext context, BuildParameters buildParameters)
    {
        if (context == null)
        {
            throw new ArgumentNullException("context");
        }

        var buildSystem = context.BuildSystem();

        var prefix = context.EnvironmentVariable("APPVEYOR_BUILD_VERSION") ?? "1.0.0";

        // Appveyor needs a unique build version, so it's always MAJOR.MINOR.PATCH.{buildnumber}, but
        // this doesn't follow SemVer 2.0, we put the build number as metadata
        prefix = string.Join(".", prefix.Split('.').Take(3));

        string suffix = "alpha.9999";
        string metadata = null;

        if(!buildParameters.IsLocalBuild)
        {
            var buildNumber = buildSystem.AppVeyor.Environment.Build.Number;
            var commitHash = buildSystem.AppVeyor.Environment.Repository.Commit.Id;
            commitHash = commitHash.Substring(0,Math.Min(commitHash.Length,7));

            suffix =
                buildParameters.IsMasterBranch ? null
                : buildParameters.IsDevelopBranch ? "develop"
                : "beta";

            if(suffix != null)
            {
                suffix += $".{buildNumber}";
                metadata = $"+sha.{commitHash}";
            }
            else
            {
                metadata = $"+build.{buildNumber}.sha.{commitHash}";
            }
        }


        return new BuildVersion
        {
            Prefix = prefix,
            Suffix = suffix,
            Metadata = metadata
        };
    }
}
