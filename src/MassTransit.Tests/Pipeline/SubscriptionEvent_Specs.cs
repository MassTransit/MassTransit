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
namespace MassTransit.Tests.Pipeline
{
	using System;
	using MassTransit.Pipeline;
	using MassTransit.Pipeline.Configuration;
	using Messages;
	using NUnit.Framework;
	using Rhino.Mocks;
	using TestConsumers;

	[TestFixture]
	public class The_SubscriptionPublisher_should_add_subscriptions
	{
		[SetUp]
		public void Setup()
		{
			_endpoint = MockRepository.GenerateMock<IEndpoint>();
			_endpoint.Stub(x => x.Address.Uri).Return(_uri);

			_bus = MockRepository.GenerateMock<IServiceBus>();
			_bus.Stub(x => x.Endpoint).Return(_endpoint);

			_unsubscribe = MockRepository.GenerateMock<UnsubscribeAction>();

			_subscriptionEvent = MockRepository.GenerateMock<ISubscriptionEvent>();
			_pipeline = InboundPipelineConfigurator.CreateDefault(_bus);
			_pipeline.Configure(x => x.Register(_subscriptionEvent));
		}

		private IServiceBus _bus;

		private readonly Uri _uri = new Uri("msmq://localhost/mt_client");
		private IEndpoint _endpoint;
		private IInboundMessagePipeline _pipeline;
		private ISubscriptionEvent _subscriptionEvent;
		private UnsubscribeAction _unsubscribe;

		[Test]
		public void for_component_subscriptions()
		{
			_subscriptionEvent.Expect(x => x.SubscribedTo<PingMessage>()).Return(() =>
				{
					_unsubscribe();
					return true;
				});

			_pipeline.ConnectConsumer<TestMessageConsumer<PingMessage>>();

			_subscriptionEvent.VerifyAllExpectations();
		}

		[Test]
		public void for_correlated_subscriptions()
		{
			Guid pongGuid = Guid.NewGuid();

			_subscriptionEvent.Expect(x => x.SubscribedTo<PongMessage,Guid>(pongGuid)).Return(() =>
				{
					_unsubscribe();
					return true;
				});

			var consumer = new TestCorrelatedConsumer<PongMessage, Guid>(pongGuid);
			_pipeline.ConnectInstance(consumer);

			_subscriptionEvent.VerifyAllExpectations();
		}

		[Test]
		public void for_regular_subscriptions()
		{
			_subscriptionEvent.Expect(x => x.SubscribedTo<PingMessage>()).Return(() =>
				{
					_unsubscribe();
					return true;
				});

			var consumer = new TestMessageConsumer<PingMessage>();
			_pipeline.ConnectInstance(consumer);

			_subscriptionEvent.VerifyAllExpectations();
		}
	}

	[TestFixture]
	public class The_SubscriptionPublisher_should_remove_subscriptions
	{
		[SetUp]
		public void Setup()
		{
			_endpoint = MockRepository.GenerateMock<IEndpoint>();
			_endpoint.Stub(x => x.Address.Uri).Return(_uri);

			_unsubscribe = MockRepository.GenerateMock<UnsubscribeAction>();

			_bus = MockRepository.GenerateMock<IServiceBus>();
			_bus.Stub(x => x.Endpoint).Return(_endpoint);

			_subscriptionEvent = MockRepository.GenerateMock<ISubscriptionEvent>();
			_pipeline = InboundPipelineConfigurator.CreateDefault(_bus);
			_pipeline.Configure(x => x.Register(_subscriptionEvent));
		}

		private IServiceBus _bus;

		private readonly Uri _uri = new Uri("msmq://localhost/mt_client");
		private IEndpoint _endpoint;
		private IInboundMessagePipeline _pipeline;
		private ISubscriptionEvent _subscriptionEvent;
		private UnsubscribeAction _unsubscribe;


		[Test]
		public void for_component_subscriptions()
		{
			_subscriptionEvent.Expect(x => x.SubscribedTo<PingMessage>()).Return(() =>
				{
					_unsubscribe();
					return true;
				});

			var token = _pipeline.ConnectConsumer<TestMessageConsumer<PingMessage>>();

			token();

			_subscriptionEvent.VerifyAllExpectations();
			_unsubscribe.AssertWasCalled(x => x());
		}

		[Test]
		public void for_correlated_subscriptions()
		{
			Guid pongGuid = Guid.NewGuid();

			_subscriptionEvent.Expect(x => x.SubscribedTo<PongMessage,Guid>(pongGuid)).Return(() =>
				{
					_unsubscribe();
					return true;
				});

			var consumer = new TestCorrelatedConsumer<PongMessage, Guid>(pongGuid);
			var remove = _pipeline.ConnectInstance(consumer);

			remove();

			_subscriptionEvent.VerifyAllExpectations();
			_unsubscribe.AssertWasCalled(x => x());
		}

		[Test]
		public void for_correlated_subscriptions_but_not_when_another_exists()
		{
			Guid pongGuid = Guid.NewGuid();

			_subscriptionEvent.Expect(x => x.SubscribedTo<PongMessage, Guid>(pongGuid)).Repeat.Twice().Return(() =>
				{
					_unsubscribe();
					return true;
				});

			var consumer = new TestCorrelatedConsumer<PongMessage, Guid>(pongGuid);
			var otherConsumer = new TestCorrelatedConsumer<PongMessage, Guid>(pongGuid);
			var remove = _pipeline.ConnectInstance(consumer);
			var removeOther = _pipeline.ConnectInstance(otherConsumer);

			remove();
			_subscriptionEvent.VerifyAllExpectations();
			_unsubscribe.AssertWasNotCalled(x => x());


			_unsubscribe.BackToRecord();
			_unsubscribe.Replay();

			removeOther();
			_unsubscribe.AssertWasCalled(x => x());
		}

		[Test]
		public void for_regular_subscriptions()
		{
			_subscriptionEvent.Expect(x => x.SubscribedTo<PingMessage>()).Return(() =>
				{
					_unsubscribe();
					return true;
				});

			var consumer = new TestMessageConsumer<PingMessage>();
			var token = _pipeline.ConnectInstance(consumer);

			token();

			_subscriptionEvent.VerifyAllExpectations();
			_unsubscribe.AssertWasCalled(x => x());
		}
	}
}