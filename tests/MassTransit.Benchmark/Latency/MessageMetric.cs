namespace MassTransitBenchmark.Latency
{
    using System;


    public class MessageMetric
    {
        public MessageMetric(Guid messageId, long ackLatency, long consumeLatency)
        {
            MessageId = messageId;
            AckLatency = ackLatency;
            ConsumeLatency = consumeLatency;
        }

        public Guid MessageId { get; }
        public long AckLatency { get; set; }
        public long ConsumeLatency { get; set; }
    }
}