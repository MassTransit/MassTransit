namespace MassTransit.StructureMapIntegration.ScopeProviders
{
    using Courier;
    using GreenPipes;
    using Scoping;
    using Scoping.CourierContexts;
    using StructureMap;


    public class StructureMapExecuteActivityScopeProvider<TActivity, TArguments> :
        IExecuteActivityScopeProvider<TActivity, TArguments>
        where TActivity : class, IExecuteActivity<TArguments>
        where TArguments : class
    {
        readonly IContainer _container;
        readonly IContext _context;

        public StructureMapExecuteActivityScopeProvider(IContext context)
        {
            _context = context;
        }

        public StructureMapExecuteActivityScopeProvider(IContainer container)
        {
            _container = container;
        }

        public IExecuteActivityScopeContext<TActivity, TArguments> GetScope(ExecuteContext<TArguments> context)
        {
            if (context.TryGetPayload<IContainer>(out var existingContainer))
            {
                existingContainer.Inject<ConsumeContext>(context);

                var activity = existingContainer
                    .With(context.Arguments)
                    .GetInstance<TActivity>();

                ExecuteActivityContext<TActivity, TArguments> activityContext = context.CreateActivityContext(activity);

                return new ExistingExecuteActivityScopeContext<TActivity, TArguments>(activityContext);
            }

            var nestedContainer = _container?.CreateNestedContainer(context) ?? _context?.CreateNestedContainer(context);
            try
            {
                var activity = nestedContainer
                    .With(context.Arguments)
                    .GetInstance<TActivity>();

                ExecuteActivityContext<TActivity, TArguments> activityContext = context.CreateActivityContext(activity);
                activityContext.UpdatePayload(nestedContainer);

                return new CreatedExecuteActivityScopeContext<IContainer, TActivity, TArguments>(nestedContainer, activityContext);
            }
            catch
            {
                nestedContainer.Dispose();
                throw;
            }
        }

        public void Probe(ProbeContext context)
        {
            context.Add("provider", "structureMap");
        }
    }
}
