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


    [Explicit]
    [TestFixture]
    public class When_a_slow_consumer_takes_one_message_out_of_many :
        AmazonSqsTestFixture
    {
        public When_a_slow_consumer_takes_one_message_out_of_many()
        {
            TestTimeout = TimeSpan.FromMinutes(5);
        }

        [Test]
        public async Task Should_leave_the_rest_of_the_messages_in_the_queue()
        {
            var message = new PingMessage();

            for (int i = 0; i < 7; i++)
            {
                await InputQueueSendEndpoint.Send(message);
            }

            var lastMessage = new PingMessage();

            await InputQueueSendEndpoint.Send(lastMessage);

            await Task.Delay(TimeSpan.FromSeconds(30));
            //            await AmazonSqsTestHarness.Consumed.SelectAsync<PingMessage>(x => x.Context.Message.CorrelationId == lastMessage.CorrelationId).Count();

            await AmazonSqsTestHarness.Stop();
        }

        protected override void ConfigureAmazonSqsReceiveEndpoint(IAmazonSqsReceiveEndpointConfigurator configurator)
        {
            configurator.PrefetchCount = 1;
            configurator.ConcurrentMessageLimit = 2;
            configurator.WaitTimeSeconds = 1;

            configurator.Consumer<Consumer>();
        }


        class Consumer :
            IConsumer<PingMessage>
        {
            public async Task Consume(ConsumeContext<PingMessage> context)
            {
                await Task.Delay(TimeSpan.FromSeconds(10));
            }
        }
    }
}
