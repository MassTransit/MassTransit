USE [master]
GO

IF EXISTS(SELECT * FROM sysdatabases WHERE UPPER([name]) = 'MASSTRANSITSUBSCRIPTIONS')
  BEGIN
	EXEC msdb.dbo.sp_delete_database_backuphistory @database_name = N'MassTransitSubscriptions'
	USE [master]
	DROP DATABASE [MassTransitSubscriptions]
	PRINT '<<< REMOVED MassTransitSubscriptions Database >>>'
  END
GO

IF EXISTS(SELECT * FROM msdb.sys.syslogins WHERE UPPER([name]) = 'APP-MASSTRANSITSUBSCRIPTIONMANAGER') 
  BEGIN
	DROP LOGIN [App-MassTransitSubscriptionManager]
	PRINT '<<< REMOVED INSTANCE LOGIN FOR App-MassTransitSubscriptionManager >>>'
  END
GO


PRINT '<<< Database Uninstall Complete >>>'