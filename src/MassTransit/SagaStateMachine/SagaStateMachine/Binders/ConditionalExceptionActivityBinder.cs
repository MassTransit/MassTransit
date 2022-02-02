namespace MassTransit.SagaStateMachine
{
    using System;
    using System.Threading.Tasks;


    public class ConditionalExceptionActivityBinder<TInstance, TException> :
        IActivityBinder<TInstance>
        where TInstance : class, ISaga
        where TException : Exception
    {
        readonly StateMachineAsyncExceptionCondition<TInstance, TException> _condition;
        readonly EventActivities<TInstance> _elseActivities;
        readonly EventActivities<TInstance> _thenActivities;
        public Event Event { get; }

        public ConditionalExceptionActivityBinder(Event @event, StateMachineExceptionCondition<TInstance, TException> condition,
            EventActivities<TInstance> thenActivities, EventActivities<TInstance> elseActivities)
            : this(@event, context => Task.FromResult(condition(context)), thenActivities, elseActivities)
        {
        }

        public ConditionalExceptionActivityBinder(Event @event, StateMachineAsyncExceptionCondition<TInstance, TException> condition,
            EventActivities<TInstance> thenActivities, EventActivities<TInstance> elseActivities)
        {
            _thenActivities = thenActivities;
            _elseActivities = elseActivities;
            _condition = condition;
            Event = @event;
        }

        public bool IsStateTransitionEvent(State state)
        {
            return Equals(Event, state.Enter) || Equals(Event, state.BeforeEnter)
                   || Equals(Event, state.AfterLeave) || Equals(Event, state.Leave);
        }

        public void Bind(State<TInstance> state)
        {
            var thenBehavior = GetBehavior(_thenActivities);
            var elseBehavior = GetBehavior(_elseActivities);

            var conditionActivity = new ConditionExceptionActivity<TInstance, TException>(_condition, thenBehavior, elseBehavior);

            state.Bind(Event, conditionActivity);
        }

        public void Bind(IBehaviorBuilder<TInstance> builder)
        {
            var thenBehavior = GetBehavior(_thenActivities);
            var elseBehavior = GetBehavior(_elseActivities);

            var conditionActivity = new ConditionExceptionActivity<TInstance, TException>(_condition, thenBehavior, elseBehavior);

            builder.Add(conditionActivity);
        }

        static IBehavior<TInstance> GetBehavior(EventActivities<TInstance> activities)
        {
            var builder = new CatchBehaviorBuilder<TInstance>();

            foreach (var activity in activities.GetStateActivityBinders())
                activity.Bind(builder);

            return builder.Behavior;
        }
    }


    public class ConditionalExceptionActivityBinder<TInstance, TData, TException> :
        IActivityBinder<TInstance>
        where TInstance : class, ISaga
        where TException : Exception
        where TData : class
    {
        readonly StateMachineAsyncExceptionCondition<TInstance, TData, TException> _condition;
        readonly EventActivities<TInstance> _elseActivities;
        readonly EventActivities<TInstance> _thenActivities;
        public Event Event { get; }

        public ConditionalExceptionActivityBinder(Event @event, StateMachineExceptionCondition<TInstance, TData, TException> condition,
            EventActivities<TInstance> thenActivities, EventActivities<TInstance> elseActivities)
            : this(@event, context => Task.FromResult(condition(context)), thenActivities, elseActivities)
        {
        }

        public ConditionalExceptionActivityBinder(Event @event, StateMachineAsyncExceptionCondition<TInstance, TData, TException> condition,
            EventActivities<TInstance> thenActivities, EventActivities<TInstance> elseActivities)
        {
            _thenActivities = thenActivities;
            _elseActivities = elseActivities;
            _condition = condition;
            Event = @event;
        }

        public bool IsStateTransitionEvent(State state)
        {
            return Equals(Event, state.Enter) || Equals(Event, state.BeforeEnter)
                   || Equals(Event, state.AfterLeave) || Equals(Event, state.Leave);
        }

        public void Bind(State<TInstance> state)
        {
            var thenBehavior = GetBehavior(_thenActivities);
            var elseBehavior = GetBehavior(_elseActivities);

            var conditionActivity = new ConditionExceptionActivity<TInstance, TData, TException>(_condition, thenBehavior, elseBehavior);

            state.Bind(Event, conditionActivity);
        }

        public void Bind(IBehaviorBuilder<TInstance> builder)
        {
            var thenBehavior = GetBehavior(_thenActivities);
            var elseBehavior = GetBehavior(_elseActivities);

            var conditionActivity = new ConditionExceptionActivity<TInstance, TData, TException>(_condition, thenBehavior, elseBehavior);

            builder.Add(conditionActivity);
        }

        static IBehavior<TInstance> GetBehavior(EventActivities<TInstance> activities)
        {
            var builder = new CatchBehaviorBuilder<TInstance>();

            foreach (var activity in activities.GetStateActivityBinders())
                activity.Bind(builder);

            return builder.Behavior;
        }
    }
}
