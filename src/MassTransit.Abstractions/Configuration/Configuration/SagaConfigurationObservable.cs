namespace MassTransit.Configuration
{
    using Util;


    public class SagaConfigurationObservable :
        Connectable<ISagaConfigurationObserver>,
        ISagaConfigurationObserver
    {
        public void SagaConfigured<TSaga>(ISagaConfigurator<TSaga> configurator)
            where TSaga : class, ISaga
        {
            ForEach(observer => observer.SagaConfigured(configurator));
        }

        public void StateMachineSagaConfigured<TInstance>(ISagaConfigurator<TInstance> configurator, SagaStateMachine<TInstance> stateMachine)
            where TInstance : class, ISaga, SagaStateMachineInstance
        {
            ForEach(observer => observer.StateMachineSagaConfigured(configurator, stateMachine));
        }

        public void SagaMessageConfigured<TSaga, TMessage>(ISagaMessageConfigurator<TSaga, TMessage> configurator)
            where TSaga : class, ISaga
            where TMessage : class
        {
            ForEach(observer => observer.SagaMessageConfigured(configurator));
        }
    }
}
