namespace MassTransit.ServiceBus.Tests.Subscriptions
{
	using System;
	using MassTransit.ServiceBus.Subscriptions;
	using NUnit.Framework;
	using NUnit.Framework.SyntaxHelpers;

	[TestFixture]
	public class As_Local_Subscription_Storage
	{
		[SetUp]
		public void SetUp()
		{
		}

		[TearDown]
		public void TearDown()
		{
		}

		[Test]
		public void Add_With_Event()
		{
			Uri sendTo = new Uri("msmq://localhost/test");
			bool wasFired = false;
			LocalSubscriptionCache cache = new LocalSubscriptionCache();
			cache.OnAddSubscription += delegate(object sender, SubscriptionEventArgs e)
			                           	{
			                           		wasFired = true;
			                           		Assert.That(e.Subscription.EndpointUri, Is.EqualTo(sendTo));
			                           		Assert.That(e.Subscription.MessageName,
			                           		            Is.EqualTo(typeof (PingMessage).FullName));
			                           	};

			cache.Add(new Subscription(typeof (PingMessage).FullName, sendTo));

			Assert.That(cache.List().Count, Is.EqualTo(1));
			Assert.That(wasFired, Is.True);
		}

		[Test]
		public void Adding_Subscription()
		{
			LocalSubscriptionCache cache = new LocalSubscriptionCache();
			cache.Add(new Subscription(typeof (PingMessage).FullName, new Uri("msmq://localhost/test")));

			Assert.That(cache.List().Count, Is.EqualTo(1));
		}

		[Test]
		public void Event_should_not_fire_if_subscription_exists()
		{
			Uri sendTo = new Uri("msmq://localhost/test");
			bool wasFired = false;
			LocalSubscriptionCache cache = new LocalSubscriptionCache();
			cache.Add(new Subscription(typeof (PingMessage).FullName, sendTo));
			cache.OnAddSubscription += delegate(object sender, SubscriptionEventArgs e)
			                           	{
			                           		wasFired = true;
			                           		Assert.That(e.Subscription.EndpointUri, Is.EqualTo(sendTo));
			                           		Assert.That(e.Subscription.MessageName,
			                           		            Is.EqualTo(typeof (PingMessage).FullName));
			                           	};

			cache.Add(new Subscription(typeof (PingMessage).FullName, sendTo));

			Assert.That(cache.List().Count, Is.EqualTo(1));
			Assert.That(wasFired, Is.False);
		}

		[Test]
		public void Listing_Subscriptions_By_Name()
		{
			LocalSubscriptionCache cache = new LocalSubscriptionCache();
			cache.Add(new Subscription(typeof (PingMessage).FullName, new Uri("msmq://localhost/test")));
			cache.Add(new Subscription(typeof (PongMessage).FullName, new Uri("msmq://localhost/test")));

			Assert.That(cache.List(typeof (PingMessage).FullName).Count, Is.EqualTo(1));
			Assert.That(cache.List(typeof (PongMessage).FullName).Count, Is.EqualTo(1));
		}

		[Test]
		public void Remove_With_Event()
		{
			Uri sendTo = new Uri("msmq://localhost/test");
			bool wasFired = false;
			LocalSubscriptionCache cache = new LocalSubscriptionCache();
			cache.Add(new Subscription(typeof (PingMessage).FullName, sendTo));
			Assert.That(cache.List().Count, Is.EqualTo(1));

			cache.OnRemoveSubscription += delegate(object sender, SubscriptionEventArgs e)
			                              	{
			                              		wasFired = true;
			                              		Assert.That(e.Subscription.EndpointUri, Is.EqualTo(sendTo));
			                              		Assert.That(e.Subscription.MessageName,
			                              		            Is.EqualTo(typeof (PingMessage).FullName));
			                              	};
			cache.Remove(new Subscription(typeof (PingMessage).FullName, sendTo));

			Assert.That(wasFired, Is.True);
		}

		[Test]
		public void Removing_Subscription()
		{
			LocalSubscriptionCache cache = new LocalSubscriptionCache();
			cache.Add(new Subscription(typeof (PingMessage).FullName, new Uri("msmq://localhost/test")));

			Assert.That(cache.List().Count, Is.EqualTo(1));

			cache.Remove(new Subscription(typeof (PingMessage).FullName, new Uri("msmq://localhost/test")));
			Assert.That(cache.List().Count, Is.EqualTo(0));
		}
	}
}