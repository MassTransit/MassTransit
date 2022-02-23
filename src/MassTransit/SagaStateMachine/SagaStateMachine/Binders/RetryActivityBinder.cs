namespace MassTransit.SagaStateMachine
{
    public class RetryActivityBinder<TInstance> :
        IActivityBinder<TInstance>
        where TInstance : class, ISaga
    {
        readonly IStateMachineActivity<TInstance> _activity;

        public RetryActivityBinder(Event @event, IRetryPolicy retryPolicy, EventActivities<TInstance> retryActivities)
        {
            Event = @event;

            var builder = new ActivityBehaviorBuilder<TInstance>();

            foreach (IActivityBinder<TInstance> activity in retryActivities.GetStateActivityBinders())
                activity.Bind(builder);

            IBehavior<TInstance> behavior = builder.Behavior;

            _activity = new RetryActivity<TInstance>(retryPolicy, behavior);
        }

        public Event Event { get; }

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


    public class RetryActivityBinder<TInstance, TMessage> :
        IActivityBinder<TInstance>
        where TInstance : class, ISaga
        where TMessage : class
    {
        readonly IStateMachineActivity<TInstance> _activity;

        public RetryActivityBinder(Event @event, IRetryPolicy retryPolicy, EventActivities<TInstance> retryActivities)
        {
            Event = @event;

            var builder = new ActivityBehaviorBuilder<TInstance>();

            foreach (IActivityBinder<TInstance> activity in retryActivities.GetStateActivityBinders())
                activity.Bind(builder);

            IBehavior<TInstance> behavior = builder.Behavior;

            _activity = new RetryActivity<TInstance, TMessage>(retryPolicy, behavior);
        }

        public Event Event { get; }

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
