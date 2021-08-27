namespace MassTransit.Transports.OnRamp.StatementProviders
{
    public class SqlServerRepositoryNamingProvider : IRepositoryNamingProvider
    {
        public SqlServerRepositoryNamingProvider(string schema)
        {
            Schema = schema;
        }

        public string Schema { get; }

        public string FormatColumnName(string columnName) => $"[{columnName}]";

        public string GetInitializationScript()
        {
            return
                $@"
IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = '{Schema}')
BEGIN
	EXEC('CREATE SCHEMA [{Schema}]')
END;

IF OBJECT_ID(N'{GetLocksTableName()}',N'U') IS NULL
BEGIN
CREATE TABLE {GetLocksTableName()}(
	[OnRampName] [nvarchar](120) NOT NULL,
	[LockName] [nvarchar](40) NOT NULL,
 CONSTRAINT [PK_{GetLocksTableName()}] PRIMARY KEY CLUSTERED 
(
	[OnRampName] ASC,
	[LockName] ASC
))
END;

IF OBJECT_ID(N'{GetMessagesTableName()}',N'U') IS NULL
BEGIN
CREATE TABLE {GetMessagesTableName()}(
	[OnRampName] [nvarchar](120) NOT NULL,
	[Id] [uniqueidentifier] NOT NULL,
	[InstanceId] [nvarchar](100) NULL,
	[Retries] [int] NOT NULL,
	[SerializedMessage] [nvarchar](max) NOT NULL,
	[Added] [bigint] NOT NULL,
 CONSTRAINT [PK_{GetMessagesTableName()}] PRIMARY KEY CLUSTERED 
(
	[OnRampName] ASC,
	[Id] ASC
))
END;

IF OBJECT_ID(N'{GetSweepersTableName()}',N'U') IS NULL
BEGIN
CREATE TABLE {GetSweepersTableName()}(
	[OnRampName] [nvarchar](120) NOT NULL,
	[InstanceId] [nvarchar](100) NOT NULL,
	[LastCheckinTime] [bigint] NOT NULL,
	[CheckinInterval] [bigint] NOT NULL,
 CONSTRAINT [PK_{GetSweepersTableName()}] PRIMARY KEY CLUSTERED 
(
	[OnRampName] ASC,
	[InstanceId] ASC
))
END;";
        }

        public string GetLocksTableName() => $"[{Schema}].[Locks]";

        public string GetMessagesTableName() => $"[{Schema}].[Messages]";

        public string GetSweepersTableName() => $"[{Schema}].[Sweepers]";
    }
}
