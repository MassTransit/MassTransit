namespace MassTransit.GrpcTransport.Tests
{
    using System;
    using System.Threading.Tasks;
    using Internals;
    using NUnit.Framework;
    using RoutingKeyDirect;
    using Transports.Fabric;
    using Util;


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
        GrpcTestFixture
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

                    await Consumer.Foo.OrTimeout(TimeSpan.FromSeconds(5));
                    await Consumer.Bar.OrTimeout(TimeSpan.FromSeconds(5));
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
            var handle = Bus.ConnectReceiveEndpoint(queueName, x =>
            {
                x.ConfigureConsumeTopology = false;
                x.Consumer<Consumer>();

                x.Bind<Message>(ExchangeType.Direct, GetRoutingKey(key));
            });

            await handle.Ready;

            return handle;
        }

        protected override void ConfigureGrpcBus(IGrpcBusFactoryConfigurator configurator)
        {
            configurator.Message<Message>(x => x.SetEntityName(ExchangeName));
            configurator.Publish<Message>(x => x.ExchangeType = ExchangeType.Direct);

            configurator.Send<Message>(x => x.UseRoutingKeyFormatter(context => GetRoutingKey(context.Message.RoutingKey)));
        }

        string ExchangeName = "TestCase-Buffer";

        string GetRoutingKey(string routingKey)
        {
            return $"prefix-{routingKey}";
        }


        class Consumer :
            IConsumer<Message>
        {
            static readonly TaskCompletionSource<Message> _foo = TaskUtil.GetTask<Message>();
            static readonly TaskCompletionSource<Message> _bar = TaskUtil.GetTask<Message>();
            public static Task<Message> Foo => _foo.Task;
            public static Task<Message> Bar => _bar.Task;

            public Task Consume(ConsumeContext<Message> context)
            {
                Console.WriteLine($"Received {context.Message.Content} for {context.RoutingKey()}");

                if (context.Message.RoutingKey == "foo")
                    _foo.TrySetResult(context.Message);

                if (context.Message.RoutingKey == "bar")
                    _bar.TrySetResult(context.Message);

                return Task.CompletedTask;
            }
        }
    }
}
