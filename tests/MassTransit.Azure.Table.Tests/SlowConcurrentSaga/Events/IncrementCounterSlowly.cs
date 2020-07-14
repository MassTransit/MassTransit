namespace MassTransit.Azure.Table.Tests.SlowConcurrentSaga.Events
{
    using System;


    public class IncrementCounterSlowly
    {
        public Guid CorrelationId { get; set; }
    }
}
