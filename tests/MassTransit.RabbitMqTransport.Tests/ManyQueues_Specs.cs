namespace MassTransit.RabbitMqTransport.Tests
{
    using System.Threading.Tasks;
    using NUnit.Framework;
    using Util;


    [TestFixture]
    public class Creating_a_service_with_many_queues :
        RabbitMqTestFixture
    {
        [Test]
        public void Should_not_exploded()
        {
        }

        protected override void ConfigureRabbitMqBus(IRabbitMqBusFactoryConfigurator configurator)
        {
            for (var i = 0; i < 50; i++)
            {
                configurator.ReceiveEndpoint($"receiver_queue{i}", e =>
                {
                    e.Consumer<TestConsumer>();
                });
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
