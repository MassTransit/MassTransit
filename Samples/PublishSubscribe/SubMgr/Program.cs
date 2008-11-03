namespace SubMgr
{
    using Castle.Core;
    using Castle.Windsor;
    using log4net;
    using MassTransit.Host;
    using MassTransit.Host.Configurations;
    using MassTransit.ServiceBus;
    using MassTransit.ServiceBus.Subscriptions;
    using MassTransit.ServiceBus.Subscriptions.ServerHandlers;
    using MassTransit.WindsorIntegration;
    using Microsoft.Practices.ServiceLocation;

    internal class Program
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof (Program));

        private static void Main(string[] args)
        {
            _log.Info("SubMgr Loading");
            IWindsorContainer Container = new DefaultMassTransitContainer("castle.xml");

            Container.AddComponentLifeStyle<FollowerRepository>(LifestyleType.Singleton);


            Container.AddComponent<AddSubscriptionHandler>();
            Container.AddComponent<RemoveSubscriptionHandler>();
            Container.AddComponent<CancelUpdatesHandler>();
            Container.AddComponent<CacheUpdateRequestHandler>();
            Container.AddComponent<IHostedService, SubscriptionService>();


            Container.AddComponent<ISubscriptionRepository, InMemorySubscriptionRepository>();

            IInstallationConfiguration cfg = new WinServiceConfiguration(
                Credentials.LocalSystem,
                WinServiceSettings.Custom(
                    "SampleSubscriptionService",
                    "MassTransit Sample Subscription Service",
                    "Coordinates subscriptions between multiple systems", KnownServiceNames.Msmq),
                new SubscriptionManagerLifeCycle(ServiceLocator.Current));

            Runner.Run(cfg, args);
        }
    }
}