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
        readonly Event _event;

        public ExecuteActivityBinder(Event @event, IStateMachineActivity<TInstance> activity)
        {
            _event = @event;
            _activity = activity;
        }

        public bool IsStateTransitionEvent(State state)
        {
            return Equals(_event, state.Enter) || Equals(_event, state.BeforeEnter)
                || Equals(_event, state.AfterLeave) || Equals(_event, state.Leave);
        }

        public void Bind(State<TInstance> state)
        {
            state.Bind(_event, _activity);
        }

        public void Bind(IBehaviorBuilder<TInstance> builder)
        {
            builder.Add(_activity);
        }
    }
}
