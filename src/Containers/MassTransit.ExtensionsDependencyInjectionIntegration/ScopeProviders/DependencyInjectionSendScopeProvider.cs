namespace MassTransit.ExtensionsDependencyInjectionIntegration.ScopeProviders
{
    using System;
    using Context;
    using GreenPipes;
    using Microsoft.Extensions.DependencyInjection;
    using Scoping;
    using Scoping.SendContexts;


    public class DependencyInjectionSendScopeProvider :
        ISendScopeProvider
    {
        readonly IServiceProvider _serviceProvider;

        public DependencyInjectionSendScopeProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            context.Add("provider", "dependencyInjection");
        }

        public ISendScopeContext<T> GetScope<T>(SendContext<T> context)
            where T : class
        {
            if (context.TryGetPayload<IServiceScope>(out _))
                return new ExistingSendScopeContext<T>(context);

            if (!context.TryGetPayload(out IServiceProvider serviceProvider))
                serviceProvider = _serviceProvider;

            var serviceScope = serviceProvider.CreateScope();
            try
            {
                var sendContext = new SendContextScope<T>(context, serviceScope, serviceScope.ServiceProvider);

                return new CreatedSendScopeContext<IServiceScope, T>(serviceScope, sendContext);
            }
            catch
            {
                serviceScope.Dispose();

                throw;
            }
        }
    }
}
