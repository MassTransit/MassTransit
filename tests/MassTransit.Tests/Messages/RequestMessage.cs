namespace MassTransit.Tests.Messages
{
    using System;


    [Serializable]
    public class RequestMessage :
        CorrelatedBy<Guid>
    {
        public RequestMessage()
        {
            CorrelationId = Guid.NewGuid();
        }

        public Guid CorrelationId { get; set; }
    }
}
