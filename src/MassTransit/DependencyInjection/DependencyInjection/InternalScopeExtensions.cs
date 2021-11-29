namespace MassTransit.DependencyInjection
{
    using Microsoft.Extensions.DependencyInjection;


    static class InternalScopeExtensions
    {
        public static void SetCurrentConsumeContext(this IServiceScope scope, ConsumeContext context)
        {
            scope.ServiceProvider.GetRequiredService<ScopedConsumeContextProvider>().SetContext(context);
        }
    }
}
