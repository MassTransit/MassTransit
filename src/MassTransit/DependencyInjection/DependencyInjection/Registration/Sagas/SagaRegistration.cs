namespace MassTransit.DependencyInjection.Registration
{
    using System;
    using System.Collections.Generic;
    using Configuration;
    using Internals;
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
        readonly List<Action<IRegistrationContext, ISagaConfigurator<TSaga>>> _configureActions;
        readonly IContainerSelector _selector;
        ISagaDefinition<TSaga> _definition;

        public SagaRegistration(IContainerSelector selector)
        {
            _selector = selector;
            _configureActions = new List<Action<IRegistrationContext, ISagaConfigurator<TSaga>>>();
            IncludeInConfigureEndpoints = !Type.HasAttribute<ExcludeFromConfigureEndpointsAttribute>();
        }

        public Type Type => typeof(TSaga);

        public bool IncludeInConfigureEndpoints { get; set; }

        void ISagaRegistration.AddConfigureAction<T>(Action<IRegistrationContext, ISagaConfigurator<T>> configure)
        {
            if (configure is Action<IRegistrationContext, ISagaConfigurator<TSaga>> action)
                _configureActions.Add(action);
        }

        void ISagaRegistration.Configure(IReceiveEndpointConfigurator configurator, IRegistrationContext context)
        {
            ISagaRepository<TSaga> repository = new DependencyInjectionSagaRepository<TSaga>(context);

            var decoratorRegistration = context.GetService<ISagaRepositoryDecoratorRegistration<TSaga>>();
            if (decoratorRegistration != null)
                repository = decoratorRegistration.DecorateSagaRepository(repository);

            var sagaConfigurator = new SagaConfigurator<TSaga>(repository, configurator);

            GetSagaDefinition(context)
                .Configure(configurator, sagaConfigurator, context);

            foreach (Action<IRegistrationContext, ISagaConfigurator<TSaga>> action in _configureActions)
                action(context, sagaConfigurator);

            LogContext.Info?.Log("Configured endpoint {Endpoint}, Saga: {SagaType}", configurator.InputAddress.GetEndpointName(),
                TypeCache<TSaga>.ShortName);

            configurator.AddEndpointSpecification(sagaConfigurator);
        }

        ISagaDefinition ISagaRegistration.GetDefinition(IRegistrationContext context)
        {
            return GetSagaDefinition(context);
        }

        ISagaDefinition<TSaga> GetSagaDefinition(IServiceProvider provider)
        {
            if (_definition != null)
                return _definition;

            _definition = _selector.GetDefinition<ISagaDefinition<TSaga>>(provider) ?? new DefaultSagaDefinition<TSaga>();

            IEndpointDefinition<TSaga> endpointDefinition = _selector.GetEndpointDefinition<TSaga>(provider);
            if (endpointDefinition != null)
                _definition.EndpointDefinition = endpointDefinition;

            return _definition;
        }
    }
}
