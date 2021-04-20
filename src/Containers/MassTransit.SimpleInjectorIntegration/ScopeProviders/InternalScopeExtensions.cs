namespace MassTransit.SimpleInjectorIntegration.ScopeProviders
{
    using Scoping;
    using SimpleInjector;


    static class InternalScopeExtensions
    {
        public static void UpdateScope(this Scope scope, ConsumeContext context)
        {
            scope.Container.GetInstance<ScopedConsumeContextProvider>().SetContext(context);
        }
    }
}
