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
		#region Setup/Teardown

		[SetUp]
		public void Setup()
		{
			_mocks = new MockRepository();
			_cache = _mocks.CreateMock<ISubscriptionStorage>();
			_serviceBus = _mocks.CreateMock<IServiceBus>();
			_managerEndpoint = _mocks.DynamicMock<IEndpoint>();
		}

		[TearDown]
		public void Teardown()
		{
		}

		#endregion

		private MockRepository _mocks;
		private IServiceBus _serviceBus;
		private IEndpoint _managerEndpoint;
		private ISubscriptionStorage _cache;

		[Test]
		public void The_client_should_request_an_update_at_startup()
		{
			IServiceBusAsyncResult asyncResult = _mocks.CreateMock<IServiceBusAsyncResult>();

			using (_mocks.Record())
			{
				Expect.Call(delegate { _cache.OnAddSubscription += null; }).IgnoreArguments();
				Expect.Call(delegate { _cache.OnRemoveSubscription += null; }).IgnoreArguments();
				Expect.Call(delegate { _serviceBus.Subscribe<AddSubscription>(null); }).IgnoreArguments();
				Expect.Call(delegate { _serviceBus.Subscribe<RemoveSubscription>(null); }).IgnoreArguments();
				Expect.Call(_serviceBus.Request<CacheUpdateRequest>(_managerEndpoint, (AsyncCallback)null, (object)null, null))
					.IgnoreArguments()
					.Constraints(Is.Equal(_managerEndpoint),
					             Is.Anything(),
					             Is.Anything(),
					             Is.Matching<CacheUpdateRequest[]>(
					             	delegate(CacheUpdateRequest[] obj) { return true; }))
					.Return(asyncResult);

				_serviceBus.Dispose();
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

			IMessageContext<AddSubscription> context = _mocks.CreateMock<IMessageContext<AddSubscription>>();

			using (_mocks.Record())
			{
				Expect.Call(context.Message).Return(change).Repeat.AtLeastOnce();

				_cache.Add(sub);

				_serviceBus.Dispose();
			}

			using (_mocks.Playback())
			{
				using (SubscriptionClient client = new SubscriptionClient(_serviceBus, _cache, _managerEndpoint))
				{
					client.HandleAddSubscription(context);
				}
			}
		}

		[Test]
		public void The_client_should_update_the_local_subscription_cache()
		{
			IServiceBusAsyncResult asyncResult = _mocks.CreateMock<IServiceBusAsyncResult>();

			List<Subscription> subscriptions = new List<Subscription>();
			subscriptions.Add(new Subscription("Ho.Pimp, Ho", new Uri("msmq://localhost/test")));

			CacheUpdateResponse cacheUpdateResponse = new CacheUpdateResponse(subscriptions);

			using (_mocks.Record())
			{
				Expect.Call(asyncResult.Messages).Return(new IMessage[] {cacheUpdateResponse}).Repeat.AtLeastOnce();

				_cache.Add(subscriptions[0]);
				_serviceBus.Dispose();
			}

			using (_mocks.Playback())
			{
				using (SubscriptionClient client = new SubscriptionClient(_serviceBus, _cache, _managerEndpoint))
				{
					client.CacheUpdateResponse_Callback(asyncResult);
				}
			}
		}

		[Test]
		public void When_a_local_service_subscribes_to_the_bus_notify_the_manager_of_the_change()
		{
			SubscriptionEventArgs args = new SubscriptionEventArgs(new Subscription("Ho.Pimp, Ho", new Uri("msmq://" + Environment.MachineName.ToLower() + "/test")));

			using (_mocks.Record())
			{
				_serviceBus.Send<AddSubscription>(_managerEndpoint, null);
				LastCall
					.IgnoreArguments()
					.Constraints(Is.Equal(_managerEndpoint),
								 Is.Matching<AddSubscription[]>(
									delegate(AddSubscription[] obj)
					             		{
					             			if (obj[0].Subscription != args.Subscription)
					             				return false;
					             			return true;
					             		}));
				_serviceBus.Dispose();
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