namespace MassTransit.Registration
{
    using System;
    using System.Collections.Generic;
    using Automatonymous;
    using Automatonymous.SagaConfigurators;
    using Definition;
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

            IStateMachineActivityFactory activityFactory = configurationServiceProvider.GetRequiredService<IStateMachineActivityFactory>();

            void AddStateMachineActivityFactory(ConsumeContext context)
            {
                context.GetOrAddPayload(() => activityFactory);
            }

            var repositoryFactory = configurationServiceProvider.GetRequiredService<ISagaRepositoryFactory>();
            ISagaRepository<TInstance> repository = repositoryFactory.CreateSagaRepository<TInstance>(AddStateMachineActivityFactory);
            var stateMachineConfigurator = new StateMachineSagaConfigurator<TInstance>(stateMachine, repository, configurator);

            GetSagaDefinition(configurationServiceProvider)
                .Configure(configurator, stateMachineConfigurator);

            foreach (Action<ISagaConfigurator<TInstance>> action in _configureActions)
                action(stateMachineConfigurator);

            configurator.AddEndpointSpecification(stateMachineConfigurator);
        }

        ISagaDefinition ISagaRegistration.GetDefinition(IConfigurationServiceProvider provider) => GetSagaDefinition(provider);

        ISagaDefinition<TInstance> GetSagaDefinition(IConfigurationServiceProvider provider)
        {
            return _definition ?? (_definition = provider.GetService<ISagaDefinition<TInstance>>() ?? new DefaultSagaDefinition<TInstance>());
        }
    }
}
