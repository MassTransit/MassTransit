namespace OpenAllNight.PubSub
{
    using Castle.Core;
    using MassTransit.Host;
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
            Container.AddComponentLifeStyle("followerrepository", typeof(FollowerRepository), LifestyleType.Singleton);
            Container.AddComponentLifeStyle("addsubscriptionhandler", typeof(AddSubscriptionHandler), LifestyleType.Transient);
            Container.AddComponentLifeStyle("removesubscriptionhandler", typeof(RemoveSubscriptionHandler), LifestyleType.Transient);
            Container.AddComponentLifeStyle("cacheupdaterequesthandler", typeof(CacheUpdateRequestHandler), LifestyleType.Transient);
            Container.AddComponentLifeStyle("cancelupdaterequesthandler", typeof(CancelUpdatesHandler), LifestyleType.Transient);

            Container.AddComponent<IHostedService, SubscriptionService>();

            Container.AddComponent<ISubscriptionRepository, InMemorySubscriptionRepository>();
        }

        public override void Stop()
        {
            //do nothing
        }
    }
}