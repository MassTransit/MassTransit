namespace MassTransit.StructureMapIntegration.ScopeProviders
{
    using System;
    using Context;
    using GreenPipes;
    using Scoping;
    using Scoping.PublishContexts;
    using StructureMap;


    public class StructureMapPublishScopeProvider :
        IPublishScopeProvider
    {
        readonly IContainer _container;
        readonly IContext _context;

        public StructureMapPublishScopeProvider(IContainer container)
        {
            _container = container ?? throw new ArgumentNullException(nameof(container));
        }

        public StructureMapPublishScopeProvider(IContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public void Probe(ProbeContext context)
        {
            context.Add("provider", "structuremap");
        }

        public IPublishScopeContext<T> GetScope<T>(PublishContext<T> context)
            where T : class
        {
            if (context.TryGetPayload<IContainer>(out _))
                return new ExistingPublishScopeContext<T>(context);

            var nestedContainer = _container?.CreateNestedContainer(context) ?? _context?.CreateNestedContainer(context);
            try
            {
                var publishContext = new PublishContextScope<T>(context, nestedContainer);

                return new CreatedPublishScopeContext<IContainer, T>(nestedContainer, publishContext);
            }
            catch
            {
                nestedContainer.Dispose();
                throw;
            }
        }
    }
}
