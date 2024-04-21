namespace MassTransit.Azure.ServiceBus.Core.Tests
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading;
    using System.Threading.Tasks;
    using NUnit.Framework;


    [TestFixture]
    public class When_message_process_exceeds_maxAutoRenewDuration
        : AzureServiceBusTestFixture
    {
        [Test]
        [Explicit]
        public async Task Should_have_seen_all_messages()
        {
            async Task Send(LongMessageToProcess message)
            {
                await InputQueueSendEndpoint.Send(message, context =>
                {
                    context.SetSessionId("irrelevant");
                });
            }

            await Send(new LongMessageToProcess(5.1)); // this will lead to a SessionLockLost
            await Send(new LongMessageToProcess(.01)); // these won't be processed
            await Send(new LongMessageToProcess(.01));
            await _seenAll;

            await _handler;
            Assert.That(_seenMessages, Is.EqualTo(TotalMessages));
        }

        public When_message_process_exceeds_maxAutoRenewDuration()
            : base("input_queue_session_long")
        {
            TestTimeout = TimeSpan.FromMinutes(10);
        }

        Task<ConsumeContext<LongMessageToProcess>> _handler;
        readonly ConcurrentDictionary<Guid, LongMessageToProcess> _seen = new ConcurrentDictionary<Guid, LongMessageToProcess>();

        int _seenMessages;
        Task<int> _seenAll;


        public class LongMessageToProcess
        {
            public LongMessageToProcess()
            {
            }

            public LongMessageToProcess(double mnWait)
            {
                WaitMinutes = mnWait;
            }

            public double WaitMinutes { get; set; }
        }


        const int TotalMessages = 3;

        protected override void ConfigureServiceBusReceiveEndpoint(IServiceBusReceiveEndpointConfigurator configurator)
        {
            configurator.PrefetchCount = 1;
            configurator.RequiresSession = true;
            configurator.LockDuration = TimeSpan.FromMinutes(2);
            configurator.MaxAutoRenewDuration = TimeSpan.FromMinutes(2); // doesn't seem to go below 5 minutes

            TaskCompletionSource<int> handlerSessionId = GetTask<int>();
            _seenAll = handlerSessionId.Task;

            _handler = Handler<LongMessageToProcess>(configurator, async context =>
            {
                var count = Interlocked.Increment(ref _seenMessages);

                if (_seen.TryAdd(context.MessageId ?? Guid.Empty, context.Message))
                    // we only wait the first time we see the message to simulate a transient "timeout"
                    await Task.Delay(TimeSpan.FromMinutes(context.Message.WaitMinutes));

                if (count == TotalMessages)
                    handlerSessionId.TrySetResult(count);
            });
        }
    }
}
