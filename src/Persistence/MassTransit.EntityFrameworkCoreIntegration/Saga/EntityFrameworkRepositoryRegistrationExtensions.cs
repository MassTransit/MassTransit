namespace MassTransit.EntityFrameworkCoreIntegration.Saga
{
    using System;
    using Configuration;
    using Configurators;
    using MassTransit.Saga;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;


    public static class EntityFrameworkRepositoryRegistrationExtensions
    {
        /// <summary>
        /// Adds a EntityFramework saga repository to the registration
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="configure"></param>
        /// <typeparam name="TSaga"></typeparam>
        /// <returns></returns>
        public static ISagaRegistrationConfigurator<TSaga> EntityFrameworkRepository<TSaga>(this ISagaRegistrationConfigurator<TSaga> configurator,
            Action<IEntityFrameworkSagaRepositoryConfigurator> configure)
            where TSaga : class, ISaga
        {
            var entityFrameworkSagaRepositoryConfigurator = new EntityFrameworkSagaRepositoryConfigurator<TSaga>();

            configure?.Invoke(entityFrameworkSagaRepositoryConfigurator);

            BusConfigurationResult.CompileResults(entityFrameworkSagaRepositoryConfigurator.Validate());

            var factoryMethod = entityFrameworkSagaRepositoryConfigurator.BuildFactoryMethod();

            configurator.Repository(x => x.RegisterFactoryMethod(factoryMethod));

            return configurator;
        }

        /// <summary>
        /// Adds a EntityFramework saga repository to the registration
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="sagaRepository"></param>
        /// <param name="configure"></param>
        /// <param name="configureSagaMapping"></param>
        /// <typeparam name="TSaga"></typeparam>
        /// <returns></returns>
        public static ISagaRegistrationConfigurator<TSaga> EntityFrameworkRepository<TSaga>(this ISagaRegistrationConfigurator<TSaga> configurator,
            IEntityFrameworkSagaRepository sagaRepository, Action<IEntityFrameworkSagaRepositoryConfigurator> configure = null,
            Action<EntityTypeBuilder<TSaga>> configureSagaMapping = null)
            where TSaga : class, ISaga
        {
            var entityFrameworkSagaRepositoryConfigurator = new EntityFrameworkSagaRepositoryConfigurator<TSaga>();

            configure?.Invoke(entityFrameworkSagaRepositoryConfigurator);

            sagaRepository.ConfigureSaga(configureSagaMapping);

            entityFrameworkSagaRepositoryConfigurator.DatabaseFactory(sagaRepository.GetDbContext);

            BusConfigurationResult.CompileResults(entityFrameworkSagaRepositoryConfigurator.Validate());

            var factoryMethod = entityFrameworkSagaRepositoryConfigurator.BuildFactoryMethod();

            configurator.Repository(x => x.RegisterFactoryMethod(factoryMethod));

            return configurator;
        }

        /// <summary>
        /// Create EntityFramework saga repository
        /// </summary>
        /// <param name="registrationConfigurator"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IEntityFrameworkSagaRepository CreateEntityFrameworkSagaRepository(this IRegistrationConfigurator registrationConfigurator,
            Action<DbContextOptionsBuilder> configure)
        {
            if (configure == null)
                throw new ArgumentNullException(nameof(configure));
            var optionsBuilder = EntityFrameworkSagaRepository.CreateOptionsBuilder();
            configure(optionsBuilder);
            return new EntityFrameworkSagaRepository(optionsBuilder.Options);
        }
    }
}
