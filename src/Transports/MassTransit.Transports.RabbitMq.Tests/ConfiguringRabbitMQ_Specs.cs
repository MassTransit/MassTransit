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
    using System.Threading.Tasks;
    using Configuration;
    using Magnum.Extensions;
    using NUnit.Framework;
    using Policies;
    using TestFramework;


    [TestFixture]
    public class ConfiguringRabbitMQ_Specs :
        AsyncTestFixture
    {
        [Test]
        public async void Should_support_the_new_syntax()
        {
            var hostAddress = new Uri("rabbitmq://localhost/test");
            var completed = new TaskCompletionSource<A>();

            IBusControl bus = Bus.Factory.CreateUsingRabbitMq(x =>
            {
                RabbitMqHostSettings host = x.Host(hostAddress, r =>
                {
                    r.Username("guest");
                    r.Password("guest");
                    r.Heartbeat(30);
                });

                x.Mandatory();

//                x.OnPublish<A>(context => { context.Mandatory = true; });
//
//                x.OnPublish(context => context.Mandatory = true);
//

                x.ReceiveEndpoint(host, "input_queue", e =>
                {
                    e.PrefetchCount = 16;
                    e.Durable(false);
                    e.Exclusive();

                    e.Log(Console.Out, async c => string.Format("Logging: {0}", c.MessageId.Value));

                    e.Handler<A>(async context => completed.TrySetResult(context.Message));

                    // Add a message handler and configure the pipeline to retry the handler
                    // if an exception is thrown
                    e.Handler<A>(Handle, h =>
                    {
                        h.Retry(Retry.Interval(5, 100.Milliseconds()));
                    });
                });
            });

            using(bus.Start(TestCancellationToken))
            {
                var queueAddress = new Uri(hostAddress, "input_queue");
                ISendEndpoint endpoint = await bus.GetSendEndpoint(queueAddress);

                await endpoint.Send(new A());
            }
        }

        async Task Handle(ConsumeContext<A> context)
        {
        }


        class A
        {
        }
    }
}