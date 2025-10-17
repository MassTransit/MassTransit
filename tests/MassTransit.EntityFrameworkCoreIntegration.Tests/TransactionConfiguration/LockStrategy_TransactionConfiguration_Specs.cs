namespace MassTransit.EntityFrameworkCoreIntegration.Tests.TransactionConfiguration
{
    using System;
    using System.Data;
    using System.Linq;
    using EntityFrameworkCoreIntegration;
    using EntityFrameworkCoreIntegration.Saga;
    using MassTransit.Tests.Saga;
    using NUnit.Framework;
    using Shared;
    using SimpleSaga.DataAccess;


    /// <summary>
    /// Tests for lock strategy transaction configuration functionality
    /// </summary>
    [TestFixture(typeof(SqlServerTestDbParameters))]
    [TestFixture(typeof(SqlServerResiliencyTestDbParameters))]
    [TestFixture(typeof(PostgresTestDbParameters))]
    public class LockStrategy_TransactionConfiguration_Specs<T> :
        EntityFrameworkTestFixture<T, SimpleSagaDbContext>
        where T : ITestDbParameters, new()
    {
        [Test]
        public void OptimisticLockStrategy_Should_Have_Transaction_Enabled_By_Default()
        {
            var queryExecutor = new OptimisticLoadQueryExecutor<SimpleSaga>(null);
            var lockStrategy = new OptimisticSagaRepositoryLockStrategy<SimpleSaga>(
                queryExecutor, null, IsolationLevel.ReadCommitted, true);

            Assert.That(lockStrategy.IsTransactionEnabled, Is.True,
                "OptimisticSagaRepositoryLockStrategy should have transactions enabled by default");
            Assert.That(lockStrategy.IsolationLevel, Is.EqualTo(IsolationLevel.ReadCommitted),
                "IsolationLevel should be preserved");
        }

        [Test]
        public void OptimisticLockStrategy_Should_Allow_Transaction_Disabled()
        {
            var queryExecutor = new OptimisticLoadQueryExecutor<SimpleSaga>(null);
            var lockStrategy = new OptimisticSagaRepositoryLockStrategy<SimpleSaga>(
                queryExecutor, null, IsolationLevel.ReadCommitted, false);

            Assert.That(lockStrategy.IsTransactionEnabled, Is.False,
                "OptimisticSagaRepositoryLockStrategy should allow transactions to be disabled");
            Assert.That(lockStrategy.IsolationLevel, Is.EqualTo(IsolationLevel.ReadCommitted),
                "IsolationLevel should be preserved when transactions are disabled");
        }

        [Test]
        public void OptimisticLockStrategy_Should_Support_Different_Isolation_Levels()
        {
            var queryExecutor = new OptimisticLoadQueryExecutor<SimpleSaga>(null);
            
            var lockStrategyReadUncommitted = new OptimisticSagaRepositoryLockStrategy<SimpleSaga>(
                queryExecutor, null, IsolationLevel.ReadUncommitted, false);
            var lockStrategySerializable = new OptimisticSagaRepositoryLockStrategy<SimpleSaga>(
                queryExecutor, null, IsolationLevel.Serializable, false);

            Assert.That(lockStrategyReadUncommitted.IsTransactionEnabled, Is.False,
                "Transaction setting should be preserved regardless of isolation level");
            Assert.That(lockStrategySerializable.IsTransactionEnabled, Is.False,
                "Transaction setting should be preserved regardless of isolation level");
            
            Assert.That(lockStrategyReadUncommitted.IsolationLevel, Is.EqualTo(IsolationLevel.ReadUncommitted));
            Assert.That(lockStrategySerializable.IsolationLevel, Is.EqualTo(IsolationLevel.Serializable));
        }

        [Test]
        public void PessimisticLockStrategy_Should_Have_Transaction_Enabled_By_Default()
        {
            var queryExecutor = new PessimisticLoadQueryExecutor<SimpleSaga>(RawSqlLockStatements, null);
            var lockStrategy = new PessimisticSagaRepositoryLockStrategy<SimpleSaga>(
                queryExecutor, IsolationLevel.Serializable);

            Assert.That(lockStrategy.IsTransactionEnabled, Is.True,
                "PessimisticSagaRepositoryLockStrategy should have transactions enabled by default");
            Assert.That(lockStrategy.IsolationLevel, Is.EqualTo(IsolationLevel.Serializable),
                "IsolationLevel should be preserved");
        }

        [Test]
        public void PessimisticLockStrategy_Should_Support_Different_Isolation_Levels()
        {
            var queryExecutor = new PessimisticLoadQueryExecutor<SimpleSaga>(RawSqlLockStatements, null);
            
            var lockStrategyReadCommitted = new PessimisticSagaRepositoryLockStrategy<SimpleSaga>(
                queryExecutor, IsolationLevel.ReadCommitted);
            var lockStrategyRepeatableRead = new PessimisticSagaRepositoryLockStrategy<SimpleSaga>(
                queryExecutor, IsolationLevel.RepeatableRead);

            Assert.That(lockStrategyReadCommitted.IsTransactionEnabled, Is.False,
                "Transaction setting should be preserved regardless of isolation level");
            Assert.That(lockStrategyRepeatableRead.IsTransactionEnabled, Is.False,
                "Transaction setting should be preserved regardless of isolation level");
            
            Assert.That(lockStrategyReadCommitted.IsolationLevel, Is.EqualTo(IsolationLevel.ReadCommitted));
            Assert.That(lockStrategyRepeatableRead.IsolationLevel, Is.EqualTo(IsolationLevel.RepeatableRead));
        }

        [Test]
        public void Should_Create_Lock_Strategies_With_Custom_Query_Customization()
        {
            Func<IQueryable<SimpleSaga>, IQueryable<SimpleSaga>> customization = q => q.Where(s => s.Name != null);

            var optimisticExecutor = new OptimisticLoadQueryExecutor<SimpleSaga>(customization);
            var optimisticLockStrategy = new OptimisticSagaRepositoryLockStrategy<SimpleSaga>(
                optimisticExecutor, customization, IsolationLevel.ReadCommitted, false);

            var pessimisticExecutor = new PessimisticLoadQueryExecutor<SimpleSaga>(RawSqlLockStatements, customization);
            var pessimisticLockStrategy = new PessimisticSagaRepositoryLockStrategy<SimpleSaga>(
                pessimisticExecutor, IsolationLevel.Serializable);

            Assert.That(optimisticLockStrategy.IsTransactionEnabled, Is.False);
            Assert.That(pessimisticLockStrategy.IsTransactionEnabled, Is.False);
            
            // Verify strategies were created successfully with customization
            Assert.That(optimisticLockStrategy, Is.Not.Null);
            Assert.That(pessimisticLockStrategy, Is.Not.Null);
        }
    }
}
