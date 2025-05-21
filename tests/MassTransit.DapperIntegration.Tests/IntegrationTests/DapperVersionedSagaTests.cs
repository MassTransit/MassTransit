namespace MassTransit.DapperIntegration.Tests.IntegrationTests
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Dapper;
    using MassTransit.Tests;
    using Microsoft.Data.SqlClient;
    using NUnit.Framework;
    using TestFramework;

    public abstract class DapperVersionedSagaTests : InMemoryTestFixture
    {
        protected readonly string ConnectionString;
        protected readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(3);
        
        protected static readonly Guid SagaId = Guid.Parse("d747db39-0d64-49b5-85f4-2a796ba82130");

        protected DapperVersionedSagaTests()
        {
            ConnectionString = LocalDbConnectionStringProvider.GetLocalDbConnectionString();
        }
        
        [OneTimeSetUp]
        public async Task Initialize()
        {
            var sql = @"DROP TABLE IF EXISTS VersionedSagas;

CREATE TABLE VersionedSagas (
    [CorrelationId] UNIQUEIDENTIFIER NOT NULL,
    [Version] INT NOT NULL,
    [CurrentState] VARCHAR(20),

    [Name] NVARCHAR(MAX),
    
    PRIMARY KEY CLUSTERED (CorrelationId)
);";
            try
            {
                await ExecuteSql(sql);
            }
            catch ( SqlException e )
            {
                var builder = new SqlConnectionStringBuilder(ConnectionString);
                builder.Password = "********";
                var sanitized = builder.ToString();

                throw new Exception($"Failure initializing test: {e.Message}, ConnectionString: {sanitized}", e);
            }
        }

        [OneTimeTearDown]
        public async Task Teardown()
        {
            var sql = @"DROP TABLE IF EXISTS VersionedSagas;";
            await ExecuteSql(sql);
        }

        [SetUp]
        public async Task Setup()
        {
            var sql = @"TRUNCATE TABLE VersionedSagas;";
            await ExecuteSql(sql);
        }

        protected async Task ExecuteSql(string sql)
        {
            await using var connection = new SqlConnection(ConnectionString);
            await connection.ExecuteAsync(sql);
        }

        protected async Task<List<TSaga>> GetSagas<TSaga>() where TSaga : class, ISaga
        {
            await using var connection = new SqlConnection(ConnectionString);
            var sql = "SELECT * FROM VersionedSagas;";
            return (await connection.QueryAsync<TSaga>(sql)).AsList();
        }
    }
}
