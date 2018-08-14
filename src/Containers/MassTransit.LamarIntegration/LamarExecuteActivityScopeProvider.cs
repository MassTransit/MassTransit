namespace MassTransit.LamarIntegration
{
    using Courier;
    using Courier.Hosts;
    using GreenPipes;
    using Lamar;
    using Scoping;
    using Scoping.CourierContexts;


    public class LamarExecuteActivityScopeProvider<TActivity, TArguments> :
        IExecuteActivityScopeProvider<TActivity, TArguments>
        where TActivity : class, ExecuteActivity<TArguments>
        where TArguments : class
    {
        readonly IContainer _container;

        public LamarExecuteActivityScopeProvider(IContainer container)
        {
            _container = container;
        }

        public IExecuteActivityScopeContext<TActivity, TArguments> GetScope(ExecuteContext<TArguments> context)
        {
            if (context.TryGetPayload<INestedContainer>(out var existingContainer))
            {
                var activityFactory = existingContainer.GetInstance<LamarActivityFactory>();
                var activity = activityFactory.Get<TActivity, TArguments>(context.Arguments);

                var activityContext = new HostExecuteActivityContext<TActivity, TArguments>(activity, context);

                return new ExistingExecuteActivityScopeContext<TActivity, TArguments>(activityContext);
            }

            var scopeContainer = _container.GetNestedContainer(context.ConsumeContext);
            try
            {
                var activityFactory = scopeContainer.GetInstance<LamarActivityFactory>();
                var activity = activityFactory.Get<TActivity, TArguments>(context.Arguments);

                ExecuteActivityContext<TActivity, TArguments> activityContext = new HostExecuteActivityContext<TActivity, TArguments>(activity, context);

                var scope = scopeContainer;
                activityContext.GetOrAddPayload(() => scope);

                return new CreatedExecuteActivityScopeContext<INestedContainer, TActivity, TArguments>(scope, activityContext);
            }
            catch
            {
                scopeContainer.Dispose();
                throw;
            }
        }

        public void Probe(ProbeContext context)
        {
            context.Add("provider", "lamar");
        }
    }
}
