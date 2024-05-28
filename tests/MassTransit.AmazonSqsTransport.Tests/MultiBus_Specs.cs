namespace MassTransit.AmazonSqsTransport.Tests
{
    namespace MultiBusTests
    {
        using System;
        using System.Threading.Tasks;
        using Microsoft.Extensions.DependencyInjection;
        using NUnit.Framework;
        using Testing;


        public class MyMessageConsumer :
            IConsumer<MyMessage>
        {
            async Task IConsumer<MyMessage>.Consume(ConsumeContext<MyMessage> context)
            {
                Console.WriteLine($"Received Message from server {context.Message.Server}, msg: {context.Message.MsgData}");
                await Task.CompletedTask;
            }
        }


        public class BusEnvironmentNameFormatter : IEntityNameFormatter
        {
            readonly string _prefix;

            public BusEnvironmentNameFormatter(string server)
            {
                _prefix = $"{server}-topic-";
            }

            public string FormatEntityName<T>()
            {
                Console.WriteLine($"NameFormatter {_prefix}{typeof(T).Name}");
                return _prefix + typeof(T).Name;
            }
        }


        public class MyMessage
        {
            public int Server { get; set; }
            public string MsgData { get; set; }
        }


        public interface ISecondBus : IBus
        {
        }


        public class Using_two_separate_entity_name_formatters_with_multiple_buses
        {
            [Test]
            public async Task Should_work()
            {
                await using var provider = new ServiceCollection()
                    .AddMassTransitTestHarness(x =>
                    {
                        x.AddConsumer<MyMessageConsumer>(cfg => cfg.ConcurrentMessageLimit = 1);

                        x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("server1", false));

                        x.UsingAmazonSqs((context, cfg) =>
                        {
                            cfg.LocalstackHost();

                            cfg.MessageTopology.SetEntityNameFormatter(new BusEnvironmentNameFormatter("server1"));

                            cfg.ConfigureEndpoints(context);
                        });
                    })
                    .AddMassTransit<ISecondBus>(x =>
                    {
                        x.AddConsumer<MyMessageConsumer>(cfg => cfg.ConcurrentMessageLimit = 1);

                        x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("server2", false));

                        x.UsingAmazonSqs((context, cfg) =>
                        {
                            cfg.LocalstackHost();

                            cfg.MessageTopology.SetEntityNameFormatter(new BusEnvironmentNameFormatter("server2"));

                            cfg.ConfigureEndpoints(context);
                        });
                    })
                    .BuildServiceProvider(true);

                var harness = await provider.StartTestHarness();

                var bus = harness.Bus;

                Assert.That(bus.Topology.Publish<MyMessage>().TryGetPublishAddress(bus.Address, out var publishAddress), Is.True);
                Assert.That(publishAddress, Is.EqualTo(new Uri(bus.Address, "server1-topic-MyMessage?type=topic")));

                await bus.Publish(new MyMessage
                {
                    Server = 1,
                    MsgData = "Hello",
                });

                bus = provider.GetRequiredService<ISecondBus>();

                Assert.That(bus.Topology.Publish<MyMessage>().TryGetPublishAddress(bus.Address, out publishAddress), Is.True);
                Assert.That(publishAddress, Is.EqualTo(new Uri(bus.Address, "server2-topic-MyMessage?type=topic")));

                await bus.Publish(new MyMessage
                {
                    Server = 2,
                    MsgData = "Hello",
                });

                Assert.That(await harness.Consumed.Any<MyMessage>(x => x.Context.Message.Server == 1), Is.True);
            }
        }
    }
}
