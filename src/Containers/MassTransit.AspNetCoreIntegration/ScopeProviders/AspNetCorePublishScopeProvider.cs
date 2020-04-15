namespace MassTransit.AspNetCoreIntegration.ScopeProviders
{
    using System;
    using Context;
    using GreenPipes;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.DependencyInjection;
    using Scoping;
    using Scoping.PublishContexts;


    public class AspNetCorePublishScopeProvider :
        IPublishScopeProvider
    {
        readonly IServiceProvider _serviceProvider;

        public AspNetCorePublishScopeProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            context.Add("provider", "dependencyInjection");
        }

        public IPublishScopeContext<T> GetScope<T>(PublishContext<T> context)
            where T : class
        {
            if (context.TryGetPayload<IServiceScope>(out _))
                return new ExistingPublishScopeContext<T>(context);

            if (!context.TryGetPayload(out IServiceProvider serviceProvider))
                serviceProvider = _serviceProvider;

            var httpContextAccessor = serviceProvider.GetService<IHttpContextAccessor>();
            var serviceScope = httpContextAccessor?.HttpContext != null
                ? new HttpContextScope(httpContextAccessor.HttpContext)
                : serviceProvider.CreateScope();
            try
            {
                var publishContext = new PublishContextScope<T>(context, serviceScope, serviceScope.ServiceProvider);

                return new CreatedPublishScopeContext<IServiceScope, T>(serviceScope, publishContext);
            }
            catch
            {
                serviceScope.Dispose();

                throw;
            }
        }
    }
}
