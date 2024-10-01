namespace MassTransit.Configuration
{
    public partial class StateMachineInterfaceType<TInstance, TData>
    {
        public class StateMachineSagaMessageConnector :
            SagaConnector<TInstance, TData>.SagaMessageConnector
        {
            readonly IFilter<ConsumeContext<TData>> _messageFilter;
            readonly ISagaPolicy<TInstance, TData> _policy;
            readonly SagaFilterFactory<TInstance, TData> _sagaFilterFactory;

            public StateMachineSagaMessageConnector(IFilter<SagaConsumeContext<TInstance, TData>> consumeFilter, ISagaPolicy<TInstance, TData> policy,
                SagaFilterFactory<TInstance, TData> sagaFilterFactory, IFilter<ConsumeContext<TData>> messageFilter, bool configureConsumeTopology)
                : base(consumeFilter)
            {
                ConfigureConsumeTopology = configureConsumeTopology;
                _policy = policy;
                _sagaFilterFactory = sagaFilterFactory;
                _messageFilter = messageFilter;
            }

            protected override bool ConfigureConsumeTopology { get; }

            protected override void ConfigureMessagePipe(IPipeConfigurator<ConsumeContext<TData>> configurator, ISagaRepository<TInstance> repository,
                IPipe<SagaConsumeContext<TInstance, TData>> sagaPipe)
            {
                if (_messageFilter != null)
                    configurator.UseFilter(_messageFilter);

                if (_sagaFilterFactory == null)
                    throw new ConfigurationException($"The event was not properly correlated: {TypeCache<TInstance>.ShortName} - {TypeCache<TData>.ShortName}");

                configurator.UseFilter(_sagaFilterFactory(repository, _policy, sagaPipe));
            }
        }
    }
}
