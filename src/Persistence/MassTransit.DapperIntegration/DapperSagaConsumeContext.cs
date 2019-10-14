namespace MassTransit.DapperIntegration
{
    using System.Data.SqlClient;
    using System.Threading.Tasks;
    using Context;
    using Dapper;
    using Saga;


    public class DapperSagaConsumeContext<TSaga, TMessage> :
        ConsumeContextScope<TMessage>,
        SagaConsumeContext<TSaga, TMessage>
        where TMessage : class
        where TSaga : class, ISaga
    {
        readonly SqlConnection _sqlConnection;
        readonly string _tableName;
        readonly bool _existing;

        public DapperSagaConsumeContext(SqlConnection sqlConnection, ConsumeContext<TMessage> context, TSaga instance, string tableName,
            bool existing = true)
            : base(context)
        {
            _sqlConnection = sqlConnection;
            _tableName = tableName;
            _existing = existing;
            Saga = instance;
        }

        public async Task SetCompleted()
        {
            IsCompleted = true;
            if (_existing)
            {
                var correlationId = Saga.CorrelationId;
                await _sqlConnection
                    .QueryAsync($"DELETE FROM {_tableName} WHERE CorrelationId = @correlationId", new {correlationId})
                    .ConfigureAwait(false);

                this.LogRemoved();
            }
        }

        public TSaga Saga { get; }
        public bool IsCompleted { get; private set; }
    }
}
