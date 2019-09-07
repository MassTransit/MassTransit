namespace MassTransit.SimpleInjectorIntegration.ScopeProviders
{
    using Courier;
    using Courier.Hosts;
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
                existingScope.UpdateScope(context.ConsumeContext);

                var activity = existingScope
                    .Container
                    .GetInstance<TActivity>();

                CompensateActivityContext<TActivity, TLog> activityContext = new HostCompensateActivityContext<TActivity, TLog>(activity, context);

                return new ExistingCompensateActivityScopeContext<TActivity, TLog>(activityContext);
            }

            var scope = AsyncScopedLifestyle.BeginScope(_container);
            try
            {
                scope.UpdateScope(context.ConsumeContext);

                var activity = scope
                    .Container
                    .GetInstance<TActivity>();

                CompensateActivityContext<TActivity, TLog> activityContext = new HostCompensateActivityContext<TActivity, TLog>(activity, context);

                var contextScope = scope;

                activityContext.UpdatePayload(scope);

                return new CreatedCompensateActivityScopeContext<Scope, TActivity, TLog>(contextScope, activityContext);
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
