namespace MassTransit.Transports.Tests
{
    using System;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using Testing;


    public class Sending_a_message_directly_to_the_consumer :
        TransportTest
    {
        [Test]
        public async Task Should_consume_the_sent_message()
        {
            var orderId = NewId.NextGuid();

            await Harness.InputQueueSendEndpoint.Send(new SubmitOrder {Id = orderId});

            Assert.That(await _consumer.Consumed.Any<SubmitOrder>(x => x.Context.Message.Id == orderId), Is.True);
        }

        ConsumerTestHarness<SubmitOrderConsumer> _consumer;

        public Sending_a_message_directly_to_the_consumer(Type harnessType)
            : base(harnessType)
        {
        }

        protected override Task Arrange()
        {
            _consumer = Harness.Consumer<SubmitOrderConsumer>();

            return Task.CompletedTask;
        }


        class SubmitOrderConsumer :
            IConsumer<SubmitOrder>
        {
            public Task Consume(ConsumeContext<SubmitOrder> context)
            {
                return Task.CompletedTask;
            }
        }


        class SubmitOrder
        {
            public Guid Id { get; set; }
        }
    }
}
