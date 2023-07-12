namespace MassTransit
{
    using System;
    using Microsoft.Extensions.DependencyInjection;


    public interface ISetScopedConsumeContext
    {
        IDisposable PushContext(IServiceScope serviceProvider, ConsumeContext context);
    }
}
