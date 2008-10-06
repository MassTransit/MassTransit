namespace HealthServiceHost
{
    using System.IO;
    using log4net;
    using MassTransit.Host;

    internal class Program
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof (Program));

        private static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure(new FileInfo("log4net.xml"));
            _log.Info("Health Server Loading");

            HealthManagerConfiguration cfg = new HealthManagerConfiguration("health.castle.xml");

            Runner.Run(cfg, args);
        }
    }
}