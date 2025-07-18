namespace MassTransit.Persistence.SqlServer.Components.JobConsumers
{
    using System.Data;
    using System.Linq.Expressions;
    using Connections;
    using Extensions;
    using Microsoft.Data.SqlClient;


    public class JobAttemptSagaDatabaseContext : PessimisticSqlServerDatabaseContext<JobAttemptSaga>
    {
        public JobAttemptSagaDatabaseContext(string connectionString, IsolationLevel isolationLevel)
            : base(connectionString, "JobAttempts", nameof(ISaga.CorrelationId), isolationLevel)
        {
        }

        protected override string BuildInsertSql()
        {
            return @$"
INSERT INTO {TableName} 
    (CorrelationId, CurrentState, JobId, Started, Faulted, StatusCheckTokenId, RetryAttempt, ServiceAddress, InstanceAddress) 
VALUES
    (@correlationid, @currentstate, @jobid, @started, @faulted, @statuschecktokenid, @retryattempt, @serviceaddress, @instanceaddress);
";
        }

        protected override string BuildUpdateSql()
        {
            return $@"
UPDATE {TableName}
SET
	CurrentState = @currentstate,
	JobId = @jobid,
	Started = @started,
	Faulted = @faulted,
	StatusCheckTokenId = @statuschecktokenid,
	RetryAttempt = @retryattempt,
	ServiceAddress = @serviceaddress,
	InstanceAddress = @instanceaddress
WHERE
    CorrelationId = @correlationid;
";
        }

        protected override string BuildDeleteSql()
        {
            return $@"DELETE FROM {TableName} WHERE CorrelationId = @correlationid;";
        }

        protected override string BuildLoadSql()
        {
            return $@"SELECT TOP 1 * FROM {TableName} WITH (UPDLOCK, ROWLOCK) WHERE CorrelationId = @correlationid;";
        }

        protected override string BuildQuerySql(Expression<Func<JobAttemptSaga, bool>> filterExpression, Action<string, object?> parameterCallback)
        {
            throw new NotSupportedException("JobConsumers do not support querying");
        }

        protected override Func<IDataReader, JobAttemptSaga> CreateReaderAdapter()
        {
            return ConvertFrom;
        }

        static JobAttemptSaga ConvertFrom(IDataReader dataReader)
        {
            if (dataReader is not SqlDataReader reader)
                throw new NotSupportedException("ConvertFrom only supports SqlDataReader");

            return new JobAttemptSaga
            {
                CorrelationId = reader.GetGuid("CorrelationId"),
                CurrentState = reader.GetInt32("CurrentState"),
                JobId = reader.GetGuid("JobId"),
                Started = reader.GetDateTimeOrNull("Started"),
                Faulted = reader.GetDateTimeOrNull("Faulted"),
                StatusCheckTokenId = reader.GetGuidOrNull("StatusCheckTokenId"),
                RetryAttempt = reader.GetInt32("RetryAttempt"),
                ServiceAddress = reader.GetUri("ServiceAddress"),
                InstanceAddress = reader.GetUri("InstanceAddress")
            };
        }

        protected override Action<object?, SqlParameterCollection> CreateWriterAdapter()
        {
            return ConvertTo;
        }

        static void ConvertTo(object? source, SqlParameterCollection collection)
        {
            if (source is JobAttemptSaga instance)
                ConvertJobAttempt(collection, instance);
            else
                AssignParameters(source, collection);
        }

        static void ConvertJobAttempt(SqlParameterCollection collection, JobAttemptSaga instance)
        {
            collection.Add("@correlationid", SqlDbType.UniqueIdentifier).Value = instance.CorrelationId;
            collection.Add("@currentstate", SqlDbType.Int).Value = instance.CurrentState;
            collection.Add("@jobid", SqlDbType.UniqueIdentifier).Value = instance.JobId;
            collection.Add("@started", SqlDbType.DateTime).Value = instance.Started.OrDbNull();
            collection.Add("@faulted", SqlDbType.DateTime).Value = instance.Faulted.OrDbNull();
            collection.Add("@statuschecktokenid", SqlDbType.UniqueIdentifier).Value = instance.StatusCheckTokenId.OrDbNull();
            collection.Add("@retryattempt", SqlDbType.Int).Value = instance.RetryAttempt;
            collection.Add("@serviceaddress", SqlDbType.NVarChar, 1000).Value = (instance.ServiceAddress?.ToString()).OrDbNull();
            collection.Add("@instanceaddress", SqlDbType.NVarChar, 1000).Value = (instance.InstanceAddress?.ToString()).OrDbNull();
        }
    }
}
