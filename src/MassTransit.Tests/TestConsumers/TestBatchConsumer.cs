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
namespace MassTransit.Tests.TestConsumers
{
	using System;
	using System.Collections.Generic;
	using System.Threading;
	using Batch;
	using Messages;
	using NUnit.Framework;

	public class TestBatchConsumer<TMessage, TBatchId> :
		TestConsumerBase<Batch<TMessage, TBatchId>>,
		Consumes<Batch<TMessage, TBatchId>>.Selected
		where TMessage : class, BatchedBy<TBatchId>
	{
		private static readonly List<TBatchId> _batchesReceived = new List<TBatchId>();
		private static readonly Semaphore _batchReceived = new Semaphore(0, 100);
		private readonly ManualResetEvent _completed = new ManualResetEvent(false);
		private int _messageCount;
		private readonly Action<TestBatchConsumer<TMessage, TBatchId>> _action;

		public TestBatchConsumer(Action<TestBatchConsumer<TMessage, TBatchId>> action)
		{
			_action = action;
		}

		public TestBatchConsumer()
		{
			_action = (x) => { };
		}

		public override void Consume(Batch<TMessage, TBatchId> batch)
		{
			base.Consume(batch);

			_action(this);

			foreach (IndividualBatchMessage message in batch)
			{
				Interlocked.Increment(ref _messageCount);
			}

			if (_messageCount == batch.BatchLength)
			{
				_batchesReceived.Add(batch.BatchId);
				_batchReceived.Release();

				_completed.Set();
			}
		}

		public bool Accept(Batch<TMessage, TBatchId> message)
		{
			return true;
		}

		public void ShouldHaveReceivedBatch(TimeSpan timeout)
		{
			Assert.That(_completed.WaitOne(timeout, true), Is.True, "The batch should have completed");
		}

		public void ShouldNotHaveCompletedBatch(TimeSpan timeout)
		{
			Assert.That(_completed.WaitOne(timeout, true), Is.False, "The batch should not have completed");
		}

		public static void AnyShouldHaveReceivedBatch(TBatchId batchId, TimeSpan timeout)
		{
			while (_batchesReceived.Contains(batchId) == false)
			{
				Assert.That(_batchReceived.WaitOne(timeout, true), Is.True, "The batch should have been received");
			}
		}
	}
}