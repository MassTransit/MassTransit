namespace MassTransit
{
    using System;
    using Configuration;


    public static class DynamoDbSagaRepositoryRegistrationExtensions
    {
        /// <summary>
        /// Adds a DynamoDb saga repository to the registration
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="configure"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static ISagaRegistrationConfigurator<T> DynamoDbRepository<T>(this ISagaRegistrationConfigurator<T> configurator,
            Action<IDynamoDbSagaRepositoryConfigurator<T>> configure = null)
            where T : class, ISagaVersion
        {
            var repositoryConfigurator = new DynamoDbSagaRepositoryConfigurator<T>();

            configure?.Invoke(repositoryConfigurator);

            repositoryConfigurator.Validate().ThrowIfContainsFailure("The DynamoDb saga repository configuration is invalid:");

            configurator.Repository(x => repositoryConfigurator.Register(x));

            return configurator;
        }

        /// <summary>
        /// Use the DynamoDb saga repository for sagas configured by type (without a specific generic call to AddSaga/AddSagaStateMachine)
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="configure"></param>
        public static void SetDynamoDbSagaRepositoryProvider(this IRegistrationConfigurator configurator, Action<IDynamoDbSagaRepositoryConfigurator> configure)
        {
            configurator.SetSagaRepositoryProvider(new DynamoDbSagaRepositoryRegistrationProvider(configure));
        }
    }
}
