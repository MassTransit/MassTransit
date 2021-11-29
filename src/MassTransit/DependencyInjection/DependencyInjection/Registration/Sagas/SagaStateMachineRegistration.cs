namespace MassTransit.DependencyInjection.Registration
{
    using System;
    using System.Collections.Generic;
    using Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Transports;


    /// <summary>
    /// A saga state machine represents a state machine and instance, which will use the container to resolve, as well
    /// as the saga repository.
    /// </summary>
    /// <typeparam name="TInstance"></typeparam>
    public class SagaStateMachineRegistration<TInstance> :
        ISagaRegistration
        where TInstance : class, SagaStateMachineInstance
    {
        readonly List<Action<ISagaConfigurator<TInstance>>> _configureActions;
        ISagaDefinition<TInstance> _definition;

        public SagaStateMachineRegistration()
        {
            _configureActions = new List<Action<ISagaConfigurator<TInstance>>>();
        }

        public Type Type => typeof(TInstance);

        public void AddConfigureAction<T>(Action<ISagaConfigurator<T>> configure)
            where T : class, ISaga
        {
            if (configure is Action<ISagaConfigurator<TInstance>> action)
                _configureActions.Add(action);
        }

        public void Configure(IReceiveEndpointConfigurator configurator, IServiceProvider provider)
        {
            var stateMachine = provider.GetRequiredService<SagaStateMachine<TInstance>>();
            var repository = provider.GetRequiredService<ISagaRepository<TInstance>>();

            var decoratorRegistration = provider.GetService<ISagaRepositoryDecoratorRegistration<TInstance>>();
            if (decoratorRegistration != null)
                repository = decoratorRegistration.DecorateSagaRepository(repository);

            var stateMachineConfigurator = new StateMachineSagaConfigurator<TInstance>(stateMachine, repository, configurator);

            GetSagaDefinition(provider)
                .Configure(configurator, stateMachineConfigurator);

            foreach (Action<ISagaConfigurator<TInstance>> action in _configureActions)
                action(stateMachineConfigurator);

            LogContext.Info?.Log("Configured endpoint {Endpoint}, Saga: {SagaType}, State Machine: {StateMachineType}",
                configurator.InputAddress.GetEndpointName(),
                TypeCache<TInstance>.ShortName, TypeCache.GetShortName(stateMachine.GetType()));

            configurator.AddEndpointSpecification(stateMachineConfigurator);
        }

        ISagaDefinition ISagaRegistration.GetDefinition(IServiceProvider provider)
        {
            return GetSagaDefinition(provider);
        }

        ISagaDefinition<TInstance> GetSagaDefinition(IServiceProvider provider)
        {
            if (_definition != null)
                return _definition;

            _definition = provider.GetService<ISagaDefinition<TInstance>>() ?? new DefaultSagaDefinition<TInstance>();

            var endpointDefinition = provider.GetService<IEndpointDefinition<TInstance>>();
            if (endpointDefinition != null)
                _definition.EndpointDefinition = endpointDefinition;

            return _definition;
        }
    }
}
