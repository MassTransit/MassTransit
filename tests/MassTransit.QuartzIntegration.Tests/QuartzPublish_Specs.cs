namespace MassTransit.QuartzIntegration.Tests
{
    using System;
    using System.Threading.Tasks;
    using NUnit.Framework;


    public class QuartzPublish_Specs :
        QuartzInMemoryTestFixture
    {
        [Test]
        public async Task Should_receive_the_message()
        {
            Task<ConsumeContext<SomeMessage>> handled = SubscribeHandler<SomeMessage>();

            Bus.ConnectConsumer(() => new SomeMessageConsumer());

            await Scheduler.ScheduleSend(Bus.Address, DateTime.Now, new SomeMessage
            {
                SendDate = DateTime.Now,
                Source = "Schedule"
            });

            await handled;
        }


        class SomeMessageConsumer :
            IConsumer<SomeMessage>
        {
            public Task Consume(ConsumeContext<SomeMessage> context)
            {
                return Console.Out.WriteLineAsync(context.Message.GetType().Name + " - " + context.Message.SendDate + " - " + context.Message.Source);
            }
        }


        class SomeMessage
        {
            public DateTime SendDate { get; set; }
            public string Source { get; set; }
        }
    }
}
