namespace MassTransit.Containers.Tests.Scenarios
{
    using System;


    public class ThirdSagaMessage
    {
        public Guid CorrelationId { get; set; }
    }
}
