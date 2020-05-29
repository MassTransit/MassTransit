namespace MassTransit.Tests.Messages
{
    using System;


    public class ResponseMessage :
        CorrelatedBy<Guid>
    {
        //xml serializer
        public ResponseMessage()
        {
        }

        public ResponseMessage(Guid correlationId)
        {
            CorrelationId = correlationId;
        }

        public Guid CorrelationId { get; }
    }
}
