namespace MassTransit.QuartzIntegration.Tests
{
    using System;
    using System.Threading.Tasks;
    using GreenPipes.Internals.Extensions;
    using NUnit.Framework;
    using TestFramework;
    using TestFramework.Messages;


    public class Cancel_scheduled_message_through_the_outbox :
        QuartzInMemoryTestFixture
    {
        [Test]
        public async Task Should_cancel_the_message()
        {
            await InputQueueSendEndpoint.Send(new PingMessage());

            await _pingReceived.Task;

            Console.WriteLine("Ping was received");

            var scheduledMessage = await _scheduledMessage.Task;

            Console.WriteLine("Pong was scheduled");

            Assert.That(async () => await _pongReceived.Task.OrTimeout(s:5), Throws.TypeOf<TimeoutException>());
        }

        TaskCompletionSource<ConsumeContext<PingMessage>> _pingReceived;
        TaskCompletionSource<ConsumeContext<PongMessage>> _pongReceived;
        TaskCompletionSource<Scheduling.ScheduledMessage> _scheduledMessage;


        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.UseInMemoryOutbox();

            _pingReceived = GetTask<ConsumeContext<PingMessage>>();
            _scheduledMessage = GetTask<Scheduling.ScheduledMessage>();

            configurator.Handler<PingMessage>(async context =>
            {
                _pingReceived.TrySetResult(context);

                var scheduledMessage = await context.ScheduleSend(TimeSpan.FromSeconds(3), new PongMessage());

                _scheduledMessage.TrySetResult(scheduledMessage);

                throw new IntentionalTestException("This time bad things happen");
            });

            _pongReceived = GetTask<ConsumeContext<PongMessage>>();

            configurator.Handler<PongMessage>(async context =>
            {
                _pongReceived.TrySetResult(context);
            });
        }
    }

    public class Not_cancel_scheduled_message_with_no_outbox :
        QuartzInMemoryTestFixture
    {
        [Test]
        public async Task Should_not_cancel_the_message()
        {
            await InputQueueSendEndpoint.Send(new PingMessage());

            await _pingReceived.Task;

            Console.WriteLine("Ping was received");

            await _scheduledMessage.Task;

            Console.WriteLine("Pong was scheduled");

            await _pongReceived.Task;

            Console.WriteLine("Pong was received");
        }

        TaskCompletionSource<ConsumeContext<PingMessage>> _pingReceived;
        TaskCompletionSource<ConsumeContext<PongMessage>> _pongReceived;
        TaskCompletionSource<Scheduling.ScheduledMessage> _scheduledMessage;


        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _pingReceived = GetTask<ConsumeContext<PingMessage>>();
            _scheduledMessage = GetTask<Scheduling.ScheduledMessage>();

            configurator.Handler<PingMessage>(async context =>
            {
                _pingReceived.TrySetResult(context);

                var scheduledMessage = await context.ScheduleSend(TimeSpan.FromSeconds(3), new PongMessage());

                _scheduledMessage.TrySetResult(scheduledMessage);

                throw new IntentionalTestException("This time bad things happen");
            });

            _pongReceived = GetTask<ConsumeContext<PongMessage>>();

            configurator.Handler<PongMessage>(async context =>
            {
                _pongReceived.TrySetResult(context);
            });
        }
    }
}
