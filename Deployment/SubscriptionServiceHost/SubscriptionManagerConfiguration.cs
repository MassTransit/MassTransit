namespace SubscriptionServiceHost
{
    using MassTransit.Host.Configurations;
    using MassTransit.Host.LifeCycles;

    public class SubscriptionManagerConfiguration :
        InteractiveConfiguration
    {
        private IApplicationLifeCycle _lifecycle;

        public SubscriptionManagerConfiguration(string xmlFile) 
        {
            _lifecycle = new SubscriptionLifeCycle(xmlFile);
        }

        public override IApplicationLifeCycle LifeCycle
        {
            get { return _lifecycle; }
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