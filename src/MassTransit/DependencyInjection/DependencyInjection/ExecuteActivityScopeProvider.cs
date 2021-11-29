namespace MassTransit.DependencyInjection
{
    using System;
    using System.Threading.Tasks;
    using Context;
    using Courier;
    using Microsoft.Extensions.DependencyInjection;


    public class ExecuteActivityScopeProvider<TActivity, TArguments> :
        BaseConsumeScopeProvider,
        IExecuteActivityScopeProvider<TActivity, TArguments>
        where TActivity : class, IExecuteActivity<TArguments>
        where TArguments : class
    {
        public ExecuteActivityScopeProvider(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
        }

        public ValueTask<IExecuteScopeContext<TArguments>> GetScope(ExecuteContext<TArguments> context)
        {
            return GetScopeContext(context, ExistingScopeContextFactory, CreatedScopeContextFactory, PipeContextFactory);
        }

        public ValueTask<IExecuteActivityScopeContext<TActivity, TArguments>> GetActivityScope(ExecuteContext<TArguments> context)
        {
            return GetScopeContext(context, ExistingActivityScopeContextFactory, CreatedActivityScopeContextFactory, PipeContextFactory);
        }

        public void Probe(ProbeContext context)
        {
            context.Add("provider", "dependencyInjection");
        }

        static ExecuteContext<TArguments> PipeContextFactory(ExecuteContext<TArguments> consumeContext, IServiceScope serviceScope,
            IScopeServiceProvider scopeServiceProvider)
        {
            return new ExecuteContextScope<TArguments>(consumeContext, serviceScope, serviceScope.ServiceProvider, scopeServiceProvider);
        }

        static IExecuteScopeContext<TArguments> ExistingScopeContextFactory(ExecuteContext<TArguments> consumeContext, IServiceScope serviceScope)
        {
            return new ExistingExecuteScopeContext<TArguments>(consumeContext, serviceScope);
        }

        static IExecuteScopeContext<TArguments> CreatedScopeContextFactory(ExecuteContext<TArguments> consumeContext, IServiceScope serviceScope)
        {
            return new CreatedExecuteScopeContext<TArguments>(consumeContext, serviceScope);
        }

        static IExecuteActivityScopeContext<TActivity, TArguments> ExistingActivityScopeContextFactory(ExecuteContext<TArguments> consumeContext,
            IServiceScope serviceScope)
        {
            var activity = serviceScope.ServiceProvider.GetService<TActivity>();
            if (activity == null)
                throw new ConsumerException($"Unable to resolve activity type '{TypeCache<TActivity>.ShortName}'.");

            ExecuteActivityContext<TActivity, TArguments> activityContext = consumeContext.CreateActivityContext(activity);

            return new ExistingExecuteActivityScopeContext<TActivity, TArguments>(activityContext, serviceScope);
        }

        static IExecuteActivityScopeContext<TActivity, TArguments> CreatedActivityScopeContextFactory(ExecuteContext<TArguments> consumeContext,
            IServiceScope serviceScope)
        {
            var activity = serviceScope.ServiceProvider.GetService<TActivity>();
            if (activity == null)
                throw new ConsumerException($"Unable to resolve activity type '{TypeCache<TActivity>.ShortName}'.");

            ExecuteActivityContext<TActivity, TArguments> activityContext = consumeContext.CreateActivityContext(activity);

            return new CreatedExecuteActivityScopeContext<TActivity, TArguments>(activityContext, serviceScope);
        }
    }
}
