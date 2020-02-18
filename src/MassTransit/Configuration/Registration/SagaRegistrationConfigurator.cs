namespace MassTransit.Registration
{
    using System;
    using Definition;
    using Saga;


    public class SagaRegistrationConfigurator<TSaga> :
        ISagaRegistrationConfigurator<TSaga>
        where TSaga : class, ISaga
    {
        readonly IRegistrationConfigurator _configurator;
        readonly ISagaRegistration _registration;
        readonly IContainerRegistrar _registrar;

        public SagaRegistrationConfigurator(IRegistrationConfigurator configurator, ISagaRegistration registration, IContainerRegistrar registrar)
        {
            _configurator = configurator;
            _registration = registration;
            _registrar = registrar;
        }

        public ISagaRegistrationConfigurator<TSaga> Endpoint(Action<ISagaEndpointRegistrationConfigurator<TSaga>> configure)
        {
            var configurator = new SagaEndpointRegistrationConfigurator<TSaga>();

            configure?.Invoke(configurator);

            _configurator.AddEndpoint<SagaEndpointDefinition<TSaga>, TSaga>(configurator.Settings);

            _registrar.RegisterSagaDefinition<EndpointSagaDefinition<TSaga>, TSaga>();

            return this;
        }

        public ISagaRegistrationConfigurator<TSaga> Repository(Action<ISagaRepositoryRegistrationConfigurator<TSaga>> configure)
        {
            var configurator = new SagaRepositoryRegistrationConfigurator<TSaga>(_registrar);

            configure?.Invoke(configurator);

            return this;
        }
    }
}
