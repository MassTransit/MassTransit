namespace MassTransit.SagaStateMachine
{
    /// <summary>
    /// Routes event activities to an activities
    /// </summary>
    /// <typeparam name="TInstance"></typeparam>
    public class ExecuteActivityBinder<TInstance> :
        IActivityBinder<TInstance>
        where TInstance : class, ISaga
    {
        readonly IStateMachineActivity<TInstance> _activity;
        public Event Event { get; }

        public ExecuteActivityBinder(Event @event, IStateMachineActivity<TInstance> activity)
        {
            Event = @event;
            _activity = activity;
        }

        public bool IsStateTransitionEvent(State state)
        {
            return Equals(Event, state.Enter) || Equals(Event, state.BeforeEnter)
                || Equals(Event, state.AfterLeave) || Equals(Event, state.Leave);
        }

        public void Bind(State<TInstance> state)
        {
            state.Bind(Event, _activity);
        }

        public void Bind(IBehaviorBuilder<TInstance> builder)
        {
            builder.Add(_activity);
        }
    }
}
