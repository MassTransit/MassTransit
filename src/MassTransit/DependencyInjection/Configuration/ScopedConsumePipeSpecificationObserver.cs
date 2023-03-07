namespace MassTransit.Configuration
{
    using System;
    using DependencyInjection;
    using Internals;
    using Middleware;
    using Serialization;


    public class ScopedConsumePipeSpecificationObserver :
        IConsumerConfigurationObserver,
        ISagaConfigurationObserver
    {
        readonly Type _filterType;
        readonly CompositeFilter<Type> _messageTypeFilter;
        readonly IServiceProvider _provider;

        public ScopedConsumePipeSpecificationObserver(Type filterType, IServiceProvider provider, CompositeFilter<Type> messageTypeFilter)
        {
            _filterType = filterType;
            _provider = provider;
            _messageTypeFilter = messageTypeFilter;
            // do not create filters for scheduled/outbox messages
            _messageTypeFilter.Excludes += type => type == typeof(SerializedMessageBody);
        }

        public void ConsumerConfigured<TConsumer>(IConsumerConfigurator<TConsumer> configurator)
            where TConsumer : class
        {
        }

        public void ConsumerMessageConfigured<TConsumer, TMessage>(IConsumerMessageConfigurator<TConsumer, TMessage> configurator)
            where TConsumer : class
            where TMessage : class
        {
            if (!(configurator is IConsumerMessageConfigurator<TMessage> messageConfigurator))
                throw new ConfigurationException($"The scoped filter could not be added: {TypeCache<TConsumer>.ShortName} - {TypeCache<TMessage>.ShortName}");

            AddScopedFilter(messageConfigurator);
        }

        public void SagaConfigured<TSaga>(ISagaConfigurator<TSaga> configurator)
            where TSaga : class, ISaga
        {
        }

        public void StateMachineSagaConfigured<TInstance>(ISagaConfigurator<TInstance> configurator, SagaStateMachine<TInstance> stateMachine)
            where TInstance : class, ISaga, SagaStateMachineInstance
        {
        }

        public void SagaMessageConfigured<TSaga, TMessage>(ISagaMessageConfigurator<TSaga, TMessage> configurator)
            where TSaga : class, ISaga
            where TMessage : class
        {
            if (!(configurator is ISagaMessageConfigurator<TMessage> messageConfigurator))
                throw new ConfigurationException($"The scoped filter could not be added: {TypeCache<TSaga>.ShortName} - {TypeCache<TMessage>.ShortName}");

            AddScopedFilter(messageConfigurator);
        }

        void AddScopedFilter<TMessage>(IPipeConfigurator<ConsumeContext<TMessage>> messageConfigurator)
            where TMessage : class
        {
            if (!_messageTypeFilter.Matches(typeof(TMessage)))
                return;

            var filterType = _filterType.MakeGenericType(typeof(TMessage));

            if (!filterType.HasInterface(typeof(IFilter<ConsumeContext<TMessage>>)))
                throw new ConfigurationException($"The scoped filter must implement {TypeCache<IFilter<ConsumeContext<TMessage>>>.ShortName} ");

            var scopeProvider = new ConsumeScopeProvider(_provider);

            var scopedFilterType = typeof(ScopedConsumeFilter<,>).MakeGenericType(typeof(TMessage), filterType);

            var filter = (IFilter<ConsumeContext<TMessage>>)Activator.CreateInstance(scopedFilterType, scopeProvider);

            var specification = new FilterPipeSpecification<ConsumeContext<TMessage>>(filter);

            messageConfigurator.AddPipeSpecification(specification);
        }
    }
}
