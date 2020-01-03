namespace MassTransit.EntityFrameworkCoreIntegration.Tests.DeadlockSaga.Events
{
    using System;


    public class IncrementCounterSlowly
    {
        public Guid CorrelationId { get; set; }
    }
}
