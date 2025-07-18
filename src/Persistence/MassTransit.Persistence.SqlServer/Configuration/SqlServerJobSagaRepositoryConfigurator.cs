namespace MassTransit.Persistence.SqlServer.Configuration
{
    using System.Data;
    using Components.JobConsumers;
    using Integration.Saga;
    using Microsoft.Extensions.DependencyInjection;
    using Persistence.Configuration;


    public class SqlServerJobSagaRepositoryConfigurator : ISqlServerJobSagaRepositoryConfigurator,
        ISpecification
    {
        public IEnumerable<ValidationResult> Validate()
        {
            if (string.IsNullOrWhiteSpace(ConnectionString))
                yield return this.Failure("ConnectionString must be specified");
        }

        /// <inheritdoc />
        public string? ConnectionString { get; set; }

        /// <inheritdoc />
        public IsolationLevel IsolationLevel { get; set; } = IsolationLevel.RepeatableRead;

        /// <inheritdoc />
        public ISqlServerJobSagaRepositoryConfigurator SetConnectionString(string connectionString)
        {
            ConnectionString = connectionString;
            return this;
        }

        /// <inheritdoc />
        public ISqlServerJobSagaRepositoryConfigurator SetIsolationLevel(IsolationLevel isolationLevel)
        {
            IsolationLevel = isolationLevel;
            return this;
        }

        public void Configure(ICustomJobSagaRepositoryConfigurator configurator)
        {
            (configurator as CustomJobSagaRepositoryConfigurator)?.AddCallback(RegisterDependencies);

            configurator.SetJobContextFactory(sp =>
            {
                var service = sp.GetRequiredService<DatabaseContext<JobSaga>>();
                return Task.FromResult(service);
            });

            configurator.SetJobTypeContextFactory(sp =>
            {
                var service = sp.GetRequiredService<DatabaseContext<JobTypeSaga>>();
                return Task.FromResult(service);
            });

            configurator.SetJobAttemptContextFactory(sp =>
            {
                var service = sp.GetRequiredService<DatabaseContext<JobAttemptSaga>>();
                return Task.FromResult(service);
            });
        }

        void RegisterDependencies(IServiceCollection services)
        {
            if (string.IsNullOrEmpty(ConnectionString))
                throw new ArgumentException(nameof(ConnectionString));

            services.AddScoped<DatabaseContext<JobSaga>>(_
                => new JobSagaDatabaseContext(ConnectionString!, IsolationLevel)
            );

            services.AddScoped<DatabaseContext<JobTypeSaga>>(_
                => new JobTypeSagaDatabaseContext(ConnectionString!, IsolationLevel)
            );

            services.AddScoped<DatabaseContext<JobAttemptSaga>>(_
                => new JobAttemptSagaDatabaseContext(ConnectionString!, IsolationLevel)
            );
        }
    }
}
