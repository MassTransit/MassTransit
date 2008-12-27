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
namespace MassTransit.Tests
{
	using System;
	using Configuration;
	using MassTransit.Internal;
	using MassTransit.Serialization;
	using MassTransit.Transports;
	using Messages;
	using NUnit.Framework;
	using NUnit.Framework.SyntaxHelpers;
	using Rhino.Mocks;
	using Subscriptions;

	[TestFixture]
	public class MessageQueueEndpoint_MeetsCriteria :
		Specification
	{
		private readonly PingMessage _message = new PingMessage();

		private LocalSubscriptionCache _cache;
		private IObjectBuilder _builder;
		private IEndpointFactory _resolver;
		private IEndpoint _endpoint;
		private ServiceBus _bus;

		protected override void Before_each()
		{
			_cache = new LocalSubscriptionCache();
			_builder = MockRepository.GenerateMock<IObjectBuilder>();
			_resolver = EndpointFactoryConfigurator.New(x =>
				{
					x.SetObjectBuilder(_builder);
					x.SetDefaultSerializer<BinaryMessageSerializer>();
					x.RegisterTransport<LoopbackEndpoint>();
				});
			_endpoint = _resolver.GetEndpoint(new Uri("loopback://localhost/servicebus"));
			_bus = new ServiceBus(_endpoint, _builder, _cache, _resolver, new TypeInfoCache());
			_bus.Start();
		}

		protected override void After_each()
		{
			_bus.Dispose();
			_endpoint.Dispose();
			_resolver.Dispose();
		}


		[Test]
		public void Subscring_to_an_endpoint_should_accept_and_dispatch_messages()
		{
			bool workDid = false;

			_bus.Subscribe<PingMessage>(
				delegate { workDid = true; },
				delegate { return true; });

			_bus.Dispatch(_message);

			Assert.That(workDid, Is.True, "Lazy Test!");
		}
	}
}