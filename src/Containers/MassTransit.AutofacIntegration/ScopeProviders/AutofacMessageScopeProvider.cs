namespace MassTransit.AutofacIntegration.ScopeProviders
{
    using System;
    using System.Threading.Tasks;
    using Autofac;
    using Context;
    using GreenPipes;
    using Scoping;
    using Scoping.ConsumerContexts;


    public class AutofacMessageScopeProvider :
        IMessageScopeProvider
    {
        readonly Action<ContainerBuilder, ConsumeContext> _configureScope;
        readonly string _name;
        readonly ILifetimeScopeProvider _scopeProvider;

        public AutofacMessageScopeProvider(ILifetimeScopeProvider scopeProvider, string name, Action<ContainerBuilder, ConsumeContext> configureScope)
        {
            _scopeProvider = scopeProvider;
            _name = name;
            _configureScope = configureScope;
        }

        public async ValueTask<IMessageScopeContext<T>> GetScope<T>(ConsumeContext<T> context)
            where T : class
        {
            if (context.TryGetPayload<ILifetimeScope>(out _))
                return new ExistingMessageScopeContext<T>(context);

            var parentLifetimeScope = _scopeProvider.GetLifetimeScope(context);

            var lifetimeScope = parentLifetimeScope.BeginLifetimeScope(_name, builder =>
            {
                builder.ConfigureScope(context);
                _configureScope?.Invoke(builder, context);
            });

            try
            {
                var proxy = new ConsumeContextScope<T>(context, lifetimeScope);

                return new CreatedMessageScopeContext<ILifetimeScope, T>(lifetimeScope, proxy);
            }
            catch
            {
                await lifetimeScope.DisposeAsync().ConfigureAwait(false);
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
