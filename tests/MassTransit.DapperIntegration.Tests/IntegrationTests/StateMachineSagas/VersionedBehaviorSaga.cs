namespace MassTransit.DapperIntegration.Tests.IntegrationTests.StateMachines
{
    using System;


    public class VersionedBehaviorSaga : SagaStateMachineInstance,
        ISagaVersion
    {
        public Guid CorrelationId { get; set; }
        public int Version { get; set; }
        public string CurrentState { get; set; }
        public string Name { get; set; }
    }
}
