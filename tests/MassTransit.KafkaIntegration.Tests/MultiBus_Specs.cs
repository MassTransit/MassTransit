namespace MassTransit.KafkaIntegration.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Confluent.Kafka;
    using Internals;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;
    using TestFramework;
    using Transports;


    public class MultiBus_Specs :
        InMemoryTestFixture
    {
        const string FirstTopic = "producer-bus-one";
        const string SecondTopic = "producer-bus-two";

        [Test]
        public async Task Should_receive_in_both_buses()
        {
            await using var provider = new ServiceCollection()
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
                            k.Host("localhost:9092");

                            k.TopicEndpoint<FirstBusMessage>(FirstTopic, nameof(MultiBus_Specs), c =>
                            {
                                c.AutoOffsetReset = AutoOffsetReset.Earliest;
                                c.CreateIfMissing();

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
                            k.Host("localhost:9092");

                            k.TopicEndpoint<SecondBusMessage>(SecondTopic, nameof(MultiBus_Specs), c =>
                            {
                                c.AutoOffsetReset = AutoOffsetReset.Earliest;
                                c.CreateIfMissing();

                                c.ConfigureConsumer<SecondBusMessageConsumer>(context);
                            });
                        });
                    });

                    x.UsingInMemory();
                })
                .BuildServiceProvider(true);

            IEnumerable<IHostedService> hostedServices = provider.GetServices<IHostedService>().ToArray();

            await Task.WhenAll(hostedServices.Select(x => x.StartAsync(TestCancellationToken)));

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


    public class MultiBus_ReBalance_Specs :
        InMemoryTestFixture
    {
        const string Topic = "long-receive-test-multi";

        [Test]
        public async Task Should_receive()
        {
            TaskCompletionSource<ConsumeContext<KafkaMessage>> taskCompletionSource = GetTask<ConsumeContext<KafkaMessage>>();

            var config = new AdminClientConfig
            {
                BootstrapServers = "localhost:9092",
            };

            var adminClient = new AdminClientBuilder(config).Build();

            var services = new ServiceCollection();
            services.AddSingleton(taskCompletionSource);

            services.TryAddSingleton<ILoggerFactory>(LoggerFactory);
            services.TryAddSingleton(typeof(ILogger<>), typeof(Logger<>));

            services.AddMassTransit(x =>
            {
                x.UsingInMemory((context, cfg) => cfg.ConfigureEndpoints(context));
                x.AddRider(rider =>
                {
                    rider.AddConsumer<KafkaMessageConsumer>();
                    rider.AddProducer<KafkaMessage>(Topic);

                    rider.UsingKafka((context, k) =>
                    {
                        k.Host("localhost:9092");

                        k.TopicEndpoint<KafkaMessage>(Topic, nameof(MultiBus_ReBalance_Specs), c =>
                        {
                            c.CreateIfMissing(t => t.NumPartitions = 6);
                            c.ConfigureConsumer<KafkaMessageConsumer>(context);
                        });
                    });
                });
            });

            services.AddMassTransit<ISecondBus>(x =>
            {
                x.UsingInMemory((context, cfg) => cfg.ConfigureEndpoints(context));
                x.AddRider(rider =>
                {
                    rider.AddConsumer<KafkaMessageConsumer>();
                    rider.AddProducer<KafkaMessage>(Topic);

                    rider.UsingKafka((context, k) =>
                    {
                        k.Host("localhost:9092");

                        k.TopicEndpoint<KafkaMessage>(Topic, nameof(MultiBus_ReBalance_Specs), c =>
                        {
                            c.ConfigureConsumer<KafkaMessageConsumer>(context);
                        });
                    });
                });
            });

            var provider = services.BuildServiceProvider();

            var busControl = provider.GetRequiredService<IBusControl>();

            var scope = provider.CreateScope();

            await busControl.StartAsync(TestCancellationToken);

            var producer = scope.ServiceProvider.GetRequiredService<ITopicProducer<KafkaMessage>>();

            try
            {
                var messageId = NewId.NextGuid();
                await producer.Produce(new { }, Pipe.Execute<SendContext>(context => context.MessageId = messageId), TestCancellationToken);

                var secondBus = provider.GetRequiredService<IBusInstance<ISecondBus>>();
                await secondBus.BusControl.StartAsync(TestCancellationToken);

                ConsumeContext<KafkaMessage> result = await taskCompletionSource.Task;

                var groupInfo = adminClient.ListGroup(nameof(MultiBus_ReBalance_Specs), TimeSpan.FromSeconds(5));

                while (groupInfo.State != "Stable")
                {
                    await Task.Delay(TimeSpan.FromSeconds(10));
                    groupInfo = adminClient.ListGroup(nameof(MultiBus_ReBalance_Specs), TimeSpan.FromSeconds(5));
                }

                Assert.AreEqual(messageId, result.MessageId);
                Assert.AreEqual(groupInfo.Members.Count, 2); // this fails, second instance consumer assigned to all partitions

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


    public class MultiBus_ConcurrentConsumers_ReBalance_Specs :
        InMemoryTestFixture
    {
        const string Topic = "concurrent-rebalance-receive-test-multi";

        public MultiBus_ConcurrentConsumers_ReBalance_Specs()
        {
            TestTimeout = TimeSpan.FromMinutes(2);
        }

        [Test]
        public async Task Should_receive()
        {
            TaskCompletionSource<ConsumeContext<KafkaMessage>> taskCompletionSource = GetTask<ConsumeContext<KafkaMessage>>();

            var config = new AdminClientConfig
            {
                BootstrapServers = "localhost:9092",
            };

            var adminClient = new AdminClientBuilder(config).Build();

            var services = new ServiceCollection();
            services.AddSingleton(taskCompletionSource);

            services.TryAddSingleton<ILoggerFactory>(LoggerFactory);
            services.TryAddSingleton(typeof(ILogger<>), typeof(Logger<>));

            services.AddMassTransit(x =>
            {
                x.UsingInMemory((context, cfg) => cfg.ConfigureEndpoints(context));
                x.AddRider(rider =>
                {
                    rider.AddConsumer<KafkaMessageConsumer>();
                    rider.AddProducer<KafkaMessage>(Topic);

                    rider.UsingKafka((context, k) =>
                    {
                        k.Host("localhost:9092");

                        k.TopicEndpoint<KafkaMessage>(Topic, nameof(MultiBus_ConcurrentConsumers_ReBalance_Specs), c =>
                        {
                            c.ConcurrentConsumerLimit = 3;
                            c.CreateIfMissing(t => t.NumPartitions = 6);
                            c.ConfigureConsumer<KafkaMessageConsumer>(context);
                        });
                    });
                });
            });

            services.AddMassTransit<ISecondBus>(x =>
            {
                x.UsingInMemory((context, cfg) => cfg.ConfigureEndpoints(context));
                x.AddRider(rider =>
                {
                    rider.AddConsumer<KafkaMessageConsumer>();
                    rider.AddProducer<KafkaMessage>(Topic);

                    rider.UsingKafka((context, k) =>
                    {
                        k.Host("localhost:9092");

                        k.TopicEndpoint<KafkaMessage>(Topic, nameof(MultiBus_ConcurrentConsumers_ReBalance_Specs), c =>
                        {
                            c.ConcurrentConsumerLimit = 3;
                            c.ConfigureConsumer<KafkaMessageConsumer>(context);
                        });
                    });
                });
            });

            var provider = services.BuildServiceProvider();

            var busControl = provider.GetRequiredService<IBusControl>();

            var scope = provider.CreateScope();

            await busControl.StartAsync(TestCancellationToken);

            var producer = scope.ServiceProvider.GetRequiredService<ITopicProducer<KafkaMessage>>();

            try
            {
                var messageId = NewId.NextGuid();
                await producer.Produce(new { }, Pipe.Execute<SendContext>(context => context.MessageId = messageId), TestCancellationToken);

                var secondBus = provider.GetRequiredService<IBusInstance<ISecondBus>>();
                await secondBus.BusControl.StartAsync(TestCancellationToken);

                ConsumeContext<KafkaMessage> result = await taskCompletionSource.Task;

                var groupInfo = adminClient.ListGroup(nameof(MultiBus_ConcurrentConsumers_ReBalance_Specs), TimeSpan.FromSeconds(5));

                while (groupInfo.State != "Stable")
                {
                    await Task.Delay(TimeSpan.FromSeconds(10));
                    groupInfo = adminClient.ListGroup(nameof(MultiBus_ConcurrentConsumers_ReBalance_Specs), TimeSpan.FromSeconds(5));
                }

                Assert.AreEqual(messageId, result.MessageId);
                Assert.AreEqual(groupInfo.Members.Count, 6); // this fails, second instance consumer assigned to all partitions

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
}
