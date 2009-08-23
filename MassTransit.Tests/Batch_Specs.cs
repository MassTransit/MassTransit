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
	using Batch;
	using Magnum.DateTimeExtensions;
	using Messages;
	using NUnit.Framework;
	using Rhino.Mocks;
	using TestConsumers;
	using TextFixtures;

	[TestFixture]
	public class When_a_batch_of_messages_is_published :
		LoopbackLocalAndRemoteTestFixture
	{
		private static readonly TimeSpan _timeout = TimeSpan.FromSeconds(6);
		private int _batchSize;

		protected void RunTest()
		{
			var batchConsumer = new TestBatchConsumer<IndividualBatchMessage, Guid>();

			RemoteBus.Subscribe(batchConsumer);

			Guid batchId = Guid.NewGuid();
			for (int i = 0; i < _batchSize; i++)
			{
				IndividualBatchMessage message = new IndividualBatchMessage(batchId, _batchSize);

				LocalBus.Publish(message);
			}

			batchConsumer.ShouldHaveReceivedBatch(_timeout);
		}

		[Test]
		public void A_batch_with_a_lot_of_messages_should_be_received()
		{
			_batchSize = 1027;
			RunTest();
		}

		[Test]
		public void A_batch_with_a_single_message_should_be_received()
		{
			_batchSize = 1;
			RunTest();
		}

		[Test]
		public void A_single_consumer_should_receive_the_entire_batch()
		{
			_batchSize = 2;
			RunTest();
		}
	}

	[TestFixture]
	public class When_a_batch_of_message_is_published_to_a_container_based_consumer :
		LoopbackLocalAndRemoteTestFixture
	{
		private static readonly TimeSpan _timeout = TimeSpan.FromSeconds(3);
		private int _batchSize;

		protected void RunTest()
		{
			ObjectBuilder.Stub(x => x.GetInstance<TestBatchConsumer<IndividualBatchMessage, Guid>>())
				.Return(new TestBatchConsumer<IndividualBatchMessage, Guid>());

			RemoteBus.Subscribe<TestBatchConsumer<IndividualBatchMessage, Guid>>();

			Guid batchId = Guid.NewGuid();
			for (int i = 0; i < _batchSize; i++)
			{
				IndividualBatchMessage message = new IndividualBatchMessage(batchId, _batchSize);

				LocalBus.Publish(message);
			}

			TestBatchConsumer<IndividualBatchMessage, Guid>.AnyShouldHaveReceivedBatch(batchId, _timeout);
		}

		[Test]
		public void A_batch_with_a_lot_of_messages_should_be_received()
		{
			_batchSize = 1027;
			RunTest();
		}

		[Test]
		public void A_batch_with_a_single_message_should_be_received()
		{
			_batchSize = 1;
			RunTest();
		}

		[Test]
		public void A_single_consumer_should_receive_the_entire_batch()
		{
			_batchSize = 2;
			RunTest();
		}
	}

	[TestFixture]
	public class When_an_incomplete_batch_is_published :
		LoopbackLocalAndRemoteTestFixture
	{
		private static readonly TimeSpan _timeout = TimeSpan.FromSeconds(5);
		private int _batchSize;

		[Test, Ignore]
		public void The_batch_should_throw_an_exception_that_the_timeout_occurred()
		{
			_batchSize = 2;

			var timeoutConsumer = new TestMessageConsumer<BatchTimeout<IndividualBatchMessage, Guid>>();
			RemoteBus.Subscribe(timeoutConsumer);

			var batchConsumer = new TestBatchConsumer<IndividualBatchMessage, Guid>();

			RemoteBus.Subscribe(batchConsumer);

			Guid batchId = Guid.NewGuid();
			IndividualBatchMessage message = new IndividualBatchMessage(batchId, _batchSize);

			LocalBus.Publish(message);

			timeoutConsumer.ShouldHaveReceivedMessage(new BatchTimeout<IndividualBatchMessage, Guid>(batchId), _timeout);

			batchConsumer.ShouldNotHaveCompletedBatch(TimeSpan.Zero);
		}
	}

	[TestFixture]
	public class When_the_number_of_messages_in_a_batch_is_zero :
		LoopbackTestFixture
	{
		[Test]
		public void The_batch_should_not_be_dispatched()
		{
			ObjectBuilder.Stub(x => x.GetInstance<TestBatchConsumer<IndividualBatchMessage, Guid>>())
				.Return(new TestBatchConsumer<IndividualBatchMessage, Guid>());

			LocalBus.Subscribe<TestBatchConsumer<IndividualBatchMessage, Guid>>();

			Guid batchId = Guid.NewGuid();
			const int batchLength = 0;

			LocalBus.Publish(new IndividualBatchMessage(batchId, batchLength));


			TestBatchConsumer<IndividualBatchMessage, Guid>.AnyShouldHaveReceivedBatch(batchId, 115.Seconds());


			
		}
	}

	[TestFixture]
	public class When_more_messages_are_sent_than_expected :
		LoopbackLocalAndRemoteTestFixture
	{
		private int _batchSize;

		[Test, Ignore("Not sure how to test, but this is right")]
//        [ExpectedException(typeof(SemaphoreFullException))] //TODO: Bad Exception
		public void The_batch_should_throw_an_error()
		{
			_batchSize = 1;

			var timeoutConsumer = new TestMessageConsumer<BatchTimeout<IndividualBatchMessage, Guid>>();
			RemoteBus.Subscribe(timeoutConsumer);

			var batchConsumer = new TestBatchConsumer<IndividualBatchMessage, Guid>();
			RemoteBus.Subscribe(batchConsumer);

			Guid batchId = Guid.NewGuid();
			IndividualBatchMessage message = new IndividualBatchMessage(batchId, _batchSize);

			LocalBus.Publish(message);
			LocalBus.Publish(message);
		}
	}
}