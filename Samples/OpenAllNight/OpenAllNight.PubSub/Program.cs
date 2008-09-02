using log4net.Config;

[assembly: XmlConfigurator(ConfigFile = "log4net.xml", Watch = true)]

namespace OpenAllNight.PubSub
{
    using log4net;
    using MassTransit.Host;
    using MassTransit.Host.Configurations;

    internal class Program
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof (Program));

        private static void Main(string[] args)
        {
            _log.Info("SubMgr Loading");

            IInstallationConfiguration cfg = new SubscriptionManagerEnvironment("pubsub.castle.xml");

            Runner.Run(cfg, args);
        }
    }
}