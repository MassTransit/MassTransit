namespace MassTransit.Host.Configurations
{
    using System.ServiceProcess;
    using LifeCycles;

    public interface IInstallationConfiguration
    {
        Credentials Credentials { get; }

        //life cycle
        IApplicationLifeCycle LifeCycle { get; }

        //winservice stuff
        string ServiceName { get; }
        string DispalyName { get; }
        string Description { get; }
        string[] Dependencies { get; }
        void ConfigureServiceInstaller(ServiceInstaller installer);
        void ConfigureServiceProcessInstaller(ServiceProcessInstaller installer);
    }
}