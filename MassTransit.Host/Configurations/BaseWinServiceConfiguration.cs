namespace MassTransit.Host.Configurations
{
    using System.ServiceProcess;
    using LifeCycles;

    public abstract class BaseWinServiceConfiguration :
        IInstallationConfiguration
    {

        public abstract Credentials Credentials{ get; }
        public abstract string ServiceName{ get; }
        public abstract string DispalyName { get; }
        public abstract string Description{ get; }
        public abstract string[] Dependencies{ get; }

        public abstract IApplicationLifeCycle LifeCycle { get; }

        public virtual void ConfigureServiceInstaller(ServiceInstaller installer)
        {
            installer.ServiceName = this.ServiceName;
            installer.Description = this.Description;
            installer.DisplayName = this.DispalyName;
            installer.ServicesDependedOn = this.Dependencies;
            installer.StartType = ServiceStartMode.Automatic;
        }
        public abstract void ConfigureServiceProcessInstaller(ServiceProcessInstaller installer);
    }
}