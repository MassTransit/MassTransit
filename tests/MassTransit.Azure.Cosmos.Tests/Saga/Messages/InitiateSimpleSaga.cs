namespace MassTransit.Azure.Cosmos.Tests.Saga.Messages
{
    using System;


    public class InitiateSimpleSaga : CorrelatedBy<Guid>
    {
        public InitiateSimpleSaga()
        {
        }

        public InitiateSimpleSaga(Guid correlationId)
        {
            CorrelationId = correlationId;
        }

        public string Name { get; set; }

        public Guid CorrelationId { get; }
    }
}
