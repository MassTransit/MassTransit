namespace MassTransit.LamarIntegration.ScopeProviders
{
    using Context;
    using GreenPipes;
    using Lamar;
    using Scoping;
    using Scoping.SendContexts;


    public class LamarSendScopeProvider :
        ISendScopeProvider
    {
        readonly IContainer _container;

        public LamarSendScopeProvider(IContainer container)
        {
            _container = container;
        }

        public void Probe(ProbeContext context)
        {
            context.Add("provider", "lamar");
        }

        public ISendScopeContext<T> GetScope<T>(SendContext<T> context)
            where T : class
        {
            if (context.TryGetPayload<INestedContainer>(out _))
                return new ExistingSendScopeContext<T>(context);

            var nestedContainer = _container.GetNestedContainer(context);
            try
            {
                var sendContext = new SendContextScope<T>(context, nestedContainer);

                return new CreatedSendScopeContext<INestedContainer, T>(nestedContainer, sendContext);
            }
            catch
            {
                nestedContainer.Dispose();
                throw;
            }
        }
    }
}
