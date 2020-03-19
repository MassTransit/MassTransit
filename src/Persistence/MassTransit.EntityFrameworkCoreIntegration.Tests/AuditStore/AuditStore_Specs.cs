// Copyright 2007-2017 Chris Patterson, Dru Sellers, Travis Smith, et. al.
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use
// this file except in compliance with the License. You may obtain a copy of the
// License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software distributed
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR
// CONDITIONS OF ANY KIND, either express or implied. See the License for the
// specific language governing permissions and limitations under the License.
namespace MassTransit.EntityFrameworkCoreIntegration.Tests.AuditStore
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Audit;
    using GreenPipes.Util;
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
            var consumed = _harness.Consumed;
            await Task.Delay(2000);
            (await GetAuditRecords("Consume", consumed.Count(), TimeSpan.FromSeconds(10))).ShouldBe(consumed.Count());
        }

        [Test]
        public async Task Should_have_send_audit_record()
        {
            var sent = _harness.Sent;
            await Task.Delay(2000);
            (await GetAuditRecords("Send", sent.Count(), TimeSpan.FromSeconds(10))).ShouldBe(sent.Count());
        }

        [SetUp]
        public async Task CleanAudit()
        {
        }

        InMemoryTestHarness _harness;
        ConsumerTestHarness<TestConsumer> _consumer;
        EntityFrameworkAuditStore _store;

        [OneTimeSetUp]
        public async Task SetUp()
        {
            var contextFactory = new AuditContextFactory();

            await using (var context = contextFactory.CreateDbContext(DbContextOptionsBuilder))
            {
                await context.Database.MigrateAsync();
            }

            _store = new EntityFrameworkAuditStore(DbContextOptionsBuilder.Options, "EfCoreAudit");

            _harness = new InMemoryTestHarness();
            _harness.OnConnectObservers += bus =>
            {
                bus.ConnectSendAuditObservers(_store);
                bus.ConnectConsumeAuditObserver(_store);
            };
            _harness.Consumer<TestConsumer>();

            await _harness.Start();

            await _harness.InputQueueSendEndpoint.Send(new A());
        }

        [OneTimeTearDown]
        public async Task Teardown()
        {
            await _harness.Stop();

            var contextFactory = new AuditContextFactory();

            await using (var context = contextFactory.CreateDbContext(DbContextOptionsBuilder))
            {
                context.Database.EnsureDeleted();
            }
        }

        async Task<int> GetAuditRecords(string contextType, int expected, TimeSpan timeout)
        {
            DateTime giveUpAt = DateTime.Now + timeout;

            int count = 0;
            while (DateTime.Now < giveUpAt)
            {
                await using (var dbContext = _store.AuditContext)
                    count = await dbContext.Set<AuditRecord>()
                        .Where(x => x.ContextType == contextType)
                        .CountAsync();

                if (count == expected)
                    return count;

                await Task.Delay(100).ConfigureAwait(false);
            }

            return count;
        }


        class TestConsumer : IConsumer<A>,
            IConsumer<B>
        {
            public async Task Consume(ConsumeContext<A> context) => await context.RespondAsync(new B());

            public Task Consume(ConsumeContext<B> context)
            {
                return TaskUtil.Completed;
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
