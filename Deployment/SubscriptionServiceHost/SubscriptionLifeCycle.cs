namespace SubscriptionServiceHost
{
    using Castle.Core;
    using MassTransit.Host.LifeCycles;
    using MassTransit.ServiceBus;
    using MassTransit.ServiceBus.Subscriptions;
    using MassTransit.SubscriptionStorage;

    public class SubscriptionLifeCycle :
        HostedLifeCycle
    {
        public SubscriptionLifeCycle(string xmlFile) : base(xmlFile)
        {
        }

        public override void Start()
        {
            Container.AddComponentLifeStyle("followerrepository", typeof(FollowerRepository), LifestyleType.Singleton);

            Container.AddComponent<IHostedService, SubscriptionService>();

            Container.AddComponent<ISubscriptionRepository, NHibernateSubscriptionStorage>();   
        }

        public override void Stop()
        {
            
        }
    }
}