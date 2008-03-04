namespace MassTransit.ServiceBus.Tests.Subscriptions
{
	using System;
	using MassTransit.ServiceBus.Subscriptions;
	using NUnit.Framework;
	using NUnit.Framework.SyntaxHelpers;
	using Rhino.Mocks;

	[TestFixture]
	public class When_Working_With_Subscriptions
	{
		private readonly string mockPath = "msmq://localhost/bob";

		[Test]
		public void Add_Subscription()
		{
			LocalSubscriptionCache cache = new LocalSubscriptionCache();

			cache.Add(new Subscription(typeof (PingMessage).FullName, new Uri(mockPath)));

			Assert.That(cache.List().Count, Is.EqualTo(1));
		}

		[Test]
		public void Add_Subscription_Idempotent()
		{
			int count = 0;
			LocalSubscriptionCache cache = new LocalSubscriptionCache();
			cache.OnAddSubscription += delegate { count++; };

			cache.Add(new Subscription(typeof (PingMessage).FullName, new Uri(mockPath)));
			cache.Add(new Subscription(typeof (PingMessage).FullName, new Uri(mockPath)));
			cache.Add(new Subscription(typeof (PingMessage).FullName.ToLowerInvariant(), new Uri(mockPath)));

			Assert.That(cache.List().Count, Is.EqualTo(2));
			Assert.That(count, Is.EqualTo(2));
		}


		[Test]
		public void Event_fires_on_add()
		{
			bool didEventFire = false;
			LocalSubscriptionCache cache = new LocalSubscriptionCache();
			cache.OnAddSubscription += delegate { didEventFire = true; };

			cache.Add(new Subscription(typeof(PingMessage).FullName, new Uri(mockPath)));

			Assert.IsTrue(didEventFire);
		}

		[Test]
		public void Event_fires_on_remove()
		{
			bool didEventFire = false;
			LocalSubscriptionCache cache = new LocalSubscriptionCache();
			cache.Add(new Subscription(typeof(PingMessage).FullName, new Uri(mockPath)));

			cache.OnRemoveSubscription += delegate { didEventFire = true; };
			cache.Remove(new Subscription(typeof(PingMessage).FullName, new Uri(mockPath)));

			Assert.IsTrue(didEventFire);
		}

		[Test]
		public void Remove_subscription()
		{
			LocalSubscriptionCache cache = new LocalSubscriptionCache();
			cache.Add(new Subscription(typeof(PingMessage).FullName, new Uri(mockPath)));
			Assert.That(cache.List().Count, Is.EqualTo(1));

			cache.Remove(new Subscription(typeof(PingMessage).FullName, new Uri(mockPath)));
			Assert.That(cache.List().Count, Is.EqualTo(0));
		}

		[Test]
		public void Remove_subscription_Idempotent()
		{
			LocalSubscriptionCache cache = new LocalSubscriptionCache();
			cache.Add(new Subscription(typeof(PingMessage).FullName, new Uri(mockPath)));
			Assert.That(cache.List().Count, Is.EqualTo(1));

			cache.Remove(new Subscription(typeof(PingMessage).FullName, new Uri(mockPath)));
			cache.Remove(new Subscription(typeof(PingMessage).FullName, new Uri(mockPath)));
			Assert.That(cache.List().Count, Is.EqualTo(0));
		}
	}
}