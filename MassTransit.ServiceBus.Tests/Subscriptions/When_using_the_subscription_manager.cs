namespace MassTransit.ServiceBus.Tests.Subscriptions
{
	using System;
	using System.Collections.Generic;
	using MassTransit.ServiceBus.Subscriptions;
	using MassTransit.ServiceBus.Subscriptions.Messages;
	using NUnit.Framework;
	using Rhino.Mocks;
	using Rhino.Mocks.Constraints;

	[TestFixture]
	public class When_using_the_subscription_manager
	{
		private MockRepository _mocks;
		private IServiceBus _serviceBus;
		private IEndpoint _managerEndpoint;
		private ISubscriptionCache _cache;

		[SetUp]
		public void Setup()
		{
			_mocks = new MockRepository();
			_cache = _mocks.DynamicMock<ISubscriptionCache>();
			_serviceBus = _mocks.DynamicMock<IServiceBus>();
			_managerEndpoint = _mocks.DynamicMock<IEndpoint>();
		}

		[TearDown]
		public void Teardown()
		{
		}

		[Test]
		public void The_client_should_request_an_update_at_startup()
		{
			using (_mocks.Record())
			{
				_managerEndpoint.Send<CacheUpdateRequest>(null);
				LastCall.IgnoreArguments();
			}

			using (_mocks.Playback())
			{
				using (SubscriptionClient client = new SubscriptionClient(_serviceBus, _cache, _managerEndpoint))
				{
					client.Start();
				}
			}
		}

		[Test]
		public void The_client_should_update_the_cache_when_a_SubscriptionChange_message_is_received()
		{
			Subscription sub = new Subscription("Ho.Pimp, Ho", new Uri("msmq://localhost/test"));

			AddSubscription change = new AddSubscription(sub);

			using (_mocks.Record())
			{
				_cache.Add(sub);
			}

			using (_mocks.Playback())
			{
				using (SubscriptionClient client = new SubscriptionClient(_serviceBus, _cache, _managerEndpoint))
				{
					client.Consume(change);
				}
			}
		}

		[Test]
		public void The_client_should_update_the_local_subscription_cache()
		{
			List<Subscription> subscriptions = new List<Subscription>();
			subscriptions.Add(new Subscription("Ho.Pimp, Ho", new Uri("msmq://localhost/test")));

			CacheUpdateResponse cacheUpdateResponse = new CacheUpdateResponse(subscriptions);

			using (_mocks.Record())
			{
				_cache.Add(subscriptions[0]);
			}

			using (_mocks.Playback())
			{
				using (SubscriptionClient client = new SubscriptionClient(_serviceBus, _cache, _managerEndpoint))
				{
					client.Consume(cacheUpdateResponse);
				}
			}
		}

		[Test]
		public void When_a_local_service_subscribes_to_the_bus_notify_the_manager_of_the_change()
		{
			SubscriptionEventArgs args = new SubscriptionEventArgs(new Subscription("Ho.Pimp, Ho", new Uri("msmq://" + Environment.MachineName.ToLower() + "/test")));

			using (_mocks.Record())
			{
				_managerEndpoint.Send<AddSubscription>(null);
				LastCall
					.IgnoreArguments()
					.Constraints(Is.Matching<AddSubscription>(
					             	delegate(AddSubscription obj)
					             		{
					             			if (obj.Subscription != args.Subscription)
					             				return false;
					             			return true;
					             		}));
			}

			using (_mocks.Playback())
			{
				using (SubscriptionClient client = new SubscriptionClient(_serviceBus, _cache, _managerEndpoint))
				{
					client.Cache_OnAddSubscription(_cache, args);
				}
			}
		}
	}
}