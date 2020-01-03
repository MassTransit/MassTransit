namespace MassTransit.EntityFrameworkCoreIntegration.Tests.DeadlockSaga.Events
{
    using System;


    public class Begin
    {
        public Guid CorrelationId { get; set; }
    }
}
