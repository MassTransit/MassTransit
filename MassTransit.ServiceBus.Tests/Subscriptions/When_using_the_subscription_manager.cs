namespace MassTransit.Tests.Subscriptions
{
    using System;
    using System.Collections.Generic;
    using NUnit.Framework;
    using Rhino.Mocks;
    using Rhino.Mocks.Constraints;
    using ServiceBus;
    using ServiceBus.Subscriptions;
    using ServiceBus.Subscriptions.Messages;

    [TestFixture]
    public class When_using_the_subscription_manager : 
        Specification
    {
        private IServiceBus _serviceBus;
        private IEndpoint _managerEndpoint;
        private ISubscriptionCache _cache;
        private IEndpoint _sbEndpoint;

        private readonly Uri _managerUri = new Uri("msmq://localhost/mgr");
        private Uri _sbUri;

        protected override void Before_each()
        {
            _sbUri = new Uri("msmq://" + Environment.MachineName.ToLower() + "/test");
            _sbEndpoint = DynamicMock<IEndpoint>();
            _serviceBus = DynamicMock<IServiceBus>();
            SetupResult.For(_sbEndpoint.Uri).Return(_sbUri);
            SetupResult.For(_serviceBus.Endpoint).Return(_sbEndpoint);

            _cache = DynamicMock<ISubscriptionCache>();
            _managerEndpoint = DynamicMock<IEndpoint>();
            SetupResult.For(_managerEndpoint.Uri).Return(_managerUri);
            
        }

        [Test]
        public void The_client_should_request_an_update_at_startup()
        {
            using (Record())
            {
                Expect.Call(delegate
                                {
                                    _managerEndpoint.Send<CacheUpdateRequest>(null);
                                }).IgnoreArguments();
				
            }

            using (Playback())
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

            using (Record())
            {
                _cache.Add(sub);
            }

            using (Playback())
            {
                using (SubscriptionClient client = new SubscriptionClient(_serviceBus, _cache, _managerEndpoint))
                {
                    client.Start();
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

            using (Record())
            {
                _cache.Add(subscriptions[0]);
                Expect.Call(_cache.List()).Return(new List<Subscription>());
            }

            using (Playback())
            {
                using (SubscriptionClient client = new SubscriptionClient(_serviceBus, _cache, _managerEndpoint))
                {
                    client.Start();
                    client.Consume(cacheUpdateResponse);
                }
            }
        }

        [Test]
        public void When_a_local_service_subscribes_to_the_bus_notify_the_manager_of_the_change()
        {
            SubscriptionEventArgs args = new SubscriptionEventArgs(new Subscription("Ho.Pimp, Ho", new Uri("msmq://" + Environment.MachineName.ToLower() + "/test")));

            using (Record())
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

            using (Playback())
            {
                using (SubscriptionClient client = new SubscriptionClient(_serviceBus, _cache, _managerEndpoint))
                {
                    client.Cache_OnAddSubscription(_cache, args);
                }
            }
        }
    }
}