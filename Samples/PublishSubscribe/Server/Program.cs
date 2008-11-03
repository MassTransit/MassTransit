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
            var container = new DefaultMassTransitContainer("castle.xml");

            var wob = new WindsorObjectBuilder(container.Kernel);
            ServiceLocator.SetLocatorProvider(() => wob);

            var credentials = Credentials.LocalSystem;
            var settings = WinServiceSettings.Custom(
                "SampleService",
                "Sample Service",
                "Something",
                KnownServiceNames.Msmq);
            var lifecycle = new ServerLifeCycle(ServiceLocator.Current);

            Runner.Run(credentials, settings, lifecycle, args);
        }
    }
}