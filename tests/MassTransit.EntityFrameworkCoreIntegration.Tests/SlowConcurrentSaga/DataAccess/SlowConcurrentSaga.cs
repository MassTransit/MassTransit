namespace MassTransit.EntityFrameworkCoreIntegration.Tests.SlowConcurrentSaga.DataAccess
{
    using System;
    using Automatonymous;


    public class SlowConcurrentSaga : SagaStateMachineInstance
    {
        public Guid CorrelationId { get; set; }

        public string CurrentState { get; set; }

        public string Name { get; set; }

        public int Counter { get; set; }
    }
}
