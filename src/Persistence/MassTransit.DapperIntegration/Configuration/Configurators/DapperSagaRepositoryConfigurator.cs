namespace MassTransit.DapperIntegration.Configurators
{
    using System.Collections.Generic;
    using System.Data;
    using Context;
    using GreenPipes;
    using Saga;


    public class DapperSagaRepositoryConfigurator<TSaga> :
        IDapperSagaRepositoryConfigurator<TSaga>,
        ISpecification
        where TSaga : class, ISaga
    {
        readonly string _connectionString;
        IsolationLevel _isolationLevel;

        public DapperSagaRepositoryConfigurator(string connectionString, IsolationLevel isolationLevel = IsolationLevel.Serializable)
        {
            _connectionString = connectionString;
            _isolationLevel = isolationLevel;
        }

        IsolationLevel IDapperSagaRepositoryConfigurator<TSaga>.IsolationLevel
        {
            set => _isolationLevel = value;
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (string.IsNullOrWhiteSpace(_connectionString))
                yield return this.Failure("ConnectionString", "must be specified");
        }

        public void Register<T>(ISagaRepositoryRegistrationConfigurator<T> configurator)
            where T : class, ISaga
        {
            configurator.RegisterSingleInstance(new DapperOptions<T>(_connectionString, _isolationLevel));
            configurator.RegisterSagaRepository<T, DatabaseContext<T>, DapperSagaConsumeContextFactory<T>, DapperSagaRepositoryContextFactory<T>>();
        }
    }
}
