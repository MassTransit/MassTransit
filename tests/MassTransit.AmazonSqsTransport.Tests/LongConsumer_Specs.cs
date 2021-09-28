namespace MassTransit.AmazonSqsTransport.Tests
{
    using System;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using TestFramework.Messages;


    [Explicit]
    [TestFixture]
    public class When_a_consumer_takes_a_long_time :
        AmazonSqsTestFixture
    {
        public When_a_consumer_takes_a_long_time()
        {
            TestTimeout = TimeSpan.FromMinutes(5);
        }

        [Test]
        public async Task Should_allow_it_to_complete()
        {
            var message = new PingMessage();

            await InputQueueSendEndpoint.Send(message);

            await AmazonSqsTestHarness.Consumed.Any<PingMessage>(x => x.Context.Message.CorrelationId == message.CorrelationId);
        }

        protected override void ConfigureAmazonSqsReceiveEndpoint(IAmazonSqsReceiveEndpointConfigurator configurator)
        {
            configurator.Consumer<Consumer>();
        }


        class Consumer :
            IConsumer<PingMessage>
        {
            public async Task Consume(ConsumeContext<PingMessage> context)
            {
                await Task.Delay(TimeSpan.FromSeconds(160));
            }
        }
    }
}
