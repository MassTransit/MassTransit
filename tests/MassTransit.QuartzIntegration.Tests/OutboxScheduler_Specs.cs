namespace MassTransit.QuartzIntegration.Tests
{
    using System;
    using System.Threading.Tasks;
    using Internals;
    using NUnit.Framework;
    using Scheduling;
    using TestFramework;
    using TestFramework.Messages;


    [Category("Flaky")]
    public class Cancel_scheduled_message_through_the_outbox :
        QuartzInMemoryTestFixture
    {
        TaskCompletionSource<ConsumeContext<PingMessage>> _pingReceived;
        TaskCompletionSource<ConsumeContext<PongMessage>> _pongReceived;
        TaskCompletionSource<ScheduledMessage> _scheduledMessage;

        [Test]
        public async Task Should_cancel_the_message()
        {
            Task<ConsumeContext<Fault<PingMessage>>> faulted = await ConnectPublishHandler<Fault<PingMessage>>();

            await InputQueueSendEndpoint.Send(new PingMessage());

            await _pingReceived.Task;

            LogContext.Debug?.Log("Ping was received");

            await _scheduledMessage.Task;

            LogContext.Debug?.Log("Pong was scheduled");

            await faulted;

            await InMemoryTestHarness.Consumed.Any<CancelScheduledMessage>();

            await AdvanceTime(TimeSpan.FromSeconds(20));

            Assert.That(async () => await _pongReceived.Task.OrTimeout(s: 3), Throws.TypeOf<TimeoutException>());
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.UseInMemoryOutbox();

            _pingReceived = GetTask<ConsumeContext<PingMessage>>();
            _scheduledMessage = GetTask<ScheduledMessage>();

            configurator.Handler<PingMessage>(async context =>
            {
                _pingReceived.TrySetResult(context);

                ScheduledMessage<PongMessage> scheduledMessage = await context.ScheduleSend(TimeSpan.FromSeconds(20), new PongMessage());

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


    [Category("Flaky")]
    public class Not_cancel_scheduled_message_with_no_outbox :
        QuartzInMemoryTestFixture
    {
        TaskCompletionSource<ConsumeContext<PingMessage>> _pingReceived;
        TaskCompletionSource<ConsumeContext<PongMessage>> _pongReceived;
        TaskCompletionSource<ScheduledMessage> _scheduledMessage;

        [Test]
        public async Task Should_not_cancel_the_message()
        {
            Task<ConsumeContext<Fault<PingMessage>>> faulted = await ConnectPublishHandler<Fault<PingMessage>>();

            await InputQueueSendEndpoint.Send(new PingMessage());

            await _pingReceived.Task;

            LogContext.Debug?.Log("Ping was received");

            await _scheduledMessage.Task;

            LogContext.Debug?.Log("Pong was scheduled");

            await faulted;

            await AdvanceTime(TimeSpan.FromSeconds(20));

            LogContext.Debug?.Log("Advanced the clock");

            await _pongReceived.Task;

            LogContext.Debug?.Log("Pong was received");
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _pingReceived = GetTask<ConsumeContext<PingMessage>>();
            _scheduledMessage = GetTask<ScheduledMessage>();

            configurator.Handler<PingMessage>(async context =>
            {
                _pingReceived.TrySetResult(context);

                ScheduledMessage<PongMessage> scheduledMessage = await context.ScheduleSend(TimeSpan.FromSeconds(20), new PongMessage());

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
