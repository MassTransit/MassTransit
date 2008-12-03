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
	using System.Collections.Generic;
	using System.Diagnostics;
	using Magnum.Common.DateTimeExtensions;
	using MassTransit.Pipeline;
	using MassTransit.Pipeline.Interceptors.Inbound;
	using Messages;
	using NUnit.Framework;
	using Rhino.Mocks;

	[TestFixture]
	public class When_building_a_pipeline
	{
		#region Setup/Teardown

		[SetUp]
		public void Setup()
		{
			_builder = MockRepository.GenerateMock<IObjectBuilder>();
		}

		#endregion

		private IObjectBuilder _builder;

		[Test]
		public void Batch_composition_should_work()
		{
			TestBatchConsumer<IndividualBatchMessage, Guid> batchConsumer = new TestBatchConsumer<IndividualBatchMessage, Guid>();

			InboundPipeline pipeline = new InboundPipeline(_builder);

			pipeline.Subscribe(batchConsumer);

			Guid batchId = Guid.NewGuid();
			const int _batchSize = 1;
			for (int i = 0; i < _batchSize; i++)
			{
				IndividualBatchMessage message = new IndividualBatchMessage(batchId, _batchSize);

				pipeline.Dispatch(message);
			}

			TimeSpan _timeout = 5.Seconds();

			batchConsumer.ShouldHaveReceivedBatch(_timeout);
		}

		[Test]
		public void The_pipeline_should_be_happy()
		{
			IndiscriminantConsumer<PingMessage> consumer = new IndiscriminantConsumer<PingMessage>();

			InboundPipeline pipeline = new InboundPipeline(_builder);

			pipeline.Subscribe(consumer);

			pipeline.Dispatch(new PingMessage());

			Assert.IsNotNull(consumer.Consumed);
		}
	}
}