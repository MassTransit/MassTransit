namespace MassTransit
{
    using System;


    public delegate TimeSpan ScheduleDelayExceptionProvider<TSaga, in TException>(BehaviorExceptionContext<TSaga, TException> context)
        where TSaga : class, ISaga
        where TException : Exception;


    public delegate TimeSpan ScheduleDelayExceptionProvider<TSaga, in TMessage, in TException>(BehaviorExceptionContext<TSaga, TMessage, TException> context)
        where TMessage : class
        where TSaga : class, ISaga
        where TException : Exception;
}
