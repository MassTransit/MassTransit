// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.RabbitMqTransport.Tests
{
    using System;
    using System.Threading.Tasks;
    using MassTransit.Testing;
    using NUnit.Framework;
    using TestFramework;
    using TestFramework.Messages;


    [TestFixture]
    public class Stopping_the_bus :
        AsyncTestFixture
    {
        [Test]
        public async Task Should_complete_running_consumers_nicely()
        {
            TaskCompletionSource<PingMessage> consumerStarted = GetTask<PingMessage>();

            IBusControl bus = Bus.Factory.CreateUsingRabbitMq(x =>
            {
                IRabbitMqHost host = x.Host("localhost", "test", h =>
                {
                });

                x.ReceiveEndpoint(host, "input_queue", e =>
                {
                    e.PurgeOnStartup = true;

                    e.Handler<PingMessage>(async context =>
                    {
                        await Console.Out.WriteLineAsync("Starting handler");

                        consumerStarted.TrySetResult(context.Message);

                        for (var i = 0; i < 5; i++)
                        {
                            await Task.Delay(1000);

                            await Console.Out.WriteLineAsync("Handler processing");
                        }

                        await context.RespondAsync(new PongMessage(context.Message.CorrelationId));

                        await Console.Out.WriteLineAsync("Handler complete");
                    });
                });
            });

            await Console.Out.WriteLineAsync("Starting bus");

            await bus.StartAsync(TestCancellationToken);

            await Console.Out.WriteLineAsync("Bus started");

            try
            {
                await bus.Publish(new PingMessage(), x =>
                {
                    x.RequestId = NewId.NextGuid();
                    x.ResponseAddress = bus.Address;
                });

                await consumerStarted.Task;

                await Console.Out.WriteLineAsync("Consumer Start Acknowledged");
            }
            finally
            {
                await Console.Out.WriteLineAsync("Stopping bus");

                await bus.StopAsync(TestCancellationToken);

                await Console.Out.WriteLineAsync("Bus stopped");
            }
        }

        [Test]
        public async Task Should_complete_with_nothing_running()
        {
            IBusControl bus = Bus.Factory.CreateUsingRabbitMq(x =>
            {
                IRabbitMqHost host = x.Host("localhost", "test", h =>
                {
                });

                x.ReceiveEndpoint(host, "input_queue", e =>
                {
                    e.PurgeOnStartup = true;

                    e.Handler<PingMessage>(async context =>
                    {
                        await Console.Out.WriteLineAsync("Starting handler");

                        for (var i = 0; i < 5; i++)
                        {
                            await Task.Delay(1000);

                            await Console.Out.WriteLineAsync("Handler processing");
                        }

                        await Console.Out.WriteLineAsync("Handler complete");
                    });
                });
            });

            await Console.Out.WriteLineAsync("Starting bus");

            await bus.StartAsync(TestCancellationToken);

            await Console.Out.WriteLineAsync("Bus started");

            try
            {
                await Task.Delay(1000);
            }
            finally
            {
                await Console.Out.WriteLineAsync("Stopping bus");

                await bus.StopAsync(TestCancellationToken);

                await Console.Out.WriteLineAsync("Bus stopped");
            }
        }

        public Stopping_the_bus()
            : base(new InMemoryTestHarness())
        {
        }
    }
}
