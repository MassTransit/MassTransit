namespace MassTransit.DependencyInjection
{
    using System;
    using Microsoft.Extensions.DependencyInjection;


    public class LegacySetScopedConsumeContext :
        ISetScopedConsumeContext
    {
        public static readonly ISetScopedConsumeContext Instance = new LegacySetScopedConsumeContext();

        LegacySetScopedConsumeContext()
        {
        }

        public IDisposable PushContext(IServiceScope scope, ConsumeContext context)
        {
            return scope.ServiceProvider.GetRequiredService<IScopedConsumeContextProvider>().PushContext(context);
        }
    }
}
