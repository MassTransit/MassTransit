namespace MassTransit.Transports.OnRamp.StatementProviders
{
    public class SqliteRepositoryNamingProvider : IRepositoryNamingProvider
    {
        public SqliteRepositoryNamingProvider(string schema)
        {
            Schema = schema;
        }

        public string Schema { get; }

        public string FormatColumnName(string columnName) => $"\"{columnName}\"";

        public string GetInitializationScript()
        {
            return
                $@"
CREATE TABLE IF NOT EXISTS {GetLocksTableName()}(
    ""OnRampName"" VARCHAR(120) NOT NULL,
    ""LockName"" VARCHAR(40) NOT NULL,
    PRIMARY KEY (""OnRampName"",""LockName"")
) WITHOUT ROWID;

CREATE TABLE IF NOT EXISTS {GetMessagesTableName()}(
    ""OnRampName"" VARCHAR(120) NOT NULL,
    ""Id"" BLOB NOT NULL,
    ""InstanceId"" VARCHAR(100) NULL,
    ""Retries"" INT NOT NULL,
    ""SerializedMessage"" TEXT NOT NULL,
    ""Added"" BIGINT NOT NULL,
    PRIMARY KEY (""OnRampName"",""Id"")
) WITHOUT ROWID;

CREATE TABLE IF NOT EXISTS {GetSweepersTableName()}(
    ""OnRampName"" VARCHAR(120) NOT NULL,
    ""InstanceId"" VARCHAR(100) NOT NULL,
    ""LastCheckinTime"" BIGINT NOT NULL,
    ""CheckinInterval"" BIGINT NOT NULL,
    PRIMARY KEY (""OnRampName"",""InstanceId"")
) WITHOUT ROWID;";
        }

        public string GetLocksTableName() => $"{Schema}_Locks";

        public string GetMessagesTableName() => $"{Schema}_Messages";

        public string GetSweepersTableName() => $"{Schema}_Sweepers";
    }
}
