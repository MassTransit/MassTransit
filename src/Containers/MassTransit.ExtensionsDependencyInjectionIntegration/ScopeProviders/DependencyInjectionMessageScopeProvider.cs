namespace MassTransit.ExtensionsDependencyInjectionIntegration.ScopeProviders
{
    using System;
    using Context;
    using GreenPipes;
    using Microsoft.Extensions.DependencyInjection;
    using Registration;
    using Scoping;
    using Scoping.ConsumerContexts;


    public class DependencyInjectionMessageScopeProvider :
        IMessageScopeProvider
    {
        readonly IServiceProvider _serviceProvider;

        public DependencyInjectionMessageScopeProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            context.Add("provider", "dependencyInjection");
        }

        public IMessageScopeContext<T> GetScope<T>(ConsumeContext<T> context)
            where T : class
        {
            if (context.TryGetPayload<IServiceScope>(out var existingServiceScope))
            {
                existingServiceScope.UpdateScope(context);

                return new ExistingMessageScopeContext<T>(context);
            }

            if (!context.TryGetPayload(out IServiceProvider serviceProvider))
                serviceProvider = _serviceProvider;

            var serviceScope = serviceProvider.CreateScope();
            try
            {
                var scopeServiceProvider = new DependencyInjectionScopeServiceProvider(serviceScope.ServiceProvider);

                var scopeContext = new ConsumeContextScope<T>(context, serviceScope, serviceScope.ServiceProvider, scopeServiceProvider);

                serviceScope.UpdateScope(scopeContext);

                return new CreatedMessageScopeContext<IServiceScope, T>(serviceScope, scopeContext);
            }
            catch
            {
                serviceScope.Dispose();

                throw;
            }
        }
    }
}
