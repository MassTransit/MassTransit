namespace MassTransit.DependencyInjection.Registration
{
    using System;
    using System.Collections.Generic;
    using Configuration;
    using Internals;
    using Microsoft.Extensions.DependencyInjection;
    using Transports;


    /// <summary>
    /// A saga state machine represents a state machine and instance, which will use the container to resolve, as well
    /// as the saga repository.
    /// </summary>
    /// <typeparam name="TStateMachine"></typeparam>
    /// <typeparam name="TInstance"></typeparam>
    public class SagaStateMachineRegistration<TStateMachine, TInstance> :
        ISagaRegistration
        where TStateMachine : class, SagaStateMachine<TInstance>
        where TInstance : class, SagaStateMachineInstance
    {
        readonly List<Action<ISagaConfigurator<TInstance>>> _configureActions;
        ISagaDefinition<TInstance> _definition;

        public SagaStateMachineRegistration()
        {
            _configureActions = new List<Action<ISagaConfigurator<TInstance>>>();
            IncludeInConfigureEndpoints = !Type.HasAttribute<ExcludeFromConfigureEndpointsAttribute>();
        }

        public Type Type => typeof(TInstance);

        public bool IncludeInConfigureEndpoints { get; set; }

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

            IEnumerable<IEventObserver<TInstance>> eventObservers = provider.GetServices<IEventObserver<TInstance>>();
            foreach (IEventObserver<TInstance> eventObserver in eventObservers)
                stateMachine.ConnectEventObserver(eventObserver);

            IEnumerable<IStateObserver<TInstance>> stateObservers = provider.GetServices<IStateObserver<TInstance>>();
            foreach (IStateObserver<TInstance> stateObserver in stateObservers)
                stateMachine.ConnectStateObserver(stateObserver);

            LogContext.Info?.Log("Configured endpoint {Endpoint}, Saga: {SagaType}, State Machine: {StateMachineType}",
                configurator.InputAddress.GetEndpointName(),
                TypeCache<TInstance>.ShortName, TypeCache.GetShortName(stateMachine.GetType()));

            configurator.AddEndpointSpecification(stateMachineConfigurator);

            IncludeInConfigureEndpoints = false;
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
