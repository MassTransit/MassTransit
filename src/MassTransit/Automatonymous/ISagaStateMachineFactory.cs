namespace Automatonymous
{
    /// <summary>
    /// Used to create the state machine using a container, should be a single-instance registration
    /// </summary>
    public interface ISagaStateMachineFactory
    {
        SagaStateMachine<T> CreateStateMachine<T>()
            where T : class, SagaStateMachineInstance;
    }
}
