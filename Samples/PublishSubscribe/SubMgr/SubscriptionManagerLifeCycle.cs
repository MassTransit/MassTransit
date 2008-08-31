namespace SubMgr
{
    using MassTransit.Host;
    using MassTransit.ServiceBus;
    using MassTransit.ServiceBus.Subscriptions;
    using MassTransit.SubscriptionStorage;

    public class SubscriptionManagerLifeCycle :
        HostedLifeCycle
    {
        public SubscriptionManagerLifeCycle(string xmlFile) : base(xmlFile)
        {
        }

        public override void Start()
        {
            Container.AddComponent<IHostedService, SubscriptionService>();
            Container.AddComponent<ISubscriptionRepository, NHibernateSubscriptionStorage>();
        }

        public override void Stop()
        {
            //do nothing
        }
    }
}