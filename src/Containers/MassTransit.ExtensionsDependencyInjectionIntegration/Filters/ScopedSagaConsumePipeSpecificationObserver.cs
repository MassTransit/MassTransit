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
            var scopeProviderType = typeof(DependencyInjectionFilterContextScopeProvider<,>)
                .MakeGenericType(_filterType.MakeGenericType(typeof(TMessage)), typeof(ConsumeContext<TMessage>));

            var scopeProvider = (IFilterContextScopeProvider<ConsumeContext<TMessage>>)Activator.CreateInstance(scopeProviderType, _serviceProvider);

            var filter = new ScopedFilter<ConsumeContext<TMessage>>(scopeProvider);
            var specification = new FilterPipeSpecification<ConsumeContext<TMessage>>(filter);

            configurator.Message(m => m.AddPipeSpecification(specification));
        }
    }
}
