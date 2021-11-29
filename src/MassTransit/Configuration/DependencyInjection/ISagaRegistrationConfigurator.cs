namespace MassTransit
{
    using System;


    public interface ISagaRegistrationConfigurator<TSaga> :
        ISagaRegistrationConfigurator
        where TSaga : class, ISaga
    {
        new ISagaRegistrationConfigurator<TSaga> Endpoint(Action<IEndpointRegistrationConfigurator> configure);
        ISagaRegistrationConfigurator<TSaga> Repository(Action<ISagaRepositoryRegistrationConfigurator<TSaga>> configure);
    }


    public interface ISagaRegistrationConfigurator
    {
        ISagaRegistrationConfigurator Endpoint(Action<IEndpointRegistrationConfigurator> configure);
    }
}
