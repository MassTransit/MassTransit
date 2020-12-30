namespace MassTransit.Azure.Table.Configurators
{
    using System;
    using MassTransit.Saga;
    using Registration;


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
