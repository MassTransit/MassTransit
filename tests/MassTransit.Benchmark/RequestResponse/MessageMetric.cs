namespace MassTransitBenchmark.RequestResponse
{
    using System;


    public class MessageMetric
    {
        public MessageMetric(Guid messageId, long requestLatency, long consumeLatency)
        {
            MessageId = messageId;
            RequestLatency = requestLatency;
            ConsumeLatency = consumeLatency;
        }

        public Guid MessageId { get; }
        public long RequestLatency { get; set; }
        public long ConsumeLatency { get; set; }
    }
}