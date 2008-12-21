namespace SubMgr
{
    using Castle.Core;
    using Castle.Windsor;
    using log4net;
    using MassTransit;
    using MassTransit.Host;
    using MassTransit.Services.Subscriptions;
    using MassTransit.Services.Subscriptions.Server;
    using MassTransit.WindsorIntegration;
    using Microsoft.Practices.ServiceLocation;

    internal class Program
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof (Program));

        private static void Main(string[] args)
        {
            _log.Info("SubMgr Loading");
            IWindsorContainer container = new DefaultMassTransitContainer("castle.xml");

            container.AddComponentLifeStyle<FollowerRepository>(LifestyleType.Singleton);


            container.AddComponent<RemoteEndpointCoordinator>();
            container.AddComponent<IHostedService, SubscriptionService>();


            container.AddComponent<ISubscriptionRepository, InMemorySubscriptionRepository>();

            var wob = new WindsorObjectBuilder(container.Kernel);
            ServiceLocator.SetLocatorProvider(()=>wob);

            var credentials = Credentials.LocalSystem;
            var settings = WinServiceSettings.Custom(
                "SampleSubscriptionService",
                "MassTransit Sample Subscription Service",
                "Coordinates subscriptions between multiple systems",
                KnownServiceNames.Msmq);
            var lifecycle = new SubscriptionManagerLifeCycle(ServiceLocator.Current);

            Runner.Run(credentials, settings, lifecycle, args);
        }
    }
}