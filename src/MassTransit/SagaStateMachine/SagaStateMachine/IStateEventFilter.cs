namespace MassTransit.SagaStateMachine
{
    public interface IStateEventFilter<TSaga>
        where TSaga : class, SagaStateMachineInstance
    {
        bool Filter(BehaviorContext<TSaga> context);

        bool Filter<T>(BehaviorContext<TSaga, T> context)
            where T : class;
    }
}
