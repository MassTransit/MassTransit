namespace SubMgr
{
    using MassTransit.Host;
    using MassTransit.Host.Configurations;
    using MassTransit.Host.LifeCycles;

    public class SubscriptionManagerEnvironment :
		LocalSystemConfiguration
	{
        private IApplicationLifeCycle _lifeCycle;

		public SubscriptionManagerEnvironment(string xmlFile)
		{
		    _lifeCycle = new SubscriptionManagerLifeCycle(xmlFile);
		}

	    public override string ServiceName
		{
			get { return "SampleSubscriptionService"; }
		}

		public override string DispalyName
		{
			get { return "MassTransit Sample Subscription Service"; }
		}

		public override string Description
		{
			get { return "Coordinates subscriptions between multiple systems"; }
		}

	    public override IApplicationLifeCycle LifeCycle
	    {
            get { return _lifeCycle; }
	    }
	}
}