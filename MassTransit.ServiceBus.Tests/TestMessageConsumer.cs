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
		where TMessage : class
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

	public class TestConsumerBase<TMessage>
		where TMessage : class
	{
		private readonly List<TMessage> _messages = new List<TMessage>();
		private readonly Semaphore _received = new Semaphore(0, 100);

		public void Consume(TMessage message)
		{
			_messages.Add(message);
			_received.Release();
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
	}
}