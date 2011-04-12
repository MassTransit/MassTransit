namespace MassTransit.Tests.Subscriptions
{
	using Magnum;
	using Magnum.Extensions;
	using MassTransit.Transports;
	using NUnit.Framework;
	using TestFramework;
	using TextFixtures;

	[TestFixture]
	public class Removing_a_subscription_client :
		SubscriptionServiceTestFixture<LoopbackTransportFactory>
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

	[TestFixture]
	public class Removing_a_subscription_client_and_readding_it :
		SubscriptionServiceTestFixture<LoopbackTransportFactory>
	{
		[Test]
		public void Should_remove_any_previous_subscriptions()
		{
			RemoteBus.Subscribe<A>(x => { });

			LocalBus.ShouldHaveSubscriptionFor<A>();

			RemoteBus.Dispose();

			ThreadUtil.Sleep(1.Seconds());

			SetupRemoteBus();

			LocalBus.ShouldNotHaveSubscriptionFor<A>();
		}


		class A
		{ }
	}
}
