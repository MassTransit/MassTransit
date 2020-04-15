namespace MassTransit.LamarIntegration.ScopeProviders
{
    using Context;
    using GreenPipes;
    using Lamar;
    using Scoping;
    using Scoping.PublishContexts;


    public class LamarPublishScopeProvider :
        IPublishScopeProvider
    {
        readonly IContainer _container;

        public LamarPublishScopeProvider(IContainer container)
        {
            _container = container;
        }

        public void Probe(ProbeContext context)
        {
            context.Add("provider", "lamar");
        }

        public IPublishScopeContext<T> GetScope<T>(PublishContext<T> context)
            where T : class
        {
            if (context.TryGetPayload<INestedContainer>(out _))
                return new ExistingPublishScopeContext<T>(context);

            var nestedContainer = _container.GetNestedContainer(context);
            try
            {
                var publishContext = new PublishContextScope<T>(context, nestedContainer);

                return new CreatedPublishScopeContext<INestedContainer, T>(nestedContainer, publishContext);
            }
            catch
            {
                nestedContainer.Dispose();
                throw;
            }
        }
    }
}
