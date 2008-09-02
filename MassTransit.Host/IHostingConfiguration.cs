namespace MassTransit.Host
{
    using Configurations;
    using LifeCycles;

    public interface IHostingConfiguration
    {
        IInstallationConfiguration HowToInstall { get; }
        IApplicationLifeCycle HowToRun { get; }
    }
}