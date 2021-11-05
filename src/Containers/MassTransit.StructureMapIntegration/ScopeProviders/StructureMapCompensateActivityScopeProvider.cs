namespace MassTransit.StructureMapIntegration.ScopeProviders
{
    using System;
    using System.Threading.Tasks;
    using Courier;
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

        public StructureMapCompensateActivityScopeProvider(IContainer container)
        {
            if (container == null)
                throw new ArgumentNullException(nameof(container));

            _container = container;
        }

        public ValueTask<ICompensateActivityScopeContext<TActivity, TLog>> GetScope(CompensateContext<TLog> context)
        {
            if (context.TryGetPayload<IContainer>(out var existingContainer))
            {
                existingContainer.Inject<ConsumeContext>(context);

                var activity = existingContainer
                    .With(context.Log)
                    .GetInstance<TActivity>();

                CompensateActivityContext<TActivity, TLog> activityContext = context.CreateActivityContext(activity);

                return new ValueTask<ICompensateActivityScopeContext<TActivity, TLog>>(
                    new ExistingCompensateActivityScopeContext<TActivity, TLog>(activityContext));
            }

            var nestedContainer = _container.CreateNestedContainer(context);
            try
            {
                var activity = nestedContainer
                    .With(context.Log)
                    .GetInstance<TActivity>();

                CompensateActivityContext<TActivity, TLog> activityContext = context.CreateActivityContext(activity);
                activityContext.UpdatePayload(nestedContainer);

                return new ValueTask<ICompensateActivityScopeContext<TActivity, TLog>>(
                    new CreatedCompensateActivityScopeContext<IContainer, TActivity, TLog>(nestedContainer, activityContext));
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
