namespace MassTransit.DependencyInjection.Registration
{
    using System;
    using Configuration;


    public class SagaRegistrationConfigurator<TSaga> :
        ISagaRegistrationConfigurator<TSaga>
        where TSaga : class, ISaga
    {
        readonly IRegistrationConfigurator _configurator;
        readonly ISagaRegistration _registration;

        public SagaRegistrationConfigurator(IRegistrationConfigurator configurator, ISagaRegistration registration = null)
        {
            _configurator = configurator;
            _registration = registration;
        }

        ISagaRegistrationConfigurator ISagaRegistrationConfigurator.Endpoint(Action<IEndpointRegistrationConfigurator> configure)
        {
            return Endpoint(configure);
        }

        void ISagaRegistrationConfigurator.ExcludeFromConfigureEndpoints()
        {
            if (_registration != null)
                _registration.IncludeInConfigureEndpoints = false;
        }

        public ISagaRegistrationConfigurator<TSaga> Endpoint(Action<IEndpointRegistrationConfigurator> configure)
        {
            if (_registration is { IncludeInConfigureEndpoints: false })
                throw new ConfigurationException("Saga is excluded from ConfigureEndpoints");

            var configurator = new EndpointRegistrationConfigurator<TSaga>();

            configure?.Invoke(configurator);

            _configurator.AddEndpoint<SagaEndpointDefinition<TSaga>, TSaga>(_registration, configurator.Settings);

            return this;
        }

        public ISagaRegistrationConfigurator<TSaga> Repository(Action<ISagaRepositoryRegistrationConfigurator<TSaga>> configure)
        {
            var configurator = new SagaRepositoryRegistrationConfigurator<TSaga>(_configurator);

            configure?.Invoke(configurator);

            return this;
        }
    }
}
