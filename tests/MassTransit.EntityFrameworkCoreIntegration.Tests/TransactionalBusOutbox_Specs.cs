namespace MassTransit.EntityFrameworkCoreIntegration.Tests
{
    using System;
    using System.Threading.Tasks;
    using System.Transactions;
    using Internals;
    using MassTransit.Tests.Saga.Messages;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.ChangeTracking;
    using NUnit.Framework;
    using TestFramework;
    using Transactions;


    /// <summary>
    /// This test fixture has nothing to do with the Saga, but I wanted to test EFCore with the TransactionOutbox,
    /// so this was the easiest project to add a test spec to which has EF Core already referenced.
    /// </summary>
    [TestFixture]
    public class TransactionalBusOutbox_Specs :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_not_publish_properly()
        {
            var message = new InitiateSimpleSaga();
            var product = new Product {Name = "Should_not_publish_properly"};
            var transactionOutbox = new TransactionalEnlistmentBus(Bus);

            using (var dbContext = GetDbContext())
            using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                EntityEntry<Product> entity = dbContext.Products.Add(product);
                await dbContext.SaveChangesAsync();

                await transactionOutbox.Publish(message);
            }

            Assert.That(async () => await _received.OrTimeout(s: 3), Throws.TypeOf<TimeoutException>());

            using (var dbContext = GetDbContext())
            {
                Assert.IsFalse(await dbContext.Products.AnyAsync(x => x.Id == product.Id));
            }
        }

        [Test]
        public async Task Should_publish_after_db_create()
        {
            var message = new InitiateSimpleSaga();
            var product = new Product {Name = "Should_publish_after_db_create"};
            var transactionOutbox = new TransactionalEnlistmentBus(Bus);

            using (var dbContext = GetDbContext())
            using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                dbContext.Products.Add(product);
                await dbContext.SaveChangesAsync();

                await transactionOutbox.Publish(message);

                // Hasn't published yet
                Assert.That(async () => await _received.OrTimeout(s: 3), Throws.TypeOf<TimeoutException>());

                transaction.Complete();
            }

            // Now has published
            await _received;

            using (var dbContext = GetDbContext())
            {
                Assert.IsTrue(await dbContext.Products.AnyAsync(x => x.Id == product.Id));
            }
        }

        [Test]
        [Category("Flaky")]
        public async Task Should_publish_after_db_create_outbox_bus()
        {
            var message = new InitiateSimpleSaga();
            var product = new Product { Name = "Should_publish_after_db_create" };
            var bus = new TransactionalBus(Bus);

            using (var dbContext = GetDbContext())
            {
                dbContext.Products.Add(product);
                await dbContext.SaveChangesAsync();

                await bus.Publish(message);

                // Hasn't published yet
                Assert.That(async () => await _received.OrTimeout(s: 3), Throws.TypeOf<TimeoutException>());
            }

            await bus.Release();

            // Now has published
            await _received;

            using (var dbContext = GetDbContext())
            {
                Assert.IsTrue(await dbContext.Products.AnyAsync(x => x.Id == product.Id));
            }
        }

        Task<ConsumeContext<InitiateSimpleSaga>> _received;

        TransactionOutboxTestsDbContext GetDbContext()
        {
            var dbContext = new TransactionOutboxTestsDbContext(new DbContextOptionsBuilder()
                .UseSqlServer(LocalDbConnectionStringProvider.GetLocalDbConnectionString("MassTransitUnitTests_TransactionOutbox")).Options);
            return dbContext;
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _received = Handled<InitiateSimpleSaga>(configurator);
        }

        public TransactionalBusOutbox_Specs()
        {
            using (var dbContext = GetDbContext())
            {
                dbContext.Database.EnsureDeleted();
                dbContext.Database.EnsureCreated();
                //RelationalDatabaseCreator databaseCreator = (RelationalDatabaseCreator)dbContext.Database.GetService<IDatabaseCreator>();
                //databaseCreator.CreateTables();
            }
        }
    }


    public class TransactionOutboxTestsDbContext :
        DbContext
    {
        public TransactionOutboxTestsDbContext(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
    }


    public class Product
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
    }
}
