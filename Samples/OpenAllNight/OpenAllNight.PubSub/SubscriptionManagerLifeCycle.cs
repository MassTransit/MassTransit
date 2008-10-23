namespace OpenAllNight.PubSub
{
    using MassTransit.Host.LifeCycles;
    using Microsoft.Practices.ServiceLocation;

    public class SubscriptionManagerLifeCycle :
        HostedLifeCycle
    {
        public SubscriptionManagerLifeCycle(IServiceLocator serviceLocator) : base(serviceLocator)
        {
        }

        public override void Start()
        {
        }

        public override void Stop()
        {
            //do nothing
        }
    }
}