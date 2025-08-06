namespace MassTransit.Persistence.PostgreSql.Configuration
{
    using System.Data;
    using Components.JobConsumers;
    using Integration.Saga;
    using Microsoft.Extensions.DependencyInjection;
    using Persistence.Configuration;


    public class PostgresJobSagaRepositoryConfigurator : IPostgresJobSagaRepositoryConfigurator,
        ISpecification
    {
        /// <inheritdoc />
        public string? ConnectionString { get; set; }

        /// <inheritdoc />
        public IsolationLevel IsolationLevel { get; set; } = IsolationLevel.RepeatableRead;

        /// <inheritdoc />
        public IPostgresJobSagaRepositoryConfigurator SetConnectionString(string connectionString)
        {
            ConnectionString = connectionString;
            return this;
        }

        /// <inheritdoc />
        public IPostgresJobSagaRepositoryConfigurator SetIsolationLevel(IsolationLevel isolationLevel)
        {
            IsolationLevel = isolationLevel;
            return this;
        }

        /// <inheritdoc />
        public IEnumerable<ValidationResult> Validate()
        {
            if (string.IsNullOrWhiteSpace(ConnectionString))
                yield return this.Failure("ConnectionString must be specified");
        }

        public void Configure(ICustomJobSagaRepositoryConfigurator configurator)
        {
            (configurator as CustomJobSagaRepositoryConfigurator)?.AddCallback(RegisterDependencies);

            configurator.SetJobContextFactory(sp => Task.FromResult(sp.GetRequiredService<DatabaseContext<JobSaga>>()));
            configurator.SetJobTypeContextFactory(sp => Task.FromResult(sp.GetRequiredService<DatabaseContext<JobTypeSaga>>()));
            configurator.SetJobAttemptContextFactory(sp => Task.FromResult(sp.GetRequiredService<DatabaseContext<JobAttemptSaga>>()));
        }

        void RegisterDependencies(IServiceCollection services)
        {
            if (string.IsNullOrEmpty(ConnectionString))
                throw new ArgumentException(nameof(ConnectionString));

            services.AddTransient<DatabaseContext<JobSaga>>(_
                => new JobSagaDatabaseContext(ConnectionString!, IsolationLevel)
            );

            services.AddTransient<DatabaseContext<JobTypeSaga>>(_
                => new JobTypeSagaDatabaseContext(ConnectionString!, IsolationLevel)
            );

            services.AddTransient<DatabaseContext<JobAttemptSaga>>(_
                => new JobAttemptSagaDatabaseContext(ConnectionString!, IsolationLevel)
            );
        }
    }
}
