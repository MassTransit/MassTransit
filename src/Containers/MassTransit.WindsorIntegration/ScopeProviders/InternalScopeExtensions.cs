namespace MassTransit.WindsorIntegration.ScopeProviders
{
    using System;
    using Castle.MicroKernel;
    using Castle.MicroKernel.Lifestyle;
    using Castle.MicroKernel.Lifestyle.Scoped;
    using Scoping;


    static class InternalScopeExtensions
    {
        public static IDisposable CreateNewOrUseExistingMessageScope(this IKernel kernel)
        {
            var currentScope = CallContextLifetimeScope.ObtainCurrentScope();
            if (currentScope is MessageLifetimeScope scope)
            {
                kernel.Resolve<ScopedConsumeContextProvider>().SetContext(scope.ConsumeContext);
                return null;
            }

            return kernel.BeginScope();
        }

        public static void UpdateScope(this IKernel kernel, ConsumeContext context)
        {
            kernel.Resolve<ScopedConsumeContextProvider>().SetContext(context);
        }

        public static T TryResolve<T>(this IKernel kernel)
            where T : class
        {
            return kernel.HasComponent(typeof(T)) ? kernel.Resolve<T>() : default;
        }

        public static ConsumeContext GetConsumeContext(this IKernel kernel)
        {
            return CallContextLifetimeScope.ObtainCurrentScope() != null
                ? kernel.Resolve<ScopedConsumeContextProvider>().GetContext()
                : null;
        }
    }
}
