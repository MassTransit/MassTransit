namespace MassTransit.AmazonSqsTransport.Tests
{
    using System;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using TestFramework.Messages;


    [TestFixture]
    public class When_sending_messages_using_fifo_topics_and_queues :
        AmazonSqsTestFixture
    {
        [Test]
        public async Task Should_allow_it_to_complete()
        {
            var message = new OrderedMessage()
            {
                CorrelationId = NewId.NextGuid(),
                Value = "Hello"
            };

            await Bus.Publish(message);

            await AmazonSqsTestHarness.Consumed.Any<PingMessage>(x => x.Context.Message.CorrelationId == message.CorrelationId);

            await _handled;
        }

        Task<ConsumeContext<OrderedMessage>> _handled;

        protected override void ConfigureAmazonSqsBus(IAmazonSqsBusFactoryConfigurator configurator)
        {
            configurator.Message<OrderedMessage>(x => x.SetEntityName("ordered-message.fifo"));
            configurator.Publish<OrderedMessage>(x => x.TopicAttributes.Add("ContentBasedDeduplication", "true"));

            configurator.ReceiveEndpoint("ordered-queue.fifo", x =>
            {
                x.QueueAttributes.Add("ContentBasedDeduplication", "true");

                _handled = Handled<OrderedMessage>(x);
            });
        }


        public class OrderedMessage
        {
            public Guid CorrelationId { get; set; }
            public string Value { get; set; }
        }
    }
}
