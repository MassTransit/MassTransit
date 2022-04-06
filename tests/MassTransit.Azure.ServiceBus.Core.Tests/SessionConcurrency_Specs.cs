namespace MassTransit.Azure.ServiceBus.Core.Tests
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using Shouldly;


    [TestFixture]
    public class Sending_multiple_messages_to_a_session
        : AzureServiceBusTestFixture
    {
        [Test]
        public async Task Should_have_seen_all_messages()
        {
            await _handler;
            _seenMessages.ShouldBe(TotalMessages);
        }

        [Test]
        public async Task Should_have_seen_all_sessions()
        {
            await _handler;
            _result.Count.ShouldBe(SessionCount);
        }

        [Test]
        public async Task Should_have_seen_message_in_send_order()
        {
            await _handler;
            var outOfOrderSessions = _result.Values.Count(mr => mr.ReceivedSeqNumber != mr.SentSeqNumber);
            outOfOrderSessions.ShouldBe(0);
        }

        public Sending_multiple_messages_to_a_session()
            : base("input_queue_session_multi")
        {
            TestTimeout = TimeSpan.FromMinutes(5);
        }

        Task<ConsumeContext<SequencedPing>> _handler;
        readonly ConcurrentDictionary<string, MessageReceived> _result = new ConcurrentDictionary<string, MessageReceived>();
        int _seenMessages;
        Task<int> _seenAll;


        public struct MessageReceived
        {
            public MessageReceived(int sentSeqNumber, int receivedSeqNumber)
            {
                (SentSeqNumber, ReceivedSeqNumber) = (sentSeqNumber, receivedSeqNumber);
            }

            public int SentSeqNumber { get; }
            public int ReceivedSeqNumber { get; }
        }


        public class SequencedPing
        {
            public SequencedPing()
            {
            }

            public SequencedPing(int sentSeqNumber)
            {
                SentSeqNumber = sentSeqNumber;
            }

            public int SentSeqNumber { get; set; }
        }


        const int SessionCount = 20;
        const int MessageCount = 100;
        const int TotalMessages = SessionCount * MessageCount;

        protected override void ConfigureServiceBusReceiveEndpoint(IServiceBusReceiveEndpointConfigurator configurator)
        {
            configurator.RequiresSession = true;

            TaskCompletionSource<int> handlerSessionId = GetTask<int>();
            _seenAll = handlerSessionId.Task;

            _handler = Handler<SequencedPing>(configurator, async context =>
            {
                var count = Interlocked.Increment(ref _seenMessages);
                MessageReceived sessionCount;
                if (_result.TryGetValue(context.SessionId(), out var mr))
                    sessionCount = new MessageReceived(context.Message.SentSeqNumber, mr.ReceivedSeqNumber + 1);
                else
                    sessionCount = new MessageReceived(context.Message.SentSeqNumber, 1);

                _result[context.SessionId()] = sessionCount;

                await Task.Delay(1);

                if (count == TotalMessages)
                    handlerSessionId.TrySetResult(count);
            });
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            async Task Send(string session)
            {
                for (var i = 1; i <= MessageCount; i++)
                {
                    var message = new SequencedPing(i);
                    await InputQueueSendEndpoint.Send(message, context =>
                    {
                        context.SetSessionId(session);
                    });
                }
            }

            List<Task> sendTasks = (from id in Enumerable.Range(0, SessionCount)
                let session = $"Session{id}"
                select Send(session)).ToList();

            await Task.WhenAll(sendTasks);
            await _seenAll;
        }
    }
}
