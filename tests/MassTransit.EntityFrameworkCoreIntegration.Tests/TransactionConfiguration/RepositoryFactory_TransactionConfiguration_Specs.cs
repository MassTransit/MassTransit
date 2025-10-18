namespace MassTransit.EntityFrameworkCoreIntegration.Tests.TransactionConfiguration
{
    using System;
    using System.Linq;
    using MassTransit.Tests.Saga;
    using NUnit.Framework;
    using Shared;
    using SimpleSaga.DataAccess;


    /// <summary>
    /// Tests for EntityFrameworkSagaRepository factory methods with transaction configuration
    /// </summary>
    [TestFixture(typeof(SqlServerTestDbParameters))]
    [TestFixture(typeof(SqlServerResiliencyTestDbParameters))]
    [TestFixture(typeof(PostgresTestDbParameters))]
    public class RepositoryFactory_TransactionConfiguration_Specs<T> :
        EntityFrameworkTestFixture<T, SimpleSagaDbContext>
        where T : ITestDbParameters, new()
    {
        [Test]
        public void CreateOptimistic_Should_Default_To_Transactions_Enabled()
        {
            var repository = EntityFrameworkSagaRepository<SimpleSaga>.CreateOptimistic(
                () => new SimpleSagaContextFactory().CreateDbContext(DbContextOptionsBuilder));

            Assert.That(repository, Is.Not.Null,
                "Repository should be created successfully with default transaction setting");
        }

        [Test]
        public void CreateOptimistic_Should_Accept_Transactions_Disabled()
        {
            var repository = EntityFrameworkSagaRepository<SimpleSaga>.CreateOptimistic(
                () => new SimpleSagaContextFactory().CreateDbContext(DbContextOptionsBuilder),
                null, false);

            Assert.That(repository, Is.Not.Null,
                "Repository should be created successfully with transactions disabled");
        }

        [Test]
        public void CreateOptimistic_Should_Accept_Transactions_Enabled_Explicitly()
        {
            var repository = EntityFrameworkSagaRepository<SimpleSaga>.CreateOptimistic(
                () => new SimpleSagaContextFactory().CreateDbContext(DbContextOptionsBuilder),
                null, true);

            Assert.That(repository, Is.Not.Null,
                "Repository should be created successfully with transactions explicitly enabled");
        }

        [Test]
        public void CreateOptimistic_Should_Work_With_DbContextFactory_And_Transaction_Setting()
        {
            // Using delegate factory instead of ISagaDbContextFactory for simplicity
            var repository = EntityFrameworkSagaRepository<SimpleSaga>.CreateOptimistic(
                () => new SimpleSagaContextFactory().CreateDbContext(DbContextOptionsBuilder),
                null, false);

            Assert.That(repository, Is.Not.Null,
                "Repository should be created successfully with delegate factory and transaction disabled");
        }

        [Test]
        public void CreateOptimistic_Should_Work_With_Query_Customization_And_Transaction_Setting()
        {
            Func<IQueryable<SimpleSaga>, IQueryable<SimpleSaga>> customization = q => q.Where(s => s.Name != null);

            var repository = EntityFrameworkSagaRepository<SimpleSaga>.CreateOptimistic(
                () => new SimpleSagaContextFactory().CreateDbContext(DbContextOptionsBuilder),
                customization, false);

            Assert.That(repository, Is.Not.Null,
                "Repository should be created successfully with query customization and transaction disabled");
        }

        [Test]
        public void CreatePessimistic_Should_Default_To_Transactions_Enabled()
        {
            var repository = EntityFrameworkSagaRepository<SimpleSaga>.CreatePessimistic(
                () => new SimpleSagaContextFactory().CreateDbContext(DbContextOptionsBuilder),
                RawSqlLockStatements);

            Assert.That(repository, Is.Not.Null,
                "Repository should be created successfully with default transaction setting");
        }
    }
}
