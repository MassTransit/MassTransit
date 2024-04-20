namespace MassTransit.KafkaIntegration.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Confluent.Kafka;
    using Internals;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;
    using TestFramework;
    using Testing;
    using Transports;


    public class MultiBus_Specs :
        InMemoryTestFixture
    {
        const string FirstTopic = "producer-bus-one";
        const string SecondTopic = "producer-bus-two";

        public MultiBus_Specs()
        {
            TestTimeout = TimeSpan.FromMinutes(2);
        }

        [Test]
        public async Task Should_receive_in_both_buses()
        {
            var services = new ServiceCollection();
            await using var provider = services
                .ConfigureKafkaTestOptions(options =>
                {
                    options.CreateTopicsIfNotExists = true;
                    options.TopicNames = new[] { FirstTopic, SecondTopic };
                })
                .AddSingleton<ILoggerFactory>(LoggerFactory)
                .AddSingleton(typeof(ILogger<>), typeof(Logger<>))
                .AddSingleton(GetTask<ConsumeContext<FirstBusMessage>>())
                .AddSingleton(GetTask<ConsumeContext<SecondBusMessage>>())
                .AddMassTransit<IFirstBus>(x =>
                {
                    x.AddRider(r =>
                    {
                        r.AddConsumer<FirstBusMessageConsumer>();

                        r.AddProducer<FirstBusMessage>(FirstTopic);

                        r.UsingKafka((context, k) =>
                        {
                            k.TopicEndpoint<FirstBusMessage>(FirstTopic, nameof(MultiBus_Specs), c =>
                            {
                                c.AutoOffsetReset = AutoOffsetReset.Earliest;

                                c.ConfigureConsumer<FirstBusMessageConsumer>(context);
                            });
                        });
                    });

                    x.UsingInMemory();
                })
                .AddMassTransit<ISecondBus>(x =>
                {
                    x.AddRider(r =>
                    {
                        r.AddConsumer<SecondBusMessageConsumer>();

                        r.AddProducer<SecondBusMessage>(SecondTopic);

                        r.UsingKafka((context, k) =>
                        {
                            k.TopicEndpoint<SecondBusMessage>(SecondTopic, nameof(MultiBus_Specs), c =>
                            {
                                c.AutoOffsetReset = AutoOffsetReset.Earliest;

                                c.ConfigureConsumer<SecondBusMessageConsumer>(context);
                            });
                        });
                    });

                    x.UsingInMemory();
                })
                .BuildServiceProvider(true);

            IHostedService[] hostedServices = provider.GetServices<IHostedService>().ToArray();
            for (var i = 0; i < hostedServices.Length; i++)
                await hostedServices[i].StartAsync(TestCancellationToken);

            var serviceScope = provider.CreateScope();

            var producer = serviceScope.ServiceProvider.GetRequiredService<ITopicProducer<FirstBusMessage>>();

            await producer.Produce(new FirstBusMessage(), TestCancellationToken);

            await provider.GetRequiredService<TaskCompletionSource<ConsumeContext<FirstBusMessage>>>().Task.OrCanceled(TestCancellationToken);

            await provider.GetRequiredService<TaskCompletionSource<ConsumeContext<SecondBusMessage>>>().Task.OrCanceled(TestCancellationToken);

            serviceScope.Dispose();

            await Task.WhenAll(hostedServices.Select(x => x.StopAsync(TestCancellationToken)));
        }


        public interface ISecondBus :
            IBus
        {
        }


        public interface IFirstBus :
            IBus
        {
        }


        class FirstBusMessage
        {
        }


        class SecondBusMessage
        {
        }


        class FirstBusMessageConsumer :
            IConsumer<FirstBusMessage>
        {
            readonly ITopicProducer<SecondBusMessage> _producer;
            readonly TaskCompletionSource<ConsumeContext<FirstBusMessage>> _taskCompletionSource;

            public FirstBusMessageConsumer(TaskCompletionSource<ConsumeContext<FirstBusMessage>> taskCompletionSource,
                ITopicProducer<SecondBusMessage> producer)
            {
                _taskCompletionSource = taskCompletionSource;
                _producer = producer;
            }

            public async Task Consume(ConsumeContext<FirstBusMessage> context)
            {
                await _producer.Produce(new SecondBusMessage());
                _taskCompletionSource.TrySetResult(context);
            }
        }


        class SecondBusMessageConsumer :
            IConsumer<SecondBusMessage>
        {
            readonly TaskCompletionSource<ConsumeContext<SecondBusMessage>> _taskCompletionSource;

            public SecondBusMessageConsumer(TaskCompletionSource<ConsumeContext<SecondBusMessage>> taskCompletionSource)
            {
                _taskCompletionSource = taskCompletionSource;
            }

            public async Task Consume(ConsumeContext<SecondBusMessage> context)
            {
                _taskCompletionSource.TrySetResult(context);
            }
        }
    }


    [Category("Flaky")]
    public class MultiBus_ReBalance_Specs :
        InMemoryTestFixture
    {
        const string Topic = "long-receive-test-multi";

        public MultiBus_ReBalance_Specs()
        {
            TestTimeout = TimeSpan.FromMinutes(2);
        }

        [Test]
        public async Task Should_receive_message_and_stabilize_group_with_multi_bus()
        {
            TaskCompletionSource<ConsumeContext<KafkaMessage>> taskCompletionSource = GetTask<ConsumeContext<KafkaMessage>>();
            var services = new ServiceCollection();
            await using var provider = services
                .ConfigureKafkaTestOptions(options =>
                {
                    options.CreateTopicsIfNotExists = true;
                    options.TopicNames = new[] { Topic };
                })
                .AddSingleton(taskCompletionSource)
                .AddSingleton<ILoggerFactory>(LoggerFactory)
                .AddSingleton(typeof(ILogger<>), typeof(Logger<>))
                .AddMassTransit(x =>
                {
                    x.UsingInMemory((context, cfg) => cfg.ConfigureEndpoints(context));
                    x.AddRider(rider =>
                    {
                        rider.AddConsumer<KafkaMessageConsumer>();
                        rider.AddProducer<KafkaMessage>(Topic);

                        rider.UsingKafka((context, k) =>
                        {
                            k.TopicEndpoint<KafkaMessage>(Topic, nameof(MultiBus_ReBalance_Specs), c =>
                            {
                                c.AutoOffsetReset = AutoOffsetReset.Earliest;

                                c.ConfigureConsumer<KafkaMessageConsumer>(context);
                            });
                        });
                    });
                })
                .AddMassTransit<ISecondBus>(x =>
                {
                    x.UsingInMemory((context, cfg) => cfg.ConfigureEndpoints(context));
                    x.AddRider(rider =>
                    {
                        rider.AddConsumer<KafkaMessageConsumer>();
                        rider.AddProducer<KafkaMessage>(Topic);

                        rider.UsingKafka((context, k) =>
                        {
                            k.TopicEndpoint<KafkaMessage>(Topic, nameof(MultiBus_ReBalance_Specs), c =>
                            {
                                c.AutoOffsetReset = AutoOffsetReset.Earliest;

                                c.ConfigureConsumer<KafkaMessageConsumer>(context);
                            });
                        });
                    });
                }).BuildServiceProvider();

            IEnumerable<IHostedService> hostedServices = provider.GetServices<IHostedService>().OfType<KafkaTestHarnessHostedService>();
            await Task.WhenAll(hostedServices.Select(x => x.StartAsync(TestCancellationToken)));

            var busControl = provider.GetRequiredService<IBusControl>();

            var scope = provider.CreateScope();

            await busControl.StartAsync(TestCancellationToken);

            var adminClient = provider.GetRequiredService<IAdminClient>();

            var producer = scope.ServiceProvider.GetRequiredService<ITopicProducer<KafkaMessage>>();

            try
            {
                await producer.Produce(new { }, TestCancellationToken);

                var secondBus = provider.GetRequiredService<IBusInstance<ISecondBus>>();
                await secondBus.BusControl.StartAsync(TestCancellationToken);

                await taskCompletionSource.Task.OrCanceled(TestCancellationToken);

                var groupInfo = adminClient.ListGroup(nameof(MultiBus_ReBalance_Specs), TimeSpan.FromSeconds(5));

                while (groupInfo.State != "Stable")
                {
                    await Task.Delay(TimeSpan.FromSeconds(10)).OrCanceled(TestCancellationToken);
                    groupInfo = adminClient.ListGroup(nameof(MultiBus_ReBalance_Specs), TimeSpan.FromSeconds(5));
                }

                Assert.That(groupInfo.Members, Has.Count.EqualTo(2)); // this fails, second instance consumer assigned to all partitions

                await secondBus.BusControl.StopAsync(TestCancellationToken);
            }
            finally
            {
                scope.Dispose();

                await busControl.StopAsync(TestCancellationToken);

                await provider.DisposeAsync();
            }
        }


        class KafkaMessageConsumer :
            IConsumer<KafkaMessage>
        {
            readonly TaskCompletionSource<ConsumeContext<KafkaMessage>> _taskCompletionSource;

            public KafkaMessageConsumer(TaskCompletionSource<ConsumeContext<KafkaMessage>> taskCompletionSource)
            {
                _taskCompletionSource = taskCompletionSource;
            }

            public async Task Consume(ConsumeContext<KafkaMessage> context)
            {
                await Task.Delay(TimeSpan.FromSeconds(20));
                _taskCompletionSource.TrySetResult(context);
            }
        }


        public interface ISecondBus :
            IBus
        {
        }


        public interface KafkaMessage
        {
        }
    }


    [Category("Flaky")]
    public class MultiBus_ConcurrentConsumers_ReBalance_Specs :
        InMemoryTestFixture
    {
        const string Topic = "concurrent-rebalance-multi";

        public MultiBus_ConcurrentConsumers_ReBalance_Specs()
        {
            TestTimeout = TimeSpan.FromMinutes(2);
        }

        [Test]
        public async Task Should_receive_message_and_stabilize_group_with_multi_bus_and_multiple_consumers()
        {
            TaskCompletionSource<ConsumeContext<KafkaMessage>> taskCompletionSource = GetTask<ConsumeContext<KafkaMessage>>();

            var services = new ServiceCollection();
            await using var provider = services
                .ConfigureKafkaTestOptions(options =>
                {
                    options.CreateTopicsIfNotExists = true;
                    options.TopicNames = new[] { Topic };
                    options.Partitions = 6;
                })
                .AddSingleton(taskCompletionSource)
                .AddSingleton<ILoggerFactory>(LoggerFactory)
                .AddSingleton(typeof(ILogger<>), typeof(Logger<>))
                .AddMassTransit(x =>
                {
                    x.UsingInMemory((context, cfg) => cfg.ConfigureEndpoints(context));
                    x.AddRider(rider =>
                    {
                        rider.AddConsumer<KafkaMessageConsumer>();
                        rider.AddProducer<KafkaMessage>(Topic);

                        rider.UsingKafka((context, k) =>
                        {
                            k.TopicEndpoint<KafkaMessage>(Topic, nameof(MultiBus_ConcurrentConsumers_ReBalance_Specs), c =>
                            {
                                c.AutoOffsetReset = AutoOffsetReset.Earliest;
                                c.ConcurrentConsumerLimit = 3;
                                // Should increase speed for re-balance
                                c.CheckpointInterval = TimeSpan.FromMilliseconds(100);
                                c.ConfigureConsumer<KafkaMessageConsumer>(context);
                            });
                        });
                    });
                })
                .AddMassTransit<ISecondBus>(x =>
                {
                    x.UsingInMemory((context, cfg) => cfg.ConfigureEndpoints(context));
                    x.AddRider(rider =>
                    {
                        rider.AddConsumer<KafkaMessageConsumer>();
                        rider.AddProducer<KafkaMessage>(Topic);

                        rider.UsingKafka((context, k) =>
                        {
                            k.TopicEndpoint<KafkaMessage>(Topic, nameof(MultiBus_ConcurrentConsumers_ReBalance_Specs), c =>
                            {
                                c.AutoOffsetReset = AutoOffsetReset.Earliest;
                                c.ConcurrentConsumerLimit = 3;
                                c.ConfigureConsumer<KafkaMessageConsumer>(context);
                            });
                        });
                    });
                }).BuildServiceProvider();


            IEnumerable<KafkaTestHarnessHostedService> kafkaHostedServices = provider.GetServices<IHostedService>().OfType<KafkaTestHarnessHostedService>();
            await Task.WhenAll(kafkaHostedServices.Select(x => x.StartAsync(TestCancellationToken)));

            var busControl = provider.GetRequiredService<IBusControl>();
            var secondBus = provider.GetRequiredService<IBusInstance<ISecondBus>>();

            var scope = provider.CreateScope();

            await busControl.StartAsync(TestCancellationToken);

            var producer = scope.ServiceProvider.GetRequiredService<ITopicProducer<KafkaMessage>>();
            var adminClient = provider.GetRequiredService<IAdminClient>();

            try
            {
                await producer.Produce(new { }, TestCancellationToken);

                await secondBus.BusControl.StartAsync(TestCancellationToken);

                await taskCompletionSource.Task.OrCanceled(TestCancellationToken);

                var groupInfo = adminClient.ListGroup(nameof(MultiBus_ConcurrentConsumers_ReBalance_Specs), TimeSpan.FromSeconds(5));

                while (groupInfo.State != "Stable")
                {
                    await Task.Delay(TimeSpan.FromSeconds(10), TestCancellationToken);
                    groupInfo = adminClient.ListGroup(nameof(MultiBus_ConcurrentConsumers_ReBalance_Specs), TimeSpan.FromSeconds(5));
                }

                Assert.That(groupInfo.Members, Has.Count.EqualTo(6)); // this fails, second instance consumer assigned to all partitions

                await secondBus.BusControl.StopAsync(TestCancellationToken);
            }
            finally
            {
                scope.Dispose();

                await busControl.StopAsync(TestCancellationToken);
                await secondBus.BusControl.StopAsync(TestCancellationToken);
            }
        }


        class KafkaMessageConsumer :
            IConsumer<KafkaMessage>
        {
            readonly TaskCompletionSource<ConsumeContext<KafkaMessage>> _taskCompletionSource;

            public KafkaMessageConsumer(TaskCompletionSource<ConsumeContext<KafkaMessage>> taskCompletionSource)
            {
                _taskCompletionSource = taskCompletionSource;
            }

            public async Task Consume(ConsumeContext<KafkaMessage> context)
            {
                await Task.Delay(TimeSpan.FromSeconds(10));
                _taskCompletionSource.TrySetResult(context);
            }
        }


        public interface ISecondBus :
            IBus
        {
        }


        public interface KafkaMessage
        {
        }
    }
}
