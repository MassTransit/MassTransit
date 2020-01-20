namespace MassTransit.EntityFrameworkCoreIntegration.Tests.SlowConcurrentSaga.Events
{
    using System;


    public class IncrementCounterSlowly
    {
        public Guid CorrelationId { get; set; }
    }
}
