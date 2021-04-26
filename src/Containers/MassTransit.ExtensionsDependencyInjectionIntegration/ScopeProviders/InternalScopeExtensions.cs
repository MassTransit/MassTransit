namespace MassTransit.ExtensionsDependencyInjectionIntegration.ScopeProviders
{
    using Microsoft.Extensions.DependencyInjection;
    using Scoping;


    static class InternalScopeExtensions
    {
        public static void SetCurrentConsumeContext(this IServiceScope scope, ConsumeContext context)
        {
            scope.ServiceProvider.GetRequiredService<ScopedConsumeContextProvider>().SetContext(context);
        }
    }
}
