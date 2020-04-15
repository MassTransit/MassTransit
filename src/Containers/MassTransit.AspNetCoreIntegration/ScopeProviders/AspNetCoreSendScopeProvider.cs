namespace MassTransit.AspNetCoreIntegration.ScopeProviders
{
    using System;
    using Context;
    using GreenPipes;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.DependencyInjection;
    using Scoping;
    using Scoping.SendContexts;


    public class AspNetCoreSendScopeProvider :
        ISendScopeProvider
    {
        readonly IServiceProvider _serviceProvider;

        public AspNetCoreSendScopeProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            context.Add("provider", "aspNetCore");
        }

        public ISendScopeContext<T> GetScope<T>(SendContext<T> context)
            where T : class
        {
            if (context.TryGetPayload<IServiceScope>(out _))
                return new ExistingSendScopeContext<T>(context);

            if (!context.TryGetPayload(out IServiceProvider serviceProvider))
                serviceProvider = _serviceProvider;

            var httpContextAccessor = serviceProvider.GetService<IHttpContextAccessor>();
            var serviceScope = httpContextAccessor?.HttpContext != null
                ? new HttpContextScope(httpContextAccessor.HttpContext)
                : serviceProvider.CreateScope();
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
