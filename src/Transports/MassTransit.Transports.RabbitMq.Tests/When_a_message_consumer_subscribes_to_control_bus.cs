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
namespace MassTransit.Transports.RabbitMq.Tests
{
	using System;
	using BusConfigurators;
	using Magnum.Extensions;
	using Magnum.TestFramework;
	using NUnit.Framework;
	using TestFramework;

	[Scenario]
	public class When_a_message_consumer_subscribes_to_control_bus :
        Given_a_rabbitmq_bus
	{
		Future<A> _received;
        
        protected override void ConfigureServiceBus(Uri uri, ServiceBusConfigurator configurator)
        {
            base.ConfigureServiceBus(uri, configurator);
            configurator.UseControlBus();
            _received = new Future<A>();
        }

    

		[When]
		public void A_message_is_published()
		{
            //How do you do this via configurator?
            LocalBus.ControlBus.SubscribeContextHandler<A>(ctx => _received.Complete(ctx.Message));

			LocalBus.Publish(new A
				{
					StringA = "ValueA",
				});
		}

		[Then]
		public void Should_be_received_by_the_queue()
		{
			_received.WaitUntilCompleted(8.Seconds()).ShouldBeTrue();
		}


		class A 
		{
			public string StringA { get; set; }
		}
	}

}