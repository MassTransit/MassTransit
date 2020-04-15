namespace MassTransit.Registration
{
    using System;
    using System.Collections.Generic;
    using Context;
    using Definition;
    using Metadata;
    using Saga;
    using SagaConfigurators;


    /// <summary>
    /// A saga registration represents a single saga, which will use the container for the scope provider, as well as
    /// to resolve the saga repository.
    /// </summary>
    /// <typeparam name="TSaga">The saga type</typeparam>
    public class SagaRegistration<TSaga> :
        ISagaRegistration
        where TSaga : class, ISaga
    {
        readonly List<Action<ISagaConfigurator<TSaga>>> _configureActions;
        ISagaDefinition<TSaga> _definition;

        public SagaRegistration()
        {
            _configureActions = new List<Action<ISagaConfigurator<TSaga>>>();
        }

        void ISagaRegistration.AddConfigureAction<T>(Action<ISagaConfigurator<T>> configure)
        {
            if (configure is Action<ISagaConfigurator<TSaga>> action)
                _configureActions.Add(action);
        }

        void ISagaRegistration.Configure(IReceiveEndpointConfigurator configurator, IConfigurationServiceProvider configurationServiceProvider)
        {
            var repositoryFactory = configurationServiceProvider.GetRequiredService<ISagaRepositoryFactory>();
            ISagaRepository<TSaga> sagaRepository = repositoryFactory.CreateSagaRepository<TSaga>();
            var sagaConfigurator = new SagaConfigurator<TSaga>(sagaRepository, configurator);

            LogContext.Debug?.Log("Configuring endpoint {Endpoint}, Saga: {SagaType}", configurator.InputAddress.GetLastPart(), TypeMetadataCache<TSaga>.ShortName);

            GetSagaDefinition(configurationServiceProvider)
                .Configure(configurator, sagaConfigurator);

            foreach (Action<ISagaConfigurator<TSaga>> action in _configureActions)
                action(sagaConfigurator);

            configurator.AddEndpointSpecification(sagaConfigurator);
        }

        ISagaDefinition ISagaRegistration.GetDefinition(IConfigurationServiceProvider provider)
        {
            return GetSagaDefinition(provider);
        }

        ISagaDefinition<TSaga> GetSagaDefinition(IConfigurationServiceProvider provider)
        {
            if (_definition != null)
                return _definition;

            _definition = provider.GetService<ISagaDefinition<TSaga>>() ?? new DefaultSagaDefinition<TSaga>();

            var endpointDefinition = provider.GetService<IEndpointDefinition<TSaga>>();
            if (endpointDefinition != null)
                _definition.EndpointDefinition = endpointDefinition;

            return _definition;
        }
    }
}
