namespace MassTransit.SqlTransport.PostgreSql
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Data;
    using System.Threading;
    using System.Threading.Tasks;
    using Configuration;
    using Dapper;
    using Helpers;
    using Logging;
    using MassTransit.Middleware;
    using Npgsql;
    using RetryPolicies;
    using Transports;
    using Util;


    public class PostgresDbConnectionContext :
        BasePipeContext,
        ConnectionContext,
        IAsyncDisposable
    {
        readonly NotificationAgent _agent;
        readonly NpgsqlDataSource _dataSource;
        readonly TaskExecutor _executor;
        readonly ISqlHostConfiguration _hostConfiguration;
        readonly PostgresSqlHostSettings _hostSettings;
        readonly IRetryPolicy _retryPolicy;

        static PostgresDbConnectionContext()
        {
            DefaultTypeMap.MatchNamesWithUnderscores = true;
            SqlMapper.AddTypeHandler(new UriTypeHandler());
        }

        public PostgresDbConnectionContext(ISqlHostConfiguration hostConfiguration, ITransportSupervisor<ConnectionContext> supervisor)
            : base(supervisor.Stopped)
        {
            _hostConfiguration = hostConfiguration;

            _hostSettings = hostConfiguration.Settings as PostgresSqlHostSettings
                ?? throw new ConfigurationException("The host settings were not of the expected type");

            _dataSource = _hostSettings.GetDataSource();

            _retryPolicy = Retry.CreatePolicy(x => x.Immediate(10).Handle<PostgresException>(ex => ex.IsTransient));

            Topology = hostConfiguration.Topology;

            _agent = new NotificationAgent(this, hostConfiguration);
            supervisor.AddConsumeAgent(_agent);

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
            return new PostgresClientContext(this, cancellationToken);
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
                    await using var transaction = connection.Connection.BeginTransaction(_hostSettings.IsolationLevel);
                #endif

                    var result = await callback(connection.Connection, transaction).ConfigureAwait(false);

                    await transaction.CommitAsync(cancellationToken).ConfigureAwait(false);

                    return result;
                }, false, cancellationToken);
            }, cancellationToken);
        }

        public Task DelayUntilMessageReady(long queueId, TimeSpan timeout, CancellationToken cancellationToken)
        {
            var queueToken = _agent.GetCancellationTokenForQueue(queueId);

            async Task WaitAsync()
            {
                using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, queueToken);
                var delayTask = Task.Delay(timeout, cts.Token);
                await Task.WhenAny(delayTask).ConfigureAwait(false);

                cts.Cancel();

                if (queueToken.IsCancellationRequested)
                    _agent.RemoveTokenForQueue(queueId);
            }

            return WaitAsync();
        }

        public async ValueTask DisposeAsync()
        {
            if (_hostSettings.IsProvidedDataSource == false)
                await _dataSource.DisposeAsync().ConfigureAwait(false);

            TransportLogMessages.DisconnectedHost(_hostConfiguration.HostAddress.ToString());
        }

        async Task<IPostgresSqlTransportConnection> CreateConnection(CancellationToken cancellationToken)
        {
            var connection = new PostgresSqlTransportConnection(_dataSource.CreateConnection());

            await connection.Open(cancellationToken).ConfigureAwait(false);

            return connection;
        }


        class NotificationAgent :
            Agent
        {
            readonly PostgresDbConnectionContext _context;
            readonly ISqlHostConfiguration _hostConfiguration;
            readonly ILogContext? _logContext;
            readonly ConcurrentDictionary<long, CancellationTokenSource> _notificationTokens;
            CancellationTokenSource _listenTokenSource;

            public NotificationAgent(PostgresDbConnectionContext context, ISqlHostConfiguration hostConfiguration)
            {
                _context = context;
                _hostConfiguration = hostConfiguration;
                _logContext = hostConfiguration.LogContext;

                _notificationTokens = new ConcurrentDictionary<long, CancellationTokenSource>();
                _listenTokenSource = new CancellationTokenSource();

                var runTask = Task.Run(() => ListenForNotifications(), Stopping);

                SetReady(runTask);

                SetCompleted(runTask);
            }

            public CancellationToken GetCancellationTokenForQueue(long queueId)
            {
                var added = false;
                var notifyTokenSource = _notificationTokens.GetOrAdd(queueId, _ =>
                {
                    added = true;
                    return new CancellationTokenSource();
                });

                if (added)
                    _listenTokenSource.Cancel();

                if (notifyTokenSource.IsCancellationRequested)
                    _notificationTokens.TryRemove(queueId, out _);

                return notifyTokenSource.Token;
            }

            public void RemoveTokenForQueue(long queueId)
            {
                if (_notificationTokens.TryGetValue(queueId, out var existing))
                {
                    var newValue = new CancellationTokenSource();
                    if (!_notificationTokens.TryUpdate(queueId, newValue, existing))
                        newValue.Dispose();
                }
            }

            async Task ListenForNotifications()
            {
                LogContext.SetCurrentIfNull(_logContext);

                while (!Stopping.IsCancellationRequested)
                {
                    try
                    {
                        await _hostConfiguration.Retry(async () =>
                        {
                            await using var connection = await _context.CreateConnection(Stopping);

                            var queueIds = new HashSet<long>(_notificationTokens.Keys);
                            var sanitizedSchemaName = NotifyChannel.SanitizeSchemaName(_context.Schema);

                            connection.Connection.Notification += OnConnectionOnNotification;

                            foreach (var queueId in queueIds)
                            {
                                await connection.Connection.ExecuteScalarAsync<int>($"LISTEN \"{sanitizedSchemaName}_msg_{queueId}\"", Stopping)
                                    .ConfigureAwait(false);

                                // LogContext.Debug?.Log("LISTEN \"{sanitizedSchemaName}_msg_{queueId}\"", queueId);
                            }

                            while (!Stopping.IsCancellationRequested)
                            {
                                try
                                {
                                    using var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_listenTokenSource.Token, Stopping);

                                    await connection.Connection.WaitAsync(linkedTokenSource.Token).ConfigureAwait(false);
                                }
                                catch (OperationCanceledException)
                                {
                                }

                                if (_listenTokenSource.IsCancellationRequested)
                                    _listenTokenSource = new CancellationTokenSource();

                                foreach (var queueId in _notificationTokens.Keys)
                                {
                                    if (queueIds.Contains(queueId))
                                        continue;

                                    await connection.Connection.ExecuteScalarAsync<int>($"LISTEN \"{sanitizedSchemaName}_msg_{queueId}\"", Stopping)
                                        .ConfigureAwait(false);

                                    // LogContext.Debug?.Log("LISTEN \"{sanitizedSchemaName}_msg_{queueId}\"", queueId);

                                    queueIds.Add(queueId);
                                }
                            }
                        }, Stopping, Stopping);
                    }
                    catch (OperationCanceledException)
                    {
                    }
                    catch (Exception exception)
                    {
                        LogContext.Debug?.Log(exception, "PgSql notification faulted");
                    }
                }
            }

            void OnConnectionOnNotification(object sender, NpgsqlNotificationEventArgs args)
            {
                LogContext.SetCurrentIfNull(_logContext);

                var index = args.Channel.LastIndexOf('_');
                if (index > 0 && long.TryParse(args.Channel.Substring(index + 1), out var queueId) && _notificationTokens.TryGetValue(queueId, out var source))
                {
                    // LogContext.Debug?.Log("NOTIFY {Channel}", args.Channel);
                    source.Cancel();
                }
            }
        }


        class MaintenanceAgent :
            Agent
        {
            readonly PostgresDbConnectionContext _context;
            readonly ISqlHostConfiguration _hostConfiguration;
            readonly ILogContext? _logContext;

            public MaintenanceAgent(PostgresDbConnectionContext context, ISqlHostConfiguration hostConfiguration)
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

                var processMetricsSql = string.Format(SqlStatements.DbProcessMetricsSql, _context.Schema);
                var purgeTopologySql = string.Format(SqlStatements.DbPurgeTopologySql, _context.Schema);

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
                            using var timeoutToken = new CancellationTokenSource(TimeSpan.FromSeconds(10));

                            try
                            {
                                await _context.Query(
                                    (x, t) => x.ExecuteScalarAsync<long?>(processMetricsSql,
                                        new { row_limit = _hostConfiguration.Settings.MaintenanceBatchSize }, t), timeoutToken.Token);

                                if (lastCleanup == null)
                                    await _context.Query((x, t) => x.ExecuteScalarAsync<long?>(purgeTopologySql, t), timeoutToken.Token);
                            }
                            catch (ObjectDisposedException)
                            {
                            }
                            catch (OperationCanceledException)
                            {
                            }
                            catch (TimeoutException)
                            {
                            }
                        }

                        await _hostConfiguration.Retry(async () =>
                        {
                            await _context.Query((x, t) => x.ExecuteScalarAsync<long?>(processMetricsSql, new
                            {
                                row_limit = _hostConfiguration.Settings.MaintenanceBatchSize,
                            }, t), Stopping);

                            if (lastCleanup == null || lastCleanup < DateTime.UtcNow - cleanupInterval)
                            {
                                await _context.Query((x, t) => x.ExecuteScalarAsync<long?>(purgeTopologySql, t), Stopping);

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
                        LogContext.Debug?.Log(exception, "PostgreSQL Maintenance Faulted");
                    }
                }
            }
        }
    }
}
