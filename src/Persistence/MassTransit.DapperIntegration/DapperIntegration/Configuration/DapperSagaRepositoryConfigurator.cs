namespace MassTransit.DapperIntegration.Configuration
{
    using System.Collections.Generic;
    using System.Data;
    using MassTransit.Configuration;
    using MassTransit.Saga;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Saga;


    public class DapperSagaRepositoryConfigurator<TSaga> :
        IDapperSagaRepositoryConfigurator<TSaga>,
        ISpecification
        where TSaga : class, ISaga
    {
        readonly string _connectionString;

        public DapperSagaRepositoryConfigurator(string connectionString, IsolationLevel isolationLevel = IsolationLevel.Serializable)
        {
            _connectionString = connectionString;

            IsolationLevel = isolationLevel;
        }

        public IsolationLevel IsolationLevel { get; set; }

        public IEnumerable<ValidationResult> Validate()
        {
            if (string.IsNullOrWhiteSpace(_connectionString))
                yield return this.Failure("ConnectionString", "must be specified");
        }

        public void Register<T>(ISagaRepositoryRegistrationConfigurator<T> configurator)
            where T : class, ISaga
        {
            configurator.TryAddSingleton(new DapperOptions<T>(_connectionString, IsolationLevel));
            configurator.RegisterSagaRepository<T, DatabaseContext<T>, SagaConsumeContextFactory<DatabaseContext<T>, T>,
                DapperSagaRepositoryContextFactory<T>>();
        }
    }
}
