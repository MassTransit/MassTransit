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
        readonly IContainerRegistrar _registrar;

        public SagaRegistrationConfigurator(IRegistrationConfigurator configurator, IContainerRegistrar registrar)
        {
            _configurator = configurator;
            _registrar = registrar;
        }

        ISagaRegistrationConfigurator ISagaRegistrationConfigurator.Endpoint(Action<ISagaEndpointRegistrationConfigurator> configure)
        {
            return Endpoint(configure);
        }

        public ISagaRegistrationConfigurator<TSaga> Endpoint(Action<ISagaEndpointRegistrationConfigurator> configure)
        {
            var configurator = new SagaEndpointRegistrationConfigurator<TSaga>();

            configure?.Invoke(configurator);

            _configurator.AddEndpoint<SagaEndpointDefinition<TSaga>, TSaga>(configurator.Settings);

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
