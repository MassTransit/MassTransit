namespace MassTransit.KafkaIntegration.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Confluent.Kafka;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;
    using Registration;
    using Serializers;
    using TestFramework;


    public class MyCustomTest :
        InMemoryTestFixture
    {
        const string Topic = "test";
        readonly IBusInstance _busInstance;
        readonly IServiceProvider _provider;
        readonly Task<ConsumeContext<IMessage>> _task;

        public MyCustomTest()
        {
            var services = new ServiceCollection();
            TaskCompletionSource<ConsumeContext<IMessage>> taskCompletionSource = GetTask<ConsumeContext<IMessage>>();
            _task = taskCompletionSource.Task;
            services.AddSingleton(taskCompletionSource);
            services.AddMassTransit(configurator =>
            {
                configurator.AddConsumer<MyConsumer>();
                configurator.UsingInMemory((context, cfg) =>
                {
                });
                configurator.AttachKafka((registration, cfg) =>
                {
                    cfg.Host("localhost:9092");
                    cfg.Subscribe<Null, IMessage>(new DefaultTopicNameFormatter(Topic), c =>
                    {
                        c.DisableAutoCommit();
                        c.GroupId = nameof(MyCustomTest);
                    });
                });
            });
            _provider = services.BuildServiceProvider();
            IEnumerable<IBusInstanceConfigurator> configurators = _provider.GetServices<IBusInstanceConfigurator>();
            _busInstance = _provider.GetService<IBusInstance>();
            foreach (var busInstanceConfigurator in configurators)
                busInstanceConfigurator.Configure(_busInstance);
        }

        [OneTimeSetUp]
        public async Task Start()
        {
            await _busInstance.BusControl.StartAsync();

            foreach (var attachment in _busInstance.Attachments)
                await attachment.Connect(CancellationToken.None);
        }

        [OneTimeTearDown]
        public async Task Stop()
        {
            foreach (var attachment in _busInstance.Attachments)
                await attachment.Disconnect(CancellationToken.None);

            await _busInstance.BusControl.StopAsync();
        }

        [Test]
        public async Task Should_receive()
        {
            var config = new ProducerConfig {BootstrapServers = "localhost:9092"};

            // If serializers are not specified, default serializers from
            // `Confluent.Kafka.Serializers` will be automatically used where
            // available. Note: by default strings are encoded as UTF8.
            using IProducer<Null, IMessage> p = new ProducerBuilder<Null, IMessage>(config)
                .SetValueSerializer(new MassTransitSerializer<IMessage>())
                .Build();
            try
            {
                var message = new Message<Null, IMessage>
                {
                    Value = new Message("test"),
                    Headers = new Headers {{MessageHeaders.MessageId, Encoding.UTF8.GetBytes(Guid.NewGuid().ToString())}}
                };
                DeliveryResult<Null, IMessage> dr = await p.ProduceAsync(Topic, message);
                Console.WriteLine($"Delivered '{dr.Value}' to '{dr.TopicPartitionOffset}'");
            }
            catch (ProduceException<Null, string> e)
            {
                Console.WriteLine($"Delivery failed: {e.Error.Reason}");
            }

            ConsumeContext<IMessage> result = await _task;
            Assert.IsNotNull(result.MessageId);
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
    }


    public interface IMessage
    {
        string Text { get; }
    }
}
