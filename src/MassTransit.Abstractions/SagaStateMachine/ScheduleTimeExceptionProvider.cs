namespace MassTransit
{
    using System;


    public delegate DateTime ScheduleTimeExceptionProvider<TSaga, in TException>(BehaviorExceptionContext<TSaga, TException> context)
        where TSaga : class, SagaStateMachineInstance
        where TException : Exception;


    public delegate DateTime ScheduleTimeExceptionProvider<TSaga, in TMessage, in TException>(BehaviorExceptionContext<TSaga, TMessage, TException> context)
        where TSaga : class, SagaStateMachineInstance
        where TMessage : class
        where TException : Exception;
}
