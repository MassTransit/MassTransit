namespace MassTransit.Configuration
{
    using System;
    using DependencyInjection;
    using JobService;
    using Metadata;
    using Middleware;
    using Transports;


    public class OutboxConsumePipeSpecificationObserver<TContext> :
        IConsumerConfigurationObserver,
        ISagaConfigurationObserver,
        IOutboxOptionsConfigurator
        where TContext : class
    {
        readonly IReceiveEndpointConfigurator _configurator;
        readonly IRegistrationContext _context;

        public OutboxConsumePipeSpecificationObserver(IReceiveEndpointConfigurator configurator, IRegistrationContext context)
        {
            _configurator = configurator;
            _context = context;

            MessageDeliveryLimit = 1;
            MessageDeliveryTimeout = TimeSpan.FromSeconds(30);
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

            AddScopedFilter<TConsumer, TMessage>(messageConfigurator);
        }

        public int MessageDeliveryLimit { get; set; } = 1;
        public TimeSpan MessageDeliveryTimeout { get; set; } = TimeSpan.FromSeconds(10);

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

            AddScopedFilter<TSaga, TMessage>(messageConfigurator);
        }

        void AddScopedFilter<T, TMessage>(IPipeConfigurator<ConsumeContext<TMessage>> messageConfigurator)
            where T : class
            where TMessage : class
        {
            var scopeProvider = new ConsumeScopeProvider(_context);

            var options = new OutboxConsumeOptions
            {
                ConsumerId = JobMetadataCache<T, TMessage>.GenerateJobTypeId(_configurator.InputAddress.GetEndpointName()),
                ConsumerType = TypeMetadataCache<T>.ShortName,
                MessageDeliveryLimit = MessageDeliveryLimit,
                MessageDeliveryTimeout = MessageDeliveryTimeout
            };

            var filter = new OutboxConsumeFilter<TContext, TMessage>(scopeProvider, options);

            var specification = new FilterPipeSpecification<ConsumeContext<TMessage>>(filter);

            messageConfigurator.AddPipeSpecification(specification);
        }
    }
}
