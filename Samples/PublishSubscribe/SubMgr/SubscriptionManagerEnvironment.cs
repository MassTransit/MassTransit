namespace SubMgr
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
	}
}