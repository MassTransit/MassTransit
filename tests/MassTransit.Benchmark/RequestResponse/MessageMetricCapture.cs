namespace MassTransitBenchmark.RequestResponse
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
        readonly TaskCompletionSource<TimeSpan> _requestCompleted;
        readonly ConcurrentBag<RequestResponseMessage> _sentMessages;
        readonly Stopwatch _stopwatch;
        long _consumed;
        long _sent;

        public MessageMetricCapture(long messageCount)
        {
            _messageCount = messageCount;

            _consumedMessages = new ConcurrentBag<ConsumedMessage>();
            _sentMessages = new ConcurrentBag<RequestResponseMessage>();
            _requestCompleted = new TaskCompletionSource<TimeSpan>();
            _consumeCompleted = new TaskCompletionSource<TimeSpan>();

            _stopwatch = Stopwatch.StartNew();
        }

        public Task<TimeSpan> RequestCompleted => _requestCompleted.Task;
        public Task<TimeSpan> ConsumeCompleted => _consumeCompleted.Task;

        Task IReportConsumerMetric.Consumed<T>(Guid messageId)
        {
            _consumedMessages.Add(new ConsumedMessage(messageId, _stopwatch.ElapsedTicks));

            long consumed = Interlocked.Increment(ref _consumed);
            if (consumed == _messageCount)
                _consumeCompleted.TrySetResult(_stopwatch.Elapsed);

            return TaskUtil.Completed;
        }

        public async Task<T> ResponseReceived<T>(Guid messageId, Task<T> requestTask)
            where T : class
        {
            long sendTimestamp = _stopwatch.ElapsedTicks;

            var response = await requestTask.ConfigureAwait(false);

            long responseTimestamp = _stopwatch.ElapsedTicks;

            _sentMessages.Add(new RequestResponseMessage(messageId, sendTimestamp, responseTimestamp));

            long sent = Interlocked.Increment(ref _sent);
            if (sent == _messageCount)
                _requestCompleted.TrySetResult(_stopwatch.Elapsed);

            return response;
        }

        public MessageMetric[] GetMessageMetrics()
        {
            return _sentMessages.Join(_consumedMessages, x => x.MessageId, x => x.MessageId,
                    (sent, consumed) =>
                        new MessageMetric(sent.MessageId, sent.ResponseTimestamp - sent.SendTimestamp,
                            consumed.Timestamp - sent.SendTimestamp))
                .ToArray();
        }


        struct RequestResponseMessage
        {
            public readonly Guid MessageId;
            public readonly long SendTimestamp;
            public readonly long ResponseTimestamp;

            public RequestResponseMessage(Guid messageId, long sendTimestamp, long responseTimestamp)
            {
                MessageId = messageId;
                SendTimestamp = sendTimestamp;
                ResponseTimestamp = responseTimestamp;
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