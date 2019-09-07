namespace MassTransit.UnityIntegration
{
    using Courier;
    using Courier.Hosts;
    using GreenPipes;
    using Scoping;
    using Scoping.CourierContexts;
    using Unity;
    using Unity.Injection;
    using Unity.Resolution;


    public class UnityCompensateActivityScopeProvider<TActivity, TLog> :
        ICompensateActivityScopeProvider<TActivity, TLog>
        where TActivity : class, ICompensateActivity<TLog>
        where TLog : class
    {
        readonly IUnityContainer _container;

        public UnityCompensateActivityScopeProvider(IUnityContainer container)
        {
            _container = container;
        }

        public ICompensateActivityScopeContext<TActivity, TLog> GetScope(CompensateContext<TLog> context)
        {
            if (context.TryGetPayload<IUnityContainer>(out var existingContainer))
            {
                var activity = existingContainer.Resolve<TActivity>(new DependencyOverride(typeof(TLog), new InjectionParameter(context.Log)));

                CompensateActivityContext<TActivity, TLog> activityContext = new HostCompensateActivityContext<TActivity, TLog>(activity, context);

                return new ExistingCompensateActivityScopeContext<TActivity, TLog>(activityContext);
            }

            var scope = _container.CreateChildContainer();
            try
            {
                var activity = scope.Resolve<TActivity>(new DependencyOverride(typeof(TLog), new InjectionParameter(context.Log)));

                CompensateActivityContext<TActivity, TLog> activityContext = new HostCompensateActivityContext<TActivity, TLog>(activity, context);

                var contextScope = scope;
                activityContext.GetOrAddPayload(() => contextScope);

                return new CreatedCompensateActivityScopeContext<IUnityContainer, TActivity, TLog>(contextScope, activityContext);
            }
            catch
            {
                scope.Dispose();

                throw;
            }
        }

        public void Probe(ProbeContext context)
        {
            context.Add("provider", "unity");
        }
    }
}
