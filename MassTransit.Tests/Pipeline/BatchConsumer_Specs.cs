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
		}

		private IObjectBuilder _builder;

		private static void PublishBatch(MessagePipeline pipeline, int _batchSize)
		{
			Guid batchId = Guid.NewGuid();
			for (int i = 0; i < _batchSize; i++)
			{
				IndividualBatchMessage message = new IndividualBatchMessage(batchId, _batchSize);

				pipeline.Dispatch(message);
			}
		}

		[Test]
		public void A_batch_consumer_should_be_delivered_messages()
		{
			MessagePipeline pipeline = MessagePipelineConfigurator.CreateDefault(_builder);

			var batchConsumer = new TestBatchMessageConsumer<IndividualBatchMessage, Guid>(x => PipelineViewer.Trace(pipeline));

			pipeline.Subscribe(batchConsumer);

			PipelineViewer.Trace(pipeline);

			PublishBatch(pipeline, 1);

			TimeSpan _timeout = 5.Seconds();

			batchConsumer.ShouldHaveReceivedBatch(_timeout);
		}

		[Test]
		public void A_batch_consumer_should_be_delivered_a_lot_of_messages()
		{
			MessagePipeline pipeline = MessagePipelineConfigurator.CreateDefault(_builder);

			var batchConsumer = new TestBatchMessageConsumer<IndividualBatchMessage, Guid>(x => PipelineViewer.Trace(pipeline));

			var removeSubscription = pipeline.Subscribe(batchConsumer);

			PipelineViewer.Trace(pipeline);

			PublishBatch(pipeline, 100);

			TimeSpan _timeout = 5.Seconds();

			batchConsumer.ShouldHaveReceivedBatch(_timeout);

			removeSubscription();

			PipelineViewer.Trace(pipeline);
		}
	}
}