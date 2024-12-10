namespace MassTransit
{
    using System;
    using Configuration;
    using EntityFrameworkCoreIntegration;
    using EntityFrameworkCoreIntegration.Configuration;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;


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
            var repositoryConfigurator = new EntityFrameworkSagaRepositoryConfigurator<TSaga>();

            configure?.Invoke(repositoryConfigurator);

            repositoryConfigurator.Validate().ThrowIfContainsFailure("The Entity Framework saga repository configuration is invalid:");

            configurator.Repository(x => repositoryConfigurator.Register(x));

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
        /// Configure the Job Service saga state machines to use Entity Framework Core as the saga repository
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public static IJobSagaRegistrationConfigurator EntityFrameworkRepository(this IJobSagaRegistrationConfigurator configurator,
            Action<IEntityFrameworkSagaRepositoryConfigurator> configure = null)
        {
            var registrationProvider = new EntityFrameworkSagaRepositoryRegistrationProvider(configure);

            configurator.UseRepositoryRegistrationProvider(registrationProvider);

            return configurator;
        }

        /// <summary>
        /// Use the EntityFramework saga repository for sagas configured by type (without a specific generic call to AddSaga/AddSagaStateMachine)
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="configure"></param>
        public static void SetEntityFrameworkSagaRepositoryProvider(this IRegistrationConfigurator configurator,
            Action<IEntityFrameworkSagaRepositoryConfigurator> configure)
        {
            configurator.SetSagaRepositoryProvider(new EntityFrameworkSagaRepositoryRegistrationProvider(configure));
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
        /// Configure the repository for use with SQL Server
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="schemaName">The schema name to use if the table schema cannot be discovered</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEntityFrameworkSagaRepositoryConfigurator<T> UseSqlServer<T>(this IEntityFrameworkSagaRepositoryConfigurator<T> configurator,
            string schemaName)
            where T : class, ISaga
        {
            if (schemaName == null)
                throw new ArgumentNullException(nameof(schemaName));

            configurator.LockStatementProvider = new SqlServerLockStatementProvider(schemaName);

            return configurator;
        }

        /// <summary>
        /// Configure the repository for use with SQL Server
        /// </summary>
        /// <param name="configurator"></param>
        /// <returns></returns>
        public static IEntityFrameworkSagaRepositoryConfigurator UseSqlServer(this IEntityFrameworkSagaRepositoryConfigurator configurator)
        {
            configurator.LockStatementProvider = new SqlServerLockStatementProvider();

            return configurator;
        }

        /// <summary>
        /// Configure the repository for use with SQL Server
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="schemaName">The schema name to use if the table schema cannot be discovered</param>
        /// <returns></returns>
        public static IEntityFrameworkSagaRepositoryConfigurator UseSqlServer(this IEntityFrameworkSagaRepositoryConfigurator configurator,
            string schemaName)
        {
            if (schemaName == null)
                throw new ArgumentNullException(nameof(schemaName));

            configurator.LockStatementProvider = new SqlServerLockStatementProvider(schemaName);

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
        /// Configure the repository for use with Postgres
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="schemaName">The schema name to use if the table schema cannot be discovered</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEntityFrameworkSagaRepositoryConfigurator<T> UsePostgres<T>(this IEntityFrameworkSagaRepositoryConfigurator<T> configurator,
            string schemaName)
            where T : class, ISaga
        {
            if (schemaName == null)
                throw new ArgumentNullException(nameof(schemaName));

            configurator.LockStatementProvider = new PostgresLockStatementProvider(schemaName);

            return configurator;
        }

        /// <summary>
        /// Configure the repository for use with Postgres
        /// </summary>
        /// <param name="configurator"></param>
        /// <returns></returns>
        public static IEntityFrameworkSagaRepositoryConfigurator UsePostgres(this IEntityFrameworkSagaRepositoryConfigurator configurator)
        {
            configurator.LockStatementProvider = new PostgresLockStatementProvider();

            return configurator;
        }

        /// <summary>
        /// Configure the repository for use with Postgres
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="schemaName">The schema name to use if the table schema cannot be discovered</param>
        /// <returns></returns>
        public static IEntityFrameworkSagaRepositoryConfigurator UsePostgres(this IEntityFrameworkSagaRepositoryConfigurator configurator,
            string schemaName)
        {
            if (schemaName == null)
                throw new ArgumentNullException(nameof(schemaName));

            configurator.LockStatementProvider = new PostgresLockStatementProvider(schemaName);

            return configurator;
        }

        /// <summary>
        /// Configure the repository for use with MySQL
        /// </summary>
        /// <param name="configurator"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEntityFrameworkSagaRepositoryConfigurator<T> UseMySql<T>(this IEntityFrameworkSagaRepositoryConfigurator<T> configurator)
            where T : class, ISaga
        {
            configurator.LockStatementProvider = new MySqlLockStatementProvider();

            return configurator;
        }

        /// <summary>
        /// Configure the repository for use with MySQL
        /// </summary>
        /// <param name="configurator"></param>
        /// <returns></returns>
        public static IEntityFrameworkSagaRepositoryConfigurator UseMySql(this IEntityFrameworkSagaRepositoryConfigurator configurator)
        {
            configurator.LockStatementProvider = new MySqlLockStatementProvider();

            return configurator;
        }

        /// <summary>
        /// Configure the repository for use with SQLite
        /// </summary>
        /// <param name="configurator"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEntityFrameworkSagaRepositoryConfigurator<T> UseSqlite<T>(this IEntityFrameworkSagaRepositoryConfigurator<T> configurator)
            where T : class, ISaga
        {
            configurator.LockStatementProvider = new SqliteLockStatementProvider();

            return configurator;
        }

        /// <summary>
        /// Configure the repository for use with SQLite
        /// </summary>
        /// <param name="configurator"></param>
        /// <returns></returns>
        public static IEntityFrameworkSagaRepositoryConfigurator UseSqlite(this IEntityFrameworkSagaRepositoryConfigurator configurator)
        {
            configurator.LockStatementProvider = new SqliteLockStatementProvider();

            return configurator;
        }

        /// <summary>
        /// Configure the repository for use with Oracle
        /// </summary>
        /// <param name="configurator"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEntityFrameworkSagaRepositoryConfigurator<T> UseOracle<T>(this IEntityFrameworkSagaRepositoryConfigurator<T> configurator)
            where T : class, ISaga
        {
            configurator.LockStatementProvider = new OracleLockStatementProvider();

            return configurator;
        }

        /// <summary>
        /// Configure the repository for use with Oracle
        /// </summary>
        /// <param name="configurator"></param>
        /// <returns></returns>
        public static IEntityFrameworkSagaRepositoryConfigurator UseOracle(this IEntityFrameworkSagaRepositoryConfigurator configurator)
        {
            configurator.LockStatementProvider = new OracleLockStatementProvider();

            return configurator;
        }

        /// <summary>
        /// Create EntityFramework saga repository
        /// </summary>
        /// <param name="registrationConfigurator"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        [Obsolete("This will be removed in a future release")]
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
