namespace MassTransit.RabbitMqTransport.Tests
{
    using System.Threading.Tasks;
    using NUnit.Framework;
    using RabbitMQ.Client;


    [TestFixture]
    [Category("Flaky")]
    public class Creating_a_service_with_many_queues :
        RabbitMqTestFixture
    {
        [Test]
        public void Should_not_exploded()
        {
        }

        const int Limit = 50;

        protected override void ConfigureRabbitMqBus(IRabbitMqBusFactoryConfigurator configurator)
        {
            for (var i = 0; i < Limit; i++)
            {
                configurator.ReceiveEndpoint($"receiver_queue{i}", e =>
                {
                    e.Consumer<TestConsumer>();
                });
            }
        }

        protected override async Task OnCleanupVirtualHost(IChannel channel)
        {
            await base.OnCleanupVirtualHost(channel);

            for (var i = 0; i < Limit; i++)
            {
                await channel.ExchangeDeleteAsync($"receiver_queue{i}");
                await channel.QueueDeleteAsync($"receiver_queue{i}");
            }
        }


        class TestConsumer :
            IConsumer<TestMessage>
        {
            public Task Consume(ConsumeContext<TestMessage> context)
            {
                return Task.CompletedTask;
            }
        }


        public interface TestMessage
        {
        }
    }
}
