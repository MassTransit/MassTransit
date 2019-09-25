namespace MassTransit.SagaConfigurators
{
    using Automatonymous;
    using GreenPipes.Util;
    using Saga;


    public class SagaConfigurationObservable :
        Connectable<ISagaConfigurationObserver>,
        ISagaConfigurationObserver
    {
        public void SagaConfigured<TSaga>(ISagaConfigurator<TSaga> configurator)
            where TSaga : class, ISaga
        {
            All(observer =>
            {
                observer.SagaConfigured(configurator);

                return true;
            });
        }

        public void StateMachineSagaConfigured<TInstance>(ISagaConfigurator<TInstance> configurator, SagaStateMachine<TInstance> stateMachine)
            where TInstance : class, ISaga, SagaStateMachineInstance
        {
            All(observer =>
            {
                observer.StateMachineSagaConfigured(configurator, stateMachine);

                return true;
            });
        }

        public void SagaMessageConfigured<TSaga, TMessage>(ISagaMessageConfigurator<TSaga, TMessage> configurator)
            where TSaga : class, ISaga
            where TMessage : class
        {
            All(observer =>
            {
                observer.SagaMessageConfigured(configurator);

                return true;
            });
        }
    }
}
