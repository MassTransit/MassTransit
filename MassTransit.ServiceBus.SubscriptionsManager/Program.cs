namespace MassTransit.ServiceBus.SubscriptionsManager
{
    using System.ServiceProcess;
    using NHibernate;

    public class Program : ServiceBase
    {
        private SubscriptionServiceBus bus;

        protected override void OnStart(string[] args)
        {
            base.OnStart(args);

            ISessionFactory sessionFactory = null;
            bus = new SubscriptionServiceBus(new MessageQueueEndpoint("msmq://localhost/subscriptionMgr"), new SubscriptionRepository(sessionFactory));
            
        }

        protected override void OnStop()
        {
            base.OnStop();
            bus.Dispose();
        }
    }
}