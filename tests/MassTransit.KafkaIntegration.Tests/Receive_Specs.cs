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
    using Registration.Attachments;
    using Serializers;


    public class Receive_Specs :
        KafkaTestFixture
    {
        const string Topic = "test";
        readonly IServiceProvider _provider;
        readonly Task<ConsumeContext<IMessage>> _task;

        public Receive_Specs()
        {
            var services = new ServiceCollection();
            TaskCompletionSource<ConsumeContext<IMessage>> taskCompletionSource = GetTask<ConsumeContext<IMessage>>();
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
            configurator.AddConsumer<MyConsumer>();
            base.ConfigureBusAttachment(configurator);
        }

        protected override void ConfigureKafka<T>(IBusAttachmentRegistrationContext<T> context, IKafkaFactoryConfigurator configurator)
        {
            base.ConfigureKafka(context, configurator);
            configurator.Topic<Null, IMessage>(Topic, nameof(Receive_Specs), c =>
            {
                c.DisableAutoCommit();
                c.ConfigureConsumer<MyConsumer>(context);
            });
        }

        [Test]
        public async Task Should_receive()
        {
            var config = new ProducerConfig {BootstrapServers = "localhost:9092"};

            using IProducer<Null, IMessage> p = new ProducerBuilder<Null, IMessage>(config)
                .SetValueSerializer(new MassTransitSerializer<IMessage>())
                .Build();
            var messageId = NewId.NextGuid();
            var message = new Message<Null, IMessage>
            {
                Value = new Message("test"),
                Headers = DictionaryHeadersSerialize.Serializer.Serialize(new Dictionary<string, object> {[MessageHeaders.MessageId] = messageId})
            };
            await p.ProduceAsync(Topic, message);

            ConsumeContext<IMessage> result = await _task;
            Assert.AreEqual(message.Value.Text, result.Message.Text);
            Assert.AreEqual(messageId, result.MessageId);
        }


        class Message :
            IMessage
        {
            public Message(string text)
            {
                Text = text;
            }

            public string Text { get; }
        }


        class MyConsumer :
            IConsumer<IMessage>
        {
            readonly TaskCompletionSource<ConsumeContext<IMessage>> _taskCompletionSource;

            public MyConsumer(TaskCompletionSource<ConsumeContext<IMessage>> taskCompletionSource)
            {
                _taskCompletionSource = taskCompletionSource;
            }

            public async Task Consume(ConsumeContext<IMessage> context)
            {
                _taskCompletionSource.TrySetResult(context);
            }
        }


        public interface IMessage
        {
            string Text { get; }
        }
    }
}
