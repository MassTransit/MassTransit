namespace MassTransit.Persistence.MySql.Components.JobConsumers
{
    using System.Data;
    using System.Linq.Expressions;
    using Connections;
    using Extensions;
    using MySqlConnector;


    public class JobTypeSagaDatabaseContext : PessimisticMySqlDatabaseContext<JobTypeSaga>
    {
        public JobTypeSagaDatabaseContext(string connectionString, IsolationLevel isolationLevel)
            : base(connectionString, "JobTypes", nameof(ISaga.CorrelationId), isolationLevel)
        {
            Ignore(m => m.RowVersion);
            Ignore(m => m.Version);
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
            if (dataReader is not MySqlDataReader reader)
                throw new NotSupportedException("ConvertFrom only supports MySqlDataReader");

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

        protected override Action<object?, MySqlParameterCollection> CreateWriterAdapter()
        {
            return ConvertTo;
        }

        static void ConvertTo(object? source, MySqlParameterCollection collection)
        {
            if (source is JobTypeSaga instance)
                ConvertJobType(collection, instance);
            else
                AssignParameters(source, collection);
        }

        static void ConvertJobType(MySqlParameterCollection collection, JobTypeSaga instance)
        {
            collection.Add("@correlationid", MySqlDbType.Guid).Value = instance.CorrelationId.ToByteArray();
            collection.Add("@name", MySqlDbType.VarChar, 255).Value = instance.Name;
            collection.Add("@currentstate", MySqlDbType.Int32).Value = instance.CurrentState;
            collection.Add("@activejobcount", MySqlDbType.Int32).Value = instance.ActiveJobCount;
            collection.Add("@concurrentjoblimit", MySqlDbType.Int32).Value = instance.ConcurrentJobLimit;
            collection.Add("@overridejoblimit", MySqlDbType.Int32).Value = instance.OverrideJobLimit.OrDbNull();
            collection.Add("@overridelimitexpiration", MySqlDbType.DateTime).Value = instance.OverrideLimitExpiration.OrDbNull();
            collection.Add("@globalconcurrentjoblimit", MySqlDbType.Int32).Value = instance.GlobalConcurrentJobLimit.OrDbNull();
            collection.Add("@activejobs", MySqlDbType.Text).Value = instance.ActiveJobs.ToJson().OrDbNull();
            collection.Add("@instances", MySqlDbType.Text).Value = instance.Instances.ToJson().OrDbNull();
            collection.Add("@properties", MySqlDbType.Text).Value = instance.Properties.ToJson().OrDbNull();
        }
    }
}
