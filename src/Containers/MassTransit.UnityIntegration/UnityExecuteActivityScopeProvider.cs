namespace MassTransit.UnityIntegration
{
    using Courier;
    using GreenPipes;
    using Scoping;
    using Scoping.CourierContexts;
    using Unity;
    using Unity.Injection;
    using Unity.Resolution;


    public class UnityExecuteActivityScopeProvider<TActivity, TArguments> :
        IExecuteActivityScopeProvider<TActivity, TArguments>
        where TActivity : class, IExecuteActivity<TArguments>
        where TArguments : class
    {
        readonly IUnityContainer _container;

        public UnityExecuteActivityScopeProvider(IUnityContainer container)
        {
            _container = container;
        }

        public IExecuteActivityScopeContext<TActivity, TArguments> GetScope(ExecuteContext<TArguments> context)
        {
            if (context.TryGetPayload<IUnityContainer>(out var existingContainer))
            {
                var activity = existingContainer.Resolve<TActivity>(new DependencyOverride(typeof(TArguments), new InjectionParameter(context.Arguments)));

                ExecuteActivityContext<TActivity, TArguments> activityContext = context.CreateActivityContext(activity);

                return new ExistingExecuteActivityScopeContext<TActivity, TArguments>(activityContext);
            }

            var scope = _container.CreateChildContainer();
            try
            {
                var activity = scope.Resolve<TActivity>(new DependencyOverride(typeof(TArguments), new InjectionParameter(context.Arguments)));

                ExecuteActivityContext<TActivity, TArguments> activityContext = context.CreateActivityContext(activity);

                var contextScope = scope;
                activityContext.GetOrAddPayload(() => contextScope);

                return new CreatedExecuteActivityScopeContext<IUnityContainer, TActivity, TArguments>(contextScope, activityContext);
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
