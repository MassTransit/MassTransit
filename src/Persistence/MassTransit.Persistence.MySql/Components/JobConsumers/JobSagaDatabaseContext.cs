namespace MassTransit.Persistence.MySql.Components.JobConsumers
{
    using System.Data;
    using System.Linq.Expressions;
    using Connections;
    using Extensions;
    using MySqlConnector;


    public class JobSagaDatabaseContext : PessimisticMySqlDatabaseContext<JobSaga>
    {
        public JobSagaDatabaseContext(string connectionString, IsolationLevel isolationLevel)
            : base(connectionString, "Jobs", nameof(ISaga.CorrelationId), isolationLevel)
        {
            Ignore(m => m.RowVersion);
            Ignore(m => m.Version);
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
            if (dataReader is not MySqlDataReader reader)
                throw new NotSupportedException("ConvertFrom only supports MySqlDataReader");

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

        protected override Action<object?, MySqlParameterCollection> CreateWriterAdapter()
        {
            return ConvertTo;
        }

        static void ConvertTo(object? source, MySqlParameterCollection collection)
        {
            if (source is JobSaga instance)
                ConvertJob(collection, instance);
            else
                AssignParameters(source, collection);
        }

        static void ConvertJob(MySqlParameterCollection collection, JobSaga instance)
        {
            collection.Add("@correlationid", MySqlDbType.Guid).Value = instance.CorrelationId.ToByteArray();
            collection.Add("@currentstate", MySqlDbType.Int32).Value = instance.CurrentState;
            collection.Add("@completed", MySqlDbType.DateTime).Value = instance.Completed.OrDbNull();
            collection.Add("@faulted", MySqlDbType.DateTime).Value = instance.Faulted.OrDbNull();
            collection.Add("@started", MySqlDbType.DateTime).Value = instance.Started.OrDbNull();
            collection.Add("@submitted", MySqlDbType.DateTime).Value = instance.Submitted.OrDbNull();
            collection.Add("@enddate", MySqlDbType.DateTime).Value = instance.EndDate.OrDbNull();
            collection.Add("@nextstartdate", MySqlDbType.DateTime).Value = instance.NextStartDate.OrDbNull();
            collection.Add("@startdate", MySqlDbType.DateTime).Value = instance.StartDate.OrDbNull();
            collection.Add("@attemptid", MySqlDbType.Guid).Value = instance.AttemptId;
            collection.Add("@jobtypeid", MySqlDbType.Guid).Value = instance.JobTypeId;
            collection.Add("@jobretrydelaytoken", MySqlDbType.Guid).Value = instance.JobRetryDelayToken.OrDbNull();
            collection.Add("@jobslotwaittoken", MySqlDbType.Guid).Value = instance.JobSlotWaitToken.OrDbNull();
            collection.Add("@retryattempt", MySqlDbType.Int32).Value = instance.RetryAttempt;
            collection.Add("@lastprogresslimit", MySqlDbType.Int64).Value = instance.LastProgressLimit.OrDbNull();
            collection.Add("@lastprogresssequencenumber", MySqlDbType.Int64).Value = instance.LastProgressSequenceNumber.OrDbNull();
            collection.Add("@lastprogressvalue", MySqlDbType.Int64).Value = instance.LastProgressValue.OrDbNull();
            collection.Add("@cronexpression", MySqlDbType.VarChar, 255).Value = instance.CronExpression.OrDbNull();
            collection.Add("@reason", MySqlDbType.Text).Value = instance.Reason.OrDbNull();
            collection.Add("@timezoneid", MySqlDbType.VarChar, 100).Value = instance.TimeZoneId.OrDbNull();
            collection.Add("@duration", MySqlDbType.Time).Value = instance.Duration.OrDbNull();
            collection.Add("@jobtimeout", MySqlDbType.Time).Value = instance.JobTimeout.OrDbNull();
            collection.Add("@serviceaddress", MySqlDbType.VarChar, 1000).Value = (instance.ServiceAddress?.ToString()).OrDbNull();
            collection.Add("@incompleteattempts", MySqlDbType.Text).Value = instance.IncompleteAttempts.ToJson().OrDbNull();
            collection.Add("@job", MySqlDbType.Text).Value = instance.Job.ToJson().OrDbNull();
            collection.Add("@jobproperties", MySqlDbType.Text).Value = instance.JobProperties.ToJson().OrDbNull();
            collection.Add("@jobstate", MySqlDbType.Text).Value = instance.JobState.ToJson().OrDbNull();
        }
    }
}
