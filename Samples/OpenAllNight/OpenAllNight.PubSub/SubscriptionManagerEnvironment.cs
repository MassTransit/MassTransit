namespace OpenAllNight.PubSub
{
    using MassTransit.Host;

    public class SubscriptionManagerEnvironment :
        HostedEnvironment
    {

        public SubscriptionManagerEnvironment(string xmlFile) 
            : base(xmlFile)
        {
        }


        public override void Main()
        {
            //do nothing
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