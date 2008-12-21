namespace Server
{
    using System.IO;
    using log4net;
    using MassTransit.Host;
    using MassTransit.Services.Subscriptions.Server;
    using MassTransit.WindsorIntegration;
    using Microsoft.Practices.ServiceLocation;

    internal class Program
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof (Program));

        private static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.ConfigureAndWatch(new FileInfo("server.log4net.xml"));
            _log.Info("Server Loading");
            var container = new DefaultMassTransitContainer("server.castle.xml");
            container.AddComponent<RemoteEndpointCoordinator>();

            var wob = new WindsorObjectBuilder(container.Kernel);
            ServiceLocator.SetLocatorProvider(() => wob);

            var credentials = Credentials.LocalSystem;
            var settings = WinServiceSettings.Custom(
                "SampleService",
                "Sample Service",
                "Something",
                KnownServiceNames.Msmq);
            var lifecycle = new ServerLifeCycle(ServiceLocator.Current);

            _log.Info("Server Loaded");
            Runner.Run(credentials, settings, lifecycle, args);
        }
    }
}