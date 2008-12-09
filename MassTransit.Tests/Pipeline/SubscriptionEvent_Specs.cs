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
	using MassTransit.Pipeline.Inspectors;
	using MassTransit.Subscriptions;
	using MassTransit.Subscriptions.Messages;
	using Messages;
	using NUnit.Framework;
	using Rhino.Mocks;
	using TestConsumers;

	[TestFixture]
	public class The_SubscriptionPublisher_should_add_subscriptions
	{
		#region Setup/Teardown

		[SetUp]
		public void Setup()
		{
			_builder = MockRepository.GenerateMock<IObjectBuilder>();

			_endpoint = MockRepository.GenerateMock<IEndpoint>();
			_endpoint.Stub(x => x.Uri).Return(_uri);

			_bus = MockRepository.GenerateMock<IServiceBus>();
			_bus.Stub(x => x.Endpoint).Return(_endpoint);

			_pipeline = MessagePipelineConfigurator.CreateDefault(_builder);
		}

		#endregion

		private IObjectBuilder _builder;
		private IServiceBus _bus;

		private readonly Uri _uri = new Uri("msmq://localhost/mt_client");
		private IEndpoint _endpoint;
		private MessagePipeline _pipeline;

		[Test]
		public void for_correlated_subscriptions()
		{
			Guid pongGuid = Guid.NewGuid();

			var consumer = new TestCorrelatedConsumer<PongMessage, Guid>(pongGuid);
			_pipeline.Subscribe(consumer);

			var add = new AddSubscription(new Subscription(Subscription.BuildMessageName(typeof(PongMessage)), pongGuid.ToString(), _uri));
			_bus.Expect(x => x.Publish(add));

			var publisher = new SubscriptionPublisher(_bus);
			publisher.Refresh(_pipeline);

			_bus.VerifyAllExpectations();
		}

		[Test]
		public void for_batch_subscriptions()
		{
			var consumer = new TestBatchMessageConsumer<IndividualBatchMessage, Guid>(x => { });
			_pipeline.Subscribe(consumer);

			PipelineViewer.Trace(_pipeline);

			var add = new AddSubscription(new Subscription(Subscription.BuildMessageName(typeof(IndividualBatchMessage)), _uri));
			_bus.Expect(x => x.Publish(add));

			var publisher = new SubscriptionPublisher(_bus);
			publisher.Refresh(_pipeline);

			_bus.VerifyAllExpectations();
		}

		[Test]
		public void for_regular_subscriptions()
		{
			var consumer = new TestMessageConsumer<PingMessage>();
			_pipeline.Subscribe(consumer);

			AddSubscription add = new AddSubscription(Subscription.BuildMessageName(typeof (PingMessage)), _uri);
			_bus.Expect(x => x.Publish(add));

			var publisher = new SubscriptionPublisher(_bus);
			publisher.Refresh(_pipeline);

			_bus.VerifyAllExpectations();
		}

		[Test]
		public void for_component_subscriptions()
		{
			_pipeline.Subscribe<TestMessageConsumer<PingMessage>>();

			AddSubscription add = new AddSubscription(Subscription.BuildMessageName(typeof (PingMessage)), _uri);
			_bus.Expect(x => x.Publish(add));

			var publisher = new SubscriptionPublisher(_bus);
			publisher.Refresh(_pipeline);

			_bus.VerifyAllExpectations();
		}

		[Test]
		public void for_selective_component_subscriptions()
		{
			_pipeline.Subscribe<TestSelectiveConsumer<PingMessage>>();

			AddSubscription add = new AddSubscription(Subscription.BuildMessageName(typeof (PingMessage)), _uri);
			_bus.Expect(x => x.Publish(add));

			var publisher = new SubscriptionPublisher(_bus);
			publisher.Refresh(_pipeline);

			_bus.VerifyAllExpectations();
		}

		[Test]
		public void for_selective_subscriptions()
		{
			var consumer = new TestSelectiveConsumer<PingMessage>();
			_pipeline.Subscribe(consumer);

			AddSubscription add = new AddSubscription(Subscription.BuildMessageName(typeof (PingMessage)), _uri);
			_bus.Expect(x => x.Publish(add));

			var publisher = new SubscriptionPublisher(_bus);
			publisher.Refresh(_pipeline);

			_bus.VerifyAllExpectations();
		}
	}

	[TestFixture]
	public class The_SubscriptionPublisher_should_remove_subscriptions
	{
		#region Setup/Teardown

		[SetUp]
		public void Setup()
		{
			_builder = MockRepository.GenerateMock<IObjectBuilder>();

			_endpoint = MockRepository.GenerateMock<IEndpoint>();
			_endpoint.Stub(x => x.Uri).Return(_uri);

			_bus = MockRepository.GenerateMock<IServiceBus>();
			_bus.Stub(x => x.Endpoint).Return(_endpoint);

			_pipeline = MessagePipelineConfigurator.CreateDefault(_builder);
		}

		#endregion

		private IObjectBuilder _builder;
		private IServiceBus _bus;

		private readonly Uri _uri = new Uri("msmq://localhost/mt_client");
		private IEndpoint _endpoint;
		private MessagePipeline _pipeline;

		[Test]
		public void for_correlated_subscriptions()
		{
			Guid pongGuid = Guid.NewGuid();

			var consumer = new TestCorrelatedConsumer<PongMessage, Guid>(pongGuid);
			var token = _pipeline.Subscribe(consumer);

			var remove = new RemoveSubscription(new Subscription(Subscription.BuildMessageName(typeof(PongMessage)), pongGuid.ToString(), _uri));
			_bus.Expect(x => x.Publish(remove));

			var publisher = new SubscriptionPublisher(_bus);
			publisher.Refresh(_pipeline);

			PipelineViewer.Trace(_pipeline);

			token();
			publisher.Refresh(_pipeline);

			PipelineViewer.Trace(_pipeline);

			_bus.VerifyAllExpectations();
		}

		[Test]
		public void for_batch_subscriptions()
		{
			var consumer = new TestBatchMessageConsumer<IndividualBatchMessage, Guid>(x => { });
			var token = _pipeline.Subscribe(consumer);

			PipelineViewer.Trace(_pipeline);

			var remove = new RemoveSubscription(new Subscription(Subscription.BuildMessageName(typeof(IndividualBatchMessage)), _uri));
			_bus.Expect(x => x.Publish(remove));

			var publisher = new SubscriptionPublisher(_bus);
			publisher.Refresh(_pipeline);
			
			token();
			publisher.Refresh(_pipeline);

			_bus.VerifyAllExpectations();
		}

		[Test]
		public void for_regular_subscriptions()
		{
			var consumer = new TestMessageConsumer<PingMessage>();
			var token = _pipeline.Subscribe(consumer);

			var remove = new RemoveSubscription(Subscription.BuildMessageName(typeof(PingMessage)), _uri);
			_bus.Expect(x => x.Publish(remove));

			var publisher = new SubscriptionPublisher(_bus);
			publisher.Refresh(_pipeline);

			token();
			publisher.Refresh(_pipeline);

			_bus.VerifyAllExpectations();
		}

		[Test]
		public void for_component_subscriptions()
		{
			var token = _pipeline.Subscribe<TestMessageConsumer<PingMessage>>();

			var remove = new RemoveSubscription(Subscription.BuildMessageName(typeof(PingMessage)), _uri);
			_bus.Expect(x => x.Publish(remove));

			var publisher = new SubscriptionPublisher(_bus);
			publisher.Refresh(_pipeline);

			token();
			publisher.Refresh(_pipeline);

			_bus.VerifyAllExpectations();
		}

		[Test]
		public void for_selective_component_subscriptions()
		{
			var token = _pipeline.Subscribe<TestSelectiveConsumer<PingMessage>>();

			var remove = new RemoveSubscription(Subscription.BuildMessageName(typeof(PingMessage)), _uri);
			_bus.Expect(x => x.Publish(remove));

			var publisher = new SubscriptionPublisher(_bus);
			publisher.Refresh(_pipeline);

			token();
			publisher.Refresh(_pipeline);

			_bus.VerifyAllExpectations();
		}

		[Test]
		public void for_selective_subscriptions()
		{
			var consumer = new TestSelectiveConsumer<PingMessage>();
			var token = _pipeline.Subscribe(consumer);

			var remove = new RemoveSubscription(Subscription.BuildMessageName(typeof(PingMessage)), _uri);
			_bus.Expect(x => x.Publish(remove));

			var publisher = new SubscriptionPublisher(_bus);
			publisher.Refresh(_pipeline);

			token();
			publisher.Refresh(_pipeline);

			_bus.VerifyAllExpectations();
		}
	}
}