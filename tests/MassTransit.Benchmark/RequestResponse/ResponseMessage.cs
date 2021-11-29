namespace MassTransitBenchmark.RequestResponse
{
    using System;


    public class ResponseMessage
    {
        public ResponseMessage()
        {
        }

        public ResponseMessage(Guid correlationId)
        {
            CorrelationId = correlationId;
        }

        public Guid CorrelationId { get; set; }
    }
}