namespace MassTransit.Configuration
{
    using System;


    public class InMemoryOutboxSagaConfigurationObserver<TSaga> :
        ISagaConfigurationObserver
        where TSaga : class, ISaga
    {
        readonly ISagaConfigurator<TSaga> _configurator;
        readonly Action<IOutboxConfigurator> _configure;
        readonly ISetScopedConsumeContext _setter;

        public InMemoryOutboxSagaConfigurationObserver(IRegistrationContext context, ISagaConfigurator<TSaga> configurator,
            Action<IOutboxConfigurator> configure)
            : this(context as ISetScopedConsumeContext ?? throw new ArgumentException(nameof(context)), configurator, configure)
        {
        }

        public InMemoryOutboxSagaConfigurationObserver(ISetScopedConsumeContext setter, ISagaConfigurator<TSaga> configurator,
            Action<IOutboxConfigurator> configure)
        {
            _setter = setter;
            _configurator = configurator;
            _configure = configure;
        }

        void ISagaConfigurationObserver.SagaConfigured<T>(ISagaConfigurator<T> configurator)
        {
        }

        public void StateMachineSagaConfigured<TInstance>(ISagaConfigurator<TInstance> configurator, SagaStateMachine<TInstance> stateMachine)
            where TInstance : class, ISaga, SagaStateMachineInstance
        {
        }

        void ISagaConfigurationObserver.SagaMessageConfigured<T, TMessage>(ISagaMessageConfigurator<T, TMessage> configurator)
        {
            var specification = new InMemoryOutboxSpecification<TMessage>(_setter);

            _configure?.Invoke(specification);

            _configurator.Message<TMessage>(x => x.AddPipeSpecification(specification));
        }
    }
}
