namespace MassTransit.LamarIntegration.ScopeProviders
{
    using Courier;
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
                existingContainer.Inject<ConsumeContext>(context);

                var activity = existingContainer.GetInstance<TActivity>();

                var activityContext = context.CreateActivityContext(activity);

                return new ExistingExecuteActivityScopeContext<TActivity, TArguments>(activityContext);
            }

            var nestedContainer = _container.GetNestedContainer(context);
            try
            {
                var activity = nestedContainer.GetInstance<TActivity>();

                ExecuteActivityContext<TActivity, TArguments> activityContext = context.CreateActivityContext(activity);

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
