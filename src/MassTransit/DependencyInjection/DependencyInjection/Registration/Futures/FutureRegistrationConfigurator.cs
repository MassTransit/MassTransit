namespace MassTransit.DependencyInjection.Registration
{
    using System;
    using Configuration;


    public class FutureRegistrationConfigurator<TFuture> :
        IFutureRegistrationConfigurator<TFuture>
        where TFuture : class, SagaStateMachine<FutureState>
    {
        readonly IRegistrationConfigurator _configurator;
        readonly IFutureRegistration _registration;

        public FutureRegistrationConfigurator(IRegistrationConfigurator configurator, IFutureRegistration registration)
        {
            _configurator = configurator;
            _registration = registration;
        }

        IFutureRegistrationConfigurator IFutureRegistrationConfigurator.Endpoint(Action<IEndpointRegistrationConfigurator> configure)
        {
            return Endpoint(configure);
        }

        public void ExcludeFromConfigureEndpoints()
        {
            _registration.IncludeInConfigureEndpoints = false;
        }

        public IFutureRegistrationConfigurator<TFuture> Endpoint(Action<IEndpointRegistrationConfigurator> configure)
        {
            if (!_registration.IncludeInConfigureEndpoints)
                throw new ConfigurationException("Feature is excluded from ConfigureEndpoints");

            var configurator = new EndpointRegistrationConfigurator<TFuture>();

            configure?.Invoke(configurator);

            _configurator.AddEndpoint<FutureEndpointDefinition<TFuture>, TFuture>(_registration, configurator.Settings);

            return this;
        }

        public IFutureRegistrationConfigurator<TFuture> Repository(Action<ISagaRepositoryRegistrationConfigurator<FutureState>> configure)
        {
            var configurator = new SagaRepositoryRegistrationConfigurator<FutureState>(_configurator);

            configure?.Invoke(configurator);

            return this;
        }
    }
}
