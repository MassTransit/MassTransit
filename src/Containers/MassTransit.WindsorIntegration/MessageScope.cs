namespace MassTransit.WindsorIntegration
{
    using System;
    using Castle.MicroKernel.Context;
    using Castle.MicroKernel.Lifestyle.Scoped;


    public class MessageScope :
        IScopeAccessor
    {
        public ILifetimeScope GetScope(CreationContext context)
        {
            if (CallContextLifetimeScope.ObtainCurrentScope() is MessageLifetimeScope lifetimeScope)
                return lifetimeScope;

            throw new InvalidOperationException("MessageScope was not available. Add UseMessageScope() to your receive endpoint to enable message scope");
        }

        public void Dispose()
        {
        }
    }
}
