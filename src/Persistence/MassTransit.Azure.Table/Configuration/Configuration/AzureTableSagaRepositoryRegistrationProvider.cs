namespace MassTransit.Configuration
{
    using System;


    public class AzureTableSagaRepositoryRegistrationProvider :
        ISagaRepositoryRegistrationProvider
    {
        readonly Action<IAzureTableSagaRepositoryConfigurator> _configure;

        public AzureTableSagaRepositoryRegistrationProvider(Action<IAzureTableSagaRepositoryConfigurator> configure)
        {
            _configure = configure;
        }

        public virtual void Configure<TSaga>(ISagaRegistrationConfigurator<TSaga> configurator)
            where TSaga : class, ISaga
        {
            configurator.AzureTableRepository(r => _configure?.Invoke(r));
        }
    }
}
