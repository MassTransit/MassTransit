namespace MassTransit.RabbitMqTransport.Tests
{
    using System.Threading.Tasks;
    using Definition;
    using GreenPipes.Util;
    using NUnit.Framework;
    using TestFramework.Messages;


    [TestFixture]
    public class ConsumerDefinition_Specs :
        RabbitMqTestFixture
    {
        protected override void ConfigureRabbitMqBus(IRabbitMqBusFactoryConfigurator configurator)
        {
            configurator.ReceiveEndpoint("input_queue_error", x =>
            {
                x.PurgeOnStartup = true;
            });
        }


        class Consumer :
            IConsumer<PingMessage>
        {
            public Task Consume(ConsumeContext<PingMessage> context)
            {
                return TaskUtil.Completed;
            }
        }


        class ConsumerConsumerDefinition :
            ConsumerDefinition<Consumer>
        {
            public ConsumerConsumerDefinition()
            {
                ConcurrentMessageLimit = 100;
            }
        }
    }
}
