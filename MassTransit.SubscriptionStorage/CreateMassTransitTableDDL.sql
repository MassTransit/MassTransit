USE [MassTransitSubscriptions]
GO

/*
	DROP THE TABLES IF THEY EXIST
*/


IF EXISTS(SELECT * FROM sys.all_objects WHERE UPPER([name]) = 'Subscriptions') 
  BEGIN
    DROP TABLE bus.Subscriptions
    PRINT '<<< DROPPED TABLE Subscriptions >>>'
  END
GO


IF EXISTS(SELECT * FROM sys.schemas WHERE UPPER([name]) = 'BUS')
  BEGIN
	DROP SCHEMA bus
	PRINT '<<< DROPPED SCHEMA bus >>>'
  END
GO

CREATE SCHEMA bus AUTHORIZATION [dbo]
GO
PRINT '<<< CREATED SCHEMA bus >>>'
GO


/****** Object:  Table [bus].[Subscriptions]    Script Date: 04/16/2008 11:25:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [bus].[Subscriptions](
	 [Id] [int] IDENTITY(1,1) NOT NULL
	,[Address] [nvarchar](500) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL
	,[Message] [nvarchar](500) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL
	,[IsActive] [bit] NOT NULL
	,CONSTRAINT [PK_bus_Subscriptions] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

PRINT '<<< CREATED TABLE Subscriptions >>>'
GO