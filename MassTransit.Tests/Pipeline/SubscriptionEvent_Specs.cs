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
			_builder = MockRepository.GenerateMock<IObjectBuilder>();

			_endpoint = MockRepository.GenerateMock<IEndpoint>();
			_endpoint.Stub(x => x.Uri).Return(_uri);

			_bus = MockRepository.GenerateMock<IServiceBus>();
			_bus.Stub(x => x.Endpoint).Return(_endpoint);

			_subscriptionEvent = MockRepository.GenerateMock<ISubscriptionEvent>();
			_pipeline = MessagePipelineConfigurator.CreateDefault(_builder, _subscriptionEvent);
		}

		private IObjectBuilder _builder;
		private IServiceBus _bus;

		private readonly Uri _uri = new Uri("msmq://localhost/mt_client");
		private IEndpoint _endpoint;
		private MessagePipeline _pipeline;
		private ISubscriptionEvent _subscriptionEvent;

		[Test]
		public void for_batch_component_subscriptions()
		{
			_subscriptionEvent.Expect(x => x.SubscribedTo(typeof (IndividualBatchMessage))).Return(() =>
				{
					_subscriptionEvent.UnsubscribedFrom(typeof (IndividualBatchMessage));
					return true;
				});

			_pipeline.Subscribe<TestBatchConsumer<IndividualBatchMessage, Guid>>();

			_subscriptionEvent.VerifyAllExpectations();
		}

		[Test]
		public void for_batch_subscriptions()
		{
			_subscriptionEvent.Expect(x => x.SubscribedTo(typeof (IndividualBatchMessage))).Return(() =>
				{
					_subscriptionEvent.UnsubscribedFrom(typeof (IndividualBatchMessage));
					return true;
				});

			var consumer = new TestBatchConsumer<IndividualBatchMessage, Guid>();
			_pipeline.Subscribe(consumer);

			_subscriptionEvent.VerifyAllExpectations();
		}

		[Test]
		public void for_component_subscriptions()
		{
			_subscriptionEvent.Expect(x => x.SubscribedTo(typeof (PingMessage))).Return(() =>
				{
					_subscriptionEvent.UnsubscribedFrom(typeof (PingMessage));
					return true;
				});

			_pipeline.Subscribe<TestMessageConsumer<PingMessage>>();

			_subscriptionEvent.VerifyAllExpectations();
		}

		[Test]
		public void for_correlated_subscriptions()
		{
			Guid pongGuid = Guid.NewGuid();

			_subscriptionEvent.Expect(x => x.SubscribedTo(typeof (PongMessage), pongGuid.ToString())).Return(() =>
				{
					_subscriptionEvent.UnsubscribedFrom(typeof (PongMessage), pongGuid.ToString());
					return true;
				});

			var consumer = new TestCorrelatedConsumer<PongMessage, Guid>(pongGuid);
			_pipeline.Subscribe(consumer);

			_subscriptionEvent.VerifyAllExpectations();
		}

		[Test]
		public void for_regular_subscriptions()
		{
			_subscriptionEvent.Expect(x => x.SubscribedTo(typeof (PingMessage))).Return(() =>
				{
					_subscriptionEvent.UnsubscribedFrom(typeof (PingMessage));
					return true;
				});

			var consumer = new TestMessageConsumer<PingMessage>();
			_pipeline.Subscribe(consumer);

			_subscriptionEvent.VerifyAllExpectations();
		}

		[Test]
		public void for_selective_component_subscriptions()
		{
			_subscriptionEvent.Expect(x => x.SubscribedTo(typeof (PingMessage))).Return(() =>
				{
					_subscriptionEvent.UnsubscribedFrom(typeof (PingMessage));
					return true;
				});

			_pipeline.Subscribe<TestSelectiveConsumer<PingMessage>>();

			_subscriptionEvent.VerifyAllExpectations();
		}

		[Test]
		public void for_selective_subscriptions()
		{
			_subscriptionEvent.Expect(x => x.SubscribedTo(typeof (PingMessage))).Return(() =>
				{
					_subscriptionEvent.UnsubscribedFrom(typeof (PingMessage));
					return true;
				});

			var consumer = new TestSelectiveConsumer<PingMessage>();
			_pipeline.Subscribe(consumer);

			_subscriptionEvent.VerifyAllExpectations();
		}
	}

	[TestFixture]
	public class The_SubscriptionPublisher_should_remove_subscriptions
	{
		[SetUp]
		public void Setup()
		{
			_builder = MockRepository.GenerateMock<IObjectBuilder>();

			_endpoint = MockRepository.GenerateMock<IEndpoint>();
			_endpoint.Stub(x => x.Uri).Return(_uri);

			_bus = MockRepository.GenerateMock<IServiceBus>();
			_bus.Stub(x => x.Endpoint).Return(_endpoint);

			_subscriptionEvent = MockRepository.GenerateMock<ISubscriptionEvent>();
			_pipeline = MessagePipelineConfigurator.CreateDefault(_builder, _subscriptionEvent);
		}

		private IObjectBuilder _builder;
		private IServiceBus _bus;

		private readonly Uri _uri = new Uri("msmq://localhost/mt_client");
		private IEndpoint _endpoint;
		private MessagePipeline _pipeline;
		private ISubscriptionEvent _subscriptionEvent;

		[Test]
		public void for_batch_subscriptions()
		{
			_subscriptionEvent.Expect(x => x.SubscribedTo(typeof (IndividualBatchMessage))).Return(() =>
				{
					_subscriptionEvent.UnsubscribedFrom(typeof (IndividualBatchMessage));
					return true;
				});
			_subscriptionEvent.Expect(x => x.UnsubscribedFrom(typeof (IndividualBatchMessage))).Repeat.Once();

			var consumer = new TestBatchConsumer<IndividualBatchMessage, Guid>();
			var token = _pipeline.Subscribe(consumer);

			token();

			_subscriptionEvent.VerifyAllExpectations();
		}

		[Test]
		public void for_batch_subscriptions_but_not_when_another_exists()
		{
			_subscriptionEvent.Expect(x => x.SubscribedTo(typeof (IndividualBatchMessage))).Repeat.Twice().Return(() =>
				{
					_subscriptionEvent.UnsubscribedFrom(typeof (IndividualBatchMessage));
					return true;
				});
			_subscriptionEvent.Expect(x => x.UnsubscribedFrom(typeof (IndividualBatchMessage))).Repeat.Never();

			var consumer = new TestBatchConsumer<IndividualBatchMessage, Guid>();
			var token = _pipeline.Subscribe(consumer);

			var consumerB = new TestBatchConsumer<IndividualBatchMessage, Guid>();
			var tokenB = _pipeline.Subscribe(consumerB);

			token();

			_subscriptionEvent.VerifyAllExpectations();
		}

		[Test]
		public void for_component_subscriptions()
		{
			_subscriptionEvent.Expect(x => x.SubscribedTo(typeof (PingMessage))).Return(() =>
				{
					_subscriptionEvent.UnsubscribedFrom(typeof (PingMessage));
					return true;
				});
			_subscriptionEvent.Expect(x => x.UnsubscribedFrom(typeof (PingMessage))).Repeat.Once();

			var token = _pipeline.Subscribe<TestMessageConsumer<PingMessage>>();

			token();

			_subscriptionEvent.VerifyAllExpectations();
		}

		[Test]
		public void for_correlated_subscriptions()
		{
			Guid pongGuid = Guid.NewGuid();

			_subscriptionEvent.Expect(x => x.SubscribedTo(typeof (PongMessage), pongGuid.ToString())).Return(() =>
				{
					_subscriptionEvent.UnsubscribedFrom(typeof (PongMessage), pongGuid.ToString());
					return true;
				});
			_subscriptionEvent.Expect(x => x.UnsubscribedFrom(typeof (PongMessage), pongGuid.ToString()));

			var consumer = new TestCorrelatedConsumer<PongMessage, Guid>(pongGuid);
			var remove = _pipeline.Subscribe(consumer);

			remove();

			_subscriptionEvent.VerifyAllExpectations();
		}

		[Test]
		public void for_correlated_subscriptions_but_not_when_another_exists()
		{
			Guid pongGuid = Guid.NewGuid();

			_subscriptionEvent.Expect(x => x.SubscribedTo(typeof (PongMessage), pongGuid.ToString())).Repeat.Twice().Return(() =>
				{
					_subscriptionEvent.UnsubscribedFrom(typeof (PongMessage), pongGuid.ToString());
					return true;
				});
			_subscriptionEvent.Expect(x => x.UnsubscribedFrom(typeof (PongMessage), pongGuid.ToString())).Repeat.Never();

			var consumer = new TestCorrelatedConsumer<PongMessage, Guid>(pongGuid);
			var otherConsumer = new TestCorrelatedConsumer<PongMessage, Guid>(pongGuid);
			var remove = _pipeline.Subscribe(consumer);
			var removeOther = _pipeline.Subscribe(otherConsumer);

			remove();
			_subscriptionEvent.VerifyAllExpectations();


			_subscriptionEvent.BackToRecord();
			_subscriptionEvent.Expect(x => x.UnsubscribedFrom(typeof (PongMessage), pongGuid.ToString())).Repeat.Once();
			_subscriptionEvent.Replay();

			removeOther();
			_subscriptionEvent.VerifyAllExpectations();
		}

		[Test]
		public void for_regular_subscriptions()
		{
			_subscriptionEvent.Expect(x => x.SubscribedTo(typeof (PingMessage))).Return(() =>
				{
					_subscriptionEvent.UnsubscribedFrom(typeof (PingMessage));
					return true;
				});
			_subscriptionEvent.Expect(x => x.UnsubscribedFrom(typeof (PingMessage))).Repeat.Once();

			var consumer = new TestMessageConsumer<PingMessage>();
			var token = _pipeline.Subscribe(consumer);

			token();

			_subscriptionEvent.VerifyAllExpectations();
		}

		[Test]
		public void for_selective_component_subscriptions()
		{
			_subscriptionEvent.Expect(x => x.SubscribedTo(typeof (PingMessage))).Return(() =>
				{
					_subscriptionEvent.UnsubscribedFrom(typeof (PingMessage));
					return true;
				});
			_subscriptionEvent.Expect(x => x.UnsubscribedFrom(typeof (PingMessage))).Repeat.Once();

			var token = _pipeline.Subscribe<TestSelectiveConsumer<PingMessage>>();

			token();

			_subscriptionEvent.VerifyAllExpectations();
		}

		[Test]
		public void for_selective_subscriptions()
		{
			_subscriptionEvent.Expect(x => x.SubscribedTo(typeof (PingMessage))).Return(() =>
				{
					_subscriptionEvent.UnsubscribedFrom(typeof (PingMessage));
					return true;
				});
			_subscriptionEvent.Expect(x => x.UnsubscribedFrom(typeof (PingMessage))).Repeat.Once();

			var consumer = new TestSelectiveConsumer<PingMessage>();
			var token = _pipeline.Subscribe(consumer);

			token();

			_subscriptionEvent.VerifyAllExpectations();
		}
	}
}