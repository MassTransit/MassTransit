namespace MassTransit.SimpleInjectorIntegration.ScopeProviders
{
    using Courier;
    using Courier.Contexts;
    using GreenPipes;
    using Scoping;
    using Scoping.CourierContexts;
    using SimpleInjector;
    using SimpleInjector.Lifestyles;


    public class SimpleInjectorCompensateActivityScopeProvider<TActivity, TLog> :
        ICompensateActivityScopeProvider<TActivity, TLog>
        where TActivity : class, ICompensateActivity<TLog>
        where TLog : class
    {
        readonly Container _container;

        public SimpleInjectorCompensateActivityScopeProvider(Container container)
        {
            _container = container;
        }

        public ICompensateActivityScopeContext<TActivity, TLog> GetScope(CompensateContext<TLog> context)
        {
            if (context.TryGetPayload<Scope>(out var existingScope))
            {
                existingScope.UpdateScope(context);

                var activity = existingScope
                    .Container
                    .GetInstance<TActivity>();

                CompensateActivityContext<TActivity, TLog> activityContext = context.CreateActivityContext(activity);

                return new ExistingCompensateActivityScopeContext<TActivity, TLog>(activityContext);
            }

            var scope = AsyncScopedLifestyle.BeginScope(_container);
            try
            {
                CompensateContext<TLog> scopeContext = new CompensateContextScope<TLog>(context, scope);

                scope.UpdateScope(scopeContext);

                var activity = scope.Container.GetInstance<TActivity>();

                CompensateActivityContext<TActivity, TLog> activityContext = scopeContext.CreateActivityContext(activity);

                return new CreatedCompensateActivityScopeContext<Scope, TActivity, TLog>(scope, activityContext);
            }
            catch
            {
                scope.Dispose();

                throw;
            }
        }

        public void Probe(ProbeContext context)
        {
            context.Add("provider", "simpleInjector");
        }
    }
}
