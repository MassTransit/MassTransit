namespace MassTransit
{
    using System;
    using Configuration;


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
            var repositoryConfigurator = new AzureTableSagaRepositoryConfigurator<T>();

            configure?.Invoke(repositoryConfigurator);

            repositoryConfigurator.Validate().ThrowIfContainsFailure("The Azure Table saga repository configuration is invalid:");

            configurator.Repository(x => repositoryConfigurator.Register(x));

            return configurator;
        }

        /// <summary>
        /// Use the Azure Table saga repository for sagas configured by type (without a specific generic call to AddSaga/AddSagaStateMachine)
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="configure"></param>
        public static void SetAzureTableSagaRepositoryProvider(this IRegistrationConfigurator configurator,
            Action<IAzureTableSagaRepositoryConfigurator> configure)
        {
            configurator.SetSagaRepositoryProvider(new AzureTableSagaRepositoryRegistrationProvider(configure));
        }
    }
}
