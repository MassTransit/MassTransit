namespace MassTransit.Scoping
{
    using System;


    public interface IMessageScopeContext<out T> :
        IDisposable
        where T : class
    {
        ConsumeContext<T> Context { get; }
    }
}
