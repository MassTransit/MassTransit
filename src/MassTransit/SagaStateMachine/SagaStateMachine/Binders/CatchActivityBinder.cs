namespace MassTransit.SagaStateMachine
{
    using System;


    /// <summary>
    /// Creates a compensation activity with the compensation behavior
    /// </summary>
    /// <typeparam name="TInstance"></typeparam>
    /// <typeparam name="TException"></typeparam>
    public class CatchActivityBinder<TInstance, TException> :
        IActivityBinder<TInstance>
        where TInstance : class, ISaga
        where TException : Exception
    {
        readonly EventActivities<TInstance> _activities;
        public Event Event { get; }

        public CatchActivityBinder(Event @event, EventActivities<TInstance> activities)
        {
            Event = @event;
            _activities = activities;
        }

        public bool IsStateTransitionEvent(State state)
        {
            return Equals(Event, state.Enter) || Equals(Event, state.BeforeEnter)
                || Equals(Event, state.AfterLeave) || Equals(Event, state.Leave);
        }

        public void Bind(State<TInstance> state)
        {
            var builder = new CatchBehaviorBuilder<TInstance>();
            foreach (IActivityBinder<TInstance> activity in _activities.GetStateActivityBinders())
                activity.Bind(builder);

            var compensateActivity = new CatchFaultActivity<TInstance, TException>(builder.Behavior);

            state.Bind(Event, compensateActivity);
        }

        public void Bind(IBehaviorBuilder<TInstance> builder)
        {
            var compensateActivityBuilder = new CatchBehaviorBuilder<TInstance>();
            foreach (IActivityBinder<TInstance> activity in _activities.GetStateActivityBinders())
                activity.Bind(compensateActivityBuilder);

            var compensateActivity = new CatchFaultActivity<TInstance, TException>(compensateActivityBuilder.Behavior);

            foreach (IActivityBinder<TInstance> activity in _activities.GetStateActivityBinders())
                activity.Bind(builder);

            builder.Add(compensateActivity);
        }
    }
}
