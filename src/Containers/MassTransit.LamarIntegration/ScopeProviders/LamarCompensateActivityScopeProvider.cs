namespace MassTransit.LamarIntegration.ScopeProviders
{
    using Courier;
    using GreenPipes;
    using Lamar;
    using Scoping;
    using Scoping.CourierContexts;


    public class LamarCompensateActivityScopeProvider<TActivity, TLog> : ICompensateActivityScopeProvider<TActivity, TLog>
        where TActivity : class, ICompensateActivity<TLog>
        where TLog : class
    {
        readonly IContainer _container;

        public LamarCompensateActivityScopeProvider(IContainer container)
        {
            _container = container;
        }

        public ICompensateActivityScopeContext<TActivity, TLog> GetScope(CompensateContext<TLog> context)
        {
            if (context.TryGetPayload<INestedContainer>(out var existingContainer))
            {
                existingContainer.Inject<ConsumeContext>(context);

                var activity = existingContainer.GetInstance<TActivity>();

                var activityContext = context.CreateActivityContext(activity);

                return new ExistingCompensateActivityScopeContext<TActivity, TLog>(activityContext);
            }

            var nestedContainer = _container.GetNestedContainer(context);
            try
            {
                var activity = nestedContainer.GetInstance<TActivity>();

                CompensateActivityContext<TActivity, TLog> activityContext = context.CreateActivityContext(activity);

                activityContext.UpdatePayload(nestedContainer);

                return new CreatedCompensateActivityScopeContext<INestedContainer, TActivity, TLog>(nestedContainer, activityContext);
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
