namespace OpenAllNight.PubSub
{
    using MassTransit.Host;
    using MassTransit.Host.Configurations;

    public class SubscriptionManagerEnvironment :
        LocalSystemConfiguration
    {
        private readonly IApplicationLifeCycle _lifeCycle;

        public SubscriptionManagerEnvironment(string xmlFile)
        {
            _lifeCycle = new SubscriptionManagerLifeCycle(xmlFile);
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