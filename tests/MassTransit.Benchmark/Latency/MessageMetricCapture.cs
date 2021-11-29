namespace MassTransitBenchmark.Latency
{
    using System;
    using System.Collections.Concurrent;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using MassTransit.Util;


    public class MessageMetricCapture :
        IReportConsumerMetric
    {
        readonly TaskCompletionSource<TimeSpan> _consumeCompleted;
        readonly ConcurrentBag<ConsumedMessage> _consumedMessages;
        readonly long _messageCount;
        readonly TaskCompletionSource<TimeSpan> _sendCompleted;
        readonly ConcurrentBag<SentMessage> _sentMessages;
        readonly Stopwatch _stopwatch;
        long _consumed;
        long _sent;

        public MessageMetricCapture(long messageCount)
        {
            _messageCount = messageCount;

            _consumedMessages = new ConcurrentBag<ConsumedMessage>();
            _sentMessages = new ConcurrentBag<SentMessage>();
            _sendCompleted = new TaskCompletionSource<TimeSpan>();
            _consumeCompleted = new TaskCompletionSource<TimeSpan>();

            _stopwatch = Stopwatch.StartNew();
        }

        public Task<TimeSpan> SendCompleted => _sendCompleted.Task;
        public Task<TimeSpan> ConsumeCompleted => _consumeCompleted.Task;

        Task IReportConsumerMetric.Consumed<T>(Guid messageId)
        {
            _consumedMessages.Add(new ConsumedMessage(messageId, _stopwatch.ElapsedTicks));

            long consumed = Interlocked.Increment(ref _consumed);
            if (consumed == _messageCount)
                _consumeCompleted.TrySetResult(_stopwatch.Elapsed);

            return TaskUtil.Completed;
        }

        public async Task Sent(Guid messageId, Task sendTask)
        {
            long sendTimestamp = _stopwatch.ElapsedTicks;

            await sendTask.ConfigureAwait(false);

            long ackTimestamp = _stopwatch.ElapsedTicks;

            _sentMessages.Add(new SentMessage(messageId, sendTimestamp, ackTimestamp));

            long sent = Interlocked.Increment(ref _sent);
            if (sent == _messageCount)
                _sendCompleted.TrySetResult(_stopwatch.Elapsed);
        }

        public MessageMetric[] GetMessageMetrics()
        {
            return _sentMessages.Join(_consumedMessages, x => x.MessageId, x => x.MessageId,
                    (sent, consumed) =>
                        new MessageMetric(sent.MessageId, sent.AckTimestamp - sent.SendTimestamp,
                            consumed.Timestamp - sent.SendTimestamp))
                .ToArray();
        }


        struct SentMessage
        {
            public readonly Guid MessageId;
            public readonly long SendTimestamp;
            public readonly long AckTimestamp;

            public SentMessage(Guid messageId, long sendTimestamp, long ackTimestamp)
            {
                MessageId = messageId;
                SendTimestamp = sendTimestamp;
                AckTimestamp = ackTimestamp;
            }
        }


        struct ConsumedMessage
        {
            public readonly Guid MessageId;
            public readonly long Timestamp;

            public ConsumedMessage(Guid messageId, long timestamp)
            {
                MessageId = messageId;
                Timestamp = timestamp;
            }
        }
    }
}