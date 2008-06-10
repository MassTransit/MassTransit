namespace SubMgr
{
    using MassTransit.Host2;

    public class SubscriptionManagerEnvironment :
        HostedEnvironment
    {
        public SubscriptionManagerEnvironment()
        {
        }

        public SubscriptionManagerEnvironment(string xmlFile) : base(xmlFile)
        {
        }

        public override string ServiceName
        {
            get { return "MassTransitPubSub"; }
        }

        public override string DispalyName
        {
            get { return "Mass Transit Publish Subscribe Service"; }
        }

        public override string Description
        {
            get { return "Manages Subscriptions"; }
        }
    }
}