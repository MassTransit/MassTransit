namespace MassTransit.SimpleInjectorIntegration.ScopeProviders
{
    using Courier;
    using Courier.Hosts;
    using GreenPipes;
    using Scoping;
    using Scoping.CourierContexts;
    using SimpleInjector;
    using SimpleInjector.Lifestyles;


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

        public IExecuteActivityScopeContext<TActivity, TArguments> GetScope(ExecuteContext<TArguments> context)
        {
            if (context.TryGetPayload<Scope>(out var existingScope))
            {
                existingScope.UpdateScope(context.ConsumeContext);

                var activity = existingScope
                    .Container
                    .GetInstance<TActivity>();

                ExecuteActivityContext<TActivity, TArguments> activityContext = new HostExecuteActivityContext<TActivity, TArguments>(activity, context);

                return new ExistingExecuteActivityScopeContext<TActivity, TArguments>(activityContext);
            }

            var scope = AsyncScopedLifestyle.BeginScope(_container);
            try
            {
                scope.UpdateScope(context.ConsumeContext);

                var activity = scope
                    .Container
                    .GetInstance<TActivity>();

                ExecuteActivityContext<TActivity, TArguments> activityContext = new HostExecuteActivityContext<TActivity, TArguments>(activity, context);

                activityContext.UpdatePayload(scope);

                return new CreatedExecuteActivityScopeContext<Scope, TActivity, TArguments>(scope, activityContext);
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
