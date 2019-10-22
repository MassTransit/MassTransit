#load "./paths.cake"
#load "./tests.cake"
#load "./version.cake"

public class BuildParameters
{
    public string Configuration { get; private set; }
    public bool IsLocalBuild { get; private set; }
    public bool IsRunningOnUnix { get; private set; }
    public bool IsRunningOnWindows { get; private set; }
    public bool IsRunningOnAppVeyor { get; private set; }
    public bool IsPullRequest { get; private set; }
    public bool IsMainMassTransitRepo { get; private set; }
    public bool IsMasterBranch { get; private set; }
    public bool IsDevelopBranch { get; private set; }
    public bool IsTagged { get; private set; }
    public BuildPaths Paths { get; private set; }
    public Tests Tests { get; private set; }
    public BuildVersion Version { get; private set; }

    public bool ShouldPublish
    {
        get
        {
            return !IsLocalBuild && IsRunningOnWindows && !IsPullRequest && IsMainMassTransitRepo && (IsMasterBranch || IsDevelopBranch);
        }
    }

    public void Initialize(ICakeContext context)
    {
        Version = BuildVersion.Calculate(context, this);
        Tests = Tests.GetTests(this);
    }

    public static BuildParameters GetParameters(ICakeContext context)
    {
        if (context == null)
        {
            throw new ArgumentNullException("context");
        }

        var target = context.Argument("target", "Default");
        var buildSystem = context.BuildSystem();

        return new BuildParameters {
            Configuration = context.Argument("configuration", "Release"),
            IsLocalBuild = buildSystem.IsLocalBuild,
            IsRunningOnUnix = context.IsRunningOnUnix(),
            IsRunningOnWindows = context.IsRunningOnWindows(),
            IsRunningOnAppVeyor = buildSystem.AppVeyor.IsRunningOnAppVeyor,
            IsPullRequest = buildSystem.AppVeyor.Environment.PullRequest.IsPullRequest,
            IsMainMassTransitRepo = StringComparer.OrdinalIgnoreCase.Equals("masstransit/masstransit", buildSystem.AppVeyor.Environment.Repository.Name),
            IsMasterBranch = StringComparer.OrdinalIgnoreCase.Equals("master", buildSystem.AppVeyor.Environment.Repository.Branch),
            IsDevelopBranch = StringComparer.OrdinalIgnoreCase.Equals("develop", buildSystem.AppVeyor.Environment.Repository.Branch)
              || StringComparer.OrdinalIgnoreCase.Equals("v6", buildSystem.AppVeyor.Environment.Repository.Branch),
            IsTagged = IsBuildTagged(buildSystem),
            Paths = BuildPaths.GetPaths(context)
        };
    }

    private static bool IsBuildTagged(BuildSystem buildSystem)
    {
        return buildSystem.AppVeyor.Environment.Repository.Tag.IsTag
            && !string.IsNullOrWhiteSpace(buildSystem.AppVeyor.Environment.Repository.Tag.Name);
    }
}
