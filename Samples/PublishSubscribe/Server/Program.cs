namespace Server
{
    using log4net;
    using MassTransit.Host;
    using MassTransit.Host.Configurations;
    using MassTransit.WindsorIntegration;
    using Microsoft.Practices.ServiceLocation;

    internal class Program
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof (Program));

        private static void Main(string[] args)
        {
            _log.Info("Server Loading");
            var Container = new DefaultMassTransitContainer("castle.xml");

            IInstallationConfiguration cfg = new WinServiceConfiguration(
                Credentials.LocalSystem,
                WinServiceSettings.Custom(
                    "SampleService",
                    "Sample Service",
                    "Something",
                    KnownServiceNames.Msmq),
                new ServerLifeCycle(ServiceLocator.Current));

            Runner.Run(cfg, args);
        }
    }
}