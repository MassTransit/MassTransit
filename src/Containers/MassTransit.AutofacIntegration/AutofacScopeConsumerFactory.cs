namespace MassTransit.AutofacIntegration
{
    using System;
    using System.Threading.Tasks;
    using Autofac;
    using GreenPipes;
    using Metadata;


    /// <summary>
    /// Uses a lifetime scope registry to maintain separate containers based on a scopeId that is extracted from the message
    /// </summary>
    /// <typeparam name="TConsumer"></typeparam>
    /// <typeparam name="TId"></typeparam>
    public class AutofacScopeConsumerFactory<TConsumer, TId> :
        IConsumerFactory<TConsumer>
        where TConsumer : class
    {
        readonly Action<ContainerBuilder, ConsumeContext> _configureScope;
        readonly string _name;
        readonly ILifetimeScopeRegistry<TId> _registry;

        public AutofacScopeConsumerFactory(ILifetimeScopeRegistry<TId> registry, string name, Action<ContainerBuilder, ConsumeContext> configureScope)
        {
            _registry = registry;
            _name = name;
            _configureScope = configureScope;
        }

        public Task Send<TMessage>(ConsumeContext<TMessage> context, IPipe<ConsumerConsumeContext<TConsumer, TMessage>> next)
            where TMessage : class
        {
            if (context.TryGetPayload<ILifetimeScope>(out var existingScope))
            {
                ConsumerConsumeContext<TConsumer, TMessage> consumerContext = existingScope.GetConsumer<TConsumer, TMessage>(context);

                return next.Send(consumerContext);
            }

            return SendInScope(context, next);
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateConsumerFactoryScope<TConsumer>("autofac");
            scope.Add("scopeTag", _name);
            scope.Add("scopeType", TypeMetadataCache<TId>.ShortName);
        }

        async Task SendInScope<TMessage>(ConsumeContext<TMessage> context, IPipe<ConsumerConsumeContext<TConsumer, TMessage>> next)
            where TMessage : class
        {
            var scope = _registry.GetLifetimeScope(context);

            await using var consumerScope = scope.BeginLifetimeScope(_name, builder =>
            {
                builder.ConfigureScope(context);
                _configureScope?.Invoke(builder, context);
            });
            ConsumerConsumeContext<TConsumer, TMessage> consumerContext = consumerScope.GetConsumerScope<TConsumer, TMessage>(context);

            await next.Send(consumerContext).ConfigureAwait(false);
        }
    }
}
