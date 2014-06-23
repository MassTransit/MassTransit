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
    namespace Handler_Specs
    {
        using System;
        using System.Threading.Tasks;
        using NUnit.Framework;


        [TestFixture]
        public class ConfiguringRabbitMQ_Specs :
            AsyncTestFixture
        {
            [Test]
            public async void Should_support_the_new_syntax()
            {
                var hostAddress = new Uri("rabbitmq://localhost/test");

                Task handler = null;

                using (IServiceBus bus = ServiceBusFactory.New(x => x.RabbitMQ(), x =>
                {
                    x.Host(hostAddress, r =>
                    {
                        r.Username("guest");
                        r.Password("guest");
                    });

                    x.ReceiveEndpoint("input_queue", e =>
                    {
                        e.PrefetchCount(16);
                        e.Durable(false);
                        e.Exclusive();

                        handler = TestHandler<A>(e);
                    });
                }))
                {
                    var queueAddress = new Uri(hostAddress, "input_queue");

                    ISendToEndpoint endpoint = bus.GetSendEndpoint(queueAddress);

                    await endpoint.Send(new A());

                    await handler;
                }
            }


            class A
            {
            }
        }
    }
}