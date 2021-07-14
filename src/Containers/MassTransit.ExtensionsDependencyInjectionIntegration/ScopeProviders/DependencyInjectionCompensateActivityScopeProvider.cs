namespace MassTransit.ExtensionsDependencyInjectionIntegration.ScopeProviders
{
    using System;
    using System.Threading.Tasks;
    using Courier;
    using Courier.Contexts;
    using GreenPipes;
    using Metadata;
    using Microsoft.Extensions.DependencyInjection;
    using Scoping;
    using Scoping.CourierContexts;


    public class DependencyInjectionCompensateActivityScopeProvider<TActivity, TLog> :
        ICompensateActivityScopeProvider<TActivity, TLog>
        where TActivity : class, ICompensateActivity<TLog>
        where TLog : class
    {
        readonly IServiceProvider _serviceProvider;

        public DependencyInjectionCompensateActivityScopeProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async ValueTask<ICompensateActivityScopeContext<TActivity, TLog>> GetScope(CompensateContext<TLog> context)
        {
            if (context.TryGetPayload<IServiceScope>(out var existingServiceScope))
            {
                existingServiceScope.SetCurrentConsumeContext(context);

                var activity = existingServiceScope.ServiceProvider.GetService<TActivity>();
                if (activity == null)
                    throw new ConsumerException($"Unable to resolve activity type '{TypeMetadataCache<TActivity>.ShortName}'.");

                CompensateActivityContext<TActivity, TLog> activityContext = context.CreateActivityContext(activity);

                return new ExistingCompensateActivityScopeContext<TActivity, TLog>(activityContext);
            }

            if (!context.TryGetPayload(out IServiceProvider serviceProvider))
                serviceProvider = _serviceProvider;

            var serviceScope = serviceProvider.CreateScope();
            try
            {
                CompensateContext<TLog> scopeContext = new CompensateContextScope<TLog>(context, serviceScope, serviceScope.ServiceProvider);

                serviceScope.SetCurrentConsumeContext(scopeContext);

                var activity = serviceScope.ServiceProvider.GetService<TActivity>();
                if (activity == null)
                    throw new ConsumerException($"Unable to resolve activity type '{TypeMetadataCache<TActivity>.ShortName}'.");

                CompensateActivityContext<TActivity, TLog> activityContext = scopeContext.CreateActivityContext(activity);

                return new CreatedCompensateActivityScopeContext<IServiceScope, TActivity, TLog>(serviceScope, activityContext);
            }
            catch
            {
                if (serviceScope is IAsyncDisposable asyncDisposable)
                    await asyncDisposable.DisposeAsync().ConfigureAwait(false);
                else
                    serviceScope.Dispose();

                throw;
            }
        }

        public void Probe(ProbeContext context)
        {
            context.Add("provider", "dependencyInjection");
        }
    }
}
