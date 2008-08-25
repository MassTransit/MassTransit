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
	using System.Collections.Generic;
	using System.Threading;
	using Messages;
	using NUnit.Framework;
	using NUnit.Framework.SyntaxHelpers;

	public class TestMessageConsumer<TMessage> :
		TestConsumerBase<TMessage>,
		Consumes<TMessage>.All
		where TMessage : class
	{
	}

	public class TestCorrelatedConsumer<TMessage, TKey> :
		TestConsumerBase<TMessage>,
		Consumes<TMessage>.For<TKey>
		where TMessage : class, CorrelatedBy<Guid>
	{
		private readonly TKey _correlationId;

		public TestCorrelatedConsumer(TKey correlationId)
		{
			_correlationId = correlationId;
		}

		public TKey CorrelationId
		{
			get { return _correlationId; }
		}
	}

	public class TestBatchConsumer<TMessage, TBatchId> :
		TestConsumerBase<Batch<TMessage, TBatchId>>,
		Consumes<Batch<TMessage, TBatchId>>.Selected
		where TMessage : class, BatchedBy<TBatchId>
	{
		private static readonly List<TBatchId> _batchesReceived = new List<TBatchId>();
		private static readonly Semaphore _batchReceived = new Semaphore(0, 100);
		private readonly ManualResetEvent _completed = new ManualResetEvent(false);
		private int _messageCount;

		public override void Consume(Batch<TMessage, TBatchId> batch)
		{
			base.Consume(batch);

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

	public class TestReplyService<TMessage, TKey, TReplyMessage> :
		TestConsumerBase<TMessage>,
		Consumes<TMessage>.All
		where TMessage : class, CorrelatedBy<TKey>
		where TReplyMessage : class, CorrelatedBy<TKey>
	{
		public IServiceBus Bus { get; set; }

		public override void Consume(TMessage message)
		{
			base.Consume(message);

			TReplyMessage reply = (TReplyMessage) Activator.CreateInstance(typeof (TReplyMessage), message.CorrelationId);
			Bus.Publish(reply);
		}
	}

	public class TestConsumerBase<TMessage>
		where TMessage : class
	{
		private static readonly List<TMessage> _allMessages = new List<TMessage>();
		private static readonly Semaphore _allReceived = new Semaphore(0, 100);
		private readonly List<TMessage> _messages = new List<TMessage>();
		private readonly Semaphore _received = new Semaphore(0, 100);

		public virtual void Consume(TMessage message)
		{
			_messages.Add(message);
			_received.Release();

			_allMessages.Add(message);
			_allReceived.Release();
		}

		public void MessageHandler(IMessageContext<TMessage> handler)
		{
			_messages.Add(handler.Message);
			_received.Release();
		}

		private bool ReceivedMessage(TMessage message, TimeSpan timeout)
		{
			while (_messages.Contains(message) == false)
			{
				if (_received.WaitOne(timeout, true) == false)
					return false;
			}

			return true;
		}

		public void ShouldHaveReceivedMessage(TMessage message, TimeSpan timeout)
		{
			Assert.That(ReceivedMessage(message, timeout), Is.True, "Message should have been received");
		}

		public void ShouldNotHaveReceivedMessage(TMessage message, TimeSpan timeout)
		{
			Assert.That(ReceivedMessage(message, timeout), Is.False, "Message should not have been received");
		}

		private static bool AnyReceivedMessage(TMessage message, TimeSpan timeout)
		{
			while (_allMessages.Contains(message) == false)
			{
				if (_allReceived.WaitOne(timeout, true) == false)
					return false;
			}

			return true;
		}

		public static void AnyShouldHaveReceivedMessage(TMessage message, TimeSpan timeout)
		{
			Assert.That(AnyReceivedMessage(message, timeout), Is.True, "Message should have been received");
		}
	}
}