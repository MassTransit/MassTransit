namespace MassTransit.Azure.Cosmos.Configuration
{
    using System;
    using MassTransit.Saga;
    using Registration;


    public class CosmosSagaRepositoryRegistrationProvider :
        ISagaRepositoryRegistrationProvider
    {
        readonly Action<ICosmosSagaRepositoryConfigurator> _configure;

        public CosmosSagaRepositoryRegistrationProvider(Action<ICosmosSagaRepositoryConfigurator> configure)
        {
            _configure = configure;
        }

        public virtual void Configure<TSaga>(ISagaRegistrationConfigurator<TSaga> configurator)
            where TSaga : class, ISaga
        {
            configurator.CosmosRepository(r => _configure?.Invoke(r));
        }
    }
}
