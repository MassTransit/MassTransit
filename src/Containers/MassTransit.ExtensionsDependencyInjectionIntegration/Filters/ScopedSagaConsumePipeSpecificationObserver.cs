namespace MassTransit.ExtensionsDependencyInjectionIntegration.Filters
{
    using System;
    using Automatonymous;
    using GreenPipes.Specifications;
    using Saga;
    using SagaConfigurators;
    using ScopeProviders;
    using Scoping.Filters;


    public class ScopedSagaConsumePipeSpecificationObserver :
        ISagaConfigurationObserver
    {
        readonly Type _filterType;
        readonly IServiceProvider _serviceProvider;

        public ScopedSagaConsumePipeSpecificationObserver(Type filterType, IServiceProvider serviceProvider)
        {
            _filterType = filterType;
            _serviceProvider = serviceProvider;
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
            var scopeProviderType = typeof(DependencyInjectionConsumeFilterContextScopeProvider<,,>)
                .MakeGenericType(_filterType.MakeGenericType(typeof(TMessage)), typeof(SagaConsumeContext<TSaga, TMessage>), typeof(TMessage));

            var scopeProvider = (IFilterContextScopeProvider<SagaConsumeContext<TSaga, TMessage>>)Activator.CreateInstance(scopeProviderType,
                _serviceProvider);

            var filter = new ScopedFilter<SagaConsumeContext<TSaga, TMessage>>(scopeProvider);
            var specification = new FilterPipeSpecification<SagaConsumeContext<TSaga, TMessage>>(filter);

            configurator.AddPipeSpecification(specification);
        }
    }
}
