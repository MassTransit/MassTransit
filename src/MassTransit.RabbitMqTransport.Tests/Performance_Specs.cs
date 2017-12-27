// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using TestFramework.Messages;


    [TestFixture, Explicit, Category("SlowAF")]
    public class Performance_of_the_RabbitMQ_transport :
        RabbitMqTestFixture
    {
        [Test]
        public async Task Should_be_wicked_fast()
        {
            int limit = 20000;
            int count = 0;

            await _requestClient.Request(new PingMessage());

            Stopwatch timer = Stopwatch.StartNew();

            await Task.WhenAll(Enumerable.Range(0, limit).Select(async x =>
            {
                await _requestClient.Request(new PingMessage());

                Interlocked.Increment(ref count);
            }));

            timer.Stop();

            Console.WriteLine("Time to process {0} messages = {1}", count * 2, timer.ElapsedMilliseconds + "ms");
            Console.WriteLine("Messages per second: {0}", count * 2 * 1000 / timer.ElapsedMilliseconds);
        }

        IRequestClient<PingMessage, PongMessage> _requestClient;

        [OneTimeSetUp]
        public void Setup()
        {
            _requestClient = new MessageRequestClient<PingMessage, PongMessage>(Bus, InputQueueAddress, TestTimeout);
        }

        protected override void ConfigureRabbitMqBus(IRabbitMqBusFactoryConfigurator configurator)
        {
            base.ConfigureRabbitMqBus(configurator);

            configurator.PrefetchCount = 100;
        }

        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            configurator.PrefetchCount = 100;

            configurator.Handler<PingMessage>(async context =>
            {
                try
                {
                    await context.RespondAsync(new PongMessage(context.Message.CorrelationId));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            });
        }
    }


    [TestFixture, Explicit, Category("SlowAF")]
    public class Performance_of_the_RabbitMQ_transport_non_durable :
        RabbitMqTestFixture
    {
        [Test]
        public async Task Should_be_wicked_fast()
        {
            int limit = 20000;
            int count = 0;

            await _requestClient.Request(new PingMessage());

            Stopwatch timer = Stopwatch.StartNew();

            await Task.WhenAll(Enumerable.Range(0, limit).Select(async x =>
            {
                await _requestClient.Request(new PingMessage());

                Interlocked.Increment(ref count);
            }));

            timer.Stop();

            Console.WriteLine("Time to process {0} messages = {1}", count * 2, timer.ElapsedMilliseconds + "ms");
            Console.WriteLine("Messages per second: {0}", count * 2 * 1000 / timer.ElapsedMilliseconds);
        }

        IRequestClient<PingMessage, PongMessage> _requestClient;
        Uri _serviceAddress;

        [OneTimeSetUp]
        public void Setup()
        {
            _requestClient = new MessageRequestClient<PingMessage, PongMessage>(Bus, _serviceAddress, TestTimeout);
        }

        protected override void ConfigureRabbitMqBusHost(IRabbitMqBusFactoryConfigurator configurator, IRabbitMqHost host)
        {
            base.ConfigureRabbitMqBusHost(configurator, host);

            configurator.PrefetchCount = 100;

            configurator.ReceiveEndpoint(host, "input_queue_express", x =>
            {
                x.AutoDelete = true;
                x.Durable = false;
                x.PrefetchCount = 100;

                _serviceAddress = x.InputAddress;

                x.Handler<PingMessage>(async context =>
                {
                    try
                    {
                        await context.RespondAsync(new PongMessage(context.Message.CorrelationId));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                });
            });
        }
    }
}