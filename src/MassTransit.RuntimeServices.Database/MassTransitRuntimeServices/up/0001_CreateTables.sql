CREATE TABLE "HealthSaga" 
(
	CorrelationId			UNIQUEIDENTIFIER	NOT NULL
	, CurrentState			NVARCHAR(255)		NULL
	, ControlUri			NVARCHAR(255)		NULL
	, DataUri				NVARCHAR(255)		NULL
	, LastHeartbeat			DATETIME			NULL
	, HeartbeatIntervalInSeconds INT			NULL
	, PRIMARY KEY (CorrelationId)
)

CREATE INDEX IX_HealthSaga_ ON HealthSaga ( ControlUri, CorrelationId )

CREATE TABLE "SubscriptionSaga" 
(
	CorrelationId			UNIQUEIDENTIFIER	NOT NULL
	, ClientId				UNIQUEIDENTIFIER	NULL
	, MessageCorrelationId	NVARCHAR(255)		NULL
	, EndpointUri			NVARCHAR(255)		NULL
	, MessageName			NVARCHAR(255)		NULL
	, SequenceNumber		BIGINT				NULL
	, SubscriptionId		UNIQUEIDENTIFIER	NULL
	, CurrentState			NVARCHAR(255)		NULL
	, PRIMARY KEY (CorrelationId)
)

CREATE TABLE "SubscriptionClientSaga" 
(
	CorrelationId			UNIQUEIDENTIFIER	NOT NULL
	, CurrentState			NVARCHAR(255)		NULL
	, ControlUri			NVARCHAR(255)		NULL
	, DataUri				NVARCHAR(255)		NULL
	, PRIMARY KEY (CorrelationId)
)

CREATE INDEX IX_SubscriptionClientSaga_ControlUri ON SubscriptionClientSaga ( ControlUri, CorrelationId )

CREATE TABLE "TimeoutSaga" 
(
	CorrelationId			UNIQUEIDENTIFIER	NOT NULL
	, Tag					INT					NOT NULL
	, CurrentState			NVARCHAR(255)		NULL
	, TimeoutAt				DATETIME			NULL
	, PRIMARY KEY (CorrelationId, Tag)
)

CREATE INDEX IX_Timeout_At ON TimeoutSaga ( TimeoutAt, CorrelationId, Tag )
