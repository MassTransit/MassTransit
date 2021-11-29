namespace MassTransit.Azure.Table.Tests.SlowConcurrentSaga.DataAccess
{
    using System;


    public class SlowConcurrentSaga : SagaStateMachineInstance
    {
        public string CurrentState { get; set; }

        public string Name { get; set; }

        public int Counter { get; set; }
        public Guid CorrelationId { get; set; }
    }
}
