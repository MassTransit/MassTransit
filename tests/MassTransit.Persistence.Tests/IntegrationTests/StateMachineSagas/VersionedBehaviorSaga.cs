namespace MassTransit.Persistence.Tests.IntegrationTests.StateMachineSagas
{
    public abstract class BehaviorSaga : SagaStateMachineInstance
    {
        public string CurrentState { get; set; }
        public string Name { get; set; }
        public Guid CorrelationId { get; set; }
    }
}
