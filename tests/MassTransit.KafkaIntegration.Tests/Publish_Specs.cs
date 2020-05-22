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
    using Util;


    public class Publish_Specs :
        KafkaTestFixture
    {
        const string Topic = "test2";
        readonly Task<ConsumeContext<IPing>> _pingTask;
        readonly IServiceProvider _provider;
        readonly Task<ConsumeContext<IMessage>> _task;

        public Publish_Specs()
        {
            var services = new ServiceCollection();
            TaskCompletionSource<ConsumeContext<IMessage>> taskCompletionSource = GetTask<ConsumeContext<IMessage>>();
            TaskCompletionSource<ConsumeContext<IPing>> pingTaskCompletionSource = GetTask<ConsumeContext<IPing>>();
            _task = taskCompletionSource.Task;
            _pingTask = pingTaskCompletionSource.Task;
            services.AddSingleton(taskCompletionSource);
            services.AddSingleton(pingTaskCompletionSource);

            services.TryAddSingleton<ILoggerFactory>(LoggerFactory);
            services.TryAddSingleton(typeof(ILogger<>), typeof(Logger<>));

            services.AddMassTransit(configurator =>
            {
                configurator.AddConsumer<PingConsumer>();
                configurator.UsingInMemory((context, cfg) => cfg.ConfigureEndpoints(context));
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
            ConsumeContext<IPing> ping = await _pingTask;

            Assert.AreEqual(result.CorrelationId, ping.InitiatorId);
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
            readonly IPublishEndpoint _publishEndpoint;
            readonly TaskCompletionSource<ConsumeContext<IMessage>> _taskCompletionSource;

            public MyConsumer(IPublishEndpoint publishEndpoint, TaskCompletionSource<ConsumeContext<IMessage>> taskCompletionSource)
            {
                _publishEndpoint = publishEndpoint;
                _taskCompletionSource = taskCompletionSource;
            }

            public async Task Consume(ConsumeContext<IMessage> context)
            {
                _taskCompletionSource.TrySetResult(context);
                await _publishEndpoint.Publish<IPing>(new { });
            }
        }


        class PingConsumer :
            IConsumer<IPing>
        {
            readonly TaskCompletionSource<ConsumeContext<IPing>> _taskCompletionSource;

            public PingConsumer(TaskCompletionSource<ConsumeContext<IPing>> taskCompletionSource)
            {
                _taskCompletionSource = taskCompletionSource;
            }

            public Task Consume(ConsumeContext<IPing> context)
            {
                _taskCompletionSource.TrySetResult(context);
                return TaskUtil.Completed;
            }
        }


        public interface IMessage
        {
            string Text { get; }
        }


        public interface IPing
        {
        }
    }
}
