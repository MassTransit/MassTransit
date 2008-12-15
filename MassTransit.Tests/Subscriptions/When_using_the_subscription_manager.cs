namespace MassTransit.Tests.Subscriptions
{
    using System;
    using System.Collections.Generic;
    using MassTransit.Internal;
    using MassTransit.Subscriptions;
    using MassTransit.Subscriptions.Messages;
    using NUnit.Framework;
    using Rhino.Mocks;
    using Rhino.Mocks.Constraints;
    
    [TestFixture]
    public class When_using_the_subscription_manager : 
        Specification
    {
        private IServiceBus _bus;
        private IEndpoint _subscriptionServiceEndpoint;
        private ISubscriptionCache _cache;
        private IEndpoint _endpoint;

        private readonly Uri _subscriptionServiceUri = new Uri("msmq://localhost/mgr");
		private readonly Uri _uri = new Uri("msmq://" + Environment.MachineName.ToLower() + "/test");

    	protected override void Before_each()
        {
			_bus = MockRepository.GenerateMock<IServiceBus>();
			_cache = MockRepository.GenerateMock<ISubscriptionCache>();

			_endpoint = MockRepository.GenerateMock<IEndpoint>();
			_endpoint.Stub(x => x.Uri).Return(_uri);
			_bus.Stub(x => x.Endpoint).Return(_endpoint);

			_subscriptionServiceEndpoint = MockRepository.GenerateMock<IEndpoint>();
    		_subscriptionServiceEndpoint.Stub(x => x.Uri).Return(_subscriptionServiceUri);
        }

        [Test]
        public void The_client_should_request_an_update_at_startup()
        {
        	_subscriptionServiceEndpoint.Expect(x => x.Send<CacheUpdateRequest>(null)).IgnoreArguments();

                using (SubscriptionClient client = new SubscriptionClient(_bus, _cache, _subscriptionServiceEndpoint))
                {
                    client.Start();
                }

        	_subscriptionServiceEndpoint.VerifyAllExpectations();
        }

        [Test]
        public void The_client_should_update_the_cache_when_a_SubscriptionChange_message_is_received()
        {
            Subscription sub = new Subscription("Ho.Pimp, Ho", new Uri("msmq://localhost/test"));
            AddSubscription change = new AddSubscription(sub);

        	_cache.Expect(x => x.Add(sub));

            using (SubscriptionClient client = new SubscriptionClient(_bus, _cache, _subscriptionServiceEndpoint))
            {
                client.Start();
                client.Consume(change);
            }

        	_cache.VerifyAllExpectations();
        }

        [Test]
        public void The_client_should_update_the_local_subscription_cache()
        {
            List<Subscription> subscriptions = new List<Subscription>();
            subscriptions.Add(new Subscription("Ho.Pimp, Ho", new Uri("msmq://localhost/test")));

            CacheUpdateResponse cacheUpdateResponse = new CacheUpdateResponse(subscriptions);

        	_cache.Expect(x => x.Add(subscriptions[0]));
        	_cache.Expect(x => x.List()).Return(new List<Subscription>());

                using (SubscriptionClient client = new SubscriptionClient(_bus, _cache, _subscriptionServiceEndpoint))
                {
                    client.Start();
                    client.Consume(cacheUpdateResponse);
                }

			_cache.VerifyAllExpectations();
        }

        [Test]
        public void When_a_local_service_subscribes_to_the_bus_notify_the_manager_of_the_change()
        {
        	Subscription subscription = new Subscription("Ho.Pimp, Ho", _uri);
        	SubscriptionEventArgs args = new SubscriptionEventArgs(subscription);

        	_subscriptionServiceEndpoint.Expect(x => x.Send<AddSubscription>(null))
        		.IgnoreArguments()
        		.Constraints(Is.Matching<AddSubscription>(y => y.Subscription == subscription));

                using (SubscriptionClient client = new SubscriptionClient(_bus, _cache, _subscriptionServiceEndpoint))
                {
                	client.Start();
                    client.Cache_OnAddSubscription(_cache, args);
                }

        	_subscriptionServiceEndpoint.VerifyAllExpectations();
        }
    }
}