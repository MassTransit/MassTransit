namespace MassTransit.Scoping
{
    using System;


    public interface ISendScopeContext<out T> :
        IDisposable
        where T : class
    {
        SendContext<T> Context { get; }
    }
}
