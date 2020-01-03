namespace MassTransit.EntityFrameworkCoreIntegration.Tests.DeadlockSaga.DataAccess
{
    using System;
    using Automatonymous;


    public class DeadlockSaga : SagaStateMachineInstance
    {
        public Guid CorrelationId { get; set; }

        public string CurrentState { get; set; }

        public string Name { get; set; }

        public int Counter { get; set; }
    }
}
