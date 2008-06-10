namespace SubMgr
{
    using log4net.Config;
    using MassTransit.Host2;

    internal class Program
    {
        private static void Main(string[] args)
        {
            XmlConfigurator.Configure();

            HostedEnvironment env = new SubscriptionManagerEnvironment("castle.xml");

            Runner.Run(env, args);
        }
    }
    
}