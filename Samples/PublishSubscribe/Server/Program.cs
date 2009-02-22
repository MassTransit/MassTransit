namespace Server
{
    using System.IO;
    using log4net;
    using MassTransit.Services.Subscriptions.Server;
    using MassTransit.WindsorIntegration;
    using Microsoft.Practices.ServiceLocation;
    using Topshelf;
    using Topshelf.Configuration;

    internal class Program
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof (Program));

        private static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.ConfigureAndWatch(new FileInfo("server.log4net.xml"));
            _log.Info("Server Loading");

            var cfg = RunnerConfigurator.New(c =>
                                                 {
                                                     c.SetServiceName("SampleService");
                                                     c.SetServiceName("Sample Service");
                                                     c.SetServiceName("Something");
                                                     c.DependencyOnMsmq();

                                                     c.RunAsLocalSystem();

                                                     c.BeforeStart(a=>
                                                                       {
                                                                           var container = new DefaultMassTransitContainer("server.castle.xml");
                                                                           container.AddComponent<RemoteEndpointCoordinator>();

                                                                           var wob = new WindsorObjectBuilder(container.Kernel);
                                                                           ServiceLocator.SetLocatorProvider(() => wob);
                                                                       });

                                                     c.ConfigureService<ServerLifeCycle>();
                                                 });
            Runner.Host(cfg, args);
        }
    }
}