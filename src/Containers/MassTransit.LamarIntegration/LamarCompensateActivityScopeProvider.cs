namespace MassTransit.LamarIntegration
{
    using Courier;
    using Courier.Hosts;
    using GreenPipes;
    using Lamar;
    using Scoping;
    using Scoping.CourierContexts;


    public class LamarCompensateActivityScopeProvider<TActivity, TLog> : ICompensateActivityScopeProvider<TActivity, TLog>
        where TActivity : class, CompensateActivity<TLog>
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
                var activityFactory = existingContainer.GetInstance<LamarActivityFactory>();
                var activity = activityFactory.Get<TActivity, TLog>(context.Log);

                var activityContext = new HostCompensateActivityContext<TActivity, TLog>(activity, context);
                return new ExistingCompensateActivityScopeContext<TActivity, TLog>(activityContext);
            }

            var scopeContainer = _container.GetNestedContainer(context.ConsumeContext);
            try
            {
                var activityFactory = scopeContainer.GetInstance<LamarActivityFactory>();
                var activity = activityFactory.Get<TActivity, TLog>(context.Log);

                CompensateActivityContext<TActivity, TLog> activityContext = new HostCompensateActivityContext<TActivity, TLog>(activity, context);

                var scope = scopeContainer;
                activityContext.GetOrAddPayload(() => scope);

                return new CreatedCompensateActivityScopeContext<INestedContainer, TActivity, TLog>(scope, activityContext);
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
