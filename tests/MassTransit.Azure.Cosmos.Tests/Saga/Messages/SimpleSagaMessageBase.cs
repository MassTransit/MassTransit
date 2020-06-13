namespace MassTransit.Azure.Cosmos.Tests.Saga.Messages
{
    using System;


    public class SimpleSagaMessageBase :
        CorrelatedBy<Guid>
    {
        public SimpleSagaMessageBase()
        {
        }

        public SimpleSagaMessageBase(Guid correlationId)
        {
            CorrelationId = correlationId;
        }

        public Guid CorrelationId { get; set; }
    }
}
