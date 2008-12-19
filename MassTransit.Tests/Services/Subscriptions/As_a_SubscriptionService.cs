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
namespace MassTransit.Tests.Subscriptions
{
	using System;
	using System.Collections.Generic;
	using MassTransit.Internal;
	using MassTransit.Subscriptions;
	using MassTransit.Subscriptions.ServerHandlers;
	using NUnit.Framework;
	using Rhino.Mocks;

	[TestFixture]
	public class As_a_SubscriptionService :
		Specification
	{
		private IServiceBus _bus;
		private ISubscriptionCache _cache;
		private ISubscriptionRepository _repository;
		private SubscriptionService srv;
		private IEndpointResolver _endpointResolver;


		private readonly Uri uri = new Uri("queue:\\bob");
		private IEndpoint _endpoint;

		protected override void Before_each()
		{
			_bus = MockRepository.GenerateMock<IServiceBus>();
			_repository = MockRepository.GenerateMock<ISubscriptionRepository>();
			_endpointResolver = MockRepository.GenerateMock<IEndpointResolver>();
			_cache = MockRepository.GenerateMock<ISubscriptionCache>();

			_endpoint = DynamicMock<IEndpoint>();
			_endpoint.Stub(x => x.Uri).Return(new Uri("queue://bus"));
			_bus.Stub(x => x.Endpoint).Return(_endpoint);

			srv = new SubscriptionService(_bus, _cache, _repository);
		}


		[Test]
		public void be_alive()
		{
			Assert.IsNotNull(srv);
		}

		[Test]
		public void be_startable()
		{
			_repository.Expect(x => x.List()).Return(new List<Subscription>());
			_bus.Expect(x => x.Subscribe<AddSubscriptionHandler>()).Return(() => false);
			_bus.Expect(x => x.Subscribe<RemoveSubscriptionHandler>()).Return(() => false);
			_bus.Expect(x => x.Subscribe<CacheUpdateRequestHandler>()).Return(() => false);
			_bus.Expect(x => x.Subscribe<CancelUpdatesHandler>()).Return(() => false);

			srv.Start();

			_repository.VerifyAllExpectations();
			_bus.VerifyAllExpectations();
		}


		[Test]
		public void be_startable_with_stored_subscriptions()
		{
			_repository.Expect(x => x.List()).Return(new List<Subscription> {new Subscription("bob", uri)});
			_cache.Expect(x => x.Add(null)).IgnoreArguments();

			_bus.Expect(x => x.Subscribe<AddSubscriptionHandler>()).Return(() => false);
			_bus.Expect(x => x.Subscribe<RemoveSubscriptionHandler>()).Return(() => false);
			_bus.Expect(x => x.Subscribe<CacheUpdateRequestHandler>()).Return(() => false);
			_bus.Expect(x => x.Subscribe<CancelUpdatesHandler>()).Return(() => false);

			srv.Start();

			_repository.VerifyAllExpectations();
			_bus.VerifyAllExpectations();
		}

		[Test]
		public void be_stopable()
		{
			_bus.Expect(x => x.Unsubscribe<AddSubscriptionHandler>());
			_bus.Expect(x => x.Unsubscribe<RemoveSubscriptionHandler>());
			_bus.Expect(x => x.Unsubscribe<CacheUpdateRequestHandler>());
			_bus.Expect(x => x.Unsubscribe<CancelUpdatesHandler>());

			srv.Stop();

			_repository.VerifyAllExpectations();
			_bus.VerifyAllExpectations();
		}


		[Test]
		public void Calling_dispose_twice_should_be_safe()
		{
			using (Record())
			{
				this._bus.Dispose();
				this._repository.Dispose();
				this._bus.Dispose();
				this._repository.Dispose();
			}
			using (Playback())
			{
				srv.Dispose();
				srv.Dispose();
			}
		}
	}
}