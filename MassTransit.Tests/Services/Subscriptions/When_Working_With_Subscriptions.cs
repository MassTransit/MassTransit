namespace MassTransit.Tests.Subscriptions
{
	using System;
	using MassTransit.Subscriptions;
	using Messages;
	using NUnit.Framework;
	using NUnit.Framework.SyntaxHelpers;

	[TestFixture]
	public class When_Working_With_Subscriptions
	{
		private readonly string mockPath = "msmq://localhost/bob";

		[Test]
		public void Add_Subscription()
		{
			ISubscriptionCache cache = new LocalSubscriptionCache();

			cache.Add(new Subscription(typeof (PingMessage), new Uri(mockPath)));

			Assert.That(cache.List().Count, Is.EqualTo(1));
		}

		[Test]
		public void Add_Subscription_Idempotent()
		{
			int count = 0;
			ISubscriptionCache cache = new LocalSubscriptionCache();
			cache.OnAddSubscription += delegate { count++; };

			cache.Add(new Subscription(typeof (PingMessage), new Uri(mockPath)));
			cache.Add(new Subscription(typeof (PingMessage), new Uri(mockPath)));
			cache.Add(new Subscription(typeof (PongMessage), new Uri(mockPath)));

			Assert.That(cache.List().Count, Is.EqualTo(2));
			Assert.That(count, Is.EqualTo(2));
		}


		[Test]
		public void Event_fires_on_add()
		{
			bool didEventFire = false;
			ISubscriptionCache cache = new LocalSubscriptionCache();
			cache.OnAddSubscription += delegate { didEventFire = true; };

			cache.Add(new Subscription(typeof (PingMessage), new Uri(mockPath)));

			Assert.IsTrue(didEventFire);
		}

		[Test]
		public void Event_fires_on_remove()
		{
			bool didEventFire = false;
			ISubscriptionCache cache = new LocalSubscriptionCache();
			cache.Add(new Subscription(typeof (PingMessage), new Uri(mockPath)));

			cache.OnRemoveSubscription += delegate { didEventFire = true; };
			cache.Remove(new Subscription(typeof (PingMessage), new Uri(mockPath)));

			Assert.IsTrue(didEventFire);
		}

		[Test]
		public void Remove_subscription()
		{
			ISubscriptionCache cache = new LocalSubscriptionCache();
			cache.Add(new Subscription(typeof (PingMessage), new Uri(mockPath)));
			Assert.That(cache.List().Count, Is.EqualTo(1));

			cache.Remove(new Subscription(typeof (PingMessage), new Uri(mockPath)));
			Assert.That(cache.List().Count, Is.EqualTo(0));
		}

		[Test]
		public void Remove_subscription_Idempotent()
		{
			ISubscriptionCache cache = new LocalSubscriptionCache();
			cache.Add(new Subscription(typeof (PingMessage), new Uri(mockPath)));
			Assert.That(cache.List().Count, Is.EqualTo(1));

			cache.Remove(new Subscription(typeof (PingMessage), new Uri(mockPath)));
			cache.Remove(new Subscription(typeof (PingMessage), new Uri(mockPath)));
			Assert.That(cache.List().Count, Is.EqualTo(0));
		}
	}
}
