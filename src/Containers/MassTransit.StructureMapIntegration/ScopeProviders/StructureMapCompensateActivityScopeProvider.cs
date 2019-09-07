namespace MassTransit.StructureMapIntegration.ScopeProviders
{
    using Courier;
    using Courier.Hosts;
    using GreenPipes;
    using Scoping;
    using Scoping.CourierContexts;
    using StructureMap;


    public class StructureMapCompensateActivityScopeProvider<TActivity, TLog> :
        ICompensateActivityScopeProvider<TActivity, TLog>
        where TActivity : class, ICompensateActivity<TLog>
        where TLog : class
    {
        readonly IContainer _container;
        readonly IContext _context;

        public StructureMapCompensateActivityScopeProvider(IContext context)
        {
            _context = context;
        }

        public StructureMapCompensateActivityScopeProvider(IContainer container)
        {
            _container = container;
        }

        public ICompensateActivityScopeContext<TActivity, TLog> GetScope(CompensateContext<TLog> context)
        {
            if (context.TryGetPayload<IContainer>(out var existingContainer))
            {
                existingContainer.Inject(context.ConsumeContext);

                var activity = existingContainer
                    .With(context.Log)
                    .GetInstance<TActivity>();

                CompensateActivityContext<TActivity, TLog> activityContext = new HostCompensateActivityContext<TActivity, TLog>(activity, context);

                return new ExistingCompensateActivityScopeContext<TActivity, TLog>(activityContext);
            }

            var nestedContainer = _container?.CreateNestedContainer(context.ConsumeContext) ?? _context?.CreateNestedContainer(context.ConsumeContext);
            try
            {
                var activity = nestedContainer
                    .With(context.Log)
                    .GetInstance<TActivity>();

                CompensateActivityContext<TActivity, TLog> activityContext = new HostCompensateActivityContext<TActivity, TLog>(activity, context);
                activityContext.UpdatePayload(nestedContainer);

                return new CreatedCompensateActivityScopeContext<IContainer, TActivity, TLog>(nestedContainer, activityContext);
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
