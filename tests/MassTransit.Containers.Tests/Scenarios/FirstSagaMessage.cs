namespace MassTransit.Containers.Tests.Scenarios
{
    using System;


    public class FirstSagaMessage :
        CorrelatedBy<Guid>
    {
        public Guid CorrelationId { get; set; }
    }
}
