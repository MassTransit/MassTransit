namespace MassTransit.DependencyInjection
{
    using System;
    using System.Threading.Tasks;
    using Context;
    using Courier;
    using Microsoft.Extensions.DependencyInjection;


    public class CompensateActivityScopeProvider<TActivity, TLog> :
        BaseConsumeScopeProvider,
        ICompensateActivityScopeProvider<TActivity, TLog>
        where TActivity : class, ICompensateActivity<TLog>
        where TLog : class
    {
        public CompensateActivityScopeProvider(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
        }

        public ValueTask<ICompensateScopeContext<TLog>> GetScope(CompensateContext<TLog> context)
        {
            return GetScopeContext(context, ExistingScopeContextFactory, CreatedScopeContextFactory, PipeContextFactory);
        }

        public ValueTask<ICompensateActivityScopeContext<TActivity, TLog>> GetActivityScope(CompensateContext<TLog> context)
        {
            return GetScopeContext(context, ExistingActivityScopeContextFactory, CreatedActivityScopeContextFactory, PipeContextFactory);
        }

        public void Probe(ProbeContext context)
        {
            context.Add("provider", "dependencyInjection");
        }

        static CompensateContext<TLog> PipeContextFactory(CompensateContext<TLog> consumeContext, IServiceScope serviceScope,
            IScopeServiceProvider scopeServiceProvider)
        {
            return new CompensateContextScope<TLog>(consumeContext, serviceScope, serviceScope.ServiceProvider, scopeServiceProvider);
        }

        static ICompensateScopeContext<TLog> ExistingScopeContextFactory(CompensateContext<TLog> consumeContext, IServiceScope serviceScope)
        {
            return new ExistingCompensateScopeContext<TLog>(consumeContext, serviceScope);
        }

        static ICompensateScopeContext<TLog> CreatedScopeContextFactory(CompensateContext<TLog> consumeContext, IServiceScope serviceScope)
        {
            return new CreatedCompensateScopeContext<TLog>(serviceScope, consumeContext);
        }

        static ICompensateActivityScopeContext<TActivity, TLog> ExistingActivityScopeContextFactory(CompensateContext<TLog> consumeContext,
            IServiceScope serviceScope)
        {
            var activity = serviceScope.ServiceProvider.GetService<TActivity>();
            if (activity == null)
                throw new ConsumerException($"Unable to resolve activity type '{TypeCache<TActivity>.ShortName}'.");

            CompensateActivityContext<TActivity, TLog> activityContext = consumeContext.CreateActivityContext(activity);

            return new ExistingCompensateActivityScopeContext<TActivity, TLog>(activityContext, serviceScope);
        }

        static ICompensateActivityScopeContext<TActivity, TLog> CreatedActivityScopeContextFactory(CompensateContext<TLog> consumeContext,
            IServiceScope serviceScope)
        {
            var activity = serviceScope.ServiceProvider.GetService<TActivity>();
            if (activity == null)
                throw new ConsumerException($"Unable to resolve activity type '{TypeCache<TActivity>.ShortName}'.");

            CompensateActivityContext<TActivity, TLog> activityContext = consumeContext.CreateActivityContext(activity);

            return new CreatedCompensateActivityScopeContext<TActivity, TLog>(activityContext, serviceScope);
        }
    }
}
