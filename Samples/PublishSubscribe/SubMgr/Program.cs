namespace SubMgr
{
    using log4net.Config;
    using MassTransit.Host2;
    using MassTransit.ServiceBus;
    using MassTransit.ServiceBus.Subscriptions;
    using MassTransit.SubscriptionStorage;

    internal class Program
    {
        private static void Main(string[] args)
        {
            XmlConfigurator.Configure();

            HostedEnvironment env = new SubscriptionManagerEnvironment("castle.xml");
            env.Container.AddComponent<IHostedService, SubscriptionService>();
            env.Container.AddComponent<ISubscriptionRepository, NHibernateSubscriptionStorage>();
            Runner.Run(env, args);
        }
    }
    
}