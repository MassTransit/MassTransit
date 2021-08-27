USE [master]
GO

IF DB_ID('onramp-sample1') IS NOT NULL
  set noexec on               -- prevent creation when already exists

/****** Object:  Database [outbox-sample1]    Script Date: 18.10.2019 18:33:09 ******/
CREATE DATABASE [onramp-sample1];
GO
