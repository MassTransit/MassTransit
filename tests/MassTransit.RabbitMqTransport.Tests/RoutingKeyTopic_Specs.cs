namespace MassTransit.RabbitMqTransport.Tests
{
    using System;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using RabbitMQ.Client;
    using RoutingKeyTopic;
    using Util;


    namespace RoutingKeyTopic
    {
        public class Message
        {
            public Message(decimal price, string symbol)
            {
                Price = price;
                Symbol = symbol;
            }

            public string Symbol { get; set; }

            public decimal Price { get; set; }
        }
    }


    [TestFixture]
    public class Using_a_routing_key_and_topic_exchange :
        RabbitMqTestFixture
    {
        [Test]
        public async Task Should_support_routing_by_key_and_exchange_name()
        {
            var fooHandle = await Subscribe("MSFT");
            try
            {
                var barHandle = await Subscribe("UBER");
                try
                {
                    await Bus.Publish(new Message(100.0m, "MSFT"));
                    await Bus.Publish(new Message(3.50m, "UBER"));

                    await Consumer.Microsoft;
                    await Consumer.Uber;
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
            var queueName = $"Stock-{key}";
            var handle = Bus.ConnectReceiveEndpoint(queueName, x =>
            {
                x.ConfigureConsumeTopology = false;
                x.Consumer<Consumer>();

                ((IRabbitMqReceiveEndpointConfigurator)x).Bind<Message>(e =>
                {
                    e.RoutingKey = GetRoutingKey(key);
                    e.ExchangeType = ExchangeType.Topic;
                });
            });

            await handle.Ready;

            return handle;
        }

        protected override void ConfigureRabbitMqBus(IRabbitMqBusFactoryConfigurator configurator)
        {
            configurator.Message<Message>(x => x.SetEntityName(ExchangeName));
            configurator.Publish<Message>(x => x.ExchangeType = ExchangeType.Topic);

            configurator.Send<Message>(x => x.UseRoutingKeyFormatter(context => GetRoutingKey(context.Message.Symbol)));
        }

        string ExchangeName { get; } = "Quotes";

        string GetRoutingKey(string routingKey)
        {
            return $"quote.{routingKey}";
        }


        class Consumer :
            IConsumer<Message>
        {
            static readonly TaskCompletionSource<Message> _microsoft = TaskUtil.GetTask<Message>();
            static readonly TaskCompletionSource<Message> _uber = TaskUtil.GetTask<Message>();
            public static Task<Message> Microsoft => _microsoft.Task;
            public static Task<Message> Uber => _uber.Task;

            public Task Consume(ConsumeContext<Message> context)
            {
                Console.WriteLine($"Received {context.Message.Symbol} for {context.RoutingKey()}");

                if (context.Message.Symbol == "MSFT")
                    _microsoft.TrySetResult(context.Message);

                if (context.Message.Symbol == "UBER")
                    _uber.TrySetResult(context.Message);

                return Task.CompletedTask;
            }
        }
    }
}
