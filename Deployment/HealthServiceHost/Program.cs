namespace HealthServiceHost
{
    using System.IO;
    using log4net;
    using MassTransit.Host;
    using MassTransit.ServiceBus;
    using MassTransit.ServiceBus.HealthMonitoring;

    internal class Program
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof (Program));

        private static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure(new FileInfo("log4net.xml"));
            _log.Info("Health Server Loading");

            HostedEnvironment env = new HealthManagerEnvironment("health.castle.xml");

            env.Container.AddComponent<IHostedService, HealthService>();

            env.Container.AddComponent<IHealthCache, LocalHealthCache>();

            Runner.Run(env, args);
        }
    }
}