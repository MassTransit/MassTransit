namespace MassTransitBenchmark.RequestResponse
{
    using System;


    public class RequestMessage
    {
        public RequestMessage()
        {
        }

        public RequestMessage(Guid correlationId)
        {
            CorrelationId = correlationId;
        }

        public Guid CorrelationId { get; set; }
    }
}