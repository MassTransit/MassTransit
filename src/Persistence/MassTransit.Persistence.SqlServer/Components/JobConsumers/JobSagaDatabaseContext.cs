namespace MassTransit.Persistence.SqlServer.Components.JobConsumers
{
    using System.Data;
    using System.Linq.Expressions;
    using Connections;
    using Extensions;
    using Microsoft.Data.SqlClient;


    public class JobSagaDatabaseContext : PessimisticSqlServerDatabaseContext<JobSaga>
    {
        public JobSagaDatabaseContext(string connectionString, IsolationLevel isolationLevel)
            : base(connectionString, "Jobs", nameof(ISaga.CorrelationId), isolationLevel)
        {
        }

        protected override string BuildInsertSql()
        {
            return @$"
INSERT INTO {TableName} 
    (CorrelationId, CurrentState, Completed, Faulted, Started, Submitted, EndDate, NextStartDate,
    StartDate, AttemptId, JobTypeId, JobRetryDelayToken, JobSlotWaitToken, RetryAttempt, LastProgressLimit,
    LastProgressSequenceNumber, LastProgressValue, CronExpression, Reason, TimeZoneId,
    Duration, JobTimeout, ServiceAddress, IncompleteAttempts, Job, JobProperties, JobState) 
VALUES
    (@correlationid, @currentstate, @completed, @faulted, @started, @submitted, @enddate, @nextstartdate,
    @startdate, @attemptid, @jobtypeid, @jobretrydelaytoken, @jobslotwaittoken, @retryattempt, @lastprogresslimit,
    @lastprogresssequencenumber, @lastprogressvalue, @cronexpression, @reason, @timezoneid,
    @duration, @jobtimeout, @serviceaddress, @incompleteattempts, @job, @jobproperties, @jobstate);
";
        }

        protected override string BuildUpdateSql()
        {
            return $@"
UPDATE {TableName}
SET
	CurrentState = @currentstate,
	Completed = @completed,
	Faulted = @faulted,
	Started = @started,
	Submitted = @submitted,
	EndDate = @enddate,
	NextStartDate = @nextstartdate,
	StartDate = @startdate,
	AttemptId = @attemptid,
	JobTypeId = @jobtypeid,
	JobRetryDelayToken = @jobretrydelaytoken,
	JobSlotWaitToken = @jobslotwaittoken,
	RetryAttempt = @retryattempt,
	LastProgressLimit = @lastprogresslimit,
	LastProgressSequenceNumber = @lastprogresssequencenumber,
	LastProgressValue = @lastprogressvalue,
	CronExpression = @cronexpression,
	Reason = @reason,
	TimeZoneId = @timezoneid,
	Duration = @duration,
	JobTimeout = @jobtimeout,
	ServiceAddress = @serviceaddress,
	IncompleteAttempts = @incompleteattempts,
	Job = @job,
	JobProperties = @jobproperties,
	JobState = @jobstate
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

        protected override string BuildQuerySql(Expression<Func<JobSaga, bool>> filterExpression, Action<string, object?> parameterCallback)
        {
            throw new NotSupportedException("JobConsumers do not support querying");
        }

        protected override Func<IDataReader, JobSaga> CreateReaderAdapter()
        {
            return ConvertFrom;
        }

        static JobSaga ConvertFrom(IDataReader dataReader)
        {
            if (dataReader is not SqlDataReader reader)
                throw new NotSupportedException("ConvertFrom only supports SqlDataReader");

            return new JobSaga
            {
                CorrelationId = reader.GetGuid("CorrelationId"),
                CurrentState = reader.GetInt32("CurrentState"),
                Completed = reader.GetDateTimeOrNull("Completed"),
                Faulted = reader.GetDateTimeOrNull("Faulted"),
                Started = reader.GetDateTimeOrNull("Started"),
                Submitted = reader.GetDateTimeOrNull("Submitted"),
                EndDate = reader.GetDateTimeOffsetOrNull("EndDate"),
                NextStartDate = reader.GetDateTimeOffsetOrNull("NextStartDate"),
                StartDate = reader.GetDateTimeOffsetOrNull("StartDate"),
                AttemptId = reader.GetGuid("AttemptId"),
                JobTypeId = reader.GetGuid("JobTypeId"),
                JobRetryDelayToken = reader.GetGuidOrNull("JobRetryDelayToken"),
                JobSlotWaitToken = reader.GetGuidOrNull("JobSlotWaitToken"),
                RetryAttempt = reader.GetInt32("RetryAttempt"),
                LastProgressLimit = reader.GetInt64OrNull("LastProgressLimit"),
                LastProgressSequenceNumber = reader.GetInt64OrNull("LastProgressSequenceNumber"),
                LastProgressValue = reader.GetInt64OrNull("LastProgressValue"),
                CronExpression = reader.GetStringOrNull("CronExpression"),
                Reason = reader.GetStringOrNull("Reason"),
                TimeZoneId = reader.GetStringOrNull("TimeZoneId"),
                Duration = reader.GetTimeSpanOrNull("Duration"),
                JobTimeout = reader.GetTimeSpanOrNull("JobTimeout"),
                ServiceAddress = reader.GetUri("ServiceAddress"),
                IncompleteAttempts = reader.FromJson<List<Guid>>("IncompleteAttempts") ?? new List<Guid>(),
                Job = reader.FromJson<Dictionary<string, object>>("Job") ?? new Dictionary<string, object>(),
                JobProperties = reader.FromJson<Dictionary<string, object>>("JobProperties") ?? new Dictionary<string, object>(),
                JobState = reader.FromJson<Dictionary<string, object>>("JobState") ?? new Dictionary<string, object>()
            };
        }

        protected override Action<object?, SqlParameterCollection> CreateWriterAdapter()
        {
            return ConvertTo;
        }

        static void ConvertTo(object? source, SqlParameterCollection collection)
        {
            if (source is JobSaga instance)
                ConvertJob(collection, instance);
            else
                AssignParameters(source, collection);
        }

        static void ConvertJob(SqlParameterCollection collection, JobSaga instance)
        {
            collection.Add("@correlationid", SqlDbType.UniqueIdentifier).Value = instance.CorrelationId;
            collection.Add("@currentstate", SqlDbType.Int).Value = instance.CurrentState;
            collection.Add("@completed", SqlDbType.DateTime).Value = instance.Completed.OrDbNull();
            collection.Add("@faulted", SqlDbType.DateTime).Value = instance.Faulted.OrDbNull();
            collection.Add("@started", SqlDbType.DateTime).Value = instance.Started.OrDbNull();
            collection.Add("@submitted", SqlDbType.DateTime).Value = instance.Submitted.OrDbNull();
            collection.Add("@enddate", SqlDbType.DateTimeOffset).Value = instance.EndDate.OrDbNull();
            collection.Add("@nextstartdate", SqlDbType.DateTimeOffset).Value = instance.NextStartDate.OrDbNull();
            collection.Add("@startdate", SqlDbType.DateTimeOffset).Value = instance.StartDate.OrDbNull();
            collection.Add("@attemptid", SqlDbType.UniqueIdentifier).Value = instance.AttemptId;
            collection.Add("@jobtypeid", SqlDbType.UniqueIdentifier).Value = instance.JobTypeId;
            collection.Add("@jobretrydelaytoken", SqlDbType.UniqueIdentifier).Value = instance.JobRetryDelayToken.OrDbNull();
            collection.Add("@jobslotwaittoken", SqlDbType.UniqueIdentifier).Value = instance.JobSlotWaitToken.OrDbNull();
            collection.Add("@retryattempt", SqlDbType.Int).Value = instance.RetryAttempt;
            collection.Add("@lastprogresslimit", SqlDbType.BigInt).Value = instance.LastProgressLimit.OrDbNull();
            collection.Add("@lastprogresssequencenumber", SqlDbType.BigInt).Value = instance.LastProgressSequenceNumber.OrDbNull();
            collection.Add("@lastprogressvalue", SqlDbType.BigInt).Value = instance.LastProgressValue.OrDbNull();
            collection.Add("@cronexpression", SqlDbType.NVarChar, 255).Value = instance.CronExpression.OrDbNull();
            collection.Add("@reason", SqlDbType.NVarChar, -1).Value = instance.Reason.OrDbNull();
            collection.Add("@timezoneid", SqlDbType.NVarChar, 100).Value = instance.TimeZoneId.OrDbNull();
            collection.Add("@duration", SqlDbType.Time).Value = instance.Duration.OrDbNull();
            collection.Add("@jobtimeout", SqlDbType.Time).Value = instance.JobTimeout.OrDbNull();
            collection.Add("@serviceaddress", SqlDbType.NVarChar, 1000).Value = (instance.ServiceAddress?.ToString()).OrDbNull();
            collection.Add("@incompleteattempts", SqlDbType.VarChar, -1).Value = instance.IncompleteAttempts.ToJson().OrDbNull();
            collection.Add("@job", SqlDbType.VarChar, -1).Value = instance.Job.ToJson().OrDbNull();
            collection.Add("@jobproperties", SqlDbType.VarChar, -1).Value = instance.JobProperties.ToJson().OrDbNull();
            collection.Add("@jobstate", SqlDbType.VarChar, -1).Value = instance.JobState.ToJson().OrDbNull();
        }
    }
}
