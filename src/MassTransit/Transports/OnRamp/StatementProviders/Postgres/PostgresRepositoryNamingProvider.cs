namespace MassTransit.Transports.OnRamp.StatementProviders
{
    public class PostgresRepositoryNamingProvider : IRepositoryNamingProvider
    {
        public PostgresRepositoryNamingProvider(string schema)
        {
            Schema = schema;
        }

        public string Schema { get; }

        public string FormatColumnName(string columnName) => $"\"{columnName}\"";

        public string GetInitializationScript()
        {
            return
                $@"
CREATE SCHEMA IF NOT EXISTS ""{Schema}"";

CREATE TABLE IF NOT EXISTS {GetLocksTableName()}(
    ""OnRampName"" VARCHAR(120) NOT NULL,
    ""LockName"" VARCHAR(40) NOT NULL,
    PRIMARY KEY (""OnRampName"",""LockName"")
);

CREATE TABLE IF NOT EXISTS {GetMessagesTableName()}(
    ""OnRampName"" VARCHAR(120) NOT NULL,
    ""Id"" UUID NOT NULL,
    ""InstanceId"" VARCHAR(100) NULL,
    ""Retries"" INT NOT NULL,
    ""SerializedMessage"" TEXT NOT NULL,
    ""Added"" BIGINT NOT NULL,
    PRIMARY KEY (""OnRampName"",""Id"")
);

CREATE TABLE IF NOT EXISTS {GetSweepersTableName()}(
    ""OnRampName"" VARCHAR(120) NOT NULL,
    ""InstanceId"" VARCHAR(100) NOT NULL,
    ""LastCheckinTime"" BIGINT NOT NULL,
    ""CheckinInterval"" BIGINT NOT NULL,
    PRIMARY KEY (""OnRampName"",""InstanceId"")
);";
        }

        public string GetLocksTableName() => $"\"{Schema}\".\"Locks\"";

        public string GetMessagesTableName() => $"\"{Schema}\".\"Messages\"";

        public string GetSweepersTableName() => $"\"{Schema}\".\"Sweepers\"";
    }
}
