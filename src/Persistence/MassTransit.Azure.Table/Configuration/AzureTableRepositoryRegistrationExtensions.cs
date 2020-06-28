namespace MassTransit
{
    using System;
    using Azure.Table;
    using Azure.Table.Configurators;
    using Configurators;
    using Saga;


    public static class AzureTableRepositoryRegistrationExtensions
    {
        /// <summary>
        /// Adds a Azure Table saga repository to the registration
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="configure"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static ISagaRegistrationConfigurator<T> AzureTableRepository<T>(this ISagaRegistrationConfigurator<T> configurator,
            Action<IAzureTableSagaRepositoryConfigurator<T>> configure = null)
            where T : class, ISaga
        {
            var sagaRepositoryConfigurator = new AzureTableSagaRepositoryConfigurator<T>();

            configure?.Invoke(sagaRepositoryConfigurator);

            BusConfigurationResult.CompileResults(sagaRepositoryConfigurator.Validate());

            configurator.Repository(x => sagaRepositoryConfigurator.Register(x));

            return configurator;
        }
    }
}
