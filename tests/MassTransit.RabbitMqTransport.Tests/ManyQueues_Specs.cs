namespace MassTransit.RabbitMqTransport.Tests
{
    using System.Threading.Tasks;
    using NUnit.Framework;
    using RabbitMQ.Client;
    using Util;


    [TestFixture]
    [Category("Flaky")]
    public class Creating_a_service_with_many_queues :
        RabbitMqTestFixture
    {
        const int Limit = 50;

        [Test]
        public void Should_not_exploded()
        {
        }

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

        protected override void OnCleanupVirtualHost(IModel model)
        {
            for (var i = 0; i < Limit; i++)
            {
                model.ExchangeDelete($"receiver_queue{i}");
                model.QueueDelete($"receiver_queue{i}");
            }
        }


        class TestConsumer :
            IConsumer<TestMessage>
        {
            public Task Consume(ConsumeContext<TestMessage> context)
            {
                return TaskUtil.Completed;
            }
        }


        public interface TestMessage
        {
        }
    }
}
