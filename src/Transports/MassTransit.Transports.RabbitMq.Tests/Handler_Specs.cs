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
        using Configuration;
        using NUnit.Framework;
        using TestFramework;


        [TestFixture]
        public class ConfiguringRabbitMQ_Specs :
            BusTestFixture
        {
            IBus _bus;

            [Test]
            public async void Should_support_the_new_syntax()
            {
                var hostAddress = new Uri("rabbitmq://localhost/test");

                Task handler = null;

                var busControl = MassTransit.Bus.Factory.CreateUsingRabbitMq(x =>
                {
                    RabbitMqHostSettings host = x.Host(hostAddress, r =>
                    {
                        r.Username("guest");
                        r.Password("guest");
                    });

                    x.ReceiveEndpoint(host, "input_queue", e =>
                    {
                        e.PrefetchCount = 16;
                        e.Durable(false);
                        e.Exclusive();

                        handler = Handler<A>(e);
                    });
                });

                using(busControl.Start())
                {
                    var queueAddress = new Uri(hostAddress, "input_queue");

                    ISendEndpoint endpoint = await busControl.GetSendEndpoint(queueAddress);

                    await endpoint.Send(new A());

                    await handler;
                }
            }


            class A
            {
            }


            protected override IBus Bus
            {
                get { return _bus; }
            }
        }
    }
}