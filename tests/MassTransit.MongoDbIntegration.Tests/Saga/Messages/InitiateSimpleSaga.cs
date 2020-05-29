namespace MassTransit.MongoDbIntegration.Tests.Saga
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
