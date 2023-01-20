namespace MassTransit.DependencyInjection
{
    using System;
    using Microsoft.Extensions.DependencyInjection;


    static class InternalScopeExtensions
    {
        public static IDisposable SetCurrentConsumeContext(this IServiceScope scope, ConsumeContext context)
        {
            return scope.ServiceProvider.GetRequiredService<ScopedConsumeContextProvider>().PushContext(context);
        }
    }
}
