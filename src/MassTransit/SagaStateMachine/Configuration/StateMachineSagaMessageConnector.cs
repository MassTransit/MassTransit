namespace MassTransit.Configuration
{
    public class StateMachineSagaMessageConnector<TInstance, TMessage> :
        SagaMessageConnector<TInstance, TMessage>
        where TInstance : class, ISaga, SagaStateMachineInstance
        where TMessage : class
    {
        readonly IFilter<ConsumeContext<TMessage>> _messageFilter;
        readonly ISagaPolicy<TInstance, TMessage> _policy;
        readonly SagaFilterFactory<TInstance, TMessage> _sagaFilterFactory;

        public StateMachineSagaMessageConnector(IFilter<SagaConsumeContext<TInstance, TMessage>> consumeFilter, ISagaPolicy<TInstance, TMessage> policy,
            SagaFilterFactory<TInstance, TMessage> sagaFilterFactory, IFilter<ConsumeContext<TMessage>> messageFilter, bool configureConsumeTopology)
            : base(consumeFilter)
        {
            ConfigureConsumeTopology = configureConsumeTopology;
            _policy = policy;
            _sagaFilterFactory = sagaFilterFactory;
            _messageFilter = messageFilter;
        }

        protected override bool ConfigureConsumeTopology { get; }

        protected override void ConfigureMessagePipe(IPipeConfigurator<ConsumeContext<TMessage>> configurator, ISagaRepository<TInstance> repository,
            IPipe<SagaConsumeContext<TInstance, TMessage>> sagaPipe)
        {
            if (_messageFilter != null)
                configurator.UseFilter(_messageFilter);

            configurator.UseFilter(_sagaFilterFactory(repository, _policy, sagaPipe));
        }
    }
}
