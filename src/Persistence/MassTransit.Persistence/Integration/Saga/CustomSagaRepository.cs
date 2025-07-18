namespace MassTransit.Persistence.Integration.Saga
{
    using Configuration;
    using DependencyInjection.Registration;
    using MassTransit.Saga;
    using Microsoft.Extensions.DependencyInjection;


    /// <summary>
    /// Thin wrapper over <seealso cref="SagaDatabaseContext{TSaga}" /> to
    /// allow creation of a standalone repository rather than registering it.
    /// </summary>
    /// <typeparam name="TSaga"></typeparam>
    public class CustomSagaRepository<TSaga>
        where TSaga : class, ISaga
    {
        CustomSagaRepository()
        {
        }

        /// <summary>
        /// Creates a saga repository directly, rather than using a DI container.
        /// </summary>
        public static ISagaRepository<TSaga> Create(Action<ICustomRepositoryConfigurator<TSaga>> configure, IServiceProvider? provider = null)
        {
            var configurator = new CustomRepositoryConfigurator<TSaga>();

            configure(configurator);
            configurator.Validate().ThrowIfContainsFailure("Saga repository configuration is invalid:");

            if (provider is null)
            {
                IServiceCollection services = new ServiceCollection();
                var registrationConfigurator = new SagaRepositoryRegistrationConfigurator<TSaga>(services);

                configurator.Register(registrationConfigurator);
                provider = services.BuildServiceProvider();
            }

            var consumeContextFactory = new SagaConsumeContextFactory<DatabaseContext<TSaga>, TSaga>();

            var repositoryContextFactory = new CustomSagaRepositoryContextFactory<TSaga>(
                consumeContextFactory,
                provider
            );

            return new SagaRepository<TSaga>(
                repositoryContextFactory,
                repositoryContextFactory,
                repositoryContextFactory
            );
        }
    }
}
