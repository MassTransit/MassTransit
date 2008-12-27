// Copyright 2007-2008 The Apache Software Foundation.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed 
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace MassTransit.Tests.Services.Subscriptions
{
    using System;
    using System.Collections.Generic;
    using MassTransit.Services.Subscriptions;
    using MassTransit.Services.Subscriptions.Client;
    using MassTransit.Services.Subscriptions.Messages;
    using MassTransit.Subscriptions;
    using MassTransit.Transports;
    using NUnit.Framework;
    using Rhino.Mocks;

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
        public LocalEndpointHandler _localEndpoints;

        protected override void Before_each()
        {
            _bus = MockRepository.GenerateMock<IServiceBus>();
            _cache = MockRepository.GenerateMock<ISubscriptionCache>();

            _endpoint = MockRepository.GenerateMock<IEndpoint>();
            _endpoint.Stub(x => x.Uri).Return(_uri);
            _bus.Stub(x => x.Endpoint).Return(_endpoint);
            _localEndpoints = new LocalEndpointHandler();

            _subscriptionServiceEndpoint = MockRepository.GenerateMock<IEndpoint>();
            _subscriptionServiceEndpoint.Stub(x => x.Uri).Return(_subscriptionServiceUri);
        }

        [Test]
        public void The_client_should_request_an_update_at_startup()
        {
            _subscriptionServiceEndpoint.Expect(x => x.Send<CacheUpdateRequest>(null)).IgnoreArguments();

            using (SubscriptionClient client = new SubscriptionClient(_bus, _subscriptionServiceEndpoint, _localEndpoints))
            {
                client.Start();
            }

            _subscriptionServiceEndpoint.VerifyAllExpectations();
        }

        [Test]
        public void The_client_should_update_the_cache_when_a_SubscriptionChange_message_is_received()
        {
            SubscriptionInformation subInfo = new SubscriptionInformation("Ho.Pimp, Ho", new Uri("msmq://localhost/test"));
            Subscription sub = new Subscription("Ho.Pimp, Ho", new Uri("msmq://localhost/test"));
            AddSubscription change = new AddSubscription(subInfo);
            RemoteSubscriptionCoordinator rsc = new RemoteSubscriptionCoordinator(_cache, _localEndpoints);
            
            _cache.Expect(x => x.Add(sub));

            using (SubscriptionClient client = new SubscriptionClient(_bus, _subscriptionServiceEndpoint, _localEndpoints))
            {
                client.Start();
                rsc.Consume(change);    
                
            }

            _cache.VerifyAllExpectations();
        }

        [Test]
        public void The_client_should_update_the_local_subscription_cache()
        {
            SubscriptionInformation subInfo = new SubscriptionInformation("Ho.Pimp, Ho", new Uri("msmq://localhost/test"));
            Subscription sub = new Subscription("Ho.Pimp, Ho", new Uri("msmq://localhost/test"));

            List<Subscription> subscriptions = new List<Subscription> {sub};

            RemoteSubscriptionCoordinator rsc = new RemoteSubscriptionCoordinator(_cache, _localEndpoints);
            LocalSubscriptionCoordinator lsc = new LocalSubscriptionCoordinator(_cache, _subscriptionServiceEndpoint, _localEndpoints);

            CacheUpdateResponse cacheUpdateResponse = new CacheUpdateResponse(subscriptions);

            _cache.Expect(x => x.Add(subscriptions[0]));
            _cache.Expect(x => x.List()).Return(new List<Subscription>());

            using (SubscriptionClient client = new SubscriptionClient(_bus, _subscriptionServiceEndpoint, _localEndpoints))
            {
                client.Start();
                rsc.Consume(cacheUpdateResponse);
                lsc.Consume(cacheUpdateResponse);
            }

            _cache.VerifyAllExpectations();
        }

        [Test]
        public void When_a_local_service_subscribes_to_the_bus_notify_the_manager_of_the_change()
        {
            SubscriptionInformation subInfo = new SubscriptionInformation("Ho.Pimp, Ho", new Uri("msmq://localhost/test"));
            Subscription sub = new Subscription("Ho.Pimp, Ho", new Uri("msmq://localhost/test"));
            _localEndpoints.AddLocalEndpoint(new LoopbackEndpoint(new Uri("msmq://localhost/test")));

            SubscriptionEventArgs args = new SubscriptionEventArgs(sub);

            _subscriptionServiceEndpoint.Expect(x => x.Send<AddSubscription>(null))
                .IgnoreArguments();

            LocalSubscriptionCoordinator lsc = new LocalSubscriptionCoordinator(_cache, _subscriptionServiceEndpoint, _localEndpoints);

            using (SubscriptionClient client = new SubscriptionClient(_bus, _subscriptionServiceEndpoint, _localEndpoints))
            {
                client.Start();
                lsc.WhenASubscriptionIsAdded(_cache, args);
            }

            _subscriptionServiceEndpoint.VerifyAllExpectations();
        }
    }
}