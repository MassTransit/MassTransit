namespace MassTransit.Containers.Tests
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;
    using TestFramework;
    using Util;


    [TestFixture]
    public class When_a_batch_limit_is_reached :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_receive_the_message_batch()
        {
            Task<ConsumeContext<BatchResult>> finished = await ConnectPublishHandler<BatchResult>();

            await InputQueueSendEndpoint.SendBatch(new[] {new BatchItem(), new BatchItem()});

            ConsumeContext<BatchResult> finishedContext = await finished;

            Assert.That(finishedContext.Message.Count, Is.EqualTo(2));
        }

        readonly IServiceProvider _provider;

        public When_a_batch_limit_is_reached()
        {
            var collection = new ServiceCollection();
            collection.AddMassTransit(x =>
            {
                x.AddConsumer<TestBatchConsumer>();
                x.AddBus(provider => BusControl);
            });

            _provider = collection.BuildServiceProvider(true);
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.ConfigureConsumers(_provider.GetRequiredService<IBusRegistrationContext>());
        }
    }


    [TestFixture]
    public class When_a_batch_limit_is_configured :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_receive_the_message_batch()
        {
            Task<ConsumeContext<BatchResult>> finished = await ConnectPublishHandler<BatchResult>();

            await InputQueueSendEndpoint.SendBatch(new[]
            {
                new BatchItem(), new BatchItem(), new BatchItem(), new BatchItem(), new BatchItem(), new BatchItem()
            });

            ConsumeContext<BatchResult> finishedContext = await finished;

            Assert.That(finishedContext.Message.Count, Is.EqualTo(5));
        }

        readonly IServiceProvider _provider;

        public When_a_batch_limit_is_configured()
        {
            var collection = new ServiceCollection();
            collection.AddMassTransit(x =>
            {
                x.AddConsumer<TestBatchConsumer>(c =>
                    c.Options<BatchOptions>(o => o.SetMessageLimit(5).SetTimeLimit(2000)));

                x.AddBus(provider => BusControl);
            });

            _provider = collection.BuildServiceProvider(true);
        }

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.ConcurrentMessageLimit = 16;
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.ConfigureConsumers(_provider.GetRequiredService<IBusRegistrationContext>());
        }
    }


    [TestFixture]
    public class When_a_batch_limit_is_configured_using_a_definition :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_receive_the_message_batch()
        {
            Task<ConsumeContext<BatchResult>> finished = await ConnectPublishHandler<BatchResult>();

            await Bus.PublishBatch(new[] {new BatchItem(), new BatchItem(), new BatchItem(), new BatchItem(), new BatchItem(), new BatchItem()});

            ConsumeContext<BatchResult> finishedContext = await finished;

            Assert.That(finishedContext.Message.Count, Is.EqualTo(5));
            Assert.That(finishedContext.Message.Mode, Is.EqualTo(BatchCompletionMode.Size));
        }

        readonly IServiceProvider _provider;

        public When_a_batch_limit_is_configured_using_a_definition()
        {
            var collection = new ServiceCollection();
            collection.AddMassTransit(x =>
            {
                x.AddConsumer<TestBatchConsumer>(typeof(TestBatchConsumerDefinition))
                    .Endpoint(endpoint => endpoint.PrefetchCount = 16);

                x.AddBus(provider => BusControl);
            });

            _provider = collection.BuildServiceProvider(true);
        }

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.ConfigureEndpoints(_provider.GetRequiredService<IBusRegistrationContext>());
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
        }
    }


    public class BatchItem
    {
    }


    public class BatchResult
    {
        public int Count { get; set; }
        public BatchCompletionMode Mode { get; set; }
    }


    public class TestBatchConsumerDefinition :
        ConsumerDefinition<TestBatchConsumer>
    {
        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator,
            IConsumerConfigurator<TestBatchConsumer> consumerConfigurator)
        {
            endpointConfigurator.UseInMemoryOutbox();
            consumerConfigurator.Options<BatchOptions>(o => o.SetMessageLimit(5).SetTimeLimit(2000));
        }
    }


    public class TestBatchConsumer :
        IConsumer<Batch<BatchItem>>
    {
        public Task Consume(ConsumeContext<Batch<BatchItem>> context)
        {
            context.Respond(new BatchResult
            {
                Count = context.Message.Length,
                Mode = context.Message.Mode
            });

            return Task.CompletedTask;
        }
    }
}
