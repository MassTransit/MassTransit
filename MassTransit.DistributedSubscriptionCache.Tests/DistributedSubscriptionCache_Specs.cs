namespace MassTransit.DistributedSubscriptionCache.Tests
{
	using System;
	using System.Collections.Generic;
	using Enyim.Caching;
	using MassTransit.ServiceBus.Subscriptions;
	using NUnit.Framework;
	using NUnit.Framework.SyntaxHelpers;

	[TestFixture]
	public class When_using_a_distributed_subscription_cache
	{
		private readonly string _name = "CustomMessageName";
		private readonly string _correlationId = Guid.NewGuid().ToString();

		[SetUp]
		public void Before_each()
		{
			MemcachedClient client = new MemcachedClient();

			client.Remove("/mt/" + _name);
		}

		[Test]
		public void The_subscriptions_should_be_synchronized_between_subscription_caches()
		{
			DistributedSubscriptionCache cacheA = new DistributedSubscriptionCache();
			DistributedSubscriptionCache cacheB = new DistributedSubscriptionCache();

			const string url = "http://localhost/default.html";

			Subscription sample = new Subscription(_name, new Uri(url));

			cacheA.Add(sample);

			IList<Subscription> subscriptions = cacheB.List(_name);

			Assert.That(subscriptions.Count, Is.EqualTo(1));
			Assert.That(subscriptions[0].MessageName, Is.EqualTo(_name));
			Assert.That(subscriptions[0].EndpointUri.ToString(), Is.EqualTo(url));
		}

		[Test]
		public void Multiple_subscriptions_to_the_same_endpoint_should_be_synchronized()
		{
			DistributedSubscriptionCache cacheA = new DistributedSubscriptionCache();
			DistributedSubscriptionCache cacheB = new DistributedSubscriptionCache();

			string urlA = "http://localhost/default.html";
			Subscription sample = new Subscription(_name, new Uri(urlA));
			cacheA.Add(sample);

			string urlB = "http://localhost/index.html";
			sample = new Subscription(_name, new Uri(urlB));
			cacheB.Add(sample);

			IList<Subscription> subscriptions = cacheA.List(_name);

			Assert.That(subscriptions.Count, Is.EqualTo(2));
			Assert.That(subscriptions[0].MessageName, Is.EqualTo(_name));
			Assert.That(subscriptions[0].EndpointUri.ToString(), Is.EqualTo(urlA));
			Assert.That(subscriptions[1].MessageName, Is.EqualTo(_name));
			Assert.That(subscriptions[1].EndpointUri.ToString(), Is.EqualTo(urlB));
		}

		[Test]
		public void Removing_a_subscription_should_mean_it_no_longer_exists()
		{
			DistributedSubscriptionCache cacheA = new DistributedSubscriptionCache();
			DistributedSubscriptionCache cacheB = new DistributedSubscriptionCache();

			string urlA = "http://localhost/default.html";
			Subscription sample = new Subscription(_name, new Uri(urlA));
			cacheA.Add(sample);

			string urlB = "http://localhost/index.html";
			Subscription sampleB = new Subscription(_name, new Uri(urlB));
			cacheB.Add(sampleB);

			IList<Subscription> subscriptions = cacheA.List(_name);

			Assert.That(subscriptions.Count, Is.EqualTo(2));
			Assert.That(subscriptions[0].MessageName, Is.EqualTo(_name));
			Assert.That(subscriptions[0].EndpointUri.ToString(), Is.EqualTo(urlA));
			Assert.That(subscriptions[1].MessageName, Is.EqualTo(_name));
			Assert.That(subscriptions[1].EndpointUri.ToString(), Is.EqualTo(urlB));

			cacheB.Remove(sample);

			subscriptions = cacheA.List(_name);

			Assert.That(subscriptions.Count, Is.EqualTo(1));
			Assert.That(subscriptions[0].MessageName, Is.EqualTo(_name));
			Assert.That(subscriptions[0].EndpointUri.ToString(), Is.EqualTo(urlB));
		}

		[Test]
		public void Removing_a_subscription_should_mean_it_no_longer_exists_first()
		{
			DistributedSubscriptionCache cacheA = new DistributedSubscriptionCache();
			DistributedSubscriptionCache cacheB = new DistributedSubscriptionCache();

			string urlA = "http://localhost/default.html";
			Subscription sample = new Subscription(_name, new Uri(urlA));
			cacheA.Add(sample);

			string urlB = "http://localhost/index.html";
			Subscription sampleB = new Subscription(_name, new Uri(urlB));
			cacheB.Add(sampleB);

			IList<Subscription> subscriptions = cacheA.List(_name);

			Assert.That(subscriptions.Count, Is.EqualTo(2));
			Assert.That(subscriptions[0].MessageName, Is.EqualTo(_name));
			Assert.That(subscriptions[0].EndpointUri.ToString(), Is.EqualTo(urlA));
			Assert.That(subscriptions[1].MessageName, Is.EqualTo(_name));
			Assert.That(subscriptions[1].EndpointUri.ToString(), Is.EqualTo(urlB));

			cacheB.Remove(sampleB);

			subscriptions = cacheA.List(_name);

			Assert.That(subscriptions.Count, Is.EqualTo(1));
			Assert.That(subscriptions[0].MessageName, Is.EqualTo(_name));
			Assert.That(subscriptions[0].EndpointUri.ToString(), Is.EqualTo(urlA));
		}

		[Test]
		public void A_subscription_to_a_correlated_message_type_should_only_match_a_correlated_consumer()
		{
			DistributedSubscriptionCache cacheA = new DistributedSubscriptionCache();

			const string url = "http://localhost/default.html";

			Subscription sample = new Subscription(_name, _correlationId, new Uri(url));

			cacheA.Add(sample);

			IList<Subscription> subscriptions = cacheA.List(_name, _correlationId);

			Assert.That(subscriptions.Count, Is.EqualTo(1));
			Assert.That(subscriptions[0].MessageName, Is.EqualTo(_name));
			Assert.That(subscriptions[0].CorrelationId, Is.EqualTo(_correlationId));
			Assert.That(subscriptions[0].EndpointUri.ToString(), Is.EqualTo(url));
		}

		[Test]
		public void A_subscription_to_a_correlated_message_type_should_match_a_correlated_consumer_and_a_regular_consumer()
		{
			DistributedSubscriptionCache cacheA = new DistributedSubscriptionCache();

			const string url = "http://localhost/default.html";

			Subscription sample = new Subscription(_name, _correlationId, new Uri(url));
			cacheA.Add(sample);
			Subscription allMessages = new Subscription(_name, new Uri(url));
			cacheA.Add(allMessages);

			IList<Subscription> subscriptions = cacheA.List(_name, _correlationId);

			Assert.That(subscriptions.Count, Is.EqualTo(2));
			Assert.That(subscriptions[0].MessageName, Is.EqualTo(_name));
			Assert.That(subscriptions[0].CorrelationId, Is.EqualTo(_correlationId));
			Assert.That(subscriptions[0].EndpointUri.ToString(), Is.EqualTo(url));
			Assert.That(subscriptions[1].MessageName, Is.EqualTo(_name));
			Assert.That(subscriptions[1].CorrelationId, Is.Null);
			Assert.That(subscriptions[1].EndpointUri.ToString(), Is.EqualTo(url));
		}
	}
}