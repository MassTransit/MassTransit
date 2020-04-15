namespace MassTransit.StructureMapIntegration.ScopeProviders
{
    using System;
    using Context;
    using GreenPipes;
    using Scoping;
    using Scoping.SendContexts;
    using StructureMap;


    public class StructureMapSendScopeProvider :
        ISendScopeProvider
    {
        readonly IContainer _container;
        readonly IContext _context;

        public StructureMapSendScopeProvider(IContainer container)
        {
            _container = container ?? throw new ArgumentNullException(nameof(container));
        }

        public StructureMapSendScopeProvider(IContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public void Probe(ProbeContext context)
        {
            context.Add("provider", "structuremap");
        }

        public ISendScopeContext<T> GetScope<T>(SendContext<T> context)
            where T : class
        {
            if (context.TryGetPayload<IContainer>(out _))
                return new ExistingSendScopeContext<T>(context);

            var nestedContainer = _container?.CreateNestedContainer(context) ?? _context?.CreateNestedContainer(context);
            try
            {
                var sendContext = new SendContextScope<T>(context, nestedContainer);

                return new CreatedSendScopeContext<IContainer, T>(nestedContainer, sendContext);
            }
            catch
            {
                nestedContainer.Dispose();
                throw;
            }
        }
    }
}
