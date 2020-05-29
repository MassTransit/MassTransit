namespace MassTransit.Tests.Saga.Messages
{
    using System;


    [Serializable]
    public class CorrelatedMessage :
        CorrelatedBy<Guid>
    {
        public CorrelatedMessage(Guid correlationId)
        {
            CorrelationId = correlationId;
        }

        protected CorrelatedMessage()
        {
        }

        public Guid CorrelationId { get; set; }
    }
}
