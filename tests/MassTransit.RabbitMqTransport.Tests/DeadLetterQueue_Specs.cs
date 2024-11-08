namespace MassTransit.RabbitMqTransport.Tests
{
    using System.Threading.Tasks;
    using NUnit.Framework;
    using RabbitMQ.Client;


    [TestFixture]
    public class When_a_dead_letter_queue_is_specified :
        RabbitMqTestFixture
    {
        [Test]
        public void Should_create_and_bind_the_exchange_and_properties()
        {
        }

        const string QueueName = "input-with-timeout";
        const string DeadLetterQueueName = "input-with-timeout-dlx";

        protected override void ConfigureRabbitMqBus(IRabbitMqBusFactoryConfigurator configurator)
        {
            configurator.ReceiveEndpoint(QueueName, x =>
            {
                x.BindDeadLetterQueue(DeadLetterQueueName);
            });
        }

        protected override async Task OnCleanupVirtualHost(IChannel channel)
        {
            await channel.ExchangeDeleteAsync(QueueName);
            await channel.QueueDeleteAsync(QueueName);

            await channel.ExchangeDeleteAsync(DeadLetterQueueName);
            await channel.QueueDeleteAsync(DeadLetterQueueName);
        }
    }
}
