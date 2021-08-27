using MassTransit.Context;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace MassTransit.Transports.OnRamp.Repositories
{
    public class SqlRepositoryInitializer : IRepositoryInitializer
    {
        public SqlRepositoryInitializer(DbConnection connection, IRepositoryNamingProvider repositoryNamingProvider)
        {
            _connection = connection;
            _repositoryNamingProvider = repositoryNamingProvider;
        }

        private readonly DbConnection _connection;
        private readonly IRepositoryNamingProvider _repositoryNamingProvider;

        public async Task InitializeAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested) return;

            await _connection.ExecuteNonQueryAsync(_repositoryNamingProvider.GetInitializationScript());

            LogContext.Debug?.Log("Ensuring all create database tables script are applied.");
        }
    }
}
