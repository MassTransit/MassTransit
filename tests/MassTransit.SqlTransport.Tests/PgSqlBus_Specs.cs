namespace MassTransit.DbTransport.Tests
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;
    using Testing;
    using UnitTests;


    [TestFixture]
    public class Configuring_the_postgresql_bus
    {
        [Test]
        [Explicit]
        public async Task Should_consume_a_lot_of_messages()
        {
            await using var provider = new ServiceCollection()
                .ConfigurePostgresTransport()
                .AddMassTransitTestHarness(TextWriter.Null, x =>
                {
                    x.AddConsumer<TestMultipleMessageConsumer>();
                    x.SetTestTimeouts(testInactivityTimeout: TimeSpan.FromSeconds(2));

                    x.UsingPostgres((context, cfg) =>
                    {
                        cfg.ReceiveEndpoint("input-queue", e =>
                        {
                            e.ConfigureConsumeTopology = false;
                            e.PrefetchCount = 30;

                            e.ConfigureConsumer<TestMultipleMessageConsumer>(context);
                        });
                    });
                })
                .BuildServiceProvider(true);

            var harness = provider.GetTestHarness();

            await harness.Start();

            var endpoint = await harness.Bus.GetSendEndpoint(new Uri("queue:input-queue"));

            var options = new ParallelOptions { MaxDegreeOfParallelism = 10 };

            var timer = Stopwatch.StartNew();

            const int limit = 1000;

            await Parallel.ForEachAsync(Enumerable.Range(0, limit), options, async (i, token) =>
            {
                await endpoint.Send(new TestMultipleMessage($"Hello, World! {i}"), x => x.SetPartitionKey(i.ToString()), token);
            });

            var sendElapsed = timer.Elapsed;

            await harness.Consumed.SelectAsync<TestMultipleMessage>().Take(limit).Count();

            var consumeElapsed = timer.Elapsed;

            timer.Stop();

            Console.WriteLine("Total send duration: {0:g}", sendElapsed);
            Console.WriteLine("Send message rate: {0:F2} (msg/s)",
                limit * 1000 / sendElapsed.TotalMilliseconds);
            Console.WriteLine("Total consume duration: {0:g}", consumeElapsed);
            Console.WriteLine("Consume message rate: {0:F2} (msg/s)",
                limit * 1000 / consumeElapsed.TotalMilliseconds);
        }

        [Test]
        [Explicit]
        public async Task Should_consume_a_lot_of_published_messages()
        {
            await using var provider = new ServiceCollection()
                .ConfigurePostgresTransport()
                .AddMassTransitTestHarness(TextWriter.Null, x =>
                {
                    x.AddConsumer<TestMultipleMessageConsumer>();
                    x.SetTestTimeouts(testInactivityTimeout: TimeSpan.FromSeconds(2));

                    x.UsingPostgres((context, cfg) =>
                    {
                        cfg.ReceiveEndpoint("input-queue", e =>
                        {
                            e.PollingInterval = TimeSpan.FromMilliseconds(10);
                            e.PrefetchCount = 30;

                            e.ConfigureConsumer<TestMultipleMessageConsumer>(context);
                        });
                    });
                })
                .BuildServiceProvider(true);

            var harness = provider.GetTestHarness();

            await harness.Start();

            var options = new ParallelOptions { MaxDegreeOfParallelism = 10 };

            var timer = Stopwatch.StartNew();

            const int limit = 1000;

            await Parallel.ForEachAsync(Enumerable.Range(0, limit), options, async (i, token) =>
            {
                await harness.Bus.Publish(new TestMultipleMessage($"Hello, World! {i}"), x => x.SetPartitionKey(i.ToString()), token);
            });

            var sendElapsed = timer.Elapsed;

            await harness.Consumed.SelectAsync<TestMultipleMessage>().Take(limit).Count();

            var consumeElapsed = timer.Elapsed;

            timer.Stop();

            Console.WriteLine("Total publish duration: {0:g}", sendElapsed);
            Console.WriteLine("Publish message rate: {0:F2} (msg/s)",
                limit * 1000 / sendElapsed.TotalMilliseconds);
            Console.WriteLine("Total consume duration: {0:g}", consumeElapsed);
            Console.WriteLine("Consume message rate: {0:F2} (msg/s)",
                limit * 1000 / consumeElapsed.TotalMilliseconds);
        }

        [Test]
        [Explicit]
        public async Task Should_give_me_so_much_of_the_datas()
        {
            await using var provider = new ServiceCollection()
                .ConfigurePostgresTransport()
                .AddMassTransitTestHarness(TextWriter.Null, x =>
                {
                    x.AddConsumer<TestMessageConsumer>();
                    x.SetTestTimeouts(testInactivityTimeout: TimeSpan.FromSeconds(2));

                    x.UsingPostgres((context, cfg) =>
                    {
                    });
                })
                .BuildServiceProvider(true);

            var harness = provider.GetTestHarness();

            await harness.Start();

            var endpoint = await harness.Bus.GetSendEndpoint(new Uri("queue:heavy-d"));

            var options = new ParallelOptions { MaxDegreeOfParallelism = 10 };

            var timer = Stopwatch.StartNew();

            const int limit = 1000;
            const int loop = 10;

            await Parallel.ForEachAsync(Enumerable.Range(0, limit), options, async (i, token) =>
            {
                for (int j = 0; j < loop; j++)
                {
                    await endpoint.Send(new TestMessage($"Hello, World! {i}"), context =>
                    {
                        context.Delay = TimeSpan.FromSeconds(j * 4);
                        context.SetPartitionKey(i.ToString());
                    }, token);
                }
            });

            var sendElapsed = timer.Elapsed;

            timer.Stop();

            Console.WriteLine("Total send duration: {0:g}", sendElapsed);
            Console.WriteLine("Send message rate: {0:F2} (msg/s)",
                limit * loop * 1000 / sendElapsed.TotalMilliseconds);

            await harness.Stop();
        }

        [Test]
        public async Task Should_support_standard_syntax_with_consumers()
        {
            await using var provider = new ServiceCollection()
                .ConfigurePostgresTransport()
                .AddMassTransitTestHarness(x =>
                {
                    x.AddConsumer<TestMessageConsumer>();
                    x.SetTestTimeouts(testInactivityTimeout: TimeSpan.FromSeconds(2));

                    x.UsingPostgres((context, cfg) =>
                    {
                        cfg.ReceiveEndpoint("input-queue", e =>
                        {
                            e.ConfigureConsumeTopology = false;
                            e.PrefetchCount = 30;

                            e.ConfigureConsumer<TestMessageConsumer>(context);
                        });
                    });
                })
                .BuildServiceProvider(true);

            var harness = provider.GetTestHarness();

            await harness.Start();

            var endpoint = await harness.Bus.GetSendEndpoint(new Uri("queue:input-queue"));

            await endpoint.Send(new TestMessage("Hello, World!"), x =>
            {
                x.Headers.Set("Simple-Header", "Some Value");
            });

            Assert.That(await harness.Consumed.Any<TestMessage>(), Is.True);

            IReceivedMessage<TestMessage> context = harness.Consumed.Select<TestMessage>().Single();

            Assert.Multiple(() =>
            {
                Assert.That(context.Context.MessageId, Is.Not.Null);
                Assert.That(context.Context.ConversationId, Is.Not.Null);
                Assert.That(context.Context.DestinationAddress, Is.Not.Null);
                Assert.That(context.Context.SourceAddress, Is.Not.Null);
            });

            await harness.Stop();
        }

        [Test]
        public async Task Should_support_standard_syntax_with_consumers_and_topology()
        {
            await using var provider = new ServiceCollection()
                .ConfigurePostgresTransport()
                .AddMassTransitTestHarness(x =>
                {
                    x.AddConsumer<TestMessageConsumer>();
                    x.AddConsumer<NestedMessageConsumer>();
                    x.SetTestTimeouts(testInactivityTimeout: TimeSpan.FromSeconds(2));

                    x.UsingPostgres((context, cfg) =>
                    {
                        cfg.ConfigureEndpoints(context);
                    });
                })
                .BuildServiceProvider(true);

            var harness = provider.GetTestHarness();

            await harness.Start();

            await harness.Bus.Publish(new MessageC(NewId.NextGuid()));

            Assert.That(await harness.Consumed.Any<MessageA>(), Is.True);
        }

        [Test]
        public async Task Should_support_the_standard_syntax()
        {
            await using var provider = new ServiceCollection()
                .ConfigurePostgresTransport()
                .AddMassTransitTestHarness(x =>
                {
                    x.UsingPostgres();
                })
                .BuildServiceProvider(true);

            var harness = provider.GetTestHarness();

            await harness.Start();

            var endpoint = await harness.Bus.GetSendEndpoint(new Uri("queue:input-queue"));

            await endpoint.Send(new TestMessage("Hello, World!"), x =>
            {
                x.Headers.Set("Simple-Header", "Some Value");
            });
        }

        [Test]
        public async Task Should_support_the_standard_syntax_with_three_queues()
        {
            await using var provider = new ServiceCollection()
                .ConfigurePostgresTransport()
                .AddMassTransitTestHarness(x =>
                {
                    x.UsingPostgres();
                })
                .BuildServiceProvider(true);

            var harness = provider.GetTestHarness();

            await harness.Start();

            var endpoint = await harness.Bus.GetSendEndpoint(new Uri("queue:input-queue"));

            await endpoint.Send(new TestMessage("Hello, World!"), x =>
            {
                x.Headers.Set("Simple-Header", "Some Value");
            });

            endpoint = await harness.Bus.GetSendEndpoint(new Uri("queue:input-queue-2"));

            await endpoint.Send(new TestMessage("Hello, World!"), x =>
            {
                x.Headers.Set("Simple-Header", "Some Value");
            });

            endpoint = await harness.Bus.GetSendEndpoint(new Uri("queue:input-queue-3"));

            await endpoint.Send(new TestMessage("Hello, World!"), x =>
            {
                x.Headers.Set("Simple-Header", "Some Value");
            });
        }
    }


    namespace UnitTests
    {
        using System;


        public record TestMessage
        {
            public TestMessage(Guid Id, string Value)
            {
                this.Id = Id;
                this.Value = Value;
            }

            public TestMessage(string Value)
            {
                Id = NewId.NextGuid();
                this.Value = Value;
            }

            public TestMessage()
            {
            }

            public Guid Id { get; init; }
            public string Value { get; init; }
        }


        public record TestMultipleMessage
        {
            public TestMultipleMessage(Guid Id, string Value)
            {
                this.Id = Id;
                this.Value = Value;
            }

            public TestMultipleMessage(string Value)
            {
                Id = NewId.NextGuid();
                this.Value = Value;
            }

            public TestMultipleMessage()
            {
            }

            public Guid Id { get; init; }
            public string Value { get; init; }
        }


        public record SlowMessage;


        public record MessageA(Guid CorrelationId);


        public record MessageB(Guid CorrelationId) :
            MessageA(CorrelationId);


        public record MessageC(Guid CorrelationId) :
            MessageB(CorrelationId);


        public class TestMessageConsumer :
            IConsumer<TestMessage>
        {
            public async Task Consume(ConsumeContext<TestMessage> context)
            {
            }
        }


        public class TestMultipleMessageConsumer :
            IConsumer<TestMultipleMessage>
        {
            public async Task Consume(ConsumeContext<TestMultipleMessage> context)
            {
            }
        }


        public class SlowMessageConsumer :
            IConsumer<SlowMessage>
        {
            public async Task Consume(ConsumeContext<SlowMessage> context)
            {
                await Task.Delay(TimeSpan.FromMinutes(2), context.CancellationToken);

                LogContext.Info?.Log("Consumed the message");
            }
        }


        public class NestedMessageConsumer :
            IConsumer<MessageA>
        {
            public async Task Consume(ConsumeContext<MessageA> context)
            {
            }
        }
    }
}
