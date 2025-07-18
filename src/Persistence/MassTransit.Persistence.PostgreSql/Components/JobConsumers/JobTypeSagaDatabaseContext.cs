namespace MassTransit.Persistence.PostgreSql.Components.JobConsumers
{
    using System.Data;
    using System.Linq.Expressions;
    using Connections;
    using Extensions;
    using Npgsql;
    using NpgsqlTypes;


    public class JobTypeSagaDatabaseContext : PessimisticPostgresDatabaseContext<JobTypeSaga>
    {
        public JobTypeSagaDatabaseContext(string connectionString, IsolationLevel isolationLevel)
            : base(connectionString, "JobTypes", nameof(ISaga.CorrelationId), isolationLevel)
        {
        }

        protected override string BuildInsertSql()
        {
            return @$"
INSERT INTO {TableName} 
    (CorrelationId, Name, CurrentState, ActiveJobCount, ConcurrentJobLimit,
    OverrideJobLimit, OverrideLimitExpiration, GlobalConcurrentJobLimit,
    ActiveJobs, Instances, Properties) 
VALUES
    (@correlationid, @name, @currentstate, @activejobcount, @concurrentjoblimit,
    @overridejoblimit, @overridelimitexpiration, @globalconcurrentjoblimit,
    @activejobs, @instances, @properties);
";
        }

        protected override string BuildUpdateSql()
        {
            return $@"
UPDATE {TableName}
SET
	Name = @name,
    CurrentState = @currentstate,
	ActiveJobCount = @activejobcount,
	ConcurrentJobLimit = @concurrentjoblimit,
	OverrideJobLimit = @overridejoblimit,
	OverrideLimitExpiration = @overridelimitexpiration,
	GlobalConcurrentJobLimit = @globalconcurrentjoblimit,
	ActiveJobs = @activejobs,
	Instances = @instances,
	Properties = @properties
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
            return $@"SELECT * FROM {TableName} WHERE CorrelationId = @correlationid FOR UPDATE;";
        }

        protected override string BuildQuerySql(Expression<Func<JobTypeSaga, bool>> filterExpression, Action<string, object?> parameterCallback)
        {
            throw new NotSupportedException("JobConsumers do not support querying");
        }

        protected override Func<IDataReader, JobTypeSaga> CreateReaderAdapter()
        {
            return ConvertFrom;
        }

        static JobTypeSaga ConvertFrom(IDataReader dataReader)
        {
            if (dataReader is not NpgsqlDataReader reader)
                throw new NotSupportedException("ConvertFrom only supports NpgsqlDataReader");

            return new JobTypeSaga
            {
                CorrelationId = reader.GetGuid("CorrelationId"),
                Name = reader.GetString("Name"),
                CurrentState = reader.GetInt32("CurrentState"),
                ActiveJobCount = reader.GetInt32("ActiveJobCount"),
                ConcurrentJobLimit = reader.GetInt32("ConcurrentJobLimit"),
                OverrideJobLimit = reader.GetInt32OrNull("OverrideJobLimit"),
                OverrideLimitExpiration = reader.GetDateTimeOrNull("OverrideLimitExpiration"),
                GlobalConcurrentJobLimit = reader.GetInt32OrNull("GlobalConcurrentJobLimit"),
                ActiveJobs = reader.FromJson<List<ActiveJob>>("ActiveJobs") ?? [],
                Instances = reader.FromJson<Dictionary<Uri, JobTypeInstance>>("Instances") ?? new Dictionary<Uri, JobTypeInstance>(),
                Properties = reader.FromJson<Dictionary<string, object>>("Properties") ?? new Dictionary<string, object>()
            };
        }

        protected override Action<object?, NpgsqlParameterCollection> CreateWriterAdapter()
        {
            return ConvertTo;
        }

        static void ConvertTo(object? source, NpgsqlParameterCollection collection)
        {
            if (source is JobTypeSaga instance)
                ConvertJobType(collection, instance);
            else
                AssignParameters(source, collection);
        }

        static void ConvertJobType(NpgsqlParameterCollection collection, JobTypeSaga instance)
        {
            collection.Add("@correlationid", NpgsqlDbType.Uuid).Value = instance.CorrelationId;
            collection.Add("@name", NpgsqlDbType.Varchar, 255).Value = instance.Name;
            collection.Add("@currentstate", NpgsqlDbType.Integer).Value = instance.CurrentState;
            collection.Add("@activejobcount", NpgsqlDbType.Integer).Value = instance.ActiveJobCount;
            collection.Add("@concurrentjoblimit", NpgsqlDbType.Integer).Value = instance.ConcurrentJobLimit;
            collection.Add("@overridejoblimit", NpgsqlDbType.Integer).Value = instance.OverrideJobLimit.OrDbNull();
            collection.Add("@overridelimitexpiration", NpgsqlDbType.Timestamp).Value = instance.OverrideLimitExpiration.StripKind().OrDbNull();
            collection.Add("@globalconcurrentjoblimit", NpgsqlDbType.Integer).Value = instance.GlobalConcurrentJobLimit.OrDbNull();
            collection.Add("@activejobs", NpgsqlDbType.Jsonb).Value = instance.ActiveJobs.ToJson().OrDbNull();
            collection.Add("@instances", NpgsqlDbType.Jsonb).Value = instance.Instances.ToJson().OrDbNull();
            collection.Add("@properties", NpgsqlDbType.Jsonb).Value = instance.Properties.ToJson().OrDbNull();
        }
    }
}
