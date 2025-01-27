namespace MassTransit.SqlTransport.SqlServer
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Dapper;
    using Microsoft.Extensions.Logging;


    public class SqlServerDatabaseMigrator :
        ISqlTransportDatabaseMigrator
    {
        const string DbExistsSql = @"SELECT [database_id] from [sys].[databases] WHERE name = '{0}'";
        const string DbCreateSql = @"CREATE DATABASE [{0}]";

        const string SchemaCreateSql = @"USE [{0}];
IF (SCHEMA_ID('{1}') IS NULL)
BEGIN
    EXEC('CREATE SCHEMA [{1}] AUTHORIZATION [dbo]')
END";

        const string DropSql = @"USE master;
ALTER DATABASE [{0}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
DROP DATABASE [{0}];";

        const string RoleExistsSql = @"SELECT DATABASE_PRINCIPAL_ID('{0}')";
        const string CreateRoleSql = @"CREATE ROLE {0} AUTHORIZATION [dbo]";

        const string GrantRoleSql = @"IF NOT EXISTS (
    SELECT 1
    FROM sys.schemas s
    INNER JOIN sys.database_principals p ON s.principal_id = p.principal_id
    WHERE s.name = '{1}' AND p.name = '{0}'
)
OR NOT EXISTS (
    SELECT 1
    FROM sys.database_permissions dp
    WHERE dp.grantee_principal_id = DATABASE_PRINCIPAL_ID('{0}')
      AND dp.type IN (
          'CRTB' -- CREATE TABLE
        , 'CRPR' -- CREATE PROCEDURE
        , 'CRVW' -- CREATE VIEW
        , 'RF'   -- REFERENCES
      )
)
BEGIN
    ALTER AUTHORIZATION ON SCHEMA::{1} TO [{0}];
    GRANT CREATE TABLE TO {0};
    GRANT CREATE PROCEDURE TO {0};
    GRANT CREATE VIEW TO {0};
    GRANT REFERENCES TO {0};
END
";

        const string LoginExistsSql = @"SELECT 1 FROM sys.sql_logins WHERE [name] = '{0}'";
        const string CreateLoginSql = @"CREATE LOGIN {0} WITH PASSWORD = '{1}';";

        const string CreateUserSql = @"
IF ORIGINAL_LOGIN() != '{1}' OR CURRENT_USER = '{1}'
BEGIN
    CREATE USER [{1}] FOR LOGIN [{1}] WITH DEFAULT_SCHEMA = [{0}]
END
";

        const string IsRoleMemberSql = @"
IF ORIGINAL_LOGIN() = '{1}' AND CURRENT_USER = 'dbo'
BEGIN
    SELECT 1
END
ELSE
BEGIN
    SELECT IS_ROLEMEMBER('{0}', '{1}')
END
";

        const string AddRoleMemberSql = @"USE [{0}];
IF ORIGINAL_LOGIN() = '{1}' AND CURRENT_USER = 'dbo'
BEGIN
    EXEC sp_addrolemember '{2}', 'dbo';
END
ELSE
BEGIN
    EXEC sp_addrolemember '{2}', '{1}';
END
";

        const string CreateInfrastructureSql = @"
IF OBJECT_ID('{0}.TopologySequence', 'SO') IS NULL
BEGIN
    CREATE SEQUENCE [{0}].[TopologySequence] AS BIGINT START WITH 1 INCREMENT BY 1
END;

IF OBJECT_ID('{0}.Queue', 'U') IS NULL
BEGIN
    CREATE TABLE {0}.Queue
    (
        Id               bigint          not null primary key default next value for [{0}].[TopologySequence],
        Updated          datetime2       not null default SYSUTCDATETIME(),

        Name             nvarchar(256)   not null,
        Type             tinyint         not null,
        AutoDelete       integer,
        MaxDeliveryCount integer         not null DEFAULT 10
    )
END;

IF COL_LENGTH('{0}.Queue', 'MaxDeliveryCount') IS NULL
BEGIN
    ALTER TABLE {0}.Queue ADD MaxDeliveryCount integer not null DEFAULT 10;
END

IF NOT EXISTS(SELECT TOP 1 1 FROM sys.indexes indexes
    INNER JOIN sys.objects objects ON indexes.object_id = objects.object_id
    WHERE indexes.name ='IX_Queue_Name_Type' AND objects.name = 'Queue')
BEGIN
    CREATE INDEX IX_Queue_Name_Type ON {0}.Queue (Name, Type) INCLUDE (Id);
END;

IF NOT EXISTS(SELECT TOP 1 1 FROM sys.indexes indexes
    INNER JOIN sys.objects objects ON indexes.object_id = objects.object_id
    WHERE indexes.name ='IX_Queue_AutoDelete' AND objects.name = 'Queue')
BEGIN
    CREATE INDEX IX_Queue_AutoDelete ON {0}.Queue (AutoDelete) INCLUDE (Id);
END;

IF OBJECT_ID('{0}.Topic', 'U') IS NULL
BEGIN
    CREATE TABLE {0}.Topic
    (
        Id          bigint          not null primary key default next value for [{0}].[TopologySequence],
        Updated     datetime2       not null default SYSUTCDATETIME(),

        Name        nvarchar(256) not null
    )
END;

IF NOT EXISTS(SELECT TOP 1 1 FROM sys.indexes indexes
    INNER JOIN sys.objects objects ON indexes.object_id = objects.object_id
    WHERE indexes.name ='IX_Topic_Name' AND objects.name = 'Topic')
BEGIN
    CREATE INDEX IX_Topic_Name ON {0}.Topic (Name) INCLUDE (Id);
END;

IF OBJECT_ID('{0}.TopicSubscription', 'U') IS NULL
BEGIN
    CREATE TABLE {0}.TopicSubscription
    (
        Id             bigint          not null primary key default next value for [{0}].[TopologySequence],
        Updated        datetime2       not null default SYSUTCDATETIME(),

        SourceId       bigint          not null references {0}.Topic (Id),
        DestinationId  bigint          not null references {0}.Topic (Id),

        SubType        tinyint         not null,
        RoutingKey     nvarchar(256)   not null,
        Filter         nvarchar(1024)  not null
    );
END;

IF NOT EXISTS(SELECT TOP 1 1 FROM sys.indexes indexes
    INNER JOIN sys.objects objects ON indexes.object_id = objects.object_id
    WHERE indexes.name ='IX_TopicSubscription_Unique' AND objects.name = 'TopicSubscription')
BEGIN
    CREATE UNIQUE INDEX IX_TopicSubscription_Unique ON {0}.TopicSubscription (SourceId, DestinationId, SubType, RoutingKey, Filter);
END;

IF NOT EXISTS(SELECT TOP 1 1 FROM sys.indexes indexes
    INNER JOIN sys.objects objects ON indexes.object_id = objects.object_id
    WHERE indexes.name ='IX_TopicSubscription_Source' AND objects.name = 'TopicSubscription')
BEGIN
    CREATE INDEX IX_TopicSubscription_Source ON {0}.TopicSubscription (SourceId) INCLUDE (Id, DestinationId, SubType, RoutingKey, Filter);
END;

IF NOT EXISTS(SELECT TOP 1 1 FROM sys.indexes indexes
    INNER JOIN sys.objects objects ON indexes.object_id = objects.object_id
    WHERE indexes.name ='IX_TopicSubscription_Destination' AND objects.name = 'TopicSubscription')
BEGIN
    CREATE INDEX IX_TopicSubscription_Destination ON {0}.TopicSubscription (DestinationId) INCLUDE (Id, SourceId, SubType, RoutingKey, Filter);
END;

IF OBJECT_ID('{0}.DELETE_Topic', 'TR') IS NULL
BEGIN
    EXEC('
    CREATE TRIGGER [{0}].[DELETE_Topic] ON {0}.Topic INSTEAD OF DELETE
    AS
    BEGIN
         SET NOCOUNT ON;
         DELETE FROM [{0}].[TopicSubscription] WHERE SourceId IN (SELECT Id FROM DELETED);
         DELETE FROM [{0}].[TopicSubscription] WHERE DestinationId IN (SELECT Id FROM DELETED);
         DELETE FROM [{0}].[Topic] WHERE Id IN (SELECT Id FROM DELETED);
    END;
    ');
END;

IF OBJECT_ID('{0}.QueueSubscription', 'U') IS NULL
BEGIN
    CREATE TABLE {0}.QueueSubscription
    (
        Id             bigint          not null primary key default next value for [{0}].[TopologySequence],
        Updated        datetime2       not null default SYSUTCDATETIME(),

        SourceId       bigint          not null references {0}.Topic (Id) ON DELETE CASCADE,
        DestinationId  bigint          not null references {0}.Queue (Id) ON DELETE CASCADE,

        SubType        tinyint         not null,
        RoutingKey     nvarchar(256)   not null,
        Filter         nvarchar(1024)  not null
    );
END;

IF NOT EXISTS(SELECT TOP 1 1 FROM sys.indexes indexes
    INNER JOIN sys.objects objects ON indexes.object_id = objects.object_id
    WHERE indexes.name ='IX_QueueSubscription_Unique' AND objects.name = 'QueueSubscription')
BEGIN
    CREATE UNIQUE INDEX IX_QueueSubscription_Unique ON {0}.QueueSubscription (SourceId, DestinationId, SubType, RoutingKey, Filter);
END;

IF NOT EXISTS(SELECT TOP 1 1 FROM sys.indexes indexes
    INNER JOIN sys.objects objects ON indexes.object_id = objects.object_id
    WHERE indexes.name ='IX_QueueSubscription_Source' AND objects.name = 'QueueSubscription')
BEGIN
    CREATE INDEX IX_QueueSubscription_Source ON {0}.QueueSubscription (SourceId) INCLUDE (Id, DestinationId, SubType, RoutingKey, Filter);
END;

IF NOT EXISTS(SELECT TOP 1 1 FROM sys.indexes indexes
    INNER JOIN sys.objects objects ON indexes.object_id = objects.object_id
    WHERE indexes.name ='IX_QueueSubscription_Destination' AND objects.name = 'QueueSubscription')
BEGIN
    CREATE INDEX IX_QueueSubscription_Destination ON {0}.QueueSubscription (DestinationId) INCLUDE (Id, SourceId, SubType, RoutingKey, Filter);
END;

IF OBJECT_ID('{0}.Message', 'U') IS NULL
BEGIN
    CREATE TABLE {0}.Message
    (
        TransportMessageId  uniqueidentifier  not null primary key,

        ContentType         nvarchar(max),
        MessageType         nvarchar(max),
        Body                nvarchar(max),
        BinaryBody          varbinary(max),

        MessageId           uniqueidentifier,
        CorrelationId       uniqueidentifier,
        ConversationId      uniqueidentifier,
        RequestId           uniqueidentifier,
        InitiatorId         uniqueidentifier,
        SourceAddress       nvarchar(max),
        DestinationAddress  nvarchar(max),
        ResponseAddress     nvarchar(max),
        FaultAddress        nvarchar(max),

        SentTime            datetime2 NOT NULL DEFAULT SYSUTCDATETIME(),

        Headers             nvarchar(max),
        Host                nvarchar(max),

        SchedulingTokenId   uniqueidentifier
    );
END;

IF NOT EXISTS(SELECT TOP 1 1 FROM sys.indexes indexes
    INNER JOIN sys.objects objects ON indexes.object_id = objects.object_id
    WHERE indexes.name ='IX_Message_SchedulingTokenId' AND objects.name = 'Message')
BEGIN
    CREATE INDEX IX_Message_SchedulingTokenId ON {0}.Message (SchedulingTokenId) where Message.SchedulingTokenId IS NOT NULL;
END;

IF OBJECT_ID('{0}.DeliverySequence', 'SO') IS NULL
BEGIN
    CREATE SEQUENCE [{0}].[DeliverySequence] AS BIGINT START WITH 1 INCREMENT BY 1
END;

IF OBJECT_ID('{0}.MessageDelivery', 'U') IS NULL
BEGIN
    CREATE TABLE {0}.MessageDelivery
    (
        MessageDeliveryId  bigint               not null primary key default next value for [{0}].[DeliverySequence],

        TransportMessageId  uniqueidentifier    not null REFERENCES {0}.Message ON DELETE CASCADE,
        QueueId             bigint              not null,

        Priority            smallint            not null,
        EnqueueTime         datetime2           not null,
        ExpirationTime      datetime2,

        PartitionKey        nvarchar(128),
        RoutingKey          nvarchar(256),

        ConsumerId          uniqueidentifier,
        LockId              uniqueidentifier,

        DeliveryCount       int                 not null,
        MaxDeliveryCount    int                 not null,
        LastDelivered       datetime2,
        TransportHeaders    nvarchar(max)
    );
END;

IF NOT EXISTS(SELECT TOP 1 1 FROM sys.indexes indexes
    INNER JOIN sys.objects objects ON indexes.object_id = objects.object_id
    WHERE indexes.name ='IX_MessageDelivery_Fetch' AND objects.name = 'MessageDelivery')
BEGIN
    CREATE INDEX IX_MessageDelivery_Fetch ON {0}.MessageDelivery (QueueId, Priority, EnqueueTime, MessageDeliveryId);
END;

IF NOT EXISTS(SELECT TOP 1 1 FROM sys.indexes indexes
    INNER JOIN sys.objects objects ON indexes.object_id = objects.object_id
    WHERE indexes.name ='IX_MessageDelivery_FetchPart' AND objects.name = 'MessageDelivery')
BEGIN
    CREATE INDEX IX_MessageDelivery_FetchPart ON {0}.MessageDelivery (QueueId, PartitionKey, Priority, EnqueueTime, MessageDeliveryId);
END;

IF NOT EXISTS(SELECT TOP 1 1 FROM sys.indexes indexes
    INNER JOIN sys.objects objects ON indexes.object_id = objects.object_id
    WHERE indexes.name ='IX_MessageDelivery_TransportMessageId' AND objects.name = 'MessageDelivery')
BEGIN
    CREATE INDEX IX_MessageDelivery_TransportMessageId ON {0}.MessageDelivery (TransportMessageId);
END;

IF OBJECT_ID('{0}.QueueMetricCapture', 'U') IS NULL
BEGIN
    CREATE TABLE {0}.QueueMetricCapture
    (
        Id              bigint              not null identity(1,1),

        Captured        datetime2           not null,
        QueueId         bigint              not null,
        ConsumeCount    bigint              not null,
        ErrorCount      bigint              not null,
        DeadLetterCount bigint              not null,

        CONSTRAINT [PK_QueueMetricCapture] PRIMARY KEY CLUSTERED
        (
            [Id] ASC
        ) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
    );
END;

IF OBJECT_ID('{0}.QueueMetric', 'U') IS NULL
BEGIN
    CREATE TABLE {0}.QueueMetric
    (
        Id              bigint              not null identity(1,1),

        StartTime       datetime2           not null,
        Duration        int                 not null,
        QueueId         bigint              not null,
        ConsumeCount    bigint              not null,
        ErrorCount      bigint              not null,
        DeadLetterCount bigint              not null,

        CONSTRAINT [PK_QueueMetric] PRIMARY KEY CLUSTERED
        (
            [Id] ASC
        ) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
    );
END;

IF NOT EXISTS(SELECT TOP 1 1 FROM sys.indexes indexes
    INNER JOIN sys.objects objects ON indexes.object_id = objects.object_id
    WHERE indexes.name ='IX_QueueMetric_Unique' AND objects.name = 'QueueMetric')
BEGIN
    CREATE UNIQUE INDEX IX_QueueMetric_Unique ON {0}.QueueMetric (StartTime, Duration, QueueId);
END;
";

        const string SqlFnCreateQueue = @"
CREATE OR ALTER PROCEDURE {0}.CreateQueue
    @QueueName nvarchar(256),
    @AutoDelete integer = NULL
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @temp_table TABLE
    (
        Id bigint NOT NULL
    )
    INSERT INTO @temp_table
        EXEC {0}.CreateQueueV2 @QueueName, @AutoDelete
    SELECT TOP 1 Id FROM @temp_table
END";

        const string SqlFnCreateQueueV2 = @"
CREATE OR ALTER PROCEDURE {0}.CreateQueueV2
    @QueueName nvarchar(256),
    @AutoDelete integer = NULL,
    @MaxDeliveryCount integer = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @QueueName IS NULL OR LEN(@QueueName) < 1
    BEGIN
        THROW 50000, 'Queue name was null or empty', 1;
    END

    DECLARE @QueueTable table (Id BIGINT, Type tinyint)
    MERGE INTO {0}.Queue WITH (ROWLOCK) AS target
        USING (VALUES
                   (@QueueName, 1, @AutoDelete, @MaxDeliveryCount),
                   (@QueueName, 2, @AutoDelete, @MaxDeliveryCount),
                   (@QueueName, 3, @AutoDelete, @MaxDeliveryCount)
               ) AS source (Name, Type, AutoDelete, MaxDeliveryCount)
        ON (target.Name = source.Name AND target.Type = source.Type)
        WHEN MATCHED THEN UPDATE SET Updated = SYSUTCDATETIME(),
                                     AutoDelete = COALESCE(source.AutoDelete, target.AutoDelete),
                                     MaxDeliveryCount = COALESCE(@MaxDeliveryCount, source.MaxDeliveryCount, 10)
        WHEN NOT MATCHED THEN INSERT (Name, Type, AutoDelete, MaxDeliveryCount)
        VALUES (source.Name, source.Type, source.AutoDelete, COALESCE(@MaxDeliveryCount, 10))
        OUTPUT inserted.Id, inserted.Type INTO @QueueTable;

    SET NOCOUNT OFF
    SELECT TOP 1 Id FROM @QueueTable WHERE Type = 1;
END";

        const string SqlFnCreateTopic = @"
CREATE OR ALTER PROCEDURE {0}.CreateTopic
    @TopicName nvarchar(256)
AS
BEGIN
    SET NOCOUNT ON;

    IF @TopicName IS NULL OR LEN(@TopicName) < 1
    BEGIN
        THROW 50000, 'Topic name was null or empty', 1;
    END

    DECLARE @TopicTable table (Id BIGINT)
    MERGE INTO {0}.Topic WITH (ROWLOCK) AS target
        USING (VALUES (@TopicName)) AS source (Name)
        ON (target.Name = source.Name)
        WHEN MATCHED THEN UPDATE SET Updated = SYSUTCDATETIME()
        WHEN NOT MATCHED THEN INSERT (Name)
        VALUES (source.Name)
        OUTPUT inserted.Id INTO @TopicTable;

    SET NOCOUNT OFF
    SELECT TOP 1 Id FROM @TopicTable;
END;
";

        const string SqlFnCreateTopicSubscription = @"
CREATE OR ALTER PROCEDURE {0}.CreateTopicSubscription
    @SourceTopicName nvarchar(256),
    @DestinationTopicName nvarchar(256),
    @SubscriptionType tinyint = 1,
    @RoutingKey varchar(256) = '',
    @Filter varchar(1024) = '{{}}'
AS
BEGIN
    SET NOCOUNT ON;

    IF @SourceTopicName IS NULL OR LEN(@SourceTopicName) < 1
    BEGIN
        THROW 50000, 'Source topic name was null or empty', 1;
    END

    IF @DestinationTopicName IS NULL OR LEN(@DestinationTopicName) < 1
    BEGIN
        THROW 50000, 'Destination topic name was null or empty', 1;
    END

    DECLARE @SourceTopicId BIGINT
    SELECT @SourceTopicId = t.Id FROM {0}.Topic t WHERE t.Name = @SourceTopicName;
    IF @SourceTopicId IS NULL
    BEGIN
        THROW 50000, 'Source topic not found', 1;
    END

    DECLARE @DestinationTopicId BIGINT
    SELECT @DestinationTopicId = t.Id FROM {0}.Topic t WHERE t.Name = @DestinationTopicName;
    IF @DestinationTopicId IS NULL
    BEGIN
        THROW 50000, 'Destination topic not found', 1;
    END

    DECLARE @ResultTable table (Id BIGINT)
    MERGE INTO {0}.TopicSubscription WITH (ROWLOCK) AS target
        USING (VALUES (@SourceTopicId, @DestinationTopicId, @SubscriptionType, COALESCE(@RoutingKey, ''), COALESCE(@Filter, '{{}}')))
            AS source (SourceId, DestinationId, SubType, RoutingKey, Filter)
        ON (target.SourceId = source.SourceId AND target.DestinationId = source.DestinationId AND target.SubType = source.SubType
            AND target.RoutingKey = source.RoutingKey AND target.Filter = source.Filter)
        WHEN MATCHED THEN UPDATE SET Updated = SYSUTCDATETIME()
        WHEN NOT MATCHED THEN INSERT (SourceId, DestinationId, SubType, RoutingKey, Filter)
        VALUES (source.SourceId, source.DestinationId, source.SubType, source.RoutingKey, source.Filter)
        OUTPUT inserted.Id INTO @ResultTable;

    SET NOCOUNT OFF
    SELECT TOP 1 Id FROM @ResultTable;
END;
";

        const string SqlFnCreateQueueSubscription = @"
CREATE OR ALTER PROCEDURE {0}.CreateQueueSubscription
    @SourceTopicName nvarchar(256),
    @DestinationQueueName nvarchar(256),
    @SubscriptionType tinyint = 1,
    @RoutingKey varchar(256) = '',
    @Filter varchar(1024) = '{{}}'
AS
BEGIN
    SET NOCOUNT ON;

    IF @SourceTopicName IS NULL OR LEN(@SourceTopicName) < 1
    BEGIN
        THROW 50000, 'Source topic name was null or empty', 1;
    END

    IF @DestinationQueueName IS NULL OR LEN(@DestinationQueueName) < 1
    BEGIN
        THROW 50000, 'Destination queue name was null or empty', 1;
    END

    DECLARE @SourceTopicId BIGINT
    SELECT @SourceTopicId = t.Id FROM {0}.Topic t WHERE t.Name = @SourceTopicName;
    IF @SourceTopicId IS NULL
    BEGIN
        THROW 50000, 'Destination topic name was null or empty', 1;
    END

    DECLARE @DestinationQueueId BIGINT
    SELECT @DestinationQueueId = q.Id FROM {0}.Queue q WHERE q.Name = @DestinationQueueName AND q.Type = 1;
    IF @DestinationQueueId IS NULL
    BEGIN
        THROW 50000, 'Destination queue not found', 1;
    END

    DECLARE @ResultTable table (Id BIGINT)
    MERGE INTO {0}.QueueSubscription WITH (ROWLOCK) AS target
        USING (VALUES (@SourceTopicId, @DestinationQueueId, @SubscriptionType, COALESCE(@RoutingKey, ''), COALESCE(@Filter, '{{}}')))
            AS source (SourceId, DestinationId, SubType, RoutingKey, Filter)
        ON (target.SourceId = source.SourceId AND target.DestinationId = source.DestinationId AND target.SubType = source.SubType
            AND target.RoutingKey = source.RoutingKey AND target.Filter = source.Filter)
        WHEN MATCHED THEN UPDATE SET Updated = SYSUTCDATETIME()
        WHEN NOT MATCHED THEN INSERT (SourceId, DestinationId, SubType, RoutingKey, Filter)
        VALUES (source.SourceId, source.DestinationId, source.SubType, source.RoutingKey, source.Filter)
        OUTPUT inserted.Id INTO @ResultTable;

    SET NOCOUNT OFF
    SELECT TOP 1 Id FROM @ResultTable;
END;
";

        const string SqlFnPublish = @"
CREATE OR ALTER PROCEDURE {0}.PublishMessage
    @entityName varchar(256),
    @priority int = 100,
    @transportMessageId uniqueidentifier,
    @body nvarchar(max) = NULL,
    @binaryBody varbinary(max) = NULL,
    @contentType varchar(max) = NULL,
    @messageType varchar(max) = NULL,
    @messageId uniqueidentifier = NULL,
    @correlationId uniqueidentifier = NULL,
    @conversationId uniqueidentifier = NULL,
    @requestId uniqueidentifier = NULL,
    @initiatorId uniqueidentifier = NULL,
    @sourceAddress varchar(max) = NULL,
    @destinationAddress varchar(max) = NULL,
    @responseAddress varchar(max) = NULL,
    @faultAddress varchar(max) = NULL,
    @sentTime datetimeoffset = NULL,
    @headers nvarchar(max) = NULL,
    @host nvarchar(max) = NULL,
    @partitionKey nvarchar(128) = NULL,
    @routingKey nvarchar(256) = NULL,
    @delay int = 0,
    @schedulingTokenId uniqueidentifier = NULL,
    @maxDeliveryCount int = 10
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @vDeliveryCount bigint;
    EXEC @vDeliveryCount = {0}.PublishMessageV2
    @entityName,
    @priority,
    @transportMessageId,
    @body,
    @binaryBody,
    @contentType,
    @messageType,
    @messageId,
    @correlationId,
    @conversationId,
    @requestId,
    @initiatorId,
    @sourceAddress,
    @destinationAddress,
    @responseAddress,
    @faultAddress,
    @sentTime,
    NULL,
    @headers,
    @host,
    @partitionKey,
    @routingKey,
    @delay,
    @schedulingTokenId;

    RETURN @vDeliveryCount;
END;
";

        const string SqlFnPublishV2 = @"
CREATE OR ALTER PROCEDURE {0}.PublishMessageV2
    @entityName varchar(256),
    @priority int = 100,
    @transportMessageId uniqueidentifier,
    @body nvarchar(max) = NULL,
    @binaryBody varbinary(max) = NULL,
    @contentType varchar(max) = NULL,
    @messageType varchar(max) = NULL,
    @messageId uniqueidentifier = NULL,
    @correlationId uniqueidentifier = NULL,
    @conversationId uniqueidentifier = NULL,
    @requestId uniqueidentifier = NULL,
    @initiatorId uniqueidentifier = NULL,
    @sourceAddress varchar(max) = NULL,
    @destinationAddress varchar(max) = NULL,
    @responseAddress varchar(max) = NULL,
    @faultAddress varchar(max) = NULL,
    @sentTime datetimeoffset = NULL,
    @expirationTime datetimeoffset = NULL,
    @headers nvarchar(max) = NULL,
    @host nvarchar(max) = NULL,
    @partitionKey nvarchar(128) = NULL,
    @routingKey nvarchar(256) = NULL,
    @delay int = 0,
    @schedulingTokenId uniqueidentifier = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @vTopicId bigint;
    DECLARE @vRowCount bigint;
    DECLARE @vEnqueueTime datetimeoffset;
    DECLARE @vRow table (
        queueId bigint,
        transportMessageId uniqueidentifier
    );

    IF @entityName IS NULL OR LEN(@entityName) < 1
    BEGIN
        THROW 50000, 'Topic names must not be null or empty', 1;
    END;

    SELECT @vTopicId = t.Id
    FROM {0}.Topic t
    WHERE t.Name = @entityName;

    IF @vTopicId IS NULL
    BEGIN
        THROW 50000, 'Topic not found', 1;
    END;

    SET @vEnqueueTime = SYSUTCDATETIME();

    IF @delay > 0
    BEGIN
        SET @vEnqueueTime = DATEADD(SECOND, @delay, @vEnqueueTime);
    END;

    INSERT INTO {0}.Message (
        TransportMessageId, Body, BinaryBody, ContentType, MessageType, MessageId,
        CorrelationId, ConversationId, RequestId, InitiatorId,
        SourceAddress, DestinationAddress, ResponseAddress, FaultAddress,
        SentTime, Headers, Host, SchedulingTokenId
    )
    VALUES (
        @transportMessageId, @body, @binaryBody, @contentType, @messageType, @messageId,
        @correlationId, @conversationId, @requestId, @initiatorId,
        @sourceAddress, @destinationAddress, @responseAddress, @faultAddress,
        @sentTime, @headers, @host, @schedulingTokenId
    );

    ;WITH Fabric AS (
        SELECT ts.SourceId, ts.DestinationId
        FROM {0}.Topic t
        LEFT JOIN {0}.TopicSubscription ts ON t.Id = ts.SourceId
            AND (
                (ts.SubType = 1)
                OR (ts.SubType = 2 AND @routingKey = ts.RoutingKey)
                OR (ts.SubType = 3 AND @routingKey LIKE ts.RoutingKey)
            )
        WHERE t.Id = @vTopicId

        UNION ALL

        SELECT ts.SourceId, ts.DestinationId
        FROM {0}.TopicSubscription ts
        JOIN Fabric ON ts.SourceId = fabric.DestinationId
        WHERE
            (ts.SubType = 1)
            OR (ts.SubType = 2 AND @routingKey = ts.RoutingKey)
            OR (ts.SubType = 3 AND @routingKey LIKE ts.RoutingKey)
    )
    INSERT INTO {0}.MessageDelivery (QueueId, TransportMessageId, Priority, EnqueueTime, ExpirationTime, DeliveryCount, MaxDeliveryCount, PartitionKey, RoutingKey)
    OUTPUT inserted.QueueId, inserted.TransportMessageId INTO @vRow
    SELECT DISTINCT qs.DestinationId, @transportMessageId, @priority, @vEnqueueTime, @expirationTime, 0, q.MaxDeliveryCount, @partitionKey, @routingKey
    FROM {0}.QueueSubscription qs
    JOIN Fabric ON (qs.SourceId = fabric.DestinationId OR qs.SourceId = @vTopicId)
        AND (  (qs.SubType = 1)
            OR (qs.SubType = 2 AND @routingKey = qs.RoutingKey)
            OR (qs.SubType = 3 AND @routingKey LIKE qs.RoutingKey))
    JOIN {0}.Queue q ON q.Id = qs.DestinationId;

    SELECT @vRowCount = COUNT(*) FROM @vRow;

    IF @vRowCount = 0
    BEGIN
        DELETE FROM {0}.Message WHERE TransportMessageId = @transportMessageId;
    END;

    RETURN @vRowCount;
END;
";

        const string SqlFnSend = @"
CREATE OR ALTER PROCEDURE {0}.SendMessage
    @entityName varchar(256),
    @priority int = 100,
    @transportMessageId uniqueidentifier,
    @body nvarchar(max) = NULL,
    @binaryBody varbinary(max) = NULL,
    @contentType varchar(max) = NULL,
    @messageType varchar(max) = NULL,
    @messageId uniqueidentifier = NULL,
    @correlationId uniqueidentifier = NULL,
    @conversationId uniqueidentifier = NULL,
    @requestId uniqueidentifier = NULL,
    @initiatorId uniqueidentifier = NULL,
    @sourceAddress varchar(max) = NULL,
    @destinationAddress varchar(max) = NULL,
    @responseAddress varchar(max) = NULL,
    @faultAddress varchar(max) = NULL,
    @sentTime datetimeoffset = NULL,
    @headers nvarchar(max) = NULL,
    @host nvarchar(max) = NULL,
    @partitionKey nvarchar(128) = NULL,
    @routingKey nvarchar(256) = NULL,
    @delay int = 0,
    @schedulingTokenId uniqueidentifier = NULL,
    @maxDeliveryCount int = 10
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @vDeliveryId bigint;
    EXEC @vDeliveryId = {0}.SendMessageV2
    @entityName,
    @priority,
    @transportMessageId,
    @body,
    @binaryBody,
    @contentType,
    @messageType,
    @messageId,
    @correlationId,
    @conversationId,
    @requestId,
    @initiatorId,
    @sourceAddress,
    @destinationAddress,
    @responseAddress,
    @faultAddress,
    @sentTime,
    NULL,
    @headers,
    @host,
    @partitionKey,
    @routingKey,
    @delay,
    @schedulingTokenId;

    RETURN @vDeliveryId;
END;
";

        const string SqlFnSendV2 = @"
CREATE OR ALTER PROCEDURE {0}.SendMessageV2
    @entityName varchar(256),
    @priority int = 100,
    @transportMessageId uniqueidentifier,
    @body nvarchar(max) = NULL,
    @binaryBody varbinary(max) = NULL,
    @contentType varchar(max) = NULL,
    @messageType varchar(max) = NULL,
    @messageId uniqueidentifier = NULL,
    @correlationId uniqueidentifier = NULL,
    @conversationId uniqueidentifier = NULL,
    @requestId uniqueidentifier = NULL,
    @initiatorId uniqueidentifier = NULL,
    @sourceAddress varchar(max) = NULL,
    @destinationAddress varchar(max) = NULL,
    @responseAddress varchar(max) = NULL,
    @faultAddress varchar(max) = NULL,
    @sentTime datetimeoffset = NULL,
    @expirationTime datetimeoffset = NULL,
    @headers nvarchar(max) = NULL,
    @host nvarchar(max) = NULL,
    @partitionKey nvarchar(128) = NULL,
    @routingKey nvarchar(256) = NULL,
    @delay int = 0,
    @schedulingTokenId uniqueidentifier = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @vDeliveryId bigint;
    DECLARE @vQueueId bigint;
    DECLARE @vMaxDeliveryCount int;
    DECLARE @vEnqueueTime datetimeoffset;

    IF @entityName IS NULL OR LEN(@entityName) < 1
    BEGIN
        THROW 50000, 'Queue names must not be null or empty', 1;
    END;

    SELECT @vQueueId = q.Id, @vMaxDeliveryCount = q.MaxDeliveryCount FROM {0}.Queue q WHERE q.Name = @entityName AND q.type = 1;
    IF @vQueueId IS NULL
    BEGIN
        THROW 50000, 'Queue not found', 1;
    END;

    SET @vEnqueueTime = SYSUTCDATETIME();

    IF @delay > 0
    BEGIN
        SET @vEnqueueTime = DATEADD(SECOND, @delay, @vEnqueueTime);
    END;

    INSERT INTO {0}.Message (
        TransportMessageId, Body, BinaryBody, ContentType, MessageType, MessageId,
        CorrelationId, ConversationId, RequestId, InitiatorId,
        SourceAddress, DestinationAddress, ResponseAddress, FaultAddress,
        SentTime, Headers, Host, SchedulingTokenId
    )
    VALUES (
        @transportMessageId, @body, @binaryBody, @contentType, @messageType, @messageId,
        @correlationId, @conversationId, @requestId, @initiatorId,
        @sourceAddress, @destinationAddress, @responseAddress, @faultAddress,
        @sentTime, @headers, @host, @schedulingTokenId
    );

    INSERT INTO {0}.MessageDelivery (QueueId, TransportMessageId, Priority, EnqueueTime, ExpirationTime, DeliveryCount, MaxDeliveryCount, PartitionKey, RoutingKey)
    VALUES (@vQueueId, @transportMessageId, @priority, @vEnqueueTime, @expirationTime, 0, @vMaxDeliveryCount, @partitionKey, @routingKey);
    SELECT @vDeliveryId = SCOPE_IDENTITY();

    RETURN @vDeliveryId;
END;
";

        const string SqlFnFetchMessages = @"
CREATE OR ALTER PROCEDURE {0}.FetchMessages
    @queueName varchar(256),
    @consumerId uniqueidentifier,
    @lockId uniqueidentifier,
    @lockDuration int,
    @fetchCount int = 1
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @queueId bigint;
    DECLARE @enqueueTime datetime2;
    DECLARE @now datetime2;

    SELECT @queueId = q.Id
    FROM {0}.Queue q
    WHERE q.Name = @queueName AND q.Type = 1;

    IF @queueId IS NULL
    BEGIN
        THROW 50000, 'Queue not found', 1;
    END;

    IF @lockDuration <= 0
    BEGIN
        THROW 50000, 'Invalid lock duration', 1;
    END;

    SET @now = SYSUTCDATETIME();
    SET @enqueueTime = DATEADD(SECOND, @lockDuration, @now);

    DECLARE @ResultTable TABLE (
        TransportMessageId uniqueidentifier,
        QueueId bigint,
        Priority smallint,
        MessageDeliveryId bigint,
        ConsumerId uniqueidentifier,
        LockId uniqueidentifier,
        EnqueueTime datetime2,
        ExpirationTime datetime2,
        DeliveryCount int,
        PartitionKey text,
        RoutingKey text,
        TransportHeaders nvarchar(max),
        ContentType text,
        MessageType text,
        Body nvarchar(max),
        BinaryBody varbinary(max),
        MessageId uniqueidentifier,
        CorrelationId uniqueidentifier,
        ConversationId uniqueidentifier,
        RequestId uniqueidentifier,
        InitiatorId uniqueidentifier,
        SourceAddress text,
        DestinationAddress text,
        ResponseAddress text,
        FaultAddress text,
        SentTime datetime2,
        Headers nvarchar(max),
        Host nvarchar(max)
    );

    WITH msgs AS (
        SELECT
            md.*
        FROM
            {0}.MessageDelivery md WITH (ROWLOCK, READPAST, UPDLOCK)
        WHERE
            md.QueueId = @queueId
            AND md.EnqueueTime <= @now
            AND md.DeliveryCount < md.MaxDeliveryCount
        ORDER BY
            md.Priority ASC,
            md.EnqueueTime ASC,
            md.MessageDeliveryId ASC
        OFFSET 0 ROWS
        FETCH NEXT @fetchCount ROWS ONLY
    )
    UPDATE dm
    SET
        DeliveryCount = dm.DeliveryCount + 1,
        LastDelivered = @now,
        ConsumerId = @consumerId,
        LockId = @lockId,
        EnqueueTime = @enqueueTime
    OUTPUT
        inserted.TransportMessageId,
        inserted.QueueId,
        inserted.Priority,
        inserted.MessageDeliveryId,
        inserted.ConsumerId,
        inserted.LockId,
        inserted.EnqueueTime,
        inserted.ExpirationTime,
        inserted.DeliveryCount,
        inserted.PartitionKey,
        inserted.RoutingKey,
        inserted.TransportHeaders,
        m.ContentType,
        m.MessageType,
        m.Body,
        m.BinaryBody,
        m.MessageId,
        m.CorrelationId,
        m.ConversationId,
        m.RequestId,
        m.InitiatorId,
        m.SourceAddress,
        m.DestinationAddress,
        m.ResponseAddress,
        m.FaultAddress,
        m.SentTime,
        m.Headers,
        m.Host
    INTO
        @ResultTable (
            TransportMessageId ,
            QueueId ,
            Priority ,
            MessageDeliveryId ,
            ConsumerId ,
            LockId ,
            EnqueueTime ,
            ExpirationTime ,
            DeliveryCount ,
            PartitionKey ,
            RoutingKey ,
            TransportHeaders,
            ContentType ,
            MessageType ,
            Body,
            BinaryBody,
            MessageId ,
            CorrelationId ,
            ConversationId ,
            RequestId ,
            InitiatorId ,
            SourceAddress ,
            DestinationAddress ,
            ResponseAddress ,
            FaultAddress ,
            SentTime ,
            Headers,
            Host
        )
    FROM
        {0}.MessageDelivery dm
        INNER JOIN msgs ON dm.MessageDeliveryId = msgs.MessageDeliveryId
        INNER JOIN {0}.Message m ON msgs.TransportMessageId = m.TransportMessageId;

    SELECT * FROM @ResultTable;
END";

        const string SqlFnFetchMessagesPartitioned = @"
CREATE OR ALTER PROCEDURE {0}.FetchMessagesPartitioned
    @queueName varchar(256),
    @consumerId uniqueidentifier,
    @lockId uniqueidentifier,
    @lockDuration int,
    @fetchCount int = 1,
    @concurrentCount int = 1,
    @ordered int = 0
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @queueId bigint;
    DECLARE @enqueueTime datetime2;
    DECLARE @now datetime2;

    SELECT @queueId = q.Id
    FROM {0}.Queue q
    WHERE q.Name = @queueName AND q.Type = 1;

    IF @queueId IS NULL
    BEGIN
        THROW 50000, 'Queue not found', 1;
    END;

    IF @lockDuration <= 0
    BEGIN
        THROW 50000, 'Invalid lock duration', 1;
    END;

    SET @now = SYSUTCDATETIME();
    SET @enqueueTime = DATEADD(SECOND, @lockDuration, @now);

    DECLARE @ResultTable TABLE (
        TransportMessageId uniqueidentifier,
        QueueId bigint,
        Priority smallint,
        MessageDeliveryId bigint,
        ConsumerId uniqueidentifier,
        LockId uniqueidentifier,
        EnqueueTime datetime2,
        ExpirationTime datetime2,
        DeliveryCount int,
        PartitionKey text,
        RoutingKey text,
        TransportHeaders nvarchar(max),
        ContentType text,
        MessageType text,
        Body nvarchar(max),
        BinaryBody varbinary(max),
        MessageId uniqueidentifier,
        CorrelationId uniqueidentifier,
        ConversationId uniqueidentifier,
        RequestId uniqueidentifier,
        InitiatorId uniqueidentifier,
        SourceAddress text,
        DestinationAddress text,
        ResponseAddress text,
        FaultAddress text,
        SentTime datetime2,
        Headers nvarchar(max),
        Host nvarchar(max)
    );

    WITH ready AS (SELECT mdx.MessageDeliveryId,
                          mdx.EnqueueTime,
                          mdx.LockId,
                          mdx.Priority,
                          row_number() over (partition by mdx.PartitionKey order by mdx.Priority, mdx.EnqueueTime, mdx.MessageDeliveryId) as row_normal,
                          row_number() over (partition by mdx.PartitionKey order by mdx.Priority, mdx.MessageDeliveryId, mdx.EnqueueTime) as row_ordered,
                          first_value(CASE WHEN mdx.EnqueueTime > @now THEN mdx.ConsumerId END) over (partition by mdx.PartitionKey
                           order by mdx.EnqueueTime DESC, mdx.MessageDeliveryId DESC) as ConsumerId,
                          sum(CASE WHEN mdx.EnqueueTime > @now AND mdx.ConsumerId = @consumerId AND mdx.LockId IS NOT NULL THEN 1 END)
                           over (partition by mdx.PartitionKey
                               order by mdx.EnqueueTime DESC, mdx.MessageDeliveryId DESC) as ActiveCount
                   FROM {0}.MessageDelivery mdx WITH (ROWLOCK, READPAST, UPDLOCK)
                   WHERE mdx.QueueId = @queueId
                     AND mdx.DeliveryCount < mdx.MaxDeliveryCount),
         so_ready as (SELECT ready.MessageDeliveryId
                      FROM ready
                      WHERE ( ( @ordered = 0 AND ready.row_normal <= @concurrentCount) OR ( @ordered = 1 AND ready.row_ordered <= @concurrentCount ) )
                        AND (ready.ConsumerId IS NULL OR ready.ConsumerId = @consumerId)
                        AND (ActiveCount < @concurrentCount OR ActiveCount IS NULL)
                        AND ready.EnqueueTime <= @now
                      ORDER BY ready.Priority, ready.EnqueueTime, ready.MessageDeliveryId
                      OFFSET 0 ROWS FETCH NEXT @fetchCount ROWS ONLY),
         msgs AS (SELECT md.*
                  FROM {0}.MessageDelivery md
                  WITH (ROWLOCK, READPAST, UPDLOCK)
                  WHERE md.MessageDeliveryId IN (SELECT MessageDeliveryId FROM so_ready))
    UPDATE dm
    SET
        DeliveryCount = dm.DeliveryCount + 1,
        LastDelivered = @now,
        ConsumerId = @consumerId,
        LockId = @lockId,
        EnqueueTime = @enqueueTime
    OUTPUT
        inserted.TransportMessageId,
        inserted.QueueId,
        inserted.Priority,
        inserted.MessageDeliveryId,
        inserted.ConsumerId,
        inserted.LockId,
        inserted.EnqueueTime,
        inserted.ExpirationTime,
        inserted.DeliveryCount,
        inserted.PartitionKey,
        inserted.RoutingKey,
        inserted.TransportHeaders,
        m.ContentType,
        m.MessageType,
        m.Body,
        m.BinaryBody,
        m.MessageId,
        m.CorrelationId,
        m.ConversationId,
        m.RequestId,
        m.InitiatorId,
        m.SourceAddress,
        m.DestinationAddress,
        m.ResponseAddress,
        m.FaultAddress,
        m.SentTime,
        m.Headers,
        m.Host
    INTO
        @ResultTable (
            TransportMessageId ,
            QueueId ,
            Priority ,
            MessageDeliveryId ,
            ConsumerId ,
            LockId ,
            EnqueueTime ,
            ExpirationTime ,
            DeliveryCount ,
            PartitionKey ,
            RoutingKey ,
            TransportHeaders,
            ContentType ,
            MessageType ,
            Body,
            BinaryBody,
            MessageId ,
            CorrelationId ,
            ConversationId ,
            RequestId ,
            InitiatorId ,
            SourceAddress ,
            DestinationAddress ,
            ResponseAddress ,
            FaultAddress ,
            SentTime ,
            Headers,
            Host
        )
    FROM
        {0}.MessageDelivery dm
        INNER JOIN msgs ON dm.MessageDeliveryId = msgs.MessageDeliveryId
        INNER JOIN {0}.Message m ON msgs.TransportMessageId = m.TransportMessageId;

    SELECT * FROM @ResultTable;
END";

        const string SqlFnDeleteMessage = @"
CREATE OR ALTER PROCEDURE {0}.DeleteMessage
    @messageDeliveryId bigint,
    @lockId uniqueidentifier
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @outMessageDeliveryId bigint;
    DECLARE @outTransportMessageId uniqueidentifier;
    DECLARE @outQueueId bigint;

    DECLARE @DeletedMessages TABLE (
        MessageDeliveryId bigint,
        TransportMessageId uniqueidentifier,
        QueueId bigint
    );

    DELETE
    FROM {0}.MessageDelivery
    OUTPUT deleted.MessageDeliveryId, deleted.TransportMessageId, deleted.QueueId
    INTO @DeletedMessages
    WHERE MessageDeliveryId = @messageDeliveryId
    AND LockId = @lockId;

    SELECT TOP 1 @outMessageDeliveryId = MessageDeliveryId, @outTransportMessageId = TransportMessageId, @outQueueId = QueueId
        FROM @DeletedMessages;

    IF @outTransportMessageId IS NOT NULL
    BEGIN
        DELETE m
        FROM {0}.Message m
        WHERE m.TransportMessageId = @outTransportMessageId
        AND NOT EXISTS (SELECT 1 FROM {0}.MessageDelivery md WHERE md.TransportMessageId = @outTransportMessageId);

        INSERT INTO {0}.QueueMetricCapture (Captured, QueueId, ConsumeCount, ErrorCount, DeadLetterCount)
            VALUES (SYSUTCDATETIME(), @outQueueId, 1, 0, 0);
    END;

    RETURN @outMessageDeliveryId;
END";

        const string SqlFnTouchQueue = @"
CREATE OR ALTER PROCEDURE {0}.TouchQueue
    @queueName varchar(256)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @queueId bigint
    SELECT @queueId = q.Id
    FROM {0}.Queue q
    WHERE q.Name = @queueName AND q.Type = 1;

    IF @queueId IS NULL
    BEGIN
        THROW 50000, 'Queue not found', 1;
    END;

    INSERT INTO {0}.QueueMetricCapture (Captured, QueueId, ConsumeCount, ErrorCount, DeadLetterCount)
        VALUES (SYSUTCDATETIME(), @queueId, 0, 0, 0);

END";

        const string SqlFnDeadLetterMessages = @"
CREATE OR ALTER PROCEDURE {0}.DeadLetterMessages
    @queueName varchar(256),
    @messageCount int
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @sourceQueueId bigint
    SELECT @sourceQueueId = q.Id
    FROM {0}.Queue q
    WHERE q.Name = @queueName AND q.Type = 1;

    IF @sourceQueueId IS NULL
    BEGIN
        THROW 50000, 'Queue not found', 1;
    END;

    DECLARE @targetQueueId bigint
    SELECT @targetQueueId = q.Id
    FROM {0}.Queue q
    WHERE q.Name = @queueName AND q.Type = 3;

    IF @targetQueueId IS NULL
    BEGIN
        THROW 50000, 'Dead-letter queue not found', 1;
    END;

    DECLARE @now datetime2;
    SET @now = SYSUTCDATETIME();

    UPDATE {0}.MessageDelivery
    SET QueueId = @targetQueueId
    FROM (SELECT mdx.MessageDeliveryId
          FROM {0}.MessageDelivery mdx WITH (ROWLOCK, UPDLOCK)
          WHERE mdx.QueueId = @sourceQueueId
            AND mdx.EnqueueTime < @now
            AND mdx.DeliveryCount >= mdx.MaxDeliveryCount
            AND (mdx.ExpirationTime IS NULL OR mdx.ExpirationTime > @now)
            ORDER BY mdx.MessageDeliveryId OFFSET 0 ROWS
        FETCH NEXT @messageCount ROWS ONLY) mdy
    WHERE mdy.MessageDeliveryId = MessageDelivery.MessageDeliveryId;

    DECLARE @vRowCount bigint;
    SELECT @vRowCount = @@ROWCOUNT;
    IF @vRowCount = 0
    BEGIN
        INSERT INTO {0}.QueueMetricCapture (Captured, QueueId, ConsumeCount, ErrorCount, DeadLetterCount)
            VALUES (SYSUTCDATETIME(), @sourceQueueId, 0, 0, @vRowCount);
    END;

END";

        const string SqlFnPurgeQueue = @"
CREATE OR ALTER PROCEDURE {0}.PurgeQueue
    @queueName varchar(256)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @DeletedMessages TABLE (
        TransportMessageId uniqueidentifier INDEX DMIDX CLUSTERED
    );

    DELETE FROM {0}.MessageDelivery
        OUTPUT deleted.TransportMessageId
        INTO @DeletedMessages
        FROM {0}.MessageDelivery mdx
            INNER JOIN {0}.queue q on mdx.queueid = q.Id
        WHERE q.name = @queueName

    DELETE FROM {0}.Message
        FROM {0}.Message m
            INNER JOIN @DeletedMessages dm ON m.TransportMessageId = dm.TransportMessageId
        WHERE NOT EXISTS (SELECT 1 FROM {0}.MessageDelivery md WHERE md.TransportMessageId = m.TransportMessageId);

    SELECT COUNT(*) FROM @DeletedMessages
END";

        const string SqlFnDeleteScheduledMessage = @"
CREATE OR ALTER PROCEDURE {0}.DeleteScheduledMessage
    @tokenId uniqueidentifier
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @DeletedMessages TABLE (
        TransportMessageId uniqueidentifier
    );

    DELETE m
    OUTPUT deleted.TransportMessageId
    INTO @DeletedMessages (TransportMessageId)
    FROM {0}.Message m
    LEFT JOIN {0}.MessageDelivery md ON md.TransportMessageId = m.TransportMessageId
    WHERE m.SchedulingTokenId = @tokenId
    AND md.DeliveryCount = 0
    AND md.LockId IS NULL;

    SELECT TransportMessageId
        FROM @DeletedMessages;
END
";

        const string SqlFnRenewMessageLock = @"
CREATE OR ALTER PROCEDURE {0}.RenewMessageLock
    @messageDeliveryId bigint,
    @lockId uniqueidentifier,
    @duration int
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @enqueueTime datetime2;
    SET @enqueueTime = DATEADD(SECOND, @duration, SYSUTCDATETIME());

    DECLARE @updatedMessages TABLE (
        MessageDeliveryId bigint,
        QueueId bigint
    );

    UPDATE md
    SET EnqueueTime = @enqueueTime
    OUTPUT inserted.MessageDeliveryId, inserted.QueueId INTO @updatedMessages
    FROM {0}.MessageDelivery md
    WHERE md.MessageDeliveryId = @messageDeliveryId AND md.LockId = @lockId;

    DECLARE @queueId bigint
    SELECT TOP 1 @queueId = QueueID FROM @updatedMessages;

    IF @queueId IS NOT NULL
    BEGIN
        INSERT INTO {0}.QueueMetricCapture (Captured, QueueId, ConsumeCount, ErrorCount, DeadLetterCount)
            VALUES (SYSUTCDATETIME(), @queueId, 0, 0, 0);
    END;

    SELECT MessageDeliveryId FROM @updatedMessages;
END";

        const string SqlFnUnlockMessage = @"
CREATE OR ALTER PROCEDURE {0}.UnlockMessage
    @messageDeliveryId bigint,
    @lockId uniqueidentifier,
    @delay int,
    @headers nvarchar(max)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @enqueueTime datetime2;
    SET @enqueueTime = DATEADD(SECOND, @delay, SYSUTCDATETIME());

    DECLARE @updatedMessages TABLE (
        MessageDeliveryId bigint,
        QueueId bigint
    );

    UPDATE md
    SET EnqueueTime = @enqueueTime, LockId = NULL, ConsumerId = NULL, TransportHeaders = @headers
    OUTPUT inserted.MessageDeliveryId, inserted.QueueId INTO @updatedMessages
    FROM {0}.MessageDelivery md
    WHERE md.MessageDeliveryId = @messageDeliveryId AND md.LockId = @lockId;

    DECLARE @queueId bigint
    SELECT TOP 1 @queueId = QueueID FROM @updatedMessages;

    IF @queueId IS NOT NULL
    BEGIN
        INSERT INTO {0}.QueueMetricCapture (Captured, QueueId, ConsumeCount, ErrorCount, DeadLetterCount)
            VALUES (SYSUTCDATETIME(), @queueId, 0, 0, 0);
    END;

    SELECT MessageDeliveryId FROM @updatedMessages;
END";

        const string SqlFnMoveMessage = @"
CREATE OR ALTER PROCEDURE {0}.MoveMessage
    @messageDeliveryId bigint,
    @lockId uniqueidentifier,
    @queueName nvarchar(256),
    @queueType int,
    @headers nvarchar(max)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @queueId bigint
    SELECT @queueId = q.Id
    FROM {0}.Queue q
    WHERE q.Name = @queueName AND q.Type = @queueType;

    IF @queueId IS NULL
    BEGIN
        THROW 50000, 'Queue not found', 1;
    END;

    DECLARE @updatedMessages TABLE (
        MessageDeliveryId bigint,
        QueueId bigint
    );

    UPDATE md
    SET EnqueueTime = SYSUTCDATETIME(), QueueId = @queueId, LockId = NULL, ConsumerId = NULL, TransportHeaders = @headers
    OUTPUT inserted.MessageDeliveryId, inserted.QueueId INTO @updatedMessages
    FROM {0}.MessageDelivery md
    WHERE md.MessageDeliveryId = @messageDeliveryId AND md.LockId = @lockId;

    DECLARE @outQueueId bigint
    SELECT TOP 1 @outQueueId = QueueID FROM @updatedMessages;

    IF @outQueueId IS NOT NULL
    BEGIN
        INSERT INTO {0}.QueueMetricCapture (Captured, QueueId, ConsumeCount, ErrorCount, DeadLetterCount)
            VALUES (SYSUTCDATETIME(), @outQueueId, 0, CASE WHEN @queueType = 2 THEN 1 ELSE 0 END, CASE WHEN @queueType = 3 THEN 1 ELSE 0 END);
    END;

    SELECT MessageDeliveryId FROM @updatedMessages;
END";

        const string SqlFnRequeueMessages = @"
CREATE OR ALTER PROCEDURE {0}.RequeueMessages
    @queueName nvarchar(256),
    @sourceQueueType int,
    @targetQueueType int,
    @messageCount int,
    @delay int = 0,
    @redeliveryCount int = 10
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT @sourceQueueType BETWEEN 1 AND 3
    BEGIN
        THROW 50000, 'Invalid source queue type', 1;
    END;

    IF NOT @targetQueueType BETWEEN 1 AND 3
    BEGIN
        THROW 50000, 'Invalid target queue type', 1;
    END;

    IF @sourceQueueType = @targetQueueType
    BEGIN
        THROW 50000, 'Source and target queue type must not be the same', 1;
    END;

    DECLARE @sourceQueueId bigint
    SELECT @sourceQueueId = q.Id
    FROM {0}.Queue q
    WHERE q.Name = @queueName AND q.Type = @sourceQueueType;

    IF @sourceQueueId IS NULL
    BEGIN
        THROW 50000, 'Source queue not found', 1;
    END;

    DECLARE @targetQueueId bigint
    SELECT @targetQueueId = q.Id
    FROM {0}.Queue q
    WHERE q.Name = @queueName AND q.Type = @targetQueueType;

    IF @targetQueueId IS NULL
    BEGIN
        THROW 50000, 'Target queue not found', 1;
    END;

    DECLARE @enqueueTime datetime2;
    SET @enqueueTime = DATEADD(SECOND, @delay, SYSUTCDATETIME());

    UPDATE {0}.MessageDelivery
    SET EnqueueTime      = @enqueueTime,
        QueueId          = @targetQueueId,
        MaxDeliveryCount = MessageDelivery.DeliveryCount + @redeliveryCount
    FROM (SELECT mdx.MessageDeliveryId
          FROM {0}.MessageDelivery mdx WITH (ROWLOCK, UPDLOCK)
          WHERE mdx.QueueId = @sourceQueueId
            AND mdx.LockId IS NULL
            AND mdx.ConsumerId IS NULL
            AND (mdx.ExpirationTime IS NULL OR mdx.ExpirationTime > @enqueueTime)
            ORDER BY mdx.MessageDeliveryId OFFSET 0 ROWS
        FETCH NEXT @messageCount ROWS ONLY) mdy
    WHERE mdy.MessageDeliveryId = MessageDelivery.MessageDeliveryId;

    RETURN @@ROWCOUNT
END";

        const string SqlFnRequeueMessage = @"
CREATE OR ALTER PROCEDURE {0}.RequeueMessage @messageDeliveryId bigint,
                                                   @targetQueueType int,
                                                   @delay int = 0,
                                                   @redeliveryCount int = 10
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT @targetQueueType BETWEEN 1 AND 3
        BEGIN
            THROW 50000, 'Invalid target queue type', 1;
        END;

    DECLARE @sourceQueueId bigint;
    SELECT @sourceQueueId = md.QueueId
    FROM {0}.MessageDelivery md
    WHERE md.MessageDeliveryId = @messageDeliveryId;

    IF @sourceQueueId IS NULL
        BEGIN
            THROW 50000, 'Message delivery not found', 1;
        END;

    DECLARE @sourceQueueName nvarchar(256);
    DECLARE @sourceQueueType int;
    SELECT @sourceQueueName = q.Name, @sourceQueueType = q.Type
    FROM {0}.Queue q
    WHERE q.Id = @sourceQueueId;

    IF @sourceQueueName IS NULL
        BEGIN
            THROW 50000, 'Queue not found', 1;
        END;

    IF @sourceQueueType = @targetQueueType
        BEGIN
            THROW 50000, 'Source and target queue type must not be the same', 1;
        END;

    DECLARE @targetQueueId bigint;
    SELECT @targetQueueId = q.Id
    FROM {0}.Queue q
    WHERE q.Name = @sourceQueueName
      AND q.Type = @targetQueueType;

    IF @targetQueueId IS NULL
        BEGIN
            THROW 50000, 'Queue type not found', 1;
        END;

    DECLARE @enqueueTime datetime2;
    SET @enqueueTime = DATEADD(SECOND, @delay, SYSUTCDATETIME());

    UPDATE {0}.MessageDelivery
    SET EnqueueTime      = @enqueueTime,
        QueueId          = @targetQueueId,
        MaxDeliveryCount = MessageDelivery.DeliveryCount + @redeliveryCount
    FROM (SELECT mdx.MessageDeliveryId
          FROM {0}.MessageDelivery mdx WITH (ROWLOCK, UPDLOCK)
          WHERE mdx.QueueId = @sourceQueueId
            AND mdx.LockId IS NULL
            AND mdx.ConsumerId IS NULL
            AND (mdx.ExpirationTime IS NULL OR mdx.ExpirationTime > @enqueueTime)
            AND mdx.MessageDeliveryId = @messageDeliveryId) mdy
    WHERE mdy.MessageDeliveryId = MessageDelivery.MessageDeliveryId;

    RETURN @@ROWCOUNT;
END
";

        const string SqlFnProcessMetrics = @"
CREATE OR ALTER PROCEDURE {0}.ProcessMetrics
    @rowLimit int
AS
BEGIN
    SET NOCOUNT ON;

	DECLARE @Lock int
	EXEC @Lock = sp_getapplock @Resource = '_MT_ProcessMetrics',
							   @LockMode = 'Exclusive',
							   @LockTimeout = 500
	IF (@Lock < 0)
	   RETURN;

	DECLARE @DeletedMetrics TABLE
							(
								StartTime       datetime2 not null,
								Duration        int       not null,
								QueueId         bigint    not null,
								ConsumeCount    bigint    not null,
								ErrorCount      bigint    not null,
								DeadLetterCount bigint    not null
							);

	DELETE
	FROM {0}.QueueMetricCapture
	OUTPUT CONVERT(DATETIME, CONVERT(VARCHAR(16), deleted.Captured, 120) + ':00'), 60, deleted.QueueId,
																					   deleted.ConsumeCount,
																					   deleted.ErrorCount,
																					   deleted.DeadLetterCount
		INTO @DeletedMetrics
	WHERE Id < COALESCE((SELECT MIN(id) FROM {0}.queuemetriccapture), 0) + @rowLimit;

	MERGE INTO {0}.QueueMetric WITH (TABLOCK) AS target
	USING (SELECT m.StartTime,
				  m.Duration,
				  m.QueueId,
				  sum(m.ConsumeCount),
				  sum(m.ErrorCount),
				  sum(m.DeadLetterCount)
		   FROM @DeletedMetrics m
		   GROUP BY StartTime, m.Duration, m.QueueId) as source
		(StartTime, Duration, QueueId, ConsumeCount, ErrorCount, DeadLetterCount)
	ON target.StartTime = source.starttime AND target.Duration = source.duration AND
	   target.QueueId = source.QueueId
	WHEN MATCHED THEN
		UPDATE
		SET ConsumeCount    = source.ConsumeCount + target.ConsumeCount,
			ErrorCount      = source.ErrorCount + target.ErrorCount,
			DeadLetterCount = source.DeadLetterCount + target.DeadLetterCount
	WHEN NOT MATCHED THEN
		INSERT (starttime, duration, QueueId, ConsumeCount, ErrorCount, DeadLetterCount)
		values (source.starttime, source.duration, source.QueueId, source.ConsumeCount, source.ErrorCount,
				source.DeadLetterCount);

	DELETE
	FROM @DeletedMetrics;

	DELETE
	FROM {0}.QueueMetric
	OUTPUT CONVERT(DATETIME, CONVERT(VARCHAR(13), deleted.StartTime, 120) + ':00:00'), 3600, deleted.QueueId,
																							 deleted.ConsumeCount,
																							 deleted.ErrorCount,
																							 deleted.DeadLetterCount
		INTO @DeletedMetrics
	WHERE Duration = 60
	  AND StartTime < DATEADD(HOUR, -8, SYSUTCDATETIME())

	MERGE INTO {0}.QueueMetric WITH (TABLOCK) AS target
	USING (SELECT m.StartTime,
				  m.Duration,
				  m.QueueId,
				  sum(m.ConsumeCount),
				  sum(m.ErrorCount),
				  sum(m.DeadLetterCount)
		   FROM @DeletedMetrics m
		   GROUP BY StartTime, m.Duration, m.QueueId) as source
		(StartTime, Duration, QueueId, ConsumeCount, ErrorCount, DeadLetterCount)
	ON target.StartTime = source.starttime AND target.Duration = source.duration AND
	   target.QueueId = source.QueueId
	WHEN MATCHED THEN
		UPDATE
		SET ConsumeCount    = source.ConsumeCount + target.ConsumeCount,
			ErrorCount      = source.ErrorCount + target.ErrorCount,
			DeadLetterCount = source.DeadLetterCount + target.DeadLetterCount
	WHEN NOT MATCHED THEN
		INSERT (starttime, duration, QueueId, ConsumeCount, ErrorCount, DeadLetterCount)
		values (source.starttime, source.duration, source.QueueId, source.ConsumeCount, source.ErrorCount,
				source.DeadLetterCount);

	DELETE
	FROM @DeletedMetrics;

	DELETE
	FROM {0}.QueueMetric
	OUTPUT CONVERT(DATETIME, CONVERT(VARCHAR(10), deleted.StartTime, 120)), 86400, deleted.QueueId,
																				   deleted.ConsumeCount,
																				   deleted.ErrorCount,
																				   deleted.DeadLetterCount
		INTO @DeletedMetrics
	WHERE Duration = 3600
	  AND StartTime < DATEADD(HOUR, -48, SYSUTCDATETIME())

	MERGE INTO {0}.QueueMetric WITH (TABLOCK) AS target
	USING (SELECT m.StartTime,
				  m.Duration,
				  m.QueueId,
				  sum(m.ConsumeCount),
				  sum(m.ErrorCount),
				  sum(m.DeadLetterCount)
		   FROM @DeletedMetrics m
		   GROUP BY StartTime, m.Duration, m.QueueId) as source
		(StartTime, Duration, QueueId, ConsumeCount, ErrorCount, DeadLetterCount)
	ON target.StartTime = source.starttime AND target.Duration = source.duration AND
	   target.QueueId = source.QueueId
	WHEN MATCHED THEN
		UPDATE
		SET ConsumeCount    = source.ConsumeCount + target.ConsumeCount,
			ErrorCount      = source.ErrorCount + target.ErrorCount,
			DeadLetterCount = source.DeadLetterCount + target.DeadLetterCount
	WHEN NOT MATCHED THEN
		INSERT (starttime, duration, QueueId, ConsumeCount, ErrorCount, DeadLetterCount)
		values (source.starttime, source.duration, source.QueueId, source.ConsumeCount, source.ErrorCount,
				source.DeadLetterCount);

	DELETE
	FROM {0}.QueueMetric
	WHERE StartTime < DATEADD(DAY, -90, SYSUTCDATETIME());
END
";

        const string SqlFnPurgeTopology = @"
CREATE OR ALTER PROCEDURE {0}.PurgeTopology
AS
BEGIN
    SET NOCOUNT ON;

	DECLARE @Lock int
	EXEC @Lock = sp_getapplock @Resource = '_MT_PurgeTopology',
							   @LockMode = 'Exclusive',
							   @LockTimeout = 500
	IF (@Lock < 0)
	   RETURN;

    WITH expired AS (SELECT q.Id, q.name, DATEADD(second, -q.autodelete, SYSUTCDATETIME()) as expires_at
                    FROM {0}.Queue q
                    WHERE q.Type = 1 AND  q.AutoDelete IS NOT NULL AND DATEADD(second, -q.AutoDelete, SYSUTCDATETIME()) > Updated),
        metrics AS (SELECT qm.queueid, MAX(starttime) as start_time
                    FROM {0}.queuemetric qm
                            INNER JOIN expired q2 on q2.Id = qm.QueueId
                    WHERE DATEADD(second, duration, starttime) > q2.expires_at
                    GROUP BY qm.queueid)
    DELETE FROM {0}.Queue
    FROM {0}.Queue qd
    INNER JOIN expired qdx ON qdx.Name = qd.Name
        WHERE qdx.Id NOT IN (SELECT QueueId FROM metrics);
END
";

        const string SqlFnQueuesView = """
            CREATE OR ALTER VIEW {0}.Queues
            AS
            SELECT x.QueueName,
                   MAX(x.QueueAutoDelete)                AS QueueAutoDelete,
                   SUM(x.MessageReady)                   AS Ready,
                   SUM(x.MessageScheduled)               AS Scheduled,
                   SUM(x.MessageError)                   AS Errored,
                   SUM(x.MessageDeadLetter)              AS DeadLettered,
                   SUM(x.MessageLocked)                  AS Locked,
                   ISNULL(MAX(x.ConsumeCount), 0)        AS ConsumeCount,
                   ISNULL(MAX(x.ErrorCount), 0)          AS ErrorCount,
                   ISNULL(MAX(x.DeadLetterCount), 0)     AS DeadLetterCount,
                   MAX(x.StartTime)                      AS CountStartTime,
                   ISNULL(MAX(x.Duration), 0)            AS CountDuration,
                   MAX(x.QueueMaxDeliveryCount)          AS QueueMaxDeliveryCount
            FROM (SELECT q.Name                                              AS QueueName,
                         q.AutoDelete                                        AS QueueAutoDelete,
                         qm.ConsumeCount,
                         qm.ErrorCount,
                         qm.DeadLetterCount,
                         qm.StartTime,
                         qm.Duration,

                         IIF(q.Type = 1
                                 AND md.MessageDeliveryId IS NOT NULL
                                 AND md.EnqueueTime <= SYSUTCDATETIME(), 1, 0)   AS MessageReady,
                         IIF(q.Type = 1
                                 AND md.MessageDeliveryId IS NOT NULL
                                 AND md.LockId IS NULL
                                 AND md.EnqueueTime > SYSUTCDATETIME(), 1, 0)    AS MessageScheduled,
                         IIF(q.Type = 1
                                 AND md.MessageDeliveryId IS NOT NULL
                                 AND md.LockId IS NOT NULL
                                 AND md.DeliveryCount >= 1
                                 AND md.EnqueueTime > SYSUTCDATETIME(), 1, 0)    AS MessageLocked,
                         IIF(q.Type = 2
                                 AND md.MessageDeliveryId IS NOT NULL, 1, 0) AS MessageError,
                         IIF(q.Type = 3
                                 AND md.MessageDeliveryId IS NOT NULL, 1, 0) AS MessageDeadLetter,
                         IIF(q.Type = 1, q.MaxDeliveryCount, NULL)           AS QueueMaxDeliveryCount
                  FROM {0}.Queue q
                           LEFT JOIN {0}.MessageDelivery md ON q.Id = md.QueueId
                           LEFT JOIN (SELECT qm.QueueId,
                                             qm.QueueName,
                                             qm.ConsumeCount    AS ConsumeCount,
                                             qm.ErrorCount      AS ErrorCount,
                                             qm.DeadLetterCount AS DeadLetterCount,
                                             qm.StartTime,
                                             qm.Duration
                                      FROM (SELECT qm.QueueId,
                                                   q2.Name                                                                as QueueName,
                                                   ROW_NUMBER() OVER (PARTITION BY qm.QueueId ORDER BY qm.StartTime DESC) AS RowNum,
                                                   qm.ConsumeCount,
                                                   qm.ErrorCount,
                                                   qm.DeadLetterCount,
                                                   qm.StartTime,
                                                   qm.Duration
                                            FROM {0}.QueueMetric qm
                                                     INNER JOIN {0}.Queue q2 ON qm.QueueId = q2.Id
                                            WHERE q2.Type = 1
                                              AND qm.StartTime >= DATEADD(MINUTE, -5, SYSUTCDATETIME())) qm
                                      WHERE qm.RowNum = 1) qm ON qm.QueueId = q.Id) x
            GROUP BY x.QueueName;
            """;

        const string SqlFnSubscriptionsView = """
            CREATE OR ALTER VIEW {0}.Subscriptions
            AS
                SELECT t.name as TopicName, 'topic' as DestinationType,  t2.name as DestinationName, ts.SubType as SubscriptionType, ts.RoutingKey
                FROM {0}.topic t
                         JOIN {0}.TopicSubscription ts ON t.id = ts.sourceid
                         JOIN {0}.topic t2 on t2.id = ts.destinationid
                UNION
                SELECT t.name as TopicName, 'queue' as DestinationType, q.name as DestinationName, qs.SubType as SubscriptionType, qs.RoutingKey
                FROM {0}.queuesubscription qs
                         LEFT JOIN {0}.queue q on qs.destinationid = q.id
                         LEFT JOIN {0}.topic t on qs.sourceid = t.id;
            """;

        readonly ILogger<SqlServerDatabaseMigrator> _logger;

        public SqlServerDatabaseMigrator(ILogger<SqlServerDatabaseMigrator> logger)
        {
            _logger = logger;
        }

        public async Task CreateDatabase(SqlTransportOptions options, CancellationToken cancellationToken)
        {
            await CreateDatabaseIfNotExist(options, cancellationToken).ConfigureAwait(false);
        }

        public async Task DeleteDatabase(SqlTransportOptions options, CancellationToken cancellationToken)
        {
            await using var connection = SqlServerSqlTransportConnection.GetSystemDatabaseConnection(options);
            await connection.Open(cancellationToken).ConfigureAwait(false);

            var result = await connection.Connection.ExecuteScalarAsync<int?>(string.Format(DbExistsSql, options.Database)).ConfigureAwait(false);
            if (result > 0)
            {
                await connection.Connection.ExecuteScalarAsync<int>(string.Format(DropSql, options.Database)).ConfigureAwait(false);

                _logger.LogInformation("Database {Database} deleted", options.Database);
            }
        }

        public async Task CreateInfrastructure(SqlTransportOptions options, CancellationToken cancellationToken)
        {
            await using var connection = SqlServerSqlTransportConnection.GetDatabaseConnection(options);
            await connection.Open(cancellationToken).ConfigureAwait(false);

            try
            {
                await connection.Connection.ExecuteScalarAsync<int>(string.Format(CreateInfrastructureSql, options.Schema)).ConfigureAwait(false);

                await connection.Connection.ExecuteScalarAsync<int>(string.Format(SqlFnCreateQueue, options.Schema)).ConfigureAwait(false);
                await connection.Connection.ExecuteScalarAsync<int>(string.Format(SqlFnCreateQueueV2, options.Schema)).ConfigureAwait(false);
                await connection.Connection.ExecuteScalarAsync<int>(string.Format(SqlFnCreateTopic, options.Schema)).ConfigureAwait(false);
                await connection.Connection.ExecuteScalarAsync<int>(string.Format(SqlFnCreateTopicSubscription, options.Schema)).ConfigureAwait(false);
                await connection.Connection.ExecuteScalarAsync<int>(string.Format(SqlFnCreateQueueSubscription, options.Schema)).ConfigureAwait(false);
                await connection.Connection.ExecuteScalarAsync<int>(string.Format(SqlFnPurgeQueue, options.Schema)).ConfigureAwait(false);
                await connection.Connection.ExecuteScalarAsync<int>(string.Format(SqlFnPublish, options.Schema)).ConfigureAwait(false);
                await connection.Connection.ExecuteScalarAsync<int>(string.Format(SqlFnPublishV2, options.Schema)).ConfigureAwait(false);
                await connection.Connection.ExecuteScalarAsync<int>(string.Format(SqlFnSend, options.Schema)).ConfigureAwait(false);
                await connection.Connection.ExecuteScalarAsync<int>(string.Format(SqlFnSendV2, options.Schema)).ConfigureAwait(false);
                await connection.Connection.ExecuteScalarAsync<int>(string.Format(SqlFnFetchMessages, options.Schema)).ConfigureAwait(false);
                await connection.Connection.ExecuteScalarAsync<int>(string.Format(SqlFnFetchMessagesPartitioned, options.Schema)).ConfigureAwait(false);
                await connection.Connection.ExecuteScalarAsync<int>(string.Format(SqlFnDeleteMessage, options.Schema)).ConfigureAwait(false);
                await connection.Connection.ExecuteScalarAsync<int>(string.Format(SqlFnDeleteScheduledMessage, options.Schema)).ConfigureAwait(false);
                await connection.Connection.ExecuteScalarAsync<int>(string.Format(SqlFnRenewMessageLock, options.Schema)).ConfigureAwait(false);
                await connection.Connection.ExecuteScalarAsync<int>(string.Format(SqlFnUnlockMessage, options.Schema)).ConfigureAwait(false);
                await connection.Connection.ExecuteScalarAsync<int>(string.Format(SqlFnMoveMessage, options.Schema)).ConfigureAwait(false);
                await connection.Connection.ExecuteScalarAsync<int>(string.Format(SqlFnRequeueMessage, options.Schema)).ConfigureAwait(false);
                await connection.Connection.ExecuteScalarAsync<int>(string.Format(SqlFnRequeueMessages, options.Schema)).ConfigureAwait(false);
                await connection.Connection.ExecuteScalarAsync<int>(string.Format(SqlFnProcessMetrics, options.Schema)).ConfigureAwait(false);
                await connection.Connection.ExecuteScalarAsync<int>(string.Format(SqlFnPurgeTopology, options.Schema)).ConfigureAwait(false);
                await connection.Connection.ExecuteScalarAsync<int>(string.Format(SqlFnTouchQueue, options.Schema)).ConfigureAwait(false);
                await connection.Connection.ExecuteScalarAsync<int>(string.Format(SqlFnDeadLetterMessages, options.Schema)).ConfigureAwait(false);
                await connection.Connection.ExecuteScalarAsync<int>(string.Format(SqlFnQueuesView, options.Schema)).ConfigureAwait(false);
                await connection.Connection.ExecuteScalarAsync<int>(string.Format(SqlFnSubscriptionsView, options.Schema)).ConfigureAwait(false);

                _logger.LogDebug("Transport infrastructure in schema {Schema} created (or updated)", options.Schema);
            }
            finally
            {
                await connection.Close().ConfigureAwait(false);
            }
        }

        async Task CreateDatabaseIfNotExist(SqlTransportOptions options, CancellationToken cancellationToken)
        {
            await using var connection = SqlServerSqlTransportConnection.GetSystemDatabaseConnection(options);
            await connection.Open(cancellationToken);

            try
            {
                var result = await connection.Connection.ExecuteScalarAsync<int?>(string.Format(DbExistsSql, options.Database)).ConfigureAwait(false);
                if (result > 0)
                    _logger.LogDebug("Database {Database} already exists", options.Database);
                else
                {
                    await connection.Connection.ExecuteScalarAsync<int>(string.Format(DbCreateSql, options.Database)).ConfigureAwait(false);

                    _logger.LogInformation("Database {Database} created", options.Database);
                }

                result = await connection.Connection.ExecuteScalarAsync<int?>(string.Format(LoginExistsSql, options.Username)).ConfigureAwait(false);
                if (!result.HasValue)
                {
                    await connection.Connection.ExecuteScalarAsync<int>(string.Format(CreateLoginSql, options.Username, options.Password))
                        .ConfigureAwait(false);

                    _logger.LogDebug("Login {Username} created", options.Username);
                }
            }
            finally
            {
                await connection.Close();
            }
        }

        public async Task CreateSchemaIfNotExist(SqlTransportOptions options, CancellationToken cancellationToken)
        {
            await using var connection = SqlServerSqlTransportConnection.GetDatabaseAdminConnection(options);
            await connection.Open(cancellationToken).ConfigureAwait(false);

            try
            {
                await connection.Connection.ExecuteScalarAsync<int>(string.Format(SchemaCreateSql, options.Database, options.Schema)).ConfigureAwait(false);

                _logger.LogDebug("Schema {Schema} created", options.Schema);

                await GrantAccess(connection, options).ConfigureAwait(false);
            }
            finally
            {
                await connection.Close().ConfigureAwait(false);
            }
        }

        async Task GrantAccess(ISqlServerSqlTransportConnection connection, SqlTransportOptions options)
        {
            if (string.IsNullOrWhiteSpace(options.Role))
                throw new ArgumentException("The SQL transport migrator requires a valid Role, but Role was not specified", nameof(options));

            var result = await connection.Connection.ExecuteScalarAsync<int?>(string.Format(RoleExistsSql, options.Role)).ConfigureAwait(false);
            if (!result.HasValue)
            {
                await connection.Connection.ExecuteScalarAsync<int>(string.Format(CreateRoleSql, options.Role)).ConfigureAwait(false);

                _logger.LogDebug("Role {Role} created", options.Role);
            }

            await connection.Connection.ExecuteScalarAsync<int>(string.Format(GrantRoleSql, options.Role, options.Schema)).ConfigureAwait(false);

            _logger.LogDebug("Role {Role} granted access to schema {Schema}", options.Role, options.Schema);

            if (string.IsNullOrWhiteSpace(options.Username))
                throw new ArgumentException("The SQL transport migrator requires a valid Username, but Username was not specified", nameof(options));

            result = await connection.Connection.ExecuteScalarAsync<int?>(string.Format(RoleExistsSql, options.Username)).ConfigureAwait(false);
            if (!result.HasValue)
            {
                result = await connection.Connection
                    .ExecuteScalarAsync<int?>(string.Format(CreateUserSql, options.Schema, options.Username))
                    .ConfigureAwait(false);

                if (result is 1)
                    _logger.LogDebug("User {Username} created", options.Username);
            }

            result = await connection.Connection.ExecuteScalarAsync<int?>(string.Format(IsRoleMemberSql, options.Role, options.Username)).ConfigureAwait(false);
            if (result is null or 0)
            {
                await connection.Connection
                    .ExecuteScalarAsync<int>(string.Format(AddRoleMemberSql, options.Database, options.Username, options.Role))
                    .ConfigureAwait(false);

                _logger.LogDebug("User {Username} added to role {Role}", options.Username, options.Role);
            }
        }
    }
}
