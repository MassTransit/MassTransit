namespace MassTransit.KafkaIntegration.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Confluent.Kafka;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;
    using Serializers;
    using TestFramework;
    using Util;


    public class Publish_Specs :
        InMemoryTestFixture
    {
        const string Topic = "test2";

        [Test]
        public async Task Should_receive()
        {
            TaskCompletionSource<ConsumeContext<KafkaMessage>> taskCompletionSource = GetTask<ConsumeContext<KafkaMessage>>();
            TaskCompletionSource<ConsumeContext<BusPing>> pingTaskCompletionSource = GetTask<ConsumeContext<BusPing>>();

            var services = new ServiceCollection();
            services.AddSingleton(taskCompletionSource);
            services.AddSingleton(pingTaskCompletionSource);

            services.TryAddSingleton<ILoggerFactory>(LoggerFactory);
            services.TryAddSingleton(typeof(ILogger<>), typeof(Logger<>));

            services.AddMassTransit(x =>
            {
                x.AddConsumer<BusPingConsumer>();
                x.UsingInMemory((context, cfg) => cfg.ConfigureEndpoints(context));
                x.AddRider(rider =>
                {
                    rider.AddConsumer<KafkaMessageConsumer>();

                    rider.UsingKafka((context, k) =>
                    {
                        k.Host("localhost:9092");

                        k.Topic<Null, KafkaMessage>(Topic, nameof(Receive_Specs), c =>
                        {
                            c.AutoOffsetReset = AutoOffsetReset.Earliest;
                            c.ConfigureConsumer<KafkaMessageConsumer>(context);
                        });
                    });
                });
            });

            var provider = services.BuildServiceProvider();

            var busControl = provider.GetRequiredService<IBusControl>();

            await busControl.StartAsync(TestCancellationToken);

            try
            {
                var config = new ProducerConfig {BootstrapServers = "localhost:9092"};

                using IProducer<Null, KafkaMessage> p = new ProducerBuilder<Null, KafkaMessage>(config)
                    .SetValueSerializer(new MassTransitSerializer<KafkaMessage>())
                    .Build();

                var messageId = NewId.NextGuid();
                var message = new Message<Null, KafkaMessage>
                {
                    Value = new KafkaMessageClass("test"),
                    Headers = DictionaryHeadersSerialize.Serializer.Serialize(new Dictionary<string, object> {[MessageHeaders.MessageId] = messageId})
                };

                await p.ProduceAsync(Topic, message);
                p.Flush();

                ConsumeContext<KafkaMessage> result = await taskCompletionSource.Task;
                ConsumeContext<BusPing> ping = await pingTaskCompletionSource.Task;

                Assert.AreEqual(result.CorrelationId, ping.InitiatorId);

                Assert.That(ping.SourceAddress, Is.EqualTo(new Uri("loopback://localhost/kafka/test2")));
            }
            finally
            {
                await busControl.StopAsync(TestCancellationToken);

                await provider.DisposeAsync();
            }
        }


        class KafkaMessageClass :
            KafkaMessage
        {
            public KafkaMessageClass(string text)
            {
                Text = text;
            }

            public string Text { get; }
        }


        class KafkaMessageConsumer :
            IConsumer<KafkaMessage>
        {
            readonly IPublishEndpoint _publishEndpoint;
            readonly TaskCompletionSource<ConsumeContext<KafkaMessage>> _taskCompletionSource;

            public KafkaMessageConsumer(IPublishEndpoint publishEndpoint, TaskCompletionSource<ConsumeContext<KafkaMessage>> taskCompletionSource)
            {
                _publishEndpoint = publishEndpoint;
                _taskCompletionSource = taskCompletionSource;
            }

            public async Task Consume(ConsumeContext<KafkaMessage> context)
            {
                _taskCompletionSource.TrySetResult(context);
                await _publishEndpoint.Publish<BusPing>(new { });
            }
        }


        class BusPingConsumer :
            IConsumer<BusPing>
        {
            readonly TaskCompletionSource<ConsumeContext<BusPing>> _taskCompletionSource;

            public BusPingConsumer(TaskCompletionSource<ConsumeContext<BusPing>> taskCompletionSource)
            {
                _taskCompletionSource = taskCompletionSource;
            }

            public Task Consume(ConsumeContext<BusPing> context)
            {
                _taskCompletionSource.TrySetResult(context);
                return TaskUtil.Completed;
            }
        }


        public interface KafkaMessage
        {
            string Text { get; }
        }


        public interface BusPing
        {
        }
    }
}
