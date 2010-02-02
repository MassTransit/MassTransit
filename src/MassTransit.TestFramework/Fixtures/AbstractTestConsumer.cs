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
namespace MassTransit.TestFramework.Fixtures
{
	using System;
	using System.Collections.Generic;
	using System.Threading;
	using Magnum.DateTimeExtensions;
	using NUnit.Framework;

	public abstract class AbstractTestConsumer<TMessage>
		where TMessage : class
	{
		private static readonly List<TMessage> _allMessages = new List<TMessage>();
		private static readonly Semaphore _allReceived = new Semaphore(0, 100);
		private static int _allReceivedCount;

		private readonly List<TMessage> _messages = new List<TMessage>();
		private readonly Semaphore _received = new Semaphore(0, 100);

		private Action<TMessage> _consumerAction;
		private int _receivedMessageCount;

		protected AbstractTestConsumer()
		{
			_consumerAction = x => { };
		}

		protected AbstractTestConsumer(Action<TMessage> consumerAction)
		{
			_consumerAction = consumerAction;
		}

		public static int AllReceivedCount
		{
			get { return _allReceivedCount; }
		}

		public int ReceivedMessageCount
		{
			get { return _receivedMessageCount; }
			protected set { _receivedMessageCount = value; }
		}

		public virtual void Consume(TMessage message)
		{
			Interlocked.Increment(ref _receivedMessageCount);
			Interlocked.Increment(ref _allReceivedCount);

			_consumerAction(message);

			_messages.Add(message);
			_received.Release();

			_allMessages.Add(message);
			_allReceived.Release();
		}

		public void MessageHandler(TMessage message)
		{
			_messages.Add(message);
			_received.Release();
		}

		public void ShouldHaveReceived(TMessage message)
		{
			ShouldHaveReceived(message, 0.Seconds());
		}

		public void ShouldHaveReceived(TMessage message, TimeSpan timeout)
		{
			Assert.That(ReceivedMessage(message, timeout), Is.True, "Message should have been received");
		}

		public void ShouldNotHaveReceivedMessage(TMessage message)
		{
			Assert.That(ReceivedMessage(message, 0.Seconds()), Is.False, "Message should not have been received");
		}

		public void ShouldNotHaveReceivedMessage(TMessage message, TimeSpan timeout)
		{
			Assert.That(ReceivedMessage(message, timeout), Is.False, "Message should not have been received");
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

		public static void AnyShouldHaveReceivedMessage(TMessage message, TimeSpan timeout)
		{
			Assert.That(AnyReceivedMessage(message, timeout), Is.True, "Message should have been received");
		}

		public static void OnlyOneShouldHaveReceivedMessage(TMessage message, TimeSpan timeout)
		{
			Assert.That(AnyReceivedMessage(message, timeout), Is.True, "Message should have been received");
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
	}
}