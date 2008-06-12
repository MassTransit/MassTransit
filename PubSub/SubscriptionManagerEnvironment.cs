namespace PubSub
{
    using MassTransit.Host2;

    public class SubscriptionManagerEnvironment :
        HostedEnvironment
    {
        public SubscriptionManagerEnvironment()
        {
        }

        public SubscriptionManagerEnvironment(string xmlFile) 
            : base(xmlFile)
        {
        }

        public override string ServiceName
        {
            get { return "MassTransit Subscription Manager"; }
        }

        public override string DispalyName
        {
            get { return "MassTransit Subscription Manager"; }
        }

        public override string Description
        {
            get { return "This service manages the subscriptions for Mass Transit"; }
        }
    }
}