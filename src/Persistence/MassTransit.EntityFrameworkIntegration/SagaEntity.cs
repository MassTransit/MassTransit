namespace MassTransit.EntityFrameworkIntegration
{
    using System;

    public abstract class SagaEntity : CorrelatedBy<Guid>
    {
        // EF Mapping requires property to have a setter
        public Guid CorrelationId { get; protected set; }
    }
}