namespace MassTransit.Tests.Subscriptions
{
	using Magnum;
	using Magnum.DateTimeExtensions;
	using MassTransit.Transports;
	using NUnit.Framework;
	using TestFramework;
	using TextFixtures;

	[TestFixture]
	public class Removing_a_subscription_client :
		SubscriptionServiceTestFixture<LoopbackEndpoint>
	{
		[Test]
		public void Should_not_remove_any_existing_subscriptions()
		{
			RemoteBus.Subscribe<A>(x => { });
			RemoteBus.Dispose();

			ThreadUtil.Sleep(2.Seconds());

			LocalBus.ShouldHaveSubscriptionFor<A>();
		}


		class A
		{ }
	}
}
