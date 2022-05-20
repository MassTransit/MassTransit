namespace MassTransit.KafkaIntegration.Tests
{
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
}
