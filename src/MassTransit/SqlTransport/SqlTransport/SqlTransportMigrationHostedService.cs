namespace MassTransit.SqlTransport
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;


    // TODO use named registration to resolve this one
    public class SqlTransportMigrationHostedService :
        IHostedService
    {
        readonly ILogger<SqlTransportMigrationHostedService> _logger;
        readonly ISqlTransportDatabaseMigrator _migrator;
        readonly SqlTransportMigrationOptions _options;
        readonly SqlTransportOptions _transportOptions;

        public SqlTransportMigrationHostedService(ISqlTransportDatabaseMigrator migrator, ILogger<SqlTransportMigrationHostedService> logger,
            IOptions<SqlTransportMigrationOptions> options, IOptions<SqlTransportOptions> dbOptions)
        {
            _migrator = migrator;
            _logger = logger;
            _options = options.Value;
            _transportOptions = dbOptions.Value;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            if (_options.CreateDatabase)
            {
                _logger.LogInformation("MassTransit DbTransport creating database {Database}", _transportOptions.Database);

                await _migrator.CreateDatabase(_transportOptions, cancellationToken).ConfigureAwait(false);
            }

            if (_options.CreateInfrastructure)
            {
                _logger.LogInformation("MassTransit DbTransport creating infrastructure for database {Database}", _transportOptions.Database);

                await _migrator.CreateInfrastructure(_transportOptions, cancellationToken).ConfigureAwait(false);
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_options.DeleteDatabase)
            {
                _logger.LogInformation("Deleting Database {Database}", _transportOptions.Database);

                await _migrator.DeleteDatabase(_transportOptions, cancellationToken).ConfigureAwait(false);
            }
        }
    }
}
