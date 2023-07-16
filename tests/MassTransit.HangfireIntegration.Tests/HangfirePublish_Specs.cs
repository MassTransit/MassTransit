namespace MassTransit.HangfireIntegration.Tests
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Logging;
    using NUnit.Framework;


    public class HangfirePublish_Specs :
        HangfireInMemoryTestFixture
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

        [Test]
        public async Task Should_preserve_parent_trace_id()
        {
            using var source = new ActivitySource(nameof(Should_preserve_parent_trace_id));
            using var listener = new ActivityListener
            {
                ShouldListenTo = x => x.Name == DiagnosticHeaders.DefaultListenerName || x.Name == nameof(Should_preserve_parent_trace_id),
                Sample = (ref ActivityCreationOptions<ActivityContext> options) => ActivitySamplingResult.AllDataAndRecorded
            };

            ActivitySource.AddActivityListener(listener);

            using var parentActivity = source.StartActivity();

            Task<ConsumeContext<SomeMessage>> handled = SubscribeHandler<SomeMessage>();

            Bus.ConnectConsumer(() => new SomeMessageConsumer());

            await Scheduler.ScheduleSend(Bus.Address, DateTime.Now, new SomeMessage
            {
                SendDate = DateTime.Now,
                Source = "Schedule"
            });

            var context = await handled;

            var parentContext = ActivityContext.Parse(context.GetHeader(DiagnosticHeaders.ActivityId), null);

            Assert.That(parentContext.TraceId, Is.EqualTo(parentActivity.TraceId));
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
