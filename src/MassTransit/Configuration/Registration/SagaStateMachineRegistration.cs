namespace MassTransit.Registration
{
    using System;
    using System.Collections.Generic;
    using Automatonymous;
    using Automatonymous.SagaConfigurators;
    using Context;
    using Definition;
    using Metadata;
    using Saga;


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

        public void AddConfigureAction<T>(Action<ISagaConfigurator<T>> configure)
            where T : class, ISaga
        {
            if (configure is Action<ISagaConfigurator<TInstance>> action)
                _configureActions.Add(action);
        }

        public void Configure(IReceiveEndpointConfigurator configurator, IConfigurationServiceProvider configurationServiceProvider)
        {
            ISagaStateMachineFactory stateMachineFactory = configurationServiceProvider.GetRequiredService<ISagaStateMachineFactory>();
            SagaStateMachine<TInstance> stateMachine = stateMachineFactory.CreateStateMachine<TInstance>();

            var repositoryFactory = configurationServiceProvider.GetRequiredService<ISagaRepositoryFactory>();
            ISagaRepository<TInstance> repository = repositoryFactory.CreateSagaRepository<TInstance>();
            var stateMachineConfigurator = new StateMachineSagaConfigurator<TInstance>(stateMachine, repository, configurator);

            LogContext.Debug?.Log("Configuring endpoint {Endpoint}, Saga: {SagaType}, State Machine: {StateMachineType}",
                configurator.InputAddress.GetLastPart(),
                TypeMetadataCache<TInstance>.ShortName, TypeMetadataCache.GetShortName(stateMachine.GetType()));

            GetSagaDefinition(configurationServiceProvider)
                .Configure(configurator, stateMachineConfigurator);

            foreach (Action<ISagaConfigurator<TInstance>> action in _configureActions)
                action(stateMachineConfigurator);

            configurator.AddEndpointSpecification(stateMachineConfigurator);
        }

        ISagaDefinition ISagaRegistration.GetDefinition(IConfigurationServiceProvider provider) => GetSagaDefinition(provider);

        ISagaDefinition<TInstance> GetSagaDefinition(IConfigurationServiceProvider provider)
        {
            return _definition ??= provider.GetService<ISagaDefinition<TInstance>>() ?? new DefaultSagaDefinition<TInstance>();
        }
    }
}
