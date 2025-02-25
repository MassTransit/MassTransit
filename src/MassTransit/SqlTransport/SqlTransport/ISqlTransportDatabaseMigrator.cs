namespace MassTransit.SqlTransport
{
    using System.Threading;
    using System.Threading.Tasks;


    public interface ISqlTransportDatabaseMigrator
    {
        Task CreateDatabase(SqlTransportOptions options, CancellationToken cancellationToken = default);
        Task CreateSchemaIfNotExist(SqlTransportOptions options, CancellationToken cancellationToken = default);
        Task CreateInfrastructure(SqlTransportOptions options, CancellationToken cancellationToken = default);
        Task DeleteDatabase(SqlTransportOptions options, CancellationToken cancellationToken = default);
    }
}
