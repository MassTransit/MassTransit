namespace MassTransit.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;


    class StateMachineEventActivitiesBuilder<TInstance> :
        IStateMachineEventActivitiesBuilder<TInstance>
        where TInstance : class, SagaStateMachineInstance
    {
        readonly List<EventActivities<TInstance>> _activities;
        readonly Action<EventActivities<TInstance>[]> _committer;
        readonly MassTransitStateMachine<TInstance> _machine;
        readonly IStateMachineModifier<TInstance> _modifier;

        public StateMachineEventActivitiesBuilder(MassTransitStateMachine<TInstance> machine,
            IStateMachineModifier<TInstance> modifier, Action<EventActivities<TInstance>[]> committer)
        {
            _machine = machine ?? throw new ArgumentNullException(nameof(machine));
            _modifier = modifier ?? throw new ArgumentNullException(nameof(modifier));
            _committer = committer ?? throw new ArgumentNullException(nameof(committer));
            _activities = new List<EventActivities<TInstance>>();
            IsCommitted = false;
        }

        public bool IsCommitted { get; private set; }

        public State Initial => _modifier.Initial;
        public State Final => _modifier.Final;

        public IStateMachineModifier<TInstance> CommitActivities()
        {
            _committer(_activities.ToArray());
            IsCommitted = true;
            return _modifier;
        }

        public IStateMachineEventActivitiesBuilder<TInstance> When(Event @event,
            Func<EventActivityBinder<TInstance>, EventActivityBinder<TInstance>> configure)
        {
            _activities.Add(configure(_machine.When(@event)));
            return this;
        }

        public IStateMachineEventActivitiesBuilder<TInstance> When(Event @event, StateMachineCondition<TInstance> filter,
            Func<EventActivityBinder<TInstance>, EventActivityBinder<TInstance>> configure)
        {
            _activities.Add(configure(_machine.When(@event, filter)));
            return this;
        }

        public IStateMachineEventActivitiesBuilder<TInstance> When<TData>(Event<TData> @event,
            Func<EventActivityBinder<TInstance, TData>, EventActivityBinder<TInstance, TData>> configure)
            where TData : class
        {
            _activities.Add(configure(_machine.When(@event)));
            return this;
        }

        public IStateMachineEventActivitiesBuilder<TInstance> When<TData>(Event<TData> @event,
            StateMachineCondition<TInstance, TData> filter,
            Func<EventActivityBinder<TInstance, TData>, EventActivityBinder<TInstance, TData>> configure)
            where TData : class
        {
            _activities.Add(configure(_machine.When(@event, filter)));
            return this;
        }

        public IStateMachineEventActivitiesBuilder<TInstance> Ignore(Event @event)
        {
            _activities.Add(_machine.Ignore(@event));
            return this;
        }

        public IStateMachineEventActivitiesBuilder<TInstance> Ignore<TData>(Event<TData> @event)
            where TData : class
        {
            _activities.Add(_machine.Ignore(@event));
            return this;
        }

        public IStateMachineEventActivitiesBuilder<TInstance> Ignore<TData>(Event<TData> @event,
            StateMachineCondition<TInstance, TData> filter)
            where TData : class
        {
            _activities.Add(_machine.Ignore(@event, filter));
            return this;
        }

        public void Apply()
        {
            CommitActivities().Apply();
        }

    #region Pass-through Modifier

        public IStateMachineModifier<TInstance> AfterLeave(State state,
            Func<EventActivityBinder<TInstance, State>, EventActivityBinder<TInstance, State>> activityCallback)
        {
            return CommitActivities().AfterLeave(state, activityCallback);
        }

        public IStateMachineModifier<TInstance> AfterLeaveAny(
            Func<EventActivityBinder<TInstance, State>, EventActivityBinder<TInstance, State>> activityCallback)
        {
            return CommitActivities().AfterLeaveAny(activityCallback);
        }

        public IStateMachineModifier<TInstance> BeforeEnter(State state,
            Func<EventActivityBinder<TInstance, State>, EventActivityBinder<TInstance, State>> activityCallback)
        {
            return CommitActivities().BeforeEnter(state, activityCallback);
        }

        public IStateMachineModifier<TInstance> BeforeEnterAny(
            Func<EventActivityBinder<TInstance, State>, EventActivityBinder<TInstance, State>> activityCallback)
        {
            return CommitActivities().BeforeEnterAny(activityCallback);
        }

        public IStateMachineModifier<TInstance> CompositeEvent(string name, out Event @event,
            Expression<Func<TInstance, CompositeEventStatus>> trackingPropertyExpression, params Event[] events)
        {
            return CommitActivities().CompositeEvent(name, out @event, trackingPropertyExpression, events);
        }

        public IStateMachineModifier<TInstance> CompositeEvent(string name, out Event @event,
            Expression<Func<TInstance, CompositeEventStatus>> trackingPropertyExpression, CompositeEventOptions options,
            params Event[] events)
        {
            return CommitActivities().CompositeEvent(name, out @event, trackingPropertyExpression, options, events);
        }

        public IStateMachineModifier<TInstance> CompositeEvent(string name, out Event @event, Expression<Func<TInstance, int>> trackingPropertyExpression,
            params Event[] events)
        {
            return CommitActivities().CompositeEvent(name, out @event, trackingPropertyExpression, events);
        }

        public IStateMachineModifier<TInstance> CompositeEvent(string name, out Event @event, Expression<Func<TInstance, int>> trackingPropertyExpression,
            CompositeEventOptions options, params Event[] events)
        {
            return CommitActivities().CompositeEvent(name, out @event, trackingPropertyExpression, options, events);
        }

        public IStateMachineEventActivitiesBuilder<TInstance> During(params State[] states)
        {
            return CommitActivities().During(states);
        }

        public IStateMachineEventActivitiesBuilder<TInstance> DuringAny()
        {
            return CommitActivities().DuringAny();
        }

        public IStateMachineModifier<TInstance> Event(string name, out Event @event)
        {
            return CommitActivities().Event(name, out @event);
        }

        public IStateMachineModifier<TInstance> Event<T>(string name, out Event<T> @event)
            where T : class
        {
            return CommitActivities().Event(name, out @event);
        }

        public IStateMachineModifier<TInstance> Event<TProperty, T>(Expression<Func<TProperty>> propertyExpression,
            Expression<Func<TProperty, Event<T>>> eventPropertyExpression)
            where TProperty : class
            where T : class
        {
            return CommitActivities().Event(propertyExpression, eventPropertyExpression);
        }

        public IStateMachineModifier<TInstance> Finally(Func<EventActivityBinder<TInstance>, EventActivityBinder<TInstance>> activityCallback)
        {
            return CommitActivities().Finally(activityCallback);
        }

        public IStateMachineEventActivitiesBuilder<TInstance> Initially()
        {
            return CommitActivities().Initially();
        }

        public IStateMachineModifier<TInstance> InstanceState(Expression<Func<TInstance, State>> instanceStateProperty)
        {
            return CommitActivities().InstanceState(instanceStateProperty);
        }

        public IStateMachineModifier<TInstance> InstanceState(Expression<Func<TInstance, string>> instanceStateProperty)
        {
            return CommitActivities().InstanceState(instanceStateProperty);
        }

        public IStateMachineModifier<TInstance> InstanceState(Expression<Func<TInstance, int>> instanceStateProperty, params State[] states)
        {
            return CommitActivities().InstanceState(instanceStateProperty, states);
        }

        public IStateMachineModifier<TInstance> Name(string machineName)
        {
            return CommitActivities().Name(machineName);
        }

        public IStateMachineModifier<TInstance> OnUnhandledEvent(UnhandledEventCallback<TInstance> callback)
        {
            return CommitActivities().OnUnhandledEvent(callback);
        }

        public IStateMachineModifier<TInstance> State(string name, out State<TInstance> state)
        {
            return CommitActivities().State(name, out state);
        }

        public IStateMachineModifier<TInstance> State(string name, out State state)
        {
            return CommitActivities().State(name, out state);
        }

        public IStateMachineModifier<TInstance> State<TProperty>(Expression<Func<TProperty>> propertyExpression,
            Expression<Func<TProperty, State>> statePropertyExpression)
            where TProperty : class
        {
            return CommitActivities().State(propertyExpression, statePropertyExpression);
        }

        public IStateMachineModifier<TInstance> SubState(string name, State superState, out State<TInstance> subState)
        {
            return CommitActivities().SubState(name, superState, out subState);
        }

        public IStateMachineModifier<TInstance> SubState<TProperty>(Expression<Func<TProperty>> propertyExpression,
            Expression<Func<TProperty, State>> statePropertyExpression, State superState)
            where TProperty : class
        {
            return CommitActivities().SubState(propertyExpression, statePropertyExpression, superState);
        }

        public IStateMachineModifier<TInstance> WhenEnter(State state,
            Func<EventActivityBinder<TInstance>, EventActivityBinder<TInstance>> activityCallback)
        {
            return CommitActivities().WhenEnter(state, activityCallback);
        }

        public IStateMachineModifier<TInstance> WhenEnterAny(Func<EventActivityBinder<TInstance>, EventActivityBinder<TInstance>> activityCallback)
        {
            return CommitActivities().WhenEnterAny(activityCallback);
        }

        public IStateMachineModifier<TInstance> WhenLeave(State state,
            Func<EventActivityBinder<TInstance>, EventActivityBinder<TInstance>> activityCallback)
        {
            return CommitActivities().WhenLeave(state, activityCallback);
        }

        public IStateMachineModifier<TInstance> WhenLeaveAny(Func<EventActivityBinder<TInstance>, EventActivityBinder<TInstance>> activityCallback)
        {
            return CommitActivities().WhenLeaveAny(activityCallback);
        }

    #endregion
    }
}
