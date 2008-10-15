namespace SubMgr
{
    using Castle.Core;
    using MassTransit.Host.LifeCycles;
    using MassTransit.ServiceBus;
    using MassTransit.ServiceBus.Subscriptions;
    using MassTransit.ServiceBus.Subscriptions.ServerHandlers;

    public class SubscriptionManagerLifeCycle :
        HostedLifeCycle
    {
        public SubscriptionManagerLifeCycle(string xmlFile) : base(xmlFile)
        {
        }

        public override void Start()
        {
            Container.AddComponentLifeStyle<FollowerRepository>(LifestyleType.Singleton);

            
            Container.AddComponent<AddSubscriptionHandler>();
            Container.AddComponent<RemoveSubscriptionHandler>();
            Container.AddComponent<CancelUpdatesHandler>();
            Container.AddComponent<CacheUpdateRequestHandler>();
            Container.AddComponent<IHostedService, SubscriptionService>();


            Container.AddComponent<ISubscriptionRepository, InMemorySubscriptionRepository>();
        }

        public override void Stop()
        {
            //do nothing
        }
    }
}