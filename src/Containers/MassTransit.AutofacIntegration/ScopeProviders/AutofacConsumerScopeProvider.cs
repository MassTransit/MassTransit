namespace MassTransit.AutofacIntegration.ScopeProviders
{
    using System;
    using System.Threading.Tasks;
    using Autofac;
    using Context;
    using GreenPipes;
    using Scoping;
    using Scoping.ConsumerContexts;
    using Util;


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

        public ValueTask<IConsumerScopeContext> GetScope(ConsumeContext context)
        {
            if (context.TryGetPayload<ILifetimeScope>(out _))
                return new ValueTask<IConsumerScopeContext>(new ExistingConsumerScopeContext(context));

            var parentLifetimeScope = _scopeProvider.GetLifetimeScope(context);

            var lifetimeScope = parentLifetimeScope.BeginLifetimeScope(_name, builder =>
            {
                builder.ConfigureScope(context);
                _configureScope?.Invoke(builder, context);
            });

            try
            {
                var scopeContext = new ConsumeContextScope(context, lifetimeScope);

                return new ValueTask<IConsumerScopeContext>(new CreatedConsumerScopeContext<ILifetimeScope>(lifetimeScope, scopeContext));
            }
            catch (Exception ex)
            {
                return ex.DisposeAsync<IConsumerScopeContext>(() => lifetimeScope.DisposeAsync());
            }
        }

        public ValueTask<IConsumerScopeContext<TConsumer, T>> GetScope<TConsumer, T>(ConsumeContext<T> context)
            where TConsumer : class
            where T : class
        {
            if (context.TryGetPayload<ILifetimeScope>(out var existingLifetimeScope))
            {
                ConsumerConsumeContext<TConsumer, T> consumerContext = existingLifetimeScope.GetConsumer<TConsumer, T>(context);

                return new ValueTask<IConsumerScopeContext<TConsumer, T>>(new ExistingConsumerScopeContext<TConsumer, T>(consumerContext));
            }

            var parentLifetimeScope = _scopeProvider.GetLifetimeScope(context);

            var lifetimeScope = parentLifetimeScope.BeginLifetimeScope(_name, builder =>
            {
                builder.ConfigureScope(context);
                _configureScope?.Invoke(builder, context);
            });

            try
            {
                ConsumerConsumeContext<TConsumer, T> consumerContext = lifetimeScope.GetConsumerScope<TConsumer, T>();

                return new ValueTask<IConsumerScopeContext<TConsumer, T>>(
                    new CreatedConsumerScopeContext<ILifetimeScope, TConsumer, T>(lifetimeScope, consumerContext));
            }
            catch (Exception ex)
            {
                return ex.DisposeAsync<IConsumerScopeContext<TConsumer, T>>(() => lifetimeScope.DisposeAsync());
            }
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            context.Add("provider", "autofac");
            context.Add("scopeTag", _name);
        }
    }
}
