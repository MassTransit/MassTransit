namespace MassTransit.DependencyInjection.Registration
{
    using System;
    using Configuration;


    public class SagaRegistrationConfigurator<TSaga> :
        ISagaRegistrationConfigurator<TSaga>
        where TSaga : class, ISaga
    {
        readonly IRegistrationConfigurator _configurator;

        public SagaRegistrationConfigurator(IRegistrationConfigurator configurator)
        {
            _configurator = configurator;
        }

        ISagaRegistrationConfigurator ISagaRegistrationConfigurator.Endpoint(Action<IEndpointRegistrationConfigurator> configure)
        {
            return Endpoint(configure);
        }

        public ISagaRegistrationConfigurator<TSaga> Endpoint(Action<IEndpointRegistrationConfigurator> configure)
        {
            var configurator = new EndpointRegistrationConfigurator<TSaga>();

            configure?.Invoke(configurator);

            _configurator.AddEndpoint<SagaEndpointDefinition<TSaga>, TSaga>(configurator.Settings);

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
