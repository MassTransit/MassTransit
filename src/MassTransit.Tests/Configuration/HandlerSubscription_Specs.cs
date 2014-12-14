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
	using Shouldly;
	using TestFramework;
	using TestFramework.Messages;


    
	public class When_subscribing_a_conditional_handler_to_a_bus
	{
		IServiceBus _bus;
		Future<PingMessage> _conditionChecked;
		Future<PingMessage> _received;

		[SetUp]
		public void Subscribing_a_conditional_handler_to_a_bus()
		{
			_received = new Future<PingMessage>();
			_conditionChecked = new Future<PingMessage>();

			_bus = ServiceBusFactory.New(x =>
				{
					x.ReceiveFrom("loopback://localhost/mt_test");

//					x.Subscribe(s =>
//						{
//							// a simple handler
//                            s.Handler<PingMessage>(async context => _received.Complete(context.Message))
//								.Where(context =>
//									{
//                                        _conditionChecked.Complete(context.Message);
//										return true;
//									});
//						});
				});

			_bus.Publish(new PingMessage());
		}

		[TearDown]
		public void Finally()
		{
			_bus.Dispose();
		}

		[Test]
		public void Should_have_subscribed()
		{
			_bus.ShouldHaveRemoteSubscriptionFor<PingMessage>();
		}

		[Test]
		public void Should_receive_the_message()
		{
			_received.WaitUntilCompleted(8.Seconds()).ShouldBe(true);
		}

		[Test]
		public void Should_have_checked_the_condition()
		{
			_conditionChecked.WaitUntilCompleted(8.Seconds()).ShouldBe(true);
		}
	}

	
	public class When_subscribing_a_context_handler_to_a_bus
	{
		IServiceBus _bus;
		Future<PingMessage> _received;

		[SetUp]
		public void Subscribing_a_context_handler_to_a_bus()
		{
			_received = new Future<PingMessage>();

			_bus = ServiceBusFactory.New(x =>
				{
					x.ReceiveFrom("loopback://localhost/mt_test");

//					x.Subscribe(s =>
//						{
//							// a simple handler
////                            s.Handler<PingMessage>(async (context) => _received.Complete(context.Message));
//						});
				});

			_bus.Publish(new PingMessage());
		}

		[TearDown]
		public void Finally()
		{
			_bus.Dispose();
		}

		[Test]
		public void Should_have_subscribed()
		{
			_bus.ShouldHaveRemoteSubscriptionFor<PingMessage>();
		}

		[Test]
		public void Should_receive_the_message()
		{
			_received.WaitUntilCompleted(8.Seconds()).ShouldBe(true);
		}
	}
}