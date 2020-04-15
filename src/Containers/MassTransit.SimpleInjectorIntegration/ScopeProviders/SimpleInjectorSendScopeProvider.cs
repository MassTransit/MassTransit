namespace MassTransit.SimpleInjectorIntegration.ScopeProviders
{
    using Context;
    using GreenPipes;
    using Scoping;
    using Scoping.SendContexts;
    using SimpleInjector;
    using SimpleInjector.Lifestyles;


    public class SimpleInjectorSendScopeProvider :
        ISendScopeProvider
    {
        readonly Container _container;

        public SimpleInjectorSendScopeProvider(Container container)
        {
            _container = container;
        }

        public void Probe(ProbeContext context)
        {
            context.Add("provider", "simpleInjector");
        }

        public ISendScopeContext<T> GetScope<T>(SendContext<T> context)
            where T : class
        {
            if (context.TryGetPayload<Scope>(out _))
                return new ExistingSendScopeContext<T>(context);

            var scope = AsyncScopedLifestyle.BeginScope(_container);
            try
            {
                var sendContext = new SendContextScope<T>(context, scope, scope.Container);

                return new CreatedSendScopeContext<Scope, T>(scope, sendContext);
            }
            catch
            {
                scope.Dispose();

                throw;
            }
        }
    }
}
