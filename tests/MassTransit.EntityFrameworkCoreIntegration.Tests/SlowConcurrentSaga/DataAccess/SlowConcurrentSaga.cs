namespace MassTransit.EntityFrameworkCoreIntegration.Tests.SlowConcurrentSaga.DataAccess
{
    using System;
    using Automatonymous;


    public class SlowConcurrentSaga : SagaStateMachineInstance
    {
        public string CurrentState { get; set; }

        public string Name { get; set; }

        public int Counter { get; set; }
        public Guid CorrelationId { get; set; }
    }
}
