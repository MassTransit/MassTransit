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
namespace MassTransit.Transports.Msmq.Tests
{
	using System;
	using System.Threading;
	using Magnum.Common.DateTimeExtensions;
	using MassTransit.Tests;
	using MassTransit.Tests.Messages;
	using MassTransit.Tests.TestConsumers;
	using NUnit.Framework;

	[TestFixture]
	public class When_receiving_messages_slowly :
		LocalAndRemoteTestContext
	{
        protected override string GetCastleConfigurationFile()
        {
            return "msmq.castle.xml";
        }

		private readonly TimeSpan _timeout = TimeSpan.FromSeconds(10);

		[Test]
		public void It_should_be_received_by_one_subscribed_consumer()
		{
			var consumer = new TestMessageConsumer<PingMessage>();
			RemoteBus.Subscribe(consumer);

			Thread.Sleep(5.Seconds());

			var message = new PingMessage();
			LocalBus.Publish(message);

			consumer.ShouldHaveReceivedMessage(message, _timeout);
		}
	}
}