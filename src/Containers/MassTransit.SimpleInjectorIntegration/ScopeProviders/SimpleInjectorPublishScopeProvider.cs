namespace MassTransit.SimpleInjectorIntegration.ScopeProviders
{
    using Context;
    using GreenPipes;
    using Scoping;
    using Scoping.PublishContexts;
    using SimpleInjector;
    using SimpleInjector.Lifestyles;


    public class SimpleInjectorPublishScopeProvider :
        IPublishScopeProvider
    {
        readonly Container _container;

        public SimpleInjectorPublishScopeProvider(Container container)
        {
            _container = container;
        }

        public void Probe(ProbeContext context)
        {
            context.Add("provider", "simpleInjector");
        }

        public IPublishScopeContext<T> GetScope<T>(PublishContext<T> context)
            where T : class
        {
            if (context.TryGetPayload<Scope>(out _))
                return new ExistingPublishScopeContext<T>(context);

            var scope = AsyncScopedLifestyle.BeginScope(_container);
            try
            {
                var publishContext = new PublishContextScope<T>(context, scope, scope.Container);

                return new CreatedPublishScopeContext<Scope, T>(scope, publishContext);
            }
            catch
            {
                scope.Dispose();

                throw;
            }
        }
    }
}
