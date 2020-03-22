namespace MassTransit
{
    using System;
    using Configurators;
    using EntityFrameworkCoreIntegration;
    using EntityFrameworkCoreIntegration.Configurators;
    using EntityFrameworkCoreIntegration.Mappings;
    using EntityFrameworkCoreIntegration.Saga.Configuration;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Saga;


    public static class EntityFrameworkCoreSagaRepositoryRegistrationExtensions
    {
        /// <summary>
        /// Adds a EntityFramework saga repository to the registration
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="configure"></param>
        /// <typeparam name="TSaga"></typeparam>
        /// <returns></returns>
        public static ISagaRegistrationConfigurator<TSaga> EntityFrameworkRepository<TSaga>(this ISagaRegistrationConfigurator<TSaga> configurator,
            Action<IEntityFrameworkSagaRepositoryConfigurator<TSaga>> configure)
            where TSaga : class, ISaga
        {
            var entityFrameworkSagaRepositoryConfigurator = new EntityFrameworkSagaRepositoryConfigurator<TSaga>();

            configure?.Invoke(entityFrameworkSagaRepositoryConfigurator);

            BusConfigurationResult.CompileResults(entityFrameworkSagaRepositoryConfigurator.Validate());

            configurator.Repository(x => entityFrameworkSagaRepositoryConfigurator.Register(x));

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
            IEntityFrameworkSagaRepository sagaRepository, Action<IEntityFrameworkSagaRepositoryConfigurator<TSaga>> configure = null,
            Action<EntityTypeBuilder<TSaga>> configureSagaMapping = null)
            where TSaga : class, ISaga
        {
            return configurator.EntityFrameworkRepository(sagaRepository, configure, new ActionSagaClassMap<TSaga>(configureSagaMapping));
        }

        /// <summary>
        /// Adds a EntityFramework saga repository to the registration
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="sagaRepository"></param>
        /// <param name="configure"></param>
        /// <param name="sagaClassMap"></param>
        /// <typeparam name="TSaga"></typeparam>
        /// <returns></returns>
        public static ISagaRegistrationConfigurator<TSaga> EntityFrameworkRepository<TSaga>(this ISagaRegistrationConfigurator<TSaga> configurator,
            IEntityFrameworkSagaRepository sagaRepository, Action<IEntityFrameworkSagaRepositoryConfigurator<TSaga>> configure = null,
            ISagaClassMap<TSaga> sagaClassMap = null)
            where TSaga : class, ISaga
        {
            sagaRepository.AddSagaClassMap(sagaClassMap ?? new ActionSagaClassMap<TSaga>());
            return configurator.EntityFrameworkRepository(cfg =>
            {
                cfg.DatabaseFactory(sagaRepository.GetDbContext);

                configure?.Invoke(cfg);
            });
        }

        /// <summary>
        /// Configure the repository for use with SQL Server
        /// </summary>
        /// <param name="configurator"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEntityFrameworkSagaRepositoryConfigurator<T> UseSqlServer<T>(this IEntityFrameworkSagaRepositoryConfigurator<T> configurator)
            where T : class, ISaga
        {
            configurator.LockStatementProvider = new SqlServerLockStatementProvider();

            return configurator;
        }

        /// <summary>
        /// Configure the repository for use with Postgres
        /// </summary>
        /// <param name="configurator"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEntityFrameworkSagaRepositoryConfigurator<T> UsePostgres<T>(this IEntityFrameworkSagaRepositoryConfigurator<T> configurator)
            where T : class, ISaga
        {
            configurator.LockStatementProvider = new PostgresLockStatementProvider();

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


        class ActionSagaClassMap<T> : SagaClassMap<T>
            where T : class, ISaga
        {
            readonly Action<EntityTypeBuilder<T>> _configure;

            public ActionSagaClassMap(Action<EntityTypeBuilder<T>> configure = null)
            {
                _configure = configure;
            }

            protected override void Configure(EntityTypeBuilder<T> entity, ModelBuilder model)
            {
                base.Configure(entity, model);
                _configure?.Invoke(entity);
            }
        }
    }
}
