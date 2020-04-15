namespace MassTransit.AutofacIntegration.ScopeProviders
{
    using Autofac;
    using GreenPipes;
    using Scoping;
    using Scoping.PublishContexts;


    public class AutofacPublishScopeProvider :
        IPublishScopeProvider
    {
        readonly string _name;
        readonly ILifetimeScopeProvider _scopeProvider;

        public AutofacPublishScopeProvider(ILifetimeScopeProvider scopeProvider, string name)
        {
            _scopeProvider = scopeProvider;
            _name = name;
        }

        IPublishScopeContext<T> IPublishScopeProvider.GetScope<T>(PublishContext<T> context)
            where T : class
        {
            if (context.TryGetPayload<ILifetimeScope>(out _))
                return new ExistingPublishScopeContext<T>(context);

            var parentLifetimeScope = _scopeProvider.GetLifetimeScope(context);

            var lifetimeScope = parentLifetimeScope.BeginLifetimeScope(_name, builder => builder.ConfigureScope(context));

            try
            {
                PublishContext<T> publishContext = lifetimeScope.GetPublishScope(context);

                return new CreatedPublishScopeContext<ILifetimeScope, T>(lifetimeScope, publishContext);
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
