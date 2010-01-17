namespace LegacyRuntime
{
    using System;
    using System.IO;
    using log4net;
    using log4net.Config;
    using Magnum.Reflection;
    using MassTransit.LegacySupport;
    using MassTransit.StructureMapIntegration;
    using MassTransit.Transports.Msmq;
    using StructureMap;
    using StructureMap.Configuration.DSL;
    using Topshelf;
    using Topshelf.Configuration;

    class Program
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(Program));

        static void Main(string[] args)
        {
            BootstrapLogger();

            MsmqEndpointConfigurator.Defaults(x =>
            {
                x.CreateMissingQueues = true;
            });

            var cfg = RunnerConfigurator.New(config =>
            {
                config.SetServiceName(typeof(Program).Namespace);
                config.SetDisplayName(typeof(Program).Namespace);
                config.SetDescription("MassTransit Legacy Services");

                config.RunAsLocalSystem();

                config.DependencyOnMsmq();
                config.DependencyOnMsSql();

                config.ConfigureService<LegacySubscriptionProxyService>("name", s =>
                {
                    ConfigureService<LegacySubscriptionProxyService, LegacySupportRegistry>(s, start=>start.Start(), stop=>stop.Stop());
                });

                config.AfterStoppingTheHost(x => { _log.Info("MassTransit Legacy Services are exiting..."); });
            });

            Runner.Host(cfg, args);
        }
        private static void BootstrapLogger()
        {
            var configFileName = AppDomain.CurrentDomain.BaseDirectory + Path.DirectorySeparatorChar + typeof(Program).Namespace + ".log4net.xml";

            XmlConfigurator.ConfigureAndWatch(new FileInfo(configFileName));

            _log.Info("Loading " + typeof(Program).Namespace + " Services...");
        }
        private static void ConfigureService<TService, TRegistry>(IServiceConfigurator<TService> service, Action<TService> start, Action<TService> stop)
    where TRegistry : Registry
        {
            service.CreateServiceLocator(() =>
            {
                var container = new Container(x =>
                {
                    x.ForRequestedType<IConfiguration>()
                        .CacheBy(InstanceScope.Singleton)
                        .AddConcreteType<Configuration>();

                    x.ForRequestedType<TService>()
                        .AddInstances(i => i.OfConcreteType<TService>().WithName(typeof(TService).Name));
                });

                TRegistry registry = FastActivator<TRegistry>.Create(container);

                container.Configure(x => x.AddRegistry(registry));

                return new StructureMapObjectBuilder(container);
            });
            service.WhenStarted(start);
            service.WhenStopped(stop);
        }
    }
}
