namespace MassTransit.ExtensionsDependencyInjectionIntegration.Filters
{
    using System;
    using Automatonymous;
    using Saga;
    using SagaConfigurators;
    using ScopeProviders;


    public class ScopedSagaConsumePipeSpecificationObserver :
        ISagaConfigurationObserver
    {
        readonly Type _filterType;
        readonly IServiceProvider _provider;

        public ScopedSagaConsumePipeSpecificationObserver(Type filterType, IServiceProvider provider)
        {
            _filterType = filterType;
            _provider = provider;
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
            configurator.AddScopedFilter<SagaConsumeContext<TSaga, TMessage>, ConsumeContext<TMessage>, TMessage>(_filterType, _provider);
        }
    }
}
