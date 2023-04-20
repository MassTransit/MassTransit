namespace MassTransit.DependencyInjection
{
    using System;


    public interface IScopedConsumeContextProvider
    {
        bool HasContext { get; }
        ConsumeContext GetContext();
        IDisposable PushContext(ConsumeContext context);
    }
}
