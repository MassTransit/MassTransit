namespace MassTransit.Scoping
{
    using System;


    public interface ISagaScopeContext<out TSaga> :
        IDisposable
        where TSaga : class
    {
        ConsumeContext<TSaga> Context { get; }
    }
}