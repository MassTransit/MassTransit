namespace OpenAllNight.PubSub
{
    using MassTransit.Host;
    using MassTransit.Host.Configurations;
    using Microsoft.Practices.ServiceLocation;

    public class SubscriptionManagerEnvironment :
        LocalSystemConfiguration
    {
        private readonly IApplicationLifeCycle _lifeCycle;

        public SubscriptionManagerEnvironment(IServiceLocator serviceLocator)
        {
            _lifeCycle = new SubscriptionManagerLifeCycle(serviceLocator);
        }

        public override IApplicationLifeCycle LifeCycle
        {
            get { return _lifeCycle; }
        }

        public override string ServiceName
        {
            get { return "MassTransit Subscription Manager"; }
        }

        public override string DisplayName
        {
            get { return "MassTransit Subscription Manager"; }
        }

        public override string Description
        {
            get { return "This service manages the subscriptions for Mass Transit"; }
        }
    }
}