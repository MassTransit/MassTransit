namespace MassTransit.SagaStateMachine
{
    public class IgnoreEventActivityBinder<TInstance> :
        IActivityBinder<TInstance>
        where TInstance : class, ISaga
    {
        public Event Event { get; }

        public IgnoreEventActivityBinder(Event @event)
        {
            Event = @event;
        }

        public bool IsStateTransitionEvent(State state)
        {
            return Equals(Event, state.Enter) || Equals(Event, state.BeforeEnter)
                   || Equals(Event, state.AfterLeave) || Equals(Event, state.Leave);
        }

        public void Bind(State<TInstance> state)
        {
            state.Ignore(Event);
        }

        public void Bind(IBehaviorBuilder<TInstance> builder)
        {
        }
    }


    public class IgnoreEventActivityBinder<TInstance, TData> :
        IActivityBinder<TInstance>
        where TInstance : class, ISaga
        where TData : class
    {
        readonly Event<TData> _event;
        readonly StateMachineCondition<TInstance, TData> _filter;
        public Event Event => _event;

        public IgnoreEventActivityBinder(Event<TData> @event, StateMachineCondition<TInstance, TData> filter)
        {
            _event = @event;
            _filter = filter;
        }

        public bool IsStateTransitionEvent(State state)
        {
            return Equals(_event, state.Enter) || Equals(_event, state.BeforeEnter)
                   || Equals(_event, state.AfterLeave) || Equals(_event, state.Leave);
        }

        public void Bind(State<TInstance> state)
        {
            state.Ignore(_event, _filter);
        }

        public void Bind(IBehaviorBuilder<TInstance> builder)
        {
        }
    }
}
