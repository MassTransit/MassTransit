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
    using Registration;
    using Serializers;


    public class Receive_Specs :
        KafkaTestFixture
    {
        const string Topic = "test";
        readonly IServiceProvider _provider;
        readonly Task<ConsumeContext<KafkaMessage>> _task;

        public Receive_Specs()
        {
            var services = new ServiceCollection();
            TaskCompletionSource<ConsumeContext<KafkaMessage>> taskCompletionSource = GetTask<ConsumeContext<KafkaMessage>>();
            _task = taskCompletionSource.Task;
            services.AddSingleton(taskCompletionSource);
            services.TryAddSingleton<ILoggerFactory>(LoggerFactory);
            services.TryAddSingleton(typeof(ILogger<>), typeof(Logger<>));
            services.AddMassTransit(configurator =>
            {
                configurator.UsingInMemory((context, cfg) =>
                {
                });
                configurator.AddBusAttachment(ConfigureBusAttachment);
            });
            _provider = services.BuildServiceProvider();
        }

        protected override IBusInstance BusInstance => _provider.GetRequiredService<IBusInstance>();

        protected override void ConfigureBusAttachment<T>(IBusAttachmentRegistrationConfigurator<T> configurator)
        {
            configurator.AddConsumer<KafkaMessageConsumer>();
            base.ConfigureBusAttachment(configurator);
        }

        protected override void ConfigureKafka<T>(IBusAttachmentRegistrationContext<T> context, IKafkaFactoryConfigurator configurator)
        {
            base.ConfigureKafka(context, configurator);
            configurator.Topic<Null, KafkaMessage>(Topic, nameof(Receive_Specs), c =>
            {
                c.AutoOffsetReset = AutoOffsetReset.Earliest;

                c.DisableAutoCommit();
                c.ConfigureConsumer<KafkaMessageConsumer>(context);
            });
        }

        [Test]
        public async Task Should_receive()
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

            ConsumeContext<KafkaMessage> result = await _task;
            Assert.AreEqual(message.Value.Text, result.Message.Text);
            Assert.AreEqual(messageId, result.MessageId);
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


        public interface KafkaMessage
        {
            string Text { get; }
        }
    }
}
