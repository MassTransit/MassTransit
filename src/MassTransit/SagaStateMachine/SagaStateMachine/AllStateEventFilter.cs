namespace MassTransit.SagaStateMachine
{
    public class AllStateEventFilter<TSaga> :
        IStateEventFilter<TSaga>
        where TSaga : class, SagaStateMachineInstance
    {
        public bool Filter(BehaviorContext<TSaga> context)
        {
            return true;
        }

        public bool Filter<T>(BehaviorContext<TSaga, T> context)
            where T : class
        {
            return true;
        }
    }
}
