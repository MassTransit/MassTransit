namespace MassTransit.HangfireIntegration.Tests
{
    using System;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using Scheduling;


    [TestFixture]
    public class Specifying_an_event_in_the_past :
        HangfireInMemoryTestFixture
    {
        [Test]
        [Explicit]
        public async Task Should_be_able_to_cancel_a_future_event()
        {
            Task<ConsumeContext<A>> handler = SubscribeHandler<A>();

            ScheduledMessage<A> scheduledMessage =
                await Scheduler.ScheduleSend(Bus.Address, DateTime.UtcNow + TimeSpan.FromSeconds(120), new A { Name = "Joe" });

            await Task.Delay(2000);

            await Scheduler.CancelScheduledSend(scheduledMessage);

            await Task.Delay(2000);
        }

        [Test]
        public async Task Should_handle_now_properly()
        {
            Task<ConsumeContext<A>> handler = SubscribeHandler<A>();

            await Scheduler.ScheduleSend(Bus.Address, DateTime.UtcNow, new A { Name = "Joe" });

            await handler;
        }

        [Test]
        public async Task Should_handle_slightly_in_the_future_properly()
        {
            Task<ConsumeContext<A>> handler = SubscribeHandler<A>();

            await Scheduler.ScheduleSend(Bus.Address, DateTime.UtcNow + TimeSpan.FromSeconds(2), new A { Name = "Joe" });

            await handler;
        }

        [Test]
        public async Task Should_include_message_headers()
        {
            Task<ConsumeContext<A>> handler = SubscribeHandler<A>();

            var requestId = Guid.NewGuid();
            var correlationId = Guid.NewGuid();
            var conversationId = Guid.NewGuid();
            var initiatorId = Guid.NewGuid();
            await Scheduler.ScheduleSend(Bus.Address, DateTime.UtcNow, new A { Name = "Joe" }, Pipe.Execute<SendContext>(x =>
            {
                x.FaultAddress = Bus.Address;
                x.ResponseAddress = InputQueueAddress;
                x.RequestId = requestId;
                x.CorrelationId = correlationId;
                x.ConversationId = conversationId;
                x.InitiatorId = initiatorId;

                x.Headers.Set("Hello", "World");
            }));

            ConsumeContext<A> context = await handler;

            Assert.Multiple(() =>
            {
                Assert.That(context.FaultAddress, Is.EqualTo(Bus.Address));
                Assert.That(context.ResponseAddress, Is.EqualTo(InputQueueAddress));
                Assert.That(context.RequestId.HasValue, Is.True);
                Assert.That(context.RequestId.Value, Is.EqualTo(requestId));
                Assert.That(context.CorrelationId.HasValue, Is.True);
                Assert.That(context.CorrelationId.Value, Is.EqualTo(correlationId));
                Assert.That(context.ConversationId.HasValue, Is.True);
                Assert.That(context.ConversationId.Value, Is.EqualTo(conversationId));
                Assert.That(context.InitiatorId.HasValue, Is.True);
                Assert.That(context.InitiatorId.Value, Is.EqualTo(initiatorId));

                Assert.That(context.Headers.TryGetHeader("Hello", out var value), Is.True);
                Assert.That(value, Is.EqualTo("World"));
            });
        }

        [Test]
        public async Task Should_properly_send_the_message()
        {
            Task<ConsumeContext<A>> handler = SubscribeHandler<A>();

            await Scheduler.ScheduleSend(Bus.Address, DateTime.UtcNow + TimeSpan.FromHours(-1), new A { Name = "Joe" });

            await handler;
        }


        class A
        {
            public string Name { get; set; }
        }
    }


    [TestFixture]
    public class Specifying_an_event_reschedule_if_exists :
        HangfireInMemoryTestFixture
    {
        [Test]
        public async Task Should_reschedule()
        {
            Task<ConsumeContext<A>> handler = SubscribeHandler<A>();
            var id = NewId.NextGuid();
            var expected = "Joe 2";
            await Scheduler.ScheduleSend(Bus.Address, TimeSpan.FromSeconds(120), new A
            {
                Id = id,
                Name = "Joe"
            });

            await Task.Delay(2000);

            await Scheduler.ScheduleSend(Bus.Address, TimeSpan.FromSeconds(5), new A
            {
                Id = id,
                Name = expected
            });

            ConsumeContext<A> result = await handler;
            Assert.That(result.Message.Name, Is.EqualTo(expected));
        }

        public Specifying_an_event_reschedule_if_exists()
        {
            ScheduleTokenId.UseTokenId<A>(x => x.Id);
        }


        class A
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
        }
    }
}
