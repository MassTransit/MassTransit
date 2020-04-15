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

        public static IDisposable CreateNewMessageScope<T>(this IKernel kernel, SendContext<T> sendContext)
            where T : class
        {
            var beginScope = kernel.BeginScope();
            return beginScope;
        }

        public static IDisposable CreateNewMessageScope<T>(this IKernel kernel, PublishContext<T> publishContext)
            where T : class
        {
            var beginScope = kernel.BeginScope();
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
