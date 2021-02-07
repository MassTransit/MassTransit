namespace MassTransit.ExtensionsDependencyInjectionIntegration.ScopeProviders
{
    using GreenPipes;
    using Microsoft.Extensions.DependencyInjection;
    using Registration;
    using Scoping;


    static class InternalScopeExtensions
    {
        public static void UpdateScope(this IServiceScope scope, ConsumeContext context)
        {
            scope.ServiceProvider.GetRequiredService<ScopedConsumeContextProvider>().SetContext(context);
        }

        public static void UpdatePayload(this PipeContext context, IServiceScope scope)
        {
            context.AddOrUpdatePayload(() => scope, existing => scope);

            var serviceProvider = scope.ServiceProvider;
            context.AddOrUpdatePayload(() => serviceProvider, existing => serviceProvider);

            var scopeServiceProvider = new DependencyInjectionScopeServiceProvider(serviceProvider);
            context.AddOrUpdatePayload(() => scopeServiceProvider, existing => scopeServiceProvider);
        }
    }
}
