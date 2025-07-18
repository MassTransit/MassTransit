namespace MassTransit.Persistence.Configuration
{
    public static class SagaRegistrationConfiguratorExtensions
    {
        /// <summary>
        /// Adds middleware to use custom repositories for sagas.
        /// </summary>
        public static void CustomRepository<TSaga>(this ISagaRegistrationConfigurator<TSaga> sagaRegistration,
            Action<ICustomRepositoryConfigurator<TSaga>> configure)
            where TSaga : class, ISaga
        {
            var configurator = new CustomRepositoryConfigurator<TSaga>();

            configure.Invoke(configurator);

            configurator.Validate().ThrowIfContainsFailure("The saga repository configuration is invalid:");
            sagaRegistration.Repository(configurator.Register);
        }

        /// <summary>
        /// Adds middleware to use custom repositories for Job Consumers.
        /// </summary>
        public static void CustomRepository(this IJobSagaRegistrationConfigurator jobSagaRegistration,
            Action<ICustomJobSagaRepositoryConfigurator> configure)
        {
            var configurator = new CustomJobSagaRepositoryConfigurator();

            configure.Invoke(configurator);

            configurator.Validate().ThrowIfContainsFailure("The job saga repository configuration is invalid:");
            jobSagaRegistration.UseRepositoryRegistrationProvider(configurator);
        }
    }
}
