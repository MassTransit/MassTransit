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
	using Magnum.Common.DateTimeExtensions;
	using MassTransit.Pipeline;
	using MassTransit.Pipeline.Configuration;
	using MassTransit.Pipeline.Inspectors;
	using MassTransit.Subscriptions;
	using Messages;
	using NUnit.Framework;
	using Rhino.Mocks;

	[TestFixture]
	public class BatchConsumer_Specs
	{
		[SetUp]
		public void Setup()
		{
			_builder = MockRepository.GenerateMock<IObjectBuilder>();
			_subscriptionEvent = MockRepository.GenerateMock<ISubscriptionEvent>();
			_subscriptionEvent.Expect(x => x.SubscribedTo(typeof(IndividualBatchMessage))).Repeat.Any().Return(() =>
			{
				_subscriptionEvent.UnsubscribedFrom(typeof(IndividualBatchMessage));
				return true;
			});

			_pipeline = MessagePipelineConfigurator.CreateDefault(_builder, _subscriptionEvent);
		}

		private IObjectBuilder _builder;
		private ISubscriptionCache _cache;
		private static Guid _batchId;
		private MessagePipeline _pipeline;
		private ISubscriptionEvent _subscriptionEvent;

		private static void PublishBatch(MessagePipeline _pipeline, int _batchSize)
		{
			_batchId = Guid.NewGuid();
			for (int i = 0; i < _batchSize; i++)
			{
				IndividualBatchMessage message = new IndividualBatchMessage(_batchId, _batchSize);

				_pipeline.Dispatch(message);
			}
		}

		[Test]
		public void A_batch_consumer_should_be_delivered_messages()
		{
			var batchConsumer = new TestBatchMessageConsumer<IndividualBatchMessage, Guid>(x => PipelineViewer.Trace(_pipeline));
			_pipeline.Subscribe(batchConsumer);

			PublishBatch(_pipeline, 1);

			TimeSpan _timeout = 5.Seconds();

			batchConsumer.ShouldHaveReceivedBatch(_timeout);
		}
	
		[Test]
		public void A_batch_component_should_be_delivered_messages()
		{
			var consumer = new TestBatchMessageConsumer<IndividualBatchMessage, Guid>();

			_builder.Stub(x => x.GetInstance<TestBatchMessageConsumer<IndividualBatchMessage, Guid>>()).Return(consumer);

			_pipeline.Subscribe<TestBatchMessageConsumer<IndividualBatchMessage, Guid>>();
			PipelineViewer.Trace(_pipeline);

			PublishBatch(_pipeline, 1);

			TimeSpan _timeout = 5.Seconds();

			TestBatchMessageConsumer<IndividualBatchMessage, Guid>.AnyShouldHaveReceivedBatch(_batchId, _timeout);
		}

		[Test]
		public void A_batch_consumer_should_be_delivered_a_lot_of_messages()
		{
			var batchConsumer = new TestBatchMessageConsumer<IndividualBatchMessage, Guid>(x => PipelineViewer.Trace(_pipeline));

			var removeSubscription = _pipeline.Subscribe(batchConsumer);

			PipelineViewer.Trace(_pipeline);

			PublishBatch(_pipeline, 100);

			TimeSpan _timeout = 5.Seconds();

			batchConsumer.ShouldHaveReceivedBatch(_timeout);

			removeSubscription();

			PipelineViewer.Trace(_pipeline);
		}
	}
}