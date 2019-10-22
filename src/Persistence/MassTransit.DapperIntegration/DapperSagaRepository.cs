namespace MassTransit.DapperIntegration
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Transactions;
    using Context;
    using Dapper;
    using Dapper.Contrib.Extensions;
    using GreenPipes;
    using Metadata;
    using Saga;
    using Sql;


    public class DapperSagaRepository<TSaga> :
        ISagaRepository<TSaga>,
        IQuerySagaRepository<TSaga>
        where TSaga : class, ISaga
    {
        readonly string _connectionString;

        public DapperSagaRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        async Task<IEnumerable<Guid>> IQuerySagaRepository<TSaga>.Find(ISagaQuery<TSaga> query)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var tableName = GetTableName();
                var (whereStatement, parameters) = WhereStatementHelper.GetWhereStatementAndParametersFromExpression(query.FilterExpression);

                return
                    (await connection.QueryAsync<Guid>($"SELECT CorrelationId FROM {tableName} WITH (NOLOCK) {whereStatement}",
                        parameters).ConfigureAwait(false)).ToList();
            }
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateScope("sagaRepository");
            scope.Set(new {Persistence = "dapper"});
        }

        async Task ISagaRepository<TSaga>.Send<T>(ConsumeContext<T> context, ISagaPolicy<TSaga, T> policy, IPipe<SagaConsumeContext<TSaga, T>> next)
        {
            if (!context.CorrelationId.HasValue)
                throw new SagaException("The CorrelationId was not specified", typeof(TSaga), typeof(T));

            var correlationId = context.CorrelationId.Value;
            if (policy.PreInsertInstance(context, out TSaga preInsertInstance))
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await PreInsertSagaInstance<T>(connection, context, preInsertInstance).ConfigureAwait(false);
                }
            }

            using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            using (var connection = new SqlConnection(_connectionString))
            {
                try
                {
                    var tableName = GetTableName();

                    var instance = await connection.QuerySingleOrDefaultAsync<TSaga>(
                        $"SELECT * FROM {tableName} WITH (UPDLOCK, ROWLOCK) WHERE CorrelationId = @correlationId",
                        new {correlationId}).ConfigureAwait(false);

                    if (instance == null)
                    {
                        var missingSagaPipe = new MissingPipe<T>(connection, tableName, next, InsertSagaInstance);

                        await policy.Missing(context, missingSagaPipe).ConfigureAwait(false);
                    }
                    else
                    {
                        await SendToInstance(context, connection, policy, instance, tableName, next).ConfigureAwait(false);
                    }

                    transaction.Complete();
                }
                catch (SagaException sex)
                {
                    LogContext.Error?.Log(sex, "SAGA:{SagaType} Exception {MessageType}", TypeMetadataCache<TSaga>.ShortName, TypeMetadataCache<T>.ShortName);
                    throw;
                }
                catch (Exception ex)
                {
                    LogContext.Error?.Log(ex, "SAGA:{SagaType} Exception {MessageType}", TypeMetadataCache<TSaga>.ShortName, TypeMetadataCache<T>.ShortName);

                    throw new SagaException(ex.Message, typeof(TSaga), typeof(T), correlationId, ex);
                }
            }
        }

        public async Task SendQuery<T>(SagaQueryConsumeContext<TSaga, T> context, ISagaPolicy<TSaga, T> policy, IPipe<SagaConsumeContext<TSaga, T>> next)
            where T : class
        {
            var tableName = GetTableName();
            var (whereStatement, parameters) = WhereStatementHelper.GetWhereStatementAndParametersFromExpression(context.Query.FilterExpression);

            List<Guid> correlationIds = null;
            using (var connection = new SqlConnection(_connectionString))
            {
                correlationIds =
                    (await connection.QueryAsync<Guid>(
                        $"SELECT CorrelationId FROM {tableName} WITH (NOLOCK) {whereStatement}",
                        parameters).ConfigureAwait(false)).ToList();
            }

            using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            using (var connection = new SqlConnection(_connectionString))
            {
                try
                {
                    var missingCorrelationIds = new List<Guid>();

                    foreach (var correlationId in correlationIds)
                    {
                        var instance =
                            await connection.QuerySingleOrDefaultAsync<TSaga>(
                                $"SELECT * FROM {tableName} WITH (UPDLOCK, ROWLOCK) where CorrelationId = @correlationId",
                                new {correlationId}).ConfigureAwait(false);

                        if (instance != null)
                        {
                            await SendToInstance(context, connection, policy, instance, tableName, next).ConfigureAwait(false);
                        }
                        else
                        {
                            missingCorrelationIds.Add(correlationId);
                        }
                    }

                    // If no sagas are found or all are missing
                    if (correlationIds.Count == missingCorrelationIds.Count)
                    {
                        var missingSagaPipe = new MissingPipe<T>(connection, tableName, next, InsertSagaInstance);

                        await policy.Missing(context, missingSagaPipe).ConfigureAwait(false);
                    }

                    transaction.Complete();
                }
                catch (SagaException sex)
                {
                    context.LogFault(sex);
                    throw;
                }
                catch (Exception ex)
                {
                    context.LogFault(ex);

                    throw new SagaException(ex.Message, typeof(TSaga), typeof(T), Guid.Empty, ex);
                }
            }
        }

        string GetTableName()
        {
            return $"{typeof(TSaga).Name}s";
        }

        async Task PreInsertSagaInstance<T>(SqlConnection sqlConnection, ConsumeContext<T> context, TSaga instance)
            where T : class
        {
            try
            {
                await InsertSagaInstance(sqlConnection, instance).ConfigureAwait(false);

                context.LogInsert(this, instance.CorrelationId);
            }
            catch (Exception ex)
            {
                context.LogInsertFault(this, ex, instance.CorrelationId);
            }
        }

        async Task SendToInstance<T>(ConsumeContext<T> context, SqlConnection sqlConnection, ISagaPolicy<TSaga, T> policy, TSaga instance, string tableName,
            IPipe<SagaConsumeContext<TSaga, T>> next)
            where T : class
        {
            try
            {
                var sagaConsumeContext = new DapperSagaConsumeContext<TSaga, T>(sqlConnection, context, instance, tableName);

                sagaConsumeContext.LogUsed();

                await policy.Existing(sagaConsumeContext, next).ConfigureAwait(false);

                if (!sagaConsumeContext.IsCompleted)
                {
                    await UpdateSagaInstance(sqlConnection, instance).ConfigureAwait(false);
                }
            }
            catch (SagaException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new SagaException(ex.Message, typeof(TSaga), typeof(T), instance.CorrelationId, ex);
            }
        }

        protected virtual async Task InsertSagaInstance(SqlConnection sqlConnection, TSaga instance)
        {
            await sqlConnection.InsertAsync(instance).ConfigureAwait(false);
        }

        protected virtual async Task UpdateSagaInstance(SqlConnection sqlConnection, TSaga instance)
        {
            await sqlConnection.UpdateAsync(instance).ConfigureAwait(false);
        }


        /// <summary>
        /// Once the message pipe has processed the saga instance, add it to the saga repository
        /// </summary>
        /// <typeparam name="TMessage"></typeparam>
        class MissingPipe<TMessage> :
            IPipe<SagaConsumeContext<TSaga, TMessage>>
            where TMessage : class
        {
            readonly SqlConnection _sqlConnection;
            readonly string _tableName;
            readonly IPipe<SagaConsumeContext<TSaga, TMessage>> _next;
            readonly Func<SqlConnection, TSaga, Task> _insertSagaInstance;

            public MissingPipe(SqlConnection sqlConnection, string tableName, IPipe<SagaConsumeContext<TSaga, TMessage>> next,
                Func<SqlConnection, TSaga, Task> insertSagaInstance)
            {
                _sqlConnection = sqlConnection;
                _tableName = tableName;
                _next = next;
                _insertSagaInstance = insertSagaInstance;
            }

            void IProbeSite.Probe(ProbeContext context)
            {
                _next.Probe(context);
            }

            public async Task Send(SagaConsumeContext<TSaga, TMessage> context)
            {
                var sagaConsumeContext = new DapperSagaConsumeContext<TSaga, TMessage>(_sqlConnection, context, context.Saga, _tableName, false);

                sagaConsumeContext.LogAdded();

                await _next.Send(sagaConsumeContext).ConfigureAwait(false);

                if (!sagaConsumeContext.IsCompleted)
                {
                    await _insertSagaInstance(_sqlConnection, context.Saga).ConfigureAwait(false);
                }
            }
        }
    }
}
