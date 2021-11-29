namespace MassTransit.KafkaIntegration.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Confluent.Kafka;
    using Confluent.Kafka.SyncOverAsync;
    using Confluent.SchemaRegistry;
    using Confluent.SchemaRegistry.Serdes;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;
    using TestFramework;
    using UnitTests;


    public class Avro_Specs :
        InMemoryTestFixture
    {
        const string Topic = "avrogie";

        [Test]
        public async Task Should_produce()
        {
            TaskCompletionSource<ConsumeContext<KafkaMessage>> taskCompletionSource = GetTask<ConsumeContext<KafkaMessage>>();
            var services = new ServiceCollection();
            services.AddSingleton(taskCompletionSource);

            services.AddSingleton<ISchemaRegistryClient>(new CachedSchemaRegistryClient(new Dictionary<string, string>
            {
                { "schema.registry.url", "localhost:8081" },
            }));
            services.TryAddSingleton<ILoggerFactory>(LoggerFactory);
            services.TryAddSingleton(typeof(ILogger<>), typeof(Logger<>));

            static ISerializer<T> GetSerializer<T>(IServiceProvider provider)
            {
                return new AvroSerializer<T>(provider.GetService<ISchemaRegistryClient>()).AsSyncOverAsync();
            }

            static IDeserializer<T> GetDeserializer<T>(IServiceProvider provider)
            {
                return new AvroDeserializer<T>(provider.GetService<ISchemaRegistryClient>()).AsSyncOverAsync();
            }

            services.AddMassTransit(x =>
            {
                x.UsingInMemory((context, cfg) => cfg.ConfigureEndpoints(context));
                x.AddRider(rider =>
                {
                    rider.AddConsumer<KafkaMessageConsumer>();

                    rider.AddProducer<string, KafkaMessage>(Topic, context => context.MessageId.ToString(), (context, cfg) =>
                    {
                        cfg.SetKeySerializer(GetSerializer<string>(context));
                        cfg.SetValueSerializer(GetSerializer<KafkaMessage>(context));
                    });

                    rider.UsingKafka((context, k) =>
                    {
                        k.Host("localhost:9092");

                        k.TopicEndpoint<string, KafkaMessage>(Topic, nameof(Producer_Specs), c =>
                        {
                            c.SetKeyDeserializer(GetDeserializer<string>(context));
                            c.SetValueDeserializer(GetDeserializer<KafkaMessage>(context));

                            c.AutoOffsetReset = AutoOffsetReset.Earliest;
                            c.ConfigureConsumer<KafkaMessageConsumer>(context);

                            c.CreateIfMissing(m =>
                            {
                                m.NumPartitions = 2;
                            });
                        });
                    });
                });
            });

            var provider = services.BuildServiceProvider(true);

            var busControl = provider.GetRequiredService<IBusControl>();

            await busControl.StartAsync(TestCancellationToken);

            var serviceScope = provider.CreateScope();

            var producer = serviceScope.ServiceProvider.GetRequiredService<ITopicProducer<KafkaMessage>>();

            try
            {
                var correlationId = NewId.NextGuid();
                var conversationId = NewId.NextGuid();
                var initiatorId = NewId.NextGuid();
                var messageId = NewId.NextGuid();
                await producer.Produce(new { Test = "text" }, Pipe.Execute<SendContext>(context =>
                    {
                        context.CorrelationId = correlationId;
                        context.MessageId = messageId;
                        context.InitiatorId = initiatorId;
                        context.ConversationId = conversationId;
                    }),
                    TestCancellationToken);

                ConsumeContext<KafkaMessage> result = await taskCompletionSource.Task;

                Assert.AreEqual("text", result.Message.Test);
                Assert.That(result.SourceAddress, Is.EqualTo(new Uri("loopback://localhost/")));
                Assert.That(result.DestinationAddress, Is.EqualTo(new Uri($"loopback://localhost/{KafkaTopicAddress.PathPrefix}/{Topic}")));
                Assert.That(result.MessageId, Is.EqualTo(messageId));
                Assert.That(result.CorrelationId, Is.EqualTo(correlationId));
                Assert.That(result.InitiatorId, Is.EqualTo(initiatorId));
                Assert.That(result.ConversationId, Is.EqualTo(conversationId));
            }
            finally
            {
                serviceScope.Dispose();

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
                _taskCompletionSource.TrySetResult(context);
            }
        }
    }
}
