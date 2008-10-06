namespace DeferredMessageServiceHost
{
    using System.IO;
    using log4net;
    using log4net.Config;
    using MassTransit.Host;

    class Program
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof (Program));
        static void Main(string[] args)
        {
            XmlConfigurator.ConfigureAndWatch(new FileInfo("log4net.xml"));
            _log.Info("Deferred Message Service Loading");

            var env = new DeferredMessageConfiguration("deferred.castle.xml");


            Runner.Run(env, args);
        }
    }
}