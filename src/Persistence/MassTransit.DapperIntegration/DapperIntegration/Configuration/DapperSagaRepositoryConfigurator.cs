#nullable enable
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

        public DatabaseContextFactory<TSaga>? ContextFactory { get; set; }

        public IEnumerable<ValidationResult> Validate()
        {
            if (string.IsNullOrWhiteSpace(_connectionString))
                yield return this.Failure("ConnectionString", "must be specified");
        }

        public void Register(ISagaRepositoryRegistrationConfigurator<TSaga> configurator)
        {
            configurator.TryAddSingleton(new DapperOptions<TSaga>(_connectionString, IsolationLevel, ContextFactory));
            configurator.RegisterLoadSagaRepository<TSaga, DapperSagaRepositoryContextFactory<TSaga>>();
            configurator.RegisterQuerySagaRepository<TSaga, DapperSagaRepositoryContextFactory<TSaga>>();
            configurator.RegisterSagaRepository<TSaga, DatabaseContext<TSaga>, SagaConsumeContextFactory<DatabaseContext<TSaga>, TSaga>,
                DapperSagaRepositoryContextFactory<TSaga>>();
        }
    }
}
