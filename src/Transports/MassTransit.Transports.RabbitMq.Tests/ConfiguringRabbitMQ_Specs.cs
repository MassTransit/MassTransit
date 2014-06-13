// Copyright 2007-2014 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using NUnit.Framework;


    [TestFixture]
    public class ConfiguringRabbitMQ_Specs
    {
        [Test]
        public async void Should_support_the_new_syntax()
        {
            using (IServiceBus bus = ServiceBusFactory.New(x => x.RabbitMQ(), x =>
            {
                // configure a host, using an existing URI
                x.Host(new Uri("rabbitmq://server/vhost"), r =>
                {
                    r.Username("Joe");
                    r.Password("Blow");
                    r.Heartbeat(30);
                });

                x.Mandatory();

                x.OnPublish<A>(context => { context.Mandatory = true; });

                x.OnPublish(context => context.Mandatory = true);

                x.Endpoint("input_queue", e =>
                {
                    e.ConcurrencyLimit(16);
                    e.Durable(false);
                    e.Exclusive();
                });
            }))
            {
                bus.Publish(new A());
            }
        }


        class A
        {
        }
    }
}