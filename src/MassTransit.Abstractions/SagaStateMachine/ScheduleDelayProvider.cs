namespace MassTransit
{
    using System;


    public delegate TimeSpan ScheduleDelayProvider<TSaga>(BehaviorContext<TSaga> context)
        where TSaga : class, SagaStateMachineInstance;


    public delegate TimeSpan ScheduleDelayProvider<TSaga, in TMessage>(BehaviorContext<TSaga, TMessage> context)
        where TMessage : class
        where TSaga : class, SagaStateMachineInstance;
}
