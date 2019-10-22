namespace MassTransit.AutofacIntegration.ScopeProviders
{
    using System;
    using Autofac;
    using Context;
    using GreenPipes;
    using Scoping;
    using Scoping.ConsumerContexts;


    public class AutofacConsumerScopeProvider :
        IConsumerScopeProvider
    {
        readonly Action<ContainerBuilder, ConsumeContext> _configureScope;
        readonly string _name;
        readonly ILifetimeScopeProvider _scopeProvider;

        public AutofacConsumerScopeProvider(ILifetimeScopeProvider scopeProvider, string name, Action<ContainerBuilder, ConsumeContext> configureScope)
        {
            _scopeProvider = scopeProvider;
            _name = name;
            _configureScope = configureScope;
        }

        IConsumerScopeContext IConsumerScopeProvider.GetScope(ConsumeContext context)
        {
            if (context.TryGetPayload<ILifetimeScope>(out _))
                return new ExistingConsumerScopeContext(context);

            var parentLifetimeScope = _scopeProvider.GetLifetimeScope(context);

            var lifetimeScope = parentLifetimeScope.BeginLifetimeScope(_name, builder =>
            {
                builder.ConfigureScope(context);
                _configureScope?.Invoke(builder, context);
            });

            try
            {
                var proxy = new ConsumeContextScope(context, lifetimeScope);

                return new CreatedConsumerScopeContext<ILifetimeScope>(lifetimeScope, proxy);
            }
            catch
            {
                lifetimeScope.Dispose();

                throw;
            }
        }

        IConsumerScopeContext<TConsumer, T> IConsumerScopeProvider.GetScope<TConsumer, T>(ConsumeContext<T> context)
        {
            if (context.TryGetPayload<ILifetimeScope>(out var existingLifetimeScope))
            {
                ConsumerConsumeContext<TConsumer, T> consumerContext = existingLifetimeScope.GetConsumer<TConsumer, T>(context);

                return new ExistingConsumerScopeContext<TConsumer, T>(consumerContext);
            }

            var parentLifetimeScope = _scopeProvider.GetLifetimeScope(context);

            var lifetimeScope = parentLifetimeScope.BeginLifetimeScope(_name, builder =>
            {
                builder.ConfigureScope(context);
                _configureScope?.Invoke(builder, context);
            });

            try
            {
                ConsumerConsumeContext<TConsumer, T> consumerContext = lifetimeScope.GetConsumerScope<TConsumer, T>(context);

                return new CreatedConsumerScopeContext<ILifetimeScope, TConsumer, T>(lifetimeScope, consumerContext);
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
