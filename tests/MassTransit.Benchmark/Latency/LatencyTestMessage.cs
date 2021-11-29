namespace MassTransitBenchmark.Latency
{
    using System;


    public class LatencyTestMessage
    {
        public LatencyTestMessage()
        {
        }

        public LatencyTestMessage(Guid correlationId, string payload)
        {
            CorrelationId = correlationId;
            Payload = payload;
        }

        public Guid CorrelationId { get; set; }
        public string Payload { get; set; }
    }
}