namespace MassTransit.ServiceBus.Tests.Subscriptions
{
	using System;
	using System.Collections.Generic;
	using MassTransit.ServiceBus.Subscriptions;
	using MassTransit.ServiceBus.Subscriptions.Messages;
	using NUnit.Framework;
	using Rhino.Mocks;

	[TestFixture]
	public class As_a_SubscriptionService
	{
		private MockRepository mocks;
		private IServiceBus mockBus;
		private ISubscriptionCache cache;
		private ISubscriptionRepository mockRepository;
		private SubscriptionService srv;

		private AddSubscription msgAdd;
		private RemoveSubscription msgRem;
		private CacheUpdateRequest msgUpdate;
		private CancelSubscriptionUpdates msgCancel;

		[SetUp]
		public void I_want_to()
		{
			mocks = new MockRepository();
			mockBus = mocks.CreateMock<IServiceBus>();
			mockRepository = mocks.CreateMock<ISubscriptionRepository>();

			msgAdd = new AddSubscription("bob", new Uri("queue:\\bob"));
			msgRem = new RemoveSubscription("bob", new Uri("queue:\\bob"));
			msgUpdate = new CacheUpdateRequest();
			msgCancel = new CancelSubscriptionUpdates();


			cache = new LocalSubscriptionCache();
			srv = new SubscriptionService(mockBus, cache, mockRepository);
		}

		[Test]
		public void add_subscriptions_from_messages()
		{
			using (mocks.Record())
			{
				Expect.Call(delegate { mockRepository.Save(msgAdd.Subscription); });
			}
			using (mocks.Playback())
			{
				srv.Consume(msgAdd);
			}
		}


		[Test]
		public void be_alive()
		{
			Assert.IsNotNull(srv);
		}

		[Test]
		public void be_startable()
		{
			IEnumerable<Subscription> enumer = new List<Subscription>();

			using (mocks.Record())
			{
				Expect.Call(mockRepository.List()).Return(enumer);
				Expect.Call(delegate { mockBus.Subscribe(srv); });
			}
			using (mocks.Playback())
			{
				srv.Start();
			}
		}

		[Test]
		public void be_stopable()
		{
			using (mocks.Record())
			{
				Expect.Call(delegate { mockBus.Unsubscribe(srv); });
			}
			using (mocks.Playback())
			{
				srv.Stop();
			}
		}

		[Test]
		public void remove_subscriptions_from_messages()
		{
			using (mocks.Record())
			{
				Expect.Call(delegate { mockRepository.Remove(msgRem.Subscription); });
			}
			using (mocks.Playback())
			{
				srv.Consume(msgRem);
			}
		}

		[Test]
		public void respond_to_cache_updates()
		{
			using (mocks.Record())
			{
			    Expect.Call(delegate { this.mockBus.Publish<CacheUpdateResponse>(null); }).IgnoreArguments();
                
			}
			using (mocks.Playback())
			{
				srv.Consume(msgUpdate);
			}
		}

		[Test]
		public void respond_to_update_cancel()
		{
			using (mocks.Record())
			{
			}
			using (mocks.Playback())
			{
				srv.Consume(msgCancel);
			}
		}
	}
}