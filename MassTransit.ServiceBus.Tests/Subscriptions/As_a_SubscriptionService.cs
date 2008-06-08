namespace MassTransit.ServiceBus.Tests.Subscriptions
{
	using System;
	using System.Collections.Generic;
	using Internal;
	using MassTransit.ServiceBus.Subscriptions;
	using MassTransit.ServiceBus.Subscriptions.Messages;
	using NUnit.Framework;
	using Rhino.Mocks;

	[TestFixture]
	public class As_a_SubscriptionService :
        Specification
	{
		private IServiceBus mockBus;
		private ISubscriptionCache mockCache;
		private ISubscriptionRepository mockRepository;
		private SubscriptionService srv;
	    private IEndpointResolver mockEndpointResolver;

		private AddSubscription msgAdd;
		private RemoveSubscription msgRem;
		private CacheUpdateRequest msgUpdate;
		private CancelSubscriptionUpdates msgCancel;

	    private Uri uri = new Uri("queue:\\bob");

        protected override void Before_each()
        {
			mockBus = StrictMock<IServiceBus>();
			mockRepository = StrictMock<ISubscriptionRepository>();
            mockEndpointResolver = StrictMock<IEndpointResolver>();
            mockCache = DynamicMock<ISubscriptionCache>();

            IEndpoint mockEndpoint = DynamicMock<IEndpoint>();
            SetupResult.For(mockBus.Endpoint).Return(mockEndpoint);
            SetupResult.For(mockEndpoint.Uri).Return(new Uri("queue://bus"));

			msgAdd = new AddSubscription("bob", uri);
			msgRem = new RemoveSubscription("bob", uri);
			msgUpdate = new CacheUpdateRequest(uri);
			msgCancel = new CancelSubscriptionUpdates(uri);

		
			srv = new SubscriptionService(mockBus, mockCache, mockRepository, mockEndpointResolver);
        }


		[Test]
		public void add_subscriptions_from_messages()
		{
			using (Record())
			{
				Expect.Call(delegate { mockRepository.Save(msgAdd.Subscription); });
			}
			using (Playback())
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

			using (Record())
			{
				Expect.Call(mockRepository.List()).Return(enumer);
				Expect.Call(delegate { mockBus.Subscribe(srv); });
			}
			using (Playback())
			{
				srv.Start();
			}
		}

	    [Test]
	    public void be_startable_with_stored_subscriptions()
	    {
            IList<Subscription> enumer = new List<Subscription>();
            enumer.Add(new Subscription("bob", uri));

            using (Record())
            {
                Expect.Call(mockRepository.List()).Return(enumer);
                Expect.Call(delegate { mockCache.Add(null); }).IgnoreArguments();
                Expect.Call(delegate { mockBus.Subscribe(srv); });
            }
            using (Playback())
            {
                srv.Start();
            }
	    }

		[Test]
		public void be_stopable()
		{
			using (Record())
			{
				Expect.Call(delegate { mockBus.Unsubscribe(srv); });
			}
			using (Playback())
			{
				srv.Stop();
			}
		}

		[Test]
		public void remove_subscriptions_from_messages()
		{
			using (Record())
			{
				Expect.Call(delegate { mockRepository.Remove(msgRem.Subscription); });
			}
			using (Playback())
			{
				srv.Consume(msgRem);
			}
		}

		[Test]
		public void respond_to_cache_updates()
		{
		    IEndpoint ep = DynamicMock<IEndpoint>();

			using (Record())
			{
			    Expect.Call(mockCache.List()).Return(new List<Subscription>());

			    Expect.Call(mockEndpointResolver.Resolve(uri)).Return(ep);
                Expect.Call(delegate { ep.Send<CacheUpdateResponse>(null); }).IgnoreArguments();
                
			}
			using (Playback())
			{
				srv.Consume(msgUpdate);
			}
		}

		[Test]
		public void respond_to_update_cancel()
		{
			using (Record())
			{
			}
			using (Playback())
			{
				srv.Consume(msgCancel);
			}
		}

	    [Test]
	    public void Calling_dispose_twice_should_be_safe()
	    {
            using (Record())
            {
                this.mockBus.Dispose();
                this.mockRepository.Dispose();
                this.mockBus.Dispose();
                this.mockRepository.Dispose();

            }
            using (Playback())
            {
                srv.Dispose();
                srv.Dispose();
            }
	    }
	}
}