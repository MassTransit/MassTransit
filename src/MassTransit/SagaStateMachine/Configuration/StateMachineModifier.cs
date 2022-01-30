namespace MassTransit.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;


    class StateMachineModifier<TSaga> :
        IStateMachineModifier<TSaga>
        where TSaga : class, SagaStateMachineInstance
    {
        readonly List<IStateMachineEventActivitiesBuilder<TSaga>> _activityBuilders;
        readonly MassTransitStateMachine<TSaga> _machine;

        public StateMachineModifier(MassTransitStateMachine<TSaga> machine)
        {
            _machine = machine ?? throw new ArgumentNullException(nameof(machine));
            _activityBuilders = new List<IStateMachineEventActivitiesBuilder<TSaga>>();
        }

        public State Initial => _machine.Initial;
        public State Final => _machine.Final;

        public void Apply()
        {
            IStateMachineEventActivitiesBuilder<TSaga>[] uncommittedActivities = _activityBuilders
                .Where(builder => !builder.IsCommitted)
                .ToArray();

            foreach (IStateMachineEventActivitiesBuilder<TSaga> builder in uncommittedActivities)
                builder.CommitActivities();
        }

        public IStateMachineEventActivitiesBuilder<TSaga> During(params State[] states)
        {
            var builder = new StateMachineEventActivitiesBuilder<TSaga>(_machine, this, activities => _machine.During(states, activities));
            _activityBuilders.Add(builder);
            return builder;
        }

        public IStateMachineEventActivitiesBuilder<TSaga> DuringAny()
        {
            var builder = new StateMachineEventActivitiesBuilder<TSaga>(_machine, this, activities => _machine.DuringAny(activities));
            _activityBuilders.Add(builder);
            return builder;
        }

        public IStateMachineEventActivitiesBuilder<TSaga> Initially()
        {
            var builder = new StateMachineEventActivitiesBuilder<TSaga>(_machine, this, activities => _machine.Initially(activities));
            _activityBuilders.Add(builder);
            return builder;
        }

        public IStateMachineModifier<TSaga> AfterLeave(State state,
            Func<EventActivityBinder<TSaga, State>, EventActivityBinder<TSaga, State>> activityCallback)
        {
            _machine.AfterLeave(state, activityCallback);
            return this;
        }

        public IStateMachineModifier<TSaga> AfterLeaveAny(Func<EventActivityBinder<TSaga, State>, EventActivityBinder<TSaga, State>> activityCallback)
        {
            _machine.AfterLeaveAny(activityCallback);
            return this;
        }

        public IStateMachineModifier<TSaga> BeforeEnter(State state,
            Func<EventActivityBinder<TSaga, State>, EventActivityBinder<TSaga, State>> activityCallback)
        {
            _machine.BeforeEnter(state, activityCallback);
            return this;
        }

        public IStateMachineModifier<TSaga> BeforeEnterAny(Func<EventActivityBinder<TSaga, State>, EventActivityBinder<TSaga, State>> activityCallback)
        {
            _machine.BeforeEnterAny(activityCallback);
            return this;
        }

        public IStateMachineModifier<TSaga> CompositeEvent(string name, out Event @event,
            Expression<Func<TSaga, CompositeEventStatus>> trackingPropertyExpression, params Event[] events)
        {
            Event(name, out @event);
            _machine.CompositeEvent(@event, trackingPropertyExpression, events);
            return this;
        }

        public IStateMachineModifier<TSaga> CompositeEvent(string name, out Event @event,
            Expression<Func<TSaga, CompositeEventStatus>> trackingPropertyExpression, CompositeEventOptions options,
            params Event[] events)
        {
            Event(name, out @event);
            _machine.CompositeEvent(@event, trackingPropertyExpression, options, events);
            return this;
        }

        public IStateMachineModifier<TSaga> CompositeEvent(string name, out Event @event, Expression<Func<TSaga, int>> trackingPropertyExpression,
            params Event[] events)
        {
            Event(name, out @event);
            _machine.CompositeEvent(@event, trackingPropertyExpression, events);
            return this;
        }

        public IStateMachineModifier<TSaga> CompositeEvent(string name, out Event @event, Expression<Func<TSaga, int>> trackingPropertyExpression,
            CompositeEventOptions options, params Event[] events)
        {
            Event(name, out @event);
            _machine.CompositeEvent(@event, trackingPropertyExpression, options, events);
            return this;
        }

        public IStateMachineModifier<TSaga> Event(string name, out Event @event)
        {
            @event = _machine.Event(name);
            return this;
        }

        public IStateMachineModifier<TSaga> Event<T>(string name, out Event<T> @event)
            where T : class
        {
            @event = _machine.Event<T>(name);
            return this;
        }

        public IStateMachineModifier<TSaga> Event<TProperty, T>(Expression<Func<TProperty>> propertyExpression,
            Expression<Func<TProperty, Event<T>>> eventPropertyExpression)
            where TProperty : class
            where T : class
        {
            _machine.Event(propertyExpression, eventPropertyExpression);
            return this;
        }

        public IStateMachineModifier<TSaga> Finally(Func<EventActivityBinder<TSaga>, EventActivityBinder<TSaga>> activityCallback)
        {
            _machine.Finally(activityCallback);
            return this;
        }

        public IStateMachineModifier<TSaga> InstanceState(Expression<Func<TSaga, State>> instanceStateProperty)
        {
            _machine.InstanceState(instanceStateProperty);
            return this;
        }

        public IStateMachineModifier<TSaga> InstanceState(Expression<Func<TSaga, string>> instanceStateProperty)
        {
            _machine.InstanceState(instanceStateProperty);
            return this;
        }

        public IStateMachineModifier<TSaga> InstanceState(Expression<Func<TSaga, int>> instanceStateProperty, params State[] states)
        {
            _machine.InstanceState(instanceStateProperty, states);
            return this;
        }

        public IStateMachineModifier<TSaga> Name(string machineName)
        {
            _machine.Name(machineName);
            return this;
        }

        public IStateMachineModifier<TSaga> OnUnhandledEvent(UnhandledEventCallback<TSaga> callback)
        {
            _machine.OnUnhandledEvent(callback);
            return this;
        }

        public IStateMachineModifier<TSaga> State(string name, out State<TSaga> state)
        {
            state = _machine.State(name);
            return this;
        }

        public IStateMachineModifier<TSaga> State(string name, out State state)
        {
            state = _machine.State(name);
            return this;
        }

        public IStateMachineModifier<TSaga> State<TProperty>(Expression<Func<TProperty>> propertyExpression,
            Expression<Func<TProperty, State>> statePropertyExpression)
            where TProperty : class
        {
            _machine.State(propertyExpression, statePropertyExpression);
            return this;
        }

        public IStateMachineModifier<TSaga> SubState(string name, State superState, out State<TSaga> subState)
        {
            subState = _machine.SubState(name, superState);
            return this;
        }

        public IStateMachineModifier<TSaga> SubState<TProperty>(Expression<Func<TProperty>> propertyExpression,
            Expression<Func<TProperty, State>> statePropertyExpression, State superState)
            where TProperty : class
        {
            _machine.SubState(propertyExpression, statePropertyExpression, superState);
            return this;
        }

        public IStateMachineModifier<TSaga> WhenEnter(State state,
            Func<EventActivityBinder<TSaga>, EventActivityBinder<TSaga>> activityCallback)
        {
            _machine.WhenEnter(state, activityCallback);
            return this;
        }

        public IStateMachineModifier<TSaga> WhenEnterAny(Func<EventActivityBinder<TSaga>, EventActivityBinder<TSaga>> activityCallback)
        {
            _machine.WhenEnterAny(activityCallback);
            return this;
        }

        public IStateMachineModifier<TSaga> WhenLeave(State state,
            Func<EventActivityBinder<TSaga>, EventActivityBinder<TSaga>> activityCallback)
        {
            _machine.WhenLeave(state, activityCallback);
            return this;
        }

        public IStateMachineModifier<TSaga> WhenLeaveAny(Func<EventActivityBinder<TSaga>, EventActivityBinder<TSaga>> activityCallback)
        {
            _machine.WhenLeaveAny(activityCallback);
            return this;
        }

        public IStateMachineModifier<TSaga> InstanceState(Expression<Func<TSaga, int>> instanceStateProperty,
            params string[] stateNames)
        {
            // NOTE: May need to re-think this; Assumes the states have already been declared.
            State<TSaga>[] states = stateNames
                .Select(name => _machine.GetState(name))
                .ToArray();

            _machine.InstanceState(instanceStateProperty, states.Cast<State>().ToArray());
            return this;
        }
    }
}
