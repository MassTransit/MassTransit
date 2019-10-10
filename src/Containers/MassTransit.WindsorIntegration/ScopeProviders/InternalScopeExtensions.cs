namespace MassTransit.WindsorIntegration.ScopeProviders
{
    using System;
    using Castle.MicroKernel;
    using Castle.MicroKernel.Lifestyle;
    using Castle.MicroKernel.Lifestyle.Scoped;
    using GreenPipes;
    using Scoping;


    static class InternalScopeExtensions
    {
        public static IDisposable CreateNewOrUseExistingMessageScope(this IKernel kernel, ConsumeContext consumeContext)
        {
            var currentScope = CallContextLifetimeScope.ObtainCurrentScope();
            if (currentScope is MessageLifetimeScope scope)
            {
                kernel.Resolve<ScopedConsumeContextProvider>().SetContext(scope.ConsumeContext);
                return null;
            }

            var beginScope = kernel.BeginScope();

            kernel.Resolve<ScopedConsumeContextProvider>().SetContext(consumeContext);

            return beginScope;
        }

        public static void UpdateScope(this IKernel kernel, ConsumeContext context)
        {
            kernel.Resolve<ScopedConsumeContextProvider>().SetContext(context);
        }

        public static void UpdatePayload(this PipeContext context, IKernel kernel)
        {
            context.AddOrUpdatePayload(() => kernel, existing => kernel);
        }
    }
}
