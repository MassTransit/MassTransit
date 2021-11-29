namespace MassTransit.DependencyInjection.Registration
{
    using System;
    using System.Collections.Generic;
    using Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Transports;


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

        public Type Type => typeof(TSaga);

        void ISagaRegistration.AddConfigureAction<T>(Action<ISagaConfigurator<T>> configure)
        {
            if (configure is Action<ISagaConfigurator<TSaga>> action)
                _configureActions.Add(action);
        }

        void ISagaRegistration.Configure(IReceiveEndpointConfigurator configurator, IServiceProvider provider)
        {
            var repository = provider.GetRequiredService<ISagaRepository<TSaga>>();

            var decoratorRegistration = provider.GetService<ISagaRepositoryDecoratorRegistration<TSaga>>();
            if (decoratorRegistration != null)
                repository = decoratorRegistration.DecorateSagaRepository(repository);

            var sagaConfigurator = new SagaConfigurator<TSaga>(repository, configurator);

            GetSagaDefinition(provider)
                .Configure(configurator, sagaConfigurator);

            foreach (Action<ISagaConfigurator<TSaga>> action in _configureActions)
                action(sagaConfigurator);

            LogContext.Info?.Log("Configured endpoint {Endpoint}, Saga: {SagaType}", configurator.InputAddress.GetEndpointName(),
                TypeCache<TSaga>.ShortName);

            configurator.AddEndpointSpecification(sagaConfigurator);
        }

        ISagaDefinition ISagaRegistration.GetDefinition(IServiceProvider provider)
        {
            return GetSagaDefinition(provider);
        }

        ISagaDefinition<TSaga> GetSagaDefinition(IServiceProvider provider)
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
