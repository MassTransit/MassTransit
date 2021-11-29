namespace MassTransit
{
    using System;


    public delegate DateTime ScheduleTimeExceptionProvider<TSaga, in TException>(BehaviorExceptionContext<TSaga, TException> context)
        where TSaga : class, ISaga
        where TException : Exception;


    public delegate DateTime ScheduleTimeExceptionProvider<TSaga, in TMessage, in TException>(BehaviorExceptionContext<TSaga, TMessage, TException> context)
        where TSaga : class, ISaga
        where TMessage : class
        where TException : Exception;
}
