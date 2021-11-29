namespace MassTransit.EntityFrameworkCoreIntegration.Tests.AuditStore
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Audit;
    using Microsoft.EntityFrameworkCore;
    using NUnit.Framework;
    using Shared;
    using Shouldly;
    using Testing;


    [TestFixture(typeof(SqlServerTestDbParameters))]
    [TestFixture(typeof(SqlServerResiliencyTestDbParameters))]
    [TestFixture(typeof(PostgresTestDbParameters))]
    public class Saving_audit_records_to_the_audit_store<T> :
        EntityFrameworkTestFixture<T, AuditDbContext>
        where T : ITestDbParameters, new()
    {
        [Test]
        public async Task Should_have_consume_audit_records()
        {
            var consumed = InMemoryTestHarness.Consumed;
            await Task.Delay(2000);
            (await GetAuditRecords("Consume", consumed.Count(), TimeSpan.FromSeconds(10))).ShouldBe(consumed.Count());
        }

        [Test]
        public async Task Should_have_send_audit_record()
        {
            var sent = InMemoryTestHarness.Sent;
            await Task.Delay(2000);
            (await GetAuditRecords("Send", sent.Count(), TimeSpan.FromSeconds(10))).ShouldBe(sent.Count());
        }

        [SetUp]
        public async Task CleanAudit()
        {
        }

        AuditContextFactory _contextFactory;

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.UseEntityFrameworkCoreAuditStore(DbContextOptionsBuilder, "EFCoreAudit");
            base.ConfigureInMemoryBus(configurator);
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.Consumer<TestConsumer>();
        }

        [OneTimeTearDown]
        public async Task Teardown()
        {
            var contextFactory = new AuditContextFactory();
            await using var context = contextFactory.CreateDbContext(DbContextOptionsBuilder);
            await context.Database.EnsureDeletedAsync();
        }

        [OneTimeSetUp]
        public async Task SetUp()
        {
            _contextFactory = new AuditContextFactory();
            await using var context = _contextFactory.CreateDbContext(DbContextOptionsBuilder);

            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();

            await InputQueueSendEndpoint.Send(new A());
        }

        async Task<int> GetAuditRecords(string contextType, int expected, TimeSpan timeout)
        {
            var giveUpAt = DateTime.Now + timeout;

            var count = 0;
            while (DateTime.Now < giveUpAt)
            {
                await using (var dbContext = _contextFactory.CreateDbContext(DbContextOptionsBuilder))
                {
                    count = await dbContext.Set<AuditRecord>()
                        .Where(x => x.ContextType == contextType)
                        .CountAsync();
                }

                if (count == expected)
                    return count;

                await Task.Delay(100).ConfigureAwait(false);
            }

            return count;
        }


        class TestConsumer : IConsumer<A>,
            IConsumer<B>
        {
            public async Task Consume(ConsumeContext<A> context)
            {
                await context.RespondAsync(new B());
            }

            public Task Consume(ConsumeContext<B> context)
            {
                return Task.CompletedTask;
            }
        }


        class A
        {
        }


        class B
        {
        }
    }
}
