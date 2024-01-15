namespace MassTransit.Tests.ContainerTests.Scenarios
{
    using System;


    public class ThirdSagaMessage
    {
        public Guid CorrelationId { get; set; }
    }
}
