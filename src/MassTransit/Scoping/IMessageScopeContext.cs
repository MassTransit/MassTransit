namespace MassTransit.Scoping
{
    using System;


    public interface IMessageScopeContext<out T> :
        IAsyncDisposable
        where T : class
    {
        ConsumeContext<T> Context { get; }
    }
}
