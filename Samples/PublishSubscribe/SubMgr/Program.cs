namespace SubMgr
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

            IInstallationConfiguration cfg = new SubscriptionManagerEnvironment("castle.xml");

            Runner.Run(cfg, args);
        }
    }
}