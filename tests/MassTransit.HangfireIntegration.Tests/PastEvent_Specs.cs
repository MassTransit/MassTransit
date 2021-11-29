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

            Assert.AreEqual(Bus.Address, context.FaultAddress);
            Assert.AreEqual(InputQueueAddress, context.ResponseAddress);
            Assert.IsTrue(context.RequestId.HasValue);
            Assert.AreEqual(requestId, context.RequestId.Value);
            Assert.IsTrue(context.CorrelationId.HasValue);
            Assert.AreEqual(correlationId, context.CorrelationId.Value);
            Assert.IsTrue(context.ConversationId.HasValue);
            Assert.AreEqual(conversationId, context.ConversationId.Value);
            Assert.IsTrue(context.InitiatorId.HasValue);
            Assert.AreEqual(initiatorId, context.InitiatorId.Value);

            Assert.IsTrue(context.Headers.TryGetHeader("Hello", out var value));
            Assert.AreEqual("World", value);
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
            Assert.AreEqual(expected, result.Message.Name);
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
