namespace MassTransit.SimpleInjectorIntegration.ScopeProviders
{
    using GreenPipes;
    using Scoping;
    using SimpleInjector;


    static class InternalScopeExtensions
    {
        public static void UpdateScope(this Scope scope, ConsumeContext context)
        {
            scope.Container.GetInstance<ScopedConsumeContextProvider>().SetContext(context);
        }

        public static void UpdatePayload(this PipeContext context, Scope scope)
        {
            context.GetOrAddPayload(() => scope);

            var container = scope.Container;
            context.AddOrUpdatePayload(() => container, existing => container);
        }
    }
}
