namespace MassTransit.ExtensionsDependencyInjectionIntegration.ScopeProviders
{
    using GreenPipes;
    using Microsoft.Extensions.DependencyInjection;
    using Scoping;


    static class InternalScopeExtensions
    {
        public static void UpdateScope(this IServiceScope scope, string name, ConsumeContext context)
        {
            scope.ServiceProvider.GetRequiredService<ScopedConsumeContextProvider>().SetContext(name, context);
        }

        public static void UpdatePayload(this PipeContext context, IServiceScope scope)
        {
            context.GetOrAddPayload(() => scope);

            var serviceProvider = scope.ServiceProvider;
            context.AddOrUpdatePayload(() => serviceProvider, existing => serviceProvider);
        }
    }
}
