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
    using Util;


    public class DependencyInjectionExecuteActivityScopeProvider<TActivity, TArguments> :
        IExecuteActivityScopeProvider<TActivity, TArguments>
        where TActivity : class, IExecuteActivity<TArguments>
        where TArguments : class
    {
        readonly IServiceProvider _serviceProvider;

        public DependencyInjectionExecuteActivityScopeProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public ValueTask<IExecuteActivityScopeContext<TActivity, TArguments>> GetScope(ExecuteContext<TArguments> context)
        {
            if (context.TryGetPayload<IServiceScope>(out var existingServiceScope))
            {
                existingServiceScope.SetCurrentConsumeContext(context);

                var activity = existingServiceScope.ServiceProvider.GetService<TActivity>();
                if (activity == null)
                    throw new ConsumerException($"Unable to resolve activity type '{TypeMetadataCache<TActivity>.ShortName}'.");

                ExecuteActivityContext<TActivity, TArguments> activityContext = context.CreateActivityContext(activity);

                return new ValueTask<IExecuteActivityScopeContext<TActivity, TArguments>>(
                    new ExistingExecuteActivityScopeContext<TActivity, TArguments>(activityContext));
            }

            if (!context.TryGetPayload(out IServiceProvider serviceProvider))
                serviceProvider = _serviceProvider;

            var serviceScope = serviceProvider.CreateScope();
            try
            {
                ExecuteContext<TArguments> scopeContext = new ExecuteContextScope<TArguments>(context, serviceScope, serviceScope.ServiceProvider);

                serviceScope.SetCurrentConsumeContext(scopeContext);

                var activity = serviceScope.ServiceProvider.GetService<TActivity>();
                if (activity == null)
                    throw new ConsumerException($"Unable to resolve activity type '{TypeMetadataCache<TActivity>.ShortName}'.");

                ExecuteActivityContext<TActivity, TArguments> activityContext = scopeContext.CreateActivityContext(activity);

                return new ValueTask<IExecuteActivityScopeContext<TActivity, TArguments>>(
                    new CreatedExecuteActivityScopeContext<IServiceScope, TActivity, TArguments>(serviceScope, activityContext));
            }
            catch (Exception ex)
            {
                if (serviceScope is IAsyncDisposable asyncDisposable)
                    return ex.DisposeAsync<IExecuteActivityScopeContext<TActivity, TArguments>>(() => asyncDisposable.DisposeAsync());

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
