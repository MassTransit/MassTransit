namespace MassTransit.ExtensionsDependencyInjectionIntegration.ScopeProviders
{
    using System;
    using Courier;
    using GreenPipes;
    using Metadata;
    using Microsoft.Extensions.DependencyInjection;
    using Scoping;
    using Scoping.CourierContexts;


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

        public IExecuteActivityScopeContext<TActivity, TArguments> GetScope(ExecuteContext<TArguments> context)
        {
            if (context.TryGetPayload<IServiceScope>(out var existingServiceScope))
            {
                existingServiceScope.UpdateScope(context);

                var activity = existingServiceScope.ServiceProvider.GetService<TActivity>();
                if (activity == null)
                    throw new ConsumerException($"Unable to resolve activity type '{TypeMetadataCache<TActivity>.ShortName}'.");

                ExecuteActivityContext<TActivity, TArguments> activityContext = context.CreateActivityContext(activity);

                return new ExistingExecuteActivityScopeContext<TActivity, TArguments>(activityContext);
            }

            if (!context.TryGetPayload(out IServiceProvider serviceProvider))
                serviceProvider = _serviceProvider;

            var serviceScope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope();
            try
            {
                serviceScope.UpdateScope(context);

                var activity = serviceScope.ServiceProvider.GetService<TActivity>();
                if (activity == null)
                    throw new ConsumerException($"Unable to resolve activity type '{TypeMetadataCache<TActivity>.ShortName}'.");

                ExecuteActivityContext<TActivity, TArguments> activityContext = context.CreateActivityContext(activity);

                activityContext.UpdatePayload(serviceScope);

                return new CreatedExecuteActivityScopeContext<IServiceScope, TActivity, TArguments>(serviceScope, activityContext);
            }
            catch
            {
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
