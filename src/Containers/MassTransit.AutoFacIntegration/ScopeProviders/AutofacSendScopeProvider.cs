namespace MassTransit.AutofacIntegration.ScopeProviders
{
    using Autofac;
    using GreenPipes;
    using Scoping;
    using Scoping.SendContexts;


    public class AutofacSendScopeProvider :
        ISendScopeProvider
    {
        readonly string _name;
        readonly ILifetimeScopeProvider _scopeProvider;

        public AutofacSendScopeProvider(ILifetimeScopeProvider scopeProvider, string name)
        {
            _scopeProvider = scopeProvider;
            _name = name;
        }

        ISendScopeContext<T> ISendScopeProvider.GetScope<T>(SendContext<T> context)
            where T : class
        {
            if (context.TryGetPayload<ILifetimeScope>(out _))
                return new ExistingSendScopeContext<T>(context);

            var parentLifetimeScope = _scopeProvider.GetLifetimeScope(context);

            var lifetimeScope = parentLifetimeScope.BeginLifetimeScope(_name, builder => builder.ConfigureScope(context));

            try
            {
                SendContext<T> sendContext = lifetimeScope.GetSendScope(context);

                return new CreatedSendScopeContext<ILifetimeScope, T>(lifetimeScope, sendContext);
            }
            catch
            {
                lifetimeScope.Dispose();

                throw;
            }
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            context.Add("provider", "autofac");
            context.Add("scopeTag", _name);
        }
    }
}
