

CREATE SCHEMA [bus]
GO

CREATE TABLE bus.Subscriptions
    (
	Id int NOT NULL IDENTITY (1, 1),
	Address nvarchar(500) NOT NULL,
	Message nvarchar(500) NOT NULL,
	IsActive bit NOT NULL
	)  ON [PRIMARY]
GO
ALTER TABLE bus.Subscriptions ADD CONSTRAINT
	PK_bus_Subscriptions PRIMARY KEY CLUSTERED 
	(
	Id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO