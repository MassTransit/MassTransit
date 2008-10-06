namespace SubscriptionServiceHost
{
    using System.IO;
    using log4net;
    using log4net.Config;
    using MassTransit.Host;

    internal class Program
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof (Program));

        private static void Main(string[] args)
        {
            XmlConfigurator.ConfigureAndWatch(new FileInfo("log4net.xml"));
            _log.Info("SubMgr Loading");

            var env = new SubscriptionManagerConfiguration("pubsub.castle.xml");


            Runner.Run(env, args);
        }
    }
}