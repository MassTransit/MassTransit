// Copyright 2007-2018 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using GreenPipes.Util;
    using Internals.Extensions;
    using NUnit.Framework;
    using RabbitMQ.Client;
    using RoutingKeyDirect;


    namespace RoutingKeyDirect
    {
        public class Message
        {
            public Message(string content, string routingKey)
            {
                Content = content;
                RoutingKey = routingKey;
            }

            public string RoutingKey { get; set; }

            public string Content { get; set; }
        }
    }


    [TestFixture]
    public class Using_a_routing_key_and_direct_exchange :
        RabbitMqTestFixture
    {
        [Test]
        public async Task Should_support_routing_by_key_and_exchange_name()
        {
            var fooHandle = await Subscribe("foo");
            try
            {
                var barHandle = await Subscribe("bar");
                try
                {
                    await Bus.Publish(new Message("Hello", "foo"));
                    await Bus.Publish(new Message("World", "bar"));

                    await Consumer.Foo;
                    await Consumer.Bar;
                }
                finally
                {
                    await barHandle.StopAsync(TestCancellationToken);
                }
            }
            finally
            {
                await fooHandle.StopAsync(TestCancellationToken);
            }
        }

        async Task<HostReceiveEndpointHandle> Subscribe(string key)
        {
            var queueName = $"TestCase-R-{key}";
            var handle = Host.ConnectReceiveEndpoint(queueName, x =>
            {
                x.BindMessageExchanges = false;
                x.Consumer<Consumer>();

                x.Bind<Message>(e =>
                {
                    e.RoutingKey = GetRoutingKey(key);
                    e.ExchangeType = ExchangeType.Direct;
                });
            });

            await handle.Ready;

            return handle;
        }

        protected override void ConfigureRabbitMqBusHost(IRabbitMqBusFactoryConfigurator configurator, IRabbitMqHost host)
        {
            base.ConfigureRabbitMqBusHost(configurator, host);

            configurator.Message<Message>(x => x.SetEntityName(ExchangeName));
            configurator.Publish<Message>(x => x.ExchangeType = ExchangeType.Direct);

            configurator.Send<Message>(x => x.UseRoutingKeyFormatter(context => GetRoutingKey(context.Message.RoutingKey)));
        }

        string ExchangeName { get; } = "TestCase-Buffer";

        string GetRoutingKey(string routingKey)
        {
            return $"prefix-{routingKey}";
        }


        class Consumer :
            IConsumer<Message>
        {
            static readonly TaskCompletionSource<Message> _foo = TaskCompletionSourceFactory.New<Message>();
            static readonly TaskCompletionSource<Message> _bar = TaskCompletionSourceFactory.New<Message>();
            public static Task<Message> Foo => _foo.Task;
            public static Task<Message> Bar => _bar.Task;

            public Task Consume(ConsumeContext<Message> context)
            {
                Console.WriteLine($"Received {context.Message.Content} for {context.RoutingKey()}");

                if (context.Message.RoutingKey == "foo")
                    _foo.TrySetResult(context.Message);

                if (context.Message.RoutingKey == "bar")
                    _bar.TrySetResult(context.Message);

                return TaskUtil.Completed;
            }
        }
    }
}
