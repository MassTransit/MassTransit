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
    using MassTransit.Services.Subscriptions.Messages;
    using MassTransit.Services.Subscriptions.Server;
    using MassTransit.Subscriptions;
    using NUnit.Framework;
    using Rhino.Mocks;
    using TextFixtures;

    [TestFixture]
    public class As_a_SubscriptionService :
        LoopbackLocalAndRemoteTestFixture
    {
        private IServiceBus _bus;
        private ISubscriptionCache _cache;
        private ISubscriptionRepository _repository;
        private SubscriptionService _service;
        private IEndpointFactory _endpointFactory;
        private FollowerRepository _followers;

        private RemoteEndpointCoordinator _coordinator;

        private readonly Uri uri = new Uri("queue://bob");
        private IEndpoint _endpoint;

        private AddSubscription msgAdd;
        private RemoveSubscription msgRem;
        private CancelSubscriptionUpdates msgCancel;
        private CacheUpdateRequest msgUpdate;


        protected override void EstablishContext()
        {
            base.EstablishContext();
            msgAdd = new AddSubscription("bob",uri);
            msgRem = new RemoveSubscription("bob", uri);
            msgCancel = new CancelSubscriptionUpdates(uri);
            msgUpdate = new CacheUpdateRequest(uri);

            _endpointFactory = MockRepository.GenerateMock<IEndpointFactory>();
            _bus = MockRepository.GenerateMock<IServiceBus>();
            _repository = MockRepository.GenerateMock<ISubscriptionRepository>();
            
            _cache = MockRepository.GenerateMock<ISubscriptionCache>();
            _followers = new FollowerRepository(_endpointFactory);

            _endpoint = MockRepository.GenerateMock<IEndpoint>();
            _endpoint.Stub(x => x.Uri).Return(uri);
            _bus.Stub(x => x.Endpoint).Return(_endpoint);
            
            _service = new SubscriptionService(_bus, _cache, _repository);
            _coordinator = new RemoteEndpointCoordinator(_cache,_followers, _repository, _endpointFactory);
        }


        [Test]
        public void should_be_startable()
        {
            _service = new SubscriptionService(_bus, _cache, _repository);
            _repository.Expect(x => x.List()).Return(new List<Subscription>());
            _bus.Expect(x => x.Subscribe<RemoteEndpointCoordinator>()).Return(() => false);

            _service.Start();

            _repository.VerifyAllExpectations();
            _bus.VerifyAllExpectations();
        }


        [Test]
        public void be_startable_with_stored_subscriptions()
        {
            _repository.Expect(x => x.List()).Return(new List<Subscription> {new Subscription("bob", uri)});
            _cache.Expect(x => x.Add(null)).IgnoreArguments();

            _bus.Expect(x => x.Subscribe<RemoteEndpointCoordinator>()).Return(() => false);

            _service.Start();

            _repository.VerifyAllExpectations();
            _bus.VerifyAllExpectations();
        }

        [Test]
        public void be_stopable()
        {
            _bus.Expect(x => x.Unsubscribe<RemoteEndpointCoordinator>());

            _service.Stop();

            _repository.VerifyAllExpectations();
            _bus.VerifyAllExpectations();
        }

        [Test]
        public void add_subscriptions_from_messages()
        {
            _cache.Expect(x => x.Add(null)).IgnoreArguments();
            _repository.Expect(x => x.Save(null)).IgnoreArguments();
            
            _coordinator.Consume(msgAdd);

            _cache.VerifyAllExpectations();
            _repository.VerifyAllExpectations();
        }

        [Test]
        public void remove_subscriptions_from_messages()
        {
            _cache.Expect(x => x.Remove(null)).IgnoreArguments();
            _repository.Expect(x => x.Remove(null)).IgnoreArguments();


            _coordinator.Consume(msgRem);


            _cache.VerifyAllExpectations();
            _repository.VerifyAllExpectations();
        }

        [Test]
        public void respond_to_update_cancel()
        {
            
            _coordinator.Consume(msgCancel);
        }

        [Test]
        public void respond_to_cache_updates()
        {
            IEndpoint ep = MockRepository.GenerateMock<IEndpoint>();
            _cache.Expect(x => x.List()).Return(new List<Subscription>());
            _endpointFactory.Stub(x => x.GetEndpoint(uri)).Return(ep);
            ep.Expect(x => x.Send<CacheUpdateResponse>(null)).IgnoreArguments();

            _coordinator.Consume(msgUpdate);

        }
    }
}