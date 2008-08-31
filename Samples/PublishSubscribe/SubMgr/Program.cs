namespace SubMgr
{
    using log4net;
    using MassTransit.Host;

    internal class Program
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof (Program));

        private static void Main(string[] args)
        {
            _log.Info("SubMgr Loading");

            HostedEnvironment env = new SubscriptionManagerEnvironment("castle.xml");

            Runner.Run(env, args);
        }
    }
}