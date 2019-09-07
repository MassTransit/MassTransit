namespace MassTransit.LamarIntegration.ScopeProviders
{
    using Courier;
    using Courier.Hosts;
    using GreenPipes;
    using Lamar;
    using Scoping;
    using Scoping.CourierContexts;


    public class LamarExecuteActivityScopeProvider<TActivity, TArguments> :
        IExecuteActivityScopeProvider<TActivity, TArguments>
        where TActivity : class, IExecuteActivity<TArguments>
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
                existingContainer.Inject(context.ConsumeContext);

                var activity = existingContainer.GetInstance<TActivity>();

                var activityContext = new HostExecuteActivityContext<TActivity, TArguments>(activity, context);

                return new ExistingExecuteActivityScopeContext<TActivity, TArguments>(activityContext);
            }

            var nestedContainer = _container.GetNestedContainer(context.ConsumeContext);
            try
            {
                var activity = nestedContainer.GetInstance<TActivity>();

                ExecuteActivityContext<TActivity, TArguments> activityContext = new HostExecuteActivityContext<TActivity, TArguments>(activity, context);

                activityContext.UpdatePayload(nestedContainer);

                return new CreatedExecuteActivityScopeContext<INestedContainer, TActivity, TArguments>(nestedContainer, activityContext);
            }
            catch
            {
                nestedContainer.Dispose();
                throw;
            }
        }

        public void Probe(ProbeContext context)
        {
            context.Add("provider", "lamar");
        }
    }
}
