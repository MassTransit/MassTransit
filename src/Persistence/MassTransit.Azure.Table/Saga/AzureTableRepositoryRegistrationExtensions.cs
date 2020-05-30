namespace MassTransit
{
    using System;
    using Azure.Table;
    using Configurators;


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
            where T : class, IVersionedSaga
        {
            var cosmosTableConfigurator = new AzureTableSagaRepositoryConfigurator<T>();

            configure?.Invoke(cosmosTableConfigurator);

            BusConfigurationResult.CompileResults(cosmosTableConfigurator.Validate());

            configurator.Repository(x => cosmosTableConfigurator.Register(x));

            return configurator;
        }
    }
}
