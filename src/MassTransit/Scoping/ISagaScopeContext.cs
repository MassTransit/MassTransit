namespace MassTransit.Scoping
{
    using System;


    public interface ISagaScopeContext<out T> :
        IDisposable
        where T : class
    {
        ConsumeContext<T> Context { get; }
    }
}