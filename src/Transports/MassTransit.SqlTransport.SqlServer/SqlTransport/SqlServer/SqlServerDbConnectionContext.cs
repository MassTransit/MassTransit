namespace MassTransit.SqlTransport.SqlServer;

using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Configuration;
using Dapper;
using Logging;
using MassTransit.Middleware;
using Microsoft.Data.SqlClient;
using RetryPolicies;
using Transports;
using Util;


public class SqlServerDbConnectionContext :
    BasePipeContext,
    ConnectionContext,
    IAsyncDisposable
{
    readonly TaskExecutor _executor;
    readonly ISqlHostConfiguration _hostConfiguration;
    readonly SqlServerSqlHostSettings _hostSettings;
    readonly IRetryPolicy _retryPolicy;

    static SqlServerDbConnectionContext()
    {
        DefaultTypeMap.MatchNamesWithUnderscores = true;
        SqlMapper.AddTypeHandler(new UriTypeHandler());
    }

    public SqlServerDbConnectionContext(ISqlHostConfiguration hostConfiguration, ITransportSupervisor<ConnectionContext> supervisor)
        : base(supervisor.Stopped)
    {
        _hostConfiguration = hostConfiguration;

        _hostSettings = hostConfiguration.Settings as SqlServerSqlHostSettings
            ?? throw new ConfigurationException("The host settings were not of the expected type");

        _retryPolicy = Retry.CreatePolicy(x => x.Immediate(10).Handle<SqlException>(ex => IsTransient(ex)));

        Topology = hostConfiguration.Topology;

        if(_hostSettings.MaintenanceEnabled)
            supervisor.AddConsumeAgent(new MaintenanceAgent(this, hostConfiguration));

        _executor = new TaskExecutor(hostConfiguration.Settings.ConnectionLimit);
    }

    public ISqlBusTopology Topology { get; }

    public IsolationLevel IsolationLevel => _hostSettings.IsolationLevel;

    public Uri HostAddress => _hostConfiguration.HostAddress;

    public string? Schema => _hostSettings.Schema;

    public ClientContext CreateClientContext(CancellationToken cancellationToken)
    {
        return new SqlServerClientContext(this, cancellationToken);
    }

    async Task<ISqlTransportConnection> ConnectionContext.CreateConnection(CancellationToken cancellationToken)
    {
        return await CreateConnection(cancellationToken).ConfigureAwait(false);
    }

    public Task<T> Query<T>(Func<IDbConnection, IDbTransaction, Task<T>> callback, CancellationToken cancellationToken)
    {
        return _executor.Run(() =>
        {
            return _retryPolicy.Retry(async () =>
            {
                await using var connection = await CreateConnection(cancellationToken).ConfigureAwait(false);

            #if NET6_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                await using var transaction = await connection.Connection.BeginTransactionAsync(_hostSettings.IsolationLevel, cancellationToken)
                    .ConfigureAwait(false);
            #else
                using var transaction = connection.Connection.BeginTransaction(_hostSettings.IsolationLevel);
            #endif

                var result = await callback(connection.Connection, transaction).ConfigureAwait(false);

            #if NET6_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                await transaction.CommitAsync(cancellationToken).ConfigureAwait(false);
            #else
                transaction.Commit();
            #endif

                return result;
            }, false, cancellationToken);
        }, cancellationToken);
    }

    public Task DelayUntilMessageReady(long queueId, TimeSpan timeout, CancellationToken cancellationToken)
    {
        async Task WaitAsync()
        {
            var delayTask = Task.Delay(timeout, cancellationToken);
            await Task.WhenAny(delayTask).ConfigureAwait(false);
        }

        return WaitAsync();
    }

    public async ValueTask DisposeAsync()
    {
        TransportLogMessages.DisconnectedHost(_hostConfiguration.HostAddress.ToString());
    }

    public async Task<ISqlServerSqlTransportConnection> CreateConnection(CancellationToken cancellationToken)
    {
        var connection = new SqlServerSqlTransportConnection(_hostSettings.GetConnectionString());

        await connection.Open(cancellationToken).ConfigureAwait(false);

        return connection;
    }

    bool IsTransient(SqlException exception)
    {
        return exception.Number switch
        {
            -2 => true,
            20 => true,
            64 => true,
            233 => true,
            1205 => true,
            10053 => true,
            10054 => true,
            10060 => true,
            10928 => true,
            10929 => true,
            40197 => true,
            40143 => true,
            40501 => true,
            40613 => true,
            _ => false
        };
    }


    class MaintenanceAgent :
        Agent
    {
        readonly SqlServerDbConnectionContext _context;
        readonly ISqlHostConfiguration _hostConfiguration;
        readonly ILogContext? _logContext;

        public MaintenanceAgent(SqlServerDbConnectionContext context, ISqlHostConfiguration hostConfiguration)
        {
            _context = context;
            _hostConfiguration = hostConfiguration;
            _logContext = hostConfiguration.LogContext;

            var runTask = Task.Run(() => PerformMaintenance(), Stopping);

            SetReady(runTask);

            SetCompleted(runTask);
        }

        async Task PerformMaintenance()
        {
            LogContext.SetCurrentIfNull(_logContext);

            var processMetricsSql = $"{_context.Schema}.ProcessMetrics";
            var purgeTopologySql = $"{_context.Schema}.PurgeTopology";

            var random = new Random();

            var cleanupInterval = _hostConfiguration.Settings.QueueCleanupInterval
                + TimeSpan.FromSeconds(random.Next(0, (int)(_hostConfiguration.Settings.QueueCleanupInterval.TotalSeconds / 10)));

            while (!Stopping.IsCancellationRequested)
            {
                DateTime? lastCleanup = null;

                try
                {
                    var maintenanceInterval = _hostConfiguration.Settings.MaintenanceInterval
                        + TimeSpan.FromSeconds(random.Next(0, (int)(_hostConfiguration.Settings.MaintenanceInterval.TotalSeconds / 10)));

                    try
                    {
                        await Task.Delay(maintenanceInterval, Stopping);
                    }
                    catch (OperationCanceledException)
                    {
                        await Execute<long>(processMetricsSql, new
                        {
                            rowLimit = _hostConfiguration.Settings.MaintenanceBatchSize,
                        }, CancellationToken.None);

                        if (lastCleanup == null)
                            await Execute<long>(purgeTopologySql, new { }, CancellationToken.None);
                    }

                    await _hostConfiguration.Retry(async () =>
                    {
                        await Execute<long>(processMetricsSql, new
                        {
                            rowLimit = _hostConfiguration.Settings.MaintenanceBatchSize,
                        }, Stopping);

                        if (lastCleanup == null || lastCleanup < DateTime.UtcNow - cleanupInterval)
                        {
                            await Execute<long>(purgeTopologySql, new { }, CancellationToken.None);

                            lastCleanup = DateTime.UtcNow;
                            cleanupInterval = _hostConfiguration.Settings.QueueCleanupInterval
                                + TimeSpan.FromSeconds(random.Next(0, (int)(_hostConfiguration.Settings.QueueCleanupInterval.TotalSeconds / 10)));
                        }
                    }, Stopping, Stopping);
                }
                catch (OperationCanceledException)
                {
                }
                catch (Exception exception)
                {
                    LogContext.Debug?.Log(exception, "SQL Server Maintenance Faulted");
                }
            }
        }

        Task<T?> Execute<T>(string functionName, object values, CancellationToken cancellationToken)
            where T : struct
        {
            return _context.Query((connection, transaction) => connection
                .ExecuteScalarAsync<T?>(functionName, values, transaction, commandType: CommandType.StoredProcedure), cancellationToken);
        }
    }
}
