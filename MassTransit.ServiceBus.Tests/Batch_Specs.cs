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
namespace MassTransit.ServiceBus.Tests
{
	using System;
	using System.Threading;
	using Messages;
	using NUnit.Framework;
	using TestConsumers;

    [TestFixture]
	public class When_a_batch_of_messages_is_published :
		LocalAndRemoteTestContext
	{
		private static readonly TimeSpan _timeout = TimeSpan.FromSeconds(3);
		private int _batchSize;

		protected  void RunTest()
		{
			TestBatchConsumer<IndividualBatchMessage, Guid> batchConsumer = new TestBatchConsumer<IndividualBatchMessage, Guid>();

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
		LocalAndRemoteTestContext
	{
		private static readonly TimeSpan _timeout = TimeSpan.FromSeconds(3);
		private int _batchSize;

		protected  void RunTest()
		{
			Container.AddComponent<TestBatchConsumer<IndividualBatchMessage, Guid>>();
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
		LocalAndRemoteTestContext
	{
		private static readonly TimeSpan _timeout = TimeSpan.FromSeconds(5);
		private int _batchSize;

		[Test]
		public void The_batch_should_throw_an_exception_that_the_timeout_occurred()
		{
			_batchSize = 2;

			TestMessageConsumer<BatchTimeout<IndividualBatchMessage, Guid>> timeoutConsumer = new TestMessageConsumer<BatchTimeout<IndividualBatchMessage, Guid>>();
			RemoteBus.Subscribe(timeoutConsumer);

			TestBatchConsumer<IndividualBatchMessage, Guid> batchConsumer = new TestBatchConsumer<IndividualBatchMessage, Guid>();

			RemoteBus.Subscribe(batchConsumer);

			Guid batchId = Guid.NewGuid();
			IndividualBatchMessage message = new IndividualBatchMessage(batchId, _batchSize);

			LocalBus.Publish(message);

			timeoutConsumer.ShouldHaveReceivedMessage(new BatchTimeout<IndividualBatchMessage, Guid>(batchId), _timeout);

			batchConsumer.ShouldNotHaveCompletedBatch(TimeSpan.Zero);
		}
	}

    [TestFixture]
    public class When_more_messages_are_sent_than_expected :
        LocalAndRemoteTestContext
    {
        private int _batchSize;

        [Test, Ignore("Not sure how to test, but this is right")]
        [ExpectedException(typeof(SemaphoreFullException))] //TODO: Bad Exception
        public void The_batch_should_throw_an_error()
        {
            _batchSize = 1;

            TestMessageConsumer<BatchTimeout<IndividualBatchMessage, Guid>> timeoutConsumer = new TestMessageConsumer<BatchTimeout<IndividualBatchMessage, Guid>>();
            RemoteBus.Subscribe(timeoutConsumer);

            TestBatchConsumer<IndividualBatchMessage, Guid> batchConsumer = new TestBatchConsumer<IndividualBatchMessage, Guid>();
            RemoteBus.Subscribe(batchConsumer);

            Guid batchId = Guid.NewGuid();
            IndividualBatchMessage message = new IndividualBatchMessage(batchId, _batchSize);

            LocalBus.Publish(message);
            LocalBus.Publish(message);

            
        }
    }
}