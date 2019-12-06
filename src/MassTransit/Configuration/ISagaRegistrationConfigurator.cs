namespace MassTransit
{
    using System;
    using Saga;


    public interface ISagaRegistrationConfigurator<TSaga>
        where TSaga : class, ISaga
    {
        ISagaRegistrationConfigurator<TSaga> Endpoint(Action<ISagaEndpointRegistrationConfigurator<TSaga>> configure);
        ISagaRegistrationConfigurator<TSaga> Repository(Action<ISagaRepositoryRegistrationConfigurator<TSaga>> configure);
    }
}
