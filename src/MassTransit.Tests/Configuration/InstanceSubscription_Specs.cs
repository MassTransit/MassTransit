// Copyright 2007-2011 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Tests.Configuration
{
	using Magnum.Extensions;
	using NUnit.Framework;
	using Messages;
	using TestFramework;
	using TestFramework.Messages;


    [Explicit]
	public class When_subscribing_an_object_instance_to_the_bus 
	{
		IServiceBus _bus;
		ConsumerOf<PingMessage> _consumer;
		PingMessage _ping;

		[SetUp]
		public void Subscribing_an_object_instance_to_the_bus()
		{
			_consumer = new ConsumerOf<PingMessage>();

			object instance = _consumer;

			_bus = ServiceBusFactory.New(x =>
				{
					x.ReceiveFrom("loopback://localhost/mt_test");

//					x.Subscribe(s => s.Instance(instance));
				});

			_ping = new PingMessage();
			_bus.Publish(_ping);
		}

		[Test]
		public void Should_have_received_the_message()
		{
			_consumer.ShouldHaveReceived(_ping, 8.Seconds());
		}
	}
}