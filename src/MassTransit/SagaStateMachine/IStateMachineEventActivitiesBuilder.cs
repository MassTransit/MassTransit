namespace MassTransit
{
    using System;


    public interface IStateMachineEventActivitiesBuilder<TSaga> :
        IStateMachineModifier<TSaga>
        where TSaga : class, SagaStateMachineInstance
    {
        bool IsCommitted { get; }

        IStateMachineEventActivitiesBuilder<TSaga> When(Event @event,
            Func<EventActivityBinder<TSaga>, EventActivityBinder<TSaga>> configure);

        IStateMachineEventActivitiesBuilder<TSaga> When(Event @event, StateMachineCondition<TSaga> filter,
            Func<EventActivityBinder<TSaga>, EventActivityBinder<TSaga>> configure);

        IStateMachineEventActivitiesBuilder<TSaga> When<TData>(Event<TData> @event,
            Func<EventActivityBinder<TSaga, TData>, EventActivityBinder<TSaga, TData>> configure)
            where TData : class;

        IStateMachineEventActivitiesBuilder<TSaga> When<TData>(Event<TData> @event, StateMachineCondition<TSaga, TData> filter,
            Func<EventActivityBinder<TSaga, TData>, EventActivityBinder<TSaga, TData>> configure)
            where TData : class;

        IStateMachineEventActivitiesBuilder<TSaga> Ignore(Event @event);

        IStateMachineEventActivitiesBuilder<TSaga> Ignore<TMessage>(Event<TMessage> @event)
            where TMessage : class;

        IStateMachineEventActivitiesBuilder<TSaga> Ignore<TMessage>(Event<TMessage> @event, StateMachineCondition<TSaga, TMessage> filter)
            where TMessage : class;

        IStateMachineModifier<TSaga> CommitActivities();
    }
}
