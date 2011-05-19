CREATE TABLE [HealthSaga] (
  [CorrelationId] uniqueidentifier NOT NULL
, [CurrentState] nvarchar(255) NULL
, [ControlUri] nvarchar(255) NULL
, [DataUri] nvarchar(255) NULL
, [LastHeartbeat] datetime NULL
, [HeartbeatIntervalInSeconds] int NULL
);
GO
CREATE TABLE [SubscriptionClientSaga] (
  [CorrelationId] uniqueidentifier NOT NULL
, [CurrentState] nvarchar(255) NULL
, [ControlUri] nvarchar(255) NULL
, [DataUri] nvarchar(255) NULL
);
GO
CREATE TABLE [SubscriptionSaga] (
  [CorrelationId] uniqueidentifier NOT NULL
, [ClientId] uniqueidentifier NULL
, [MessageCorrelationId] nvarchar(255) NULL
, [EndpointUri] nvarchar(255) NULL
, [MessageName] nvarchar(255) NULL
, [SequenceNumber] bigint NULL
, [SubscriptionId] uniqueidentifier NULL
, [CurrentState] nvarchar(255) NULL
);
GO
CREATE TABLE [TimeoutSaga] (
  [CorrelationId] uniqueidentifier NOT NULL
, [Tag] int NOT NULL
, [CurrentState] nvarchar(255) NULL
, [TimeoutAt] datetime NULL
);
GO
ALTER TABLE [HealthSaga] ADD CONSTRAINT [PK__HealthSaga__000000000000000E] PRIMARY KEY ([CorrelationId]);
GO
ALTER TABLE [SubscriptionClientSaga] ADD CONSTRAINT [PK__SubscriptionClientSaga__0000000000000032] PRIMARY KEY ([CorrelationId]);
GO
ALTER TABLE [SubscriptionSaga] ADD CONSTRAINT [PK__SubscriptionSaga__0000000000000024] PRIMARY KEY ([CorrelationId]);
GO
ALTER TABLE [TimeoutSaga] ADD CONSTRAINT [PK__TimeoutSaga__0000000000000040] PRIMARY KEY ([CorrelationId],[Tag]);
GO
CREATE INDEX [IX_Health_ControlUri] ON [HealthSaga] ([ControlUri] ASC,[CorrelationId] ASC);
GO
CREATE INDEX [IX_SubscriptionClient_ControlUri] ON [SubscriptionClientSaga] ([ControlUri] ASC,[CurrentState] ASC,[CorrelationId] ASC);
GO
CREATE INDEX [IX_Subscription_ClientId] ON [SubscriptionSaga] ([ClientId] ASC,[CurrentState] ASC);
GO
CREATE INDEX [IX_Subscription_Endpoint] ON [SubscriptionSaga] ([EndpointUri] ASC,[CurrentState] ASC,[ClientId] ASC);
GO
CREATE INDEX [IX_Subscription_Full] ON [SubscriptionSaga] ([EndpointUri] ASC,[MessageName] ASC,[CurrentState] ASC,[ClientId] ASC);
GO
