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
	using MassTransit.Pipeline;
	using MassTransit.Pipeline.Configuration;
	using MassTransit.Services.Subscriptions;
	using Messages;
	using NUnit.Framework;
	using Rhino.Mocks;

	[TestFixture]
	public class When_the_consumer_receives_a_subscription
	{
		[SetUp]
		public void Setup()
		{
			_service = MockRepository.GenerateMock<ISubscriptionService>();
			_endpointCache = MockRepository.GenerateMock<IEndpointCache>();
			
			_endpoint = MockRepository.GenerateMock<IEndpoint>();
			_endpoint.Stub(x => x.Address.Uri).Return(_testUri);

			_bus = MockRepository.GenerateMock<IServiceBus>();
			_bus.Stub(x => x.Endpoint).Return(_endpoint);

			_pipeline = new OutboundPipelineConfigurator(_bus).Pipeline;
			_bus.Stub(x => x.OutboundPipeline).Return(_pipeline);

			_consumer = new SubscriptionConsumer(_service);
			_consumer.Start(_bus);

			_remoteEndpoint = MockRepository.GenerateMock<IEndpoint>();
			_endpointCache.Stub(x => x.GetEndpoint(_remoteUri)).Return(_remoteEndpoint);

			_service.AssertWasCalled(x => x.Register(_consumer));
		}

		[TearDown]
		public void Teardown()
		{
			_consumer = null;
			_endpointCache = null;
			_service = null;
		}

		private ISubscriptionService _service;
		private IEndpointCache _endpointCache;
		private SubscriptionConsumer _consumer;
		private readonly Uri _testUri = new Uri("loopback://localhost/queue");
		private IServiceBus _bus;
		private IEndpoint _endpoint;
		private readonly Uri _remoteUri = new Uri("loopback://localhost/remote");
		private IOutboundMessagePipeline _pipeline;
		private IEndpoint _remoteEndpoint;

		[Test, Explicit]
		public void It_should_be_added_to_the_pipeline_for_remote_subscribers()
		{
			// okay, this is incredibly hard to mock due to the fact that the pipeline does NOT 
			// perform the actual configuration. The visitor-based configurator is sweet, but mocking
			// it is just not happening. We'll use a real pipeline instead and verify that it actually
			// calls the methods instead.

			_consumer.SubscribedTo<PingMessage>(_remoteUri);

			var message = new PingMessage();

			_pipeline.Dispatch(message);

			_remoteEndpoint.AssertWasCalled(x => x.Send(message));
		}

		[Test]
		public void It_should_not_be_added_to_the_pipeline_for_local_subscribers()
		{
			_consumer.SubscribedTo<PingMessage>(_testUri);

			var message = new PingMessage();

			_pipeline.Dispatch(message);

			_endpoint.AssertWasNotCalled(x => x.Send(message));
		}
	}
}