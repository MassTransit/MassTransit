namespace MassTransit.DependencyInjection
{
    using System;


    public interface IScopedBusContextProvider<TBus>
        where TBus : class, IBus
    {
        ScopedBusContext Context { get; }
    }


    public interface IScopedBusContextProvider
    {
        bool HasContext { get; }
        ScopedBusContext GetContext();
        IDisposable PushContext(ScopedBusContext context);
    }
}
