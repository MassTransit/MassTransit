namespace MassTransit.SimpleInjectorIntegration.ScopeProviders
{
    using System;
    using System.Threading.Tasks;
    using Courier;
    using Courier.Contexts;
    using GreenPipes;
    using Scoping;
    using Scoping.CourierContexts;
    using SimpleInjector;
    using SimpleInjector.Lifestyles;
    using Util;


    public class SimpleInjectorExecuteActivityScopeProvider<TActivity, TArguments> :
        IExecuteActivityScopeProvider<TActivity, TArguments>
        where TActivity : class, IExecuteActivity<TArguments>
        where TArguments : class
    {
        readonly Container _container;

        public SimpleInjectorExecuteActivityScopeProvider(Container container)
        {
            _container = container;
        }

        public ValueTask<IExecuteActivityScopeContext<TActivity, TArguments>> GetScope(ExecuteContext<TArguments> context)
        {
            if (context.TryGetPayload<Scope>(out var existingScope))
            {
                existingScope.UpdateScope(context);

                var activity = existingScope
                    .Container
                    .GetInstance<TActivity>();

                ExecuteActivityContext<TActivity, TArguments> activityContext = context.CreateActivityContext(activity);

                return new ValueTask<IExecuteActivityScopeContext<TActivity, TArguments>>(
                    new ExistingExecuteActivityScopeContext<TActivity, TArguments>(activityContext));
            }

            var scope = AsyncScopedLifestyle.BeginScope(_container);
            try
            {
                ExecuteContext<TArguments> scopeContext = new ExecuteContextScope<TArguments>(context, scope);

                scope.UpdateScope(scopeContext);

                var activity = scope.Container.GetInstance<TActivity>();

                ExecuteActivityContext<TActivity, TArguments> activityContext = scopeContext.CreateActivityContext(activity);

                return new ValueTask<IExecuteActivityScopeContext<TActivity, TArguments>>(
                    new CreatedExecuteActivityScopeContext<Scope, TActivity, TArguments>(scope, activityContext));
            }
            catch (Exception ex)
            {
                return ex.DisposeAsync<IExecuteActivityScopeContext<TActivity, TArguments>>(() => scope.DisposeScopeAsync());
            }
        }

        public void Probe(ProbeContext context)
        {
            context.Add("provider", "simpleInjector");
        }
    }
}
