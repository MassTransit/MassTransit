namespace MassTransit.Containers.Tests
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;
    using TestFramework;
    using TestFramework.Messages;
    using Util;


    [TestFixture]
    public class When_a_batch_limit_is_reached :
        InMemoryTestFixture
    {
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

        [Test]
        public async Task Should_receive_the_message_batch()
        {
            var finished = ConnectPublishHandler<PongMessage>();

            await InputQueueSendEndpoint.Send(new PingMessage());
            await InputQueueSendEndpoint.Send(new PingMessage());

            await finished;
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.Batch<PingMessage>(x =>
            {
                x.MessageLimit = 2;

                x.Consumer<TestBatchConsumer, PingMessage>(_provider);
            });
        }
    }


    class TestBatchConsumer :
        IConsumer<Batch<PingMessage>>
    {
        public Task Consume(ConsumeContext<Batch<PingMessage>> context)
        {
            context.Respond(new PongMessage());

            return TaskUtil.Completed;
        }
    }
}
