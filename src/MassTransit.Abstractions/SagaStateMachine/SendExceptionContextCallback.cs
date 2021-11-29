namespace MassTransit
{
    using System;


    public delegate void SendExceptionContextCallback<TSaga, in TException, in T>(BehaviorExceptionContext<TSaga, TException> context,
        SendContext<T> sendContext)
        where TSaga : class, ISaga
        where TException : Exception
        where T : class;


    public delegate void SendExceptionContextCallback<TSaga, in TMessage, in TException, in T>(BehaviorExceptionContext<TSaga, TMessage, TException> context,
        SendContext<T> sendContext)
        where TSaga : class, ISaga
        where TMessage : class
        where TException : Exception
        where T : class;
}
