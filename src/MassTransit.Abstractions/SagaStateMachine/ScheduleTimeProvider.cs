namespace MassTransit
{
    using System;


    public delegate DateTime ScheduleTimeProvider<TSaga>(BehaviorContext<TSaga> context)
        where TSaga : class, SagaStateMachineInstance;


    public delegate DateTime ScheduleTimeProvider<TSaga, in TMessage>(BehaviorContext<TSaga, TMessage> context)
        where TMessage : class
        where TSaga : class, SagaStateMachineInstance;
}
