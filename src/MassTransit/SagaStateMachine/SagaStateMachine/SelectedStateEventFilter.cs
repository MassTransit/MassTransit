namespace MassTransit.SagaStateMachine
{
    public class SelectedStateEventFilter<TSaga, TMessage> :
        IStateEventFilter<TSaga>
        where TSaga : class, ISaga
        where TMessage : class
    {
        readonly StateMachineCondition<TSaga, TMessage> _filter;

        public SelectedStateEventFilter(StateMachineCondition<TSaga, TMessage> filter)
        {
            _filter = filter;
        }

        public bool Filter<T>(BehaviorContext<TSaga, T> context)
            where T : class
        {
            if (context is BehaviorContext<TSaga, TMessage> filterContext)
                return _filter(filterContext);

            return false;
        }

        public bool Filter(BehaviorContext<TSaga> context)
        {
            return false;
        }
    }
}
