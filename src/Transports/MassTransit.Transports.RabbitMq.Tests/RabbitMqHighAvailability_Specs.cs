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
    using Magnum.TestFramework;

    [Scenario]
    public class RabbitMqHighAvailability_Specs
    {
        IServiceBus _bus;

        [When]
        public void Connecting_to_a_rabbit_mq_server_using_high_availability()
        {
            Uri inputAddress = new Uri("rabbitmq://localhost:5671/test_queue");

			_bus = ServiceBusFactory.New(c =>
				{
					c.ReceiveFrom(inputAddress);
					c.UseRabbitMq(
                        r =>r.ConfigureHost(inputAddress, 
					    ha => ha.UseHighAvailability(
					        x=>
					            {
					                x.AddNode("node1@rabbit");
					                x.AddNode("node2@rabbit");
					                x.AddNode("node3@rabbit");
					            }
					              )));

				});
        }

        [Then]
        public void Should_create_queues_on_each_node()
        {
            
        }
        [Finally]
		public void Finally()
		{
			_bus.Dispose();
		}



    }
}