namespace MassTransit
{
    using System;
    using Registration;
    using Saga;


    public interface ISagaRegistrationConfigurator<TSaga> :
        ISagaRegistrationConfigurator
        where TSaga : class, ISaga
    {
        new ISagaRegistrationConfigurator<TSaga> Endpoint(Action<ISagaEndpointRegistrationConfigurator> configure);
        ISagaRegistrationConfigurator<TSaga> Repository(Action<ISagaRepositoryRegistrationConfigurator<TSaga>> configure);
    }


    public interface ISagaRegistrationConfigurator
    {
        ISagaRegistrationConfigurator Endpoint(Action<ISagaEndpointRegistrationConfigurator> configure);
    }
}
