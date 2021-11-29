namespace MassTransit.Configuration
{
    using System;


    public class TimeoutSagaConfigurationObserver<TSaga> :
        ISagaConfigurationObserver
        where TSaga : class, ISaga
    {
        readonly ISagaConfigurator<TSaga> _configurator;
        readonly Action<ITimeoutConfigurator> _configure;

        public TimeoutSagaConfigurationObserver(ISagaConfigurator<TSaga> configurator, Action<ITimeoutConfigurator> configure)
        {
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
            var specification = new TimeoutSpecification<TMessage>();

            _configure?.Invoke(specification);

            _configurator.Message<TMessage>(x => x.AddPipeSpecification(specification));
        }
    }
}
