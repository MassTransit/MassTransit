namespace MassTransit
{
    using System;
    using System.Linq.Expressions;


    public interface IStateMachineModifier<TSaga>
        where TSaga : class, SagaStateMachineInstance
    {
        State Initial { get; }
        State Final { get; }

        IStateMachineModifier<TSaga> InstanceState(Expression<Func<TSaga, State>> instanceStateProperty);
        IStateMachineModifier<TSaga> InstanceState(Expression<Func<TSaga, string>> instanceStateProperty);
        IStateMachineModifier<TSaga> InstanceState(Expression<Func<TSaga, int>> instanceStateProperty, params State[] states);
        IStateMachineModifier<TSaga> Name(string machineName);
        IStateMachineModifier<TSaga> Event(string name, out Event @event);

        IStateMachineModifier<TSaga> Event<T>(string name, out Event<T> @event)
            where T : class;

        IStateMachineModifier<TSaga> Event<TProperty, T>(Expression<Func<TProperty>> propertyExpression,
            Expression<Func<TProperty, Event<T>>> eventPropertyExpression)
            where TProperty : class
            where T : class;

        IStateMachineModifier<TSaga> CompositeEvent(string name, out Event @event,
            Expression<Func<TSaga, CompositeEventStatus>> trackingPropertyExpression,
            params Event[] events);

        IStateMachineModifier<TSaga> CompositeEvent(string name, out Event @event,
            Expression<Func<TSaga, CompositeEventStatus>> trackingPropertyExpression,
            CompositeEventOptions options,
            params Event[] events);

        IStateMachineModifier<TSaga> CompositeEvent(string name, out Event @event,
            Expression<Func<TSaga, int>> trackingPropertyExpression,
            params Event[] events);

        IStateMachineModifier<TSaga> CompositeEvent(string name, out Event @event,
            Expression<Func<TSaga, int>> trackingPropertyExpression,
            CompositeEventOptions options,
            params Event[] events);

        IStateMachineModifier<TSaga> State(string name, out State<TSaga> state);
        IStateMachineModifier<TSaga> State(string name, out State state);

        IStateMachineModifier<TSaga> State<TProperty>(Expression<Func<TProperty>> propertyExpression,
            Expression<Func<TProperty, State>> statePropertyExpression)
            where TProperty : class;

        IStateMachineModifier<TSaga> SubState(string name, State superState, out State<TSaga> subState);

        IStateMachineModifier<TSaga> SubState<TProperty>(Expression<Func<TProperty>> propertyExpression,
            Expression<Func<TProperty, State>> statePropertyExpression, State superState)
            where TProperty : class;

        IStateMachineEventActivitiesBuilder<TSaga> During(params State[] states);
        IStateMachineEventActivitiesBuilder<TSaga> Initially();
        IStateMachineEventActivitiesBuilder<TSaga> DuringAny();
        IStateMachineModifier<TSaga> Finally(Func<EventActivityBinder<TSaga>, EventActivityBinder<TSaga>> activityCallback);

        IStateMachineModifier<TSaga> WhenEnter(State state,
            Func<EventActivityBinder<TSaga>, EventActivityBinder<TSaga>> activityCallback);

        IStateMachineModifier<TSaga> WhenEnterAny(Func<EventActivityBinder<TSaga>, EventActivityBinder<TSaga>> activityCallback);
        IStateMachineModifier<TSaga> WhenLeaveAny(Func<EventActivityBinder<TSaga>, EventActivityBinder<TSaga>> activityCallback);

        IStateMachineModifier<TSaga> BeforeEnterAny(Func<EventActivityBinder<TSaga, State>, EventActivityBinder<TSaga, State>> activityCallback);

        IStateMachineModifier<TSaga> AfterLeaveAny(Func<EventActivityBinder<TSaga, State>, EventActivityBinder<TSaga, State>> activityCallback);

        IStateMachineModifier<TSaga> WhenLeave(State state,
            Func<EventActivityBinder<TSaga>, EventActivityBinder<TSaga>> activityCallback);

        IStateMachineModifier<TSaga> BeforeEnter(State state,
            Func<EventActivityBinder<TSaga, State>, EventActivityBinder<TSaga, State>> activityCallback);

        IStateMachineModifier<TSaga> AfterLeave(State state,
            Func<EventActivityBinder<TSaga, State>, EventActivityBinder<TSaga, State>> activityCallback);

        IStateMachineModifier<TSaga> OnUnhandledEvent(UnhandledEventCallback<TSaga> callback);

        void Apply();
    }
}
