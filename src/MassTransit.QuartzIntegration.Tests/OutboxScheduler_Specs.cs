namespace MassTransit.QuartzIntegration.Tests
{
    using System;
    using System.Threading.Tasks;
    using Context;
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
            var faulted = ConnectPublishHandler<Fault<PingMessage>>();

            await InputQueueSendEndpoint.Send(new PingMessage());

            await _pingReceived.Task;

            LogContext.Debug?.Log("Ping was received");

            await _scheduledMessage.Task;

            LogContext.Debug?.Log("Pong was scheduled");

            await faulted;
            await Task.Delay(1000);

            AdvanceTime(TimeSpan.FromSeconds(20));

            Assert.That(async () => await _pongReceived.Task.OrTimeout(s: 5), Throws.TypeOf<TimeoutException>());
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

                var scheduledMessage = await context.ScheduleSend(TimeSpan.FromSeconds(20), new PongMessage());

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
            var faulted = ConnectPublishHandler<Fault<PingMessage>>();

            await InputQueueSendEndpoint.Send(new PingMessage());

            await _pingReceived.Task;

            LogContext.Debug?.Log("Ping was received");

            await _scheduledMessage.Task;

            LogContext.Debug?.Log("Pong was scheduled");

            await faulted;

            AdvanceTime(TimeSpan.FromSeconds(20));

            LogContext.Debug?.Log("Advanced the clock");

            await _pongReceived.Task;

            LogContext.Debug?.Log("Pong was received");
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

                var scheduledMessage = await context.ScheduleSend(TimeSpan.FromSeconds(20), new PongMessage());

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
