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
namespace MassTransit.EntityFrameworkIntegration.Tests
{
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using System.Threading.Tasks;
    using Audit;
    using GreenPipes.Util;
    using NUnit.Framework;
    using Shouldly;
    using Testing;


    [TestFixture]
    public class Saving_audit_records_to_the_audit_store
    {
        [Test]
        public async Task Should_have_consume_audit_records()
        {
            var consumed = _harness.Consumed;
            await Task.Delay(500);
            (await GetAuditRecords("Consume")).ShouldBe(consumed.Count());
        }

        [Test]
        public async Task Should_have_send_audit_record()
        {
            var sent = _harness.Sent;
            await Task.Delay(500);
            (await GetAuditRecords("Send")).ShouldBe(sent.Count());
        }

        [SetUp]
        public async Task CleanAudit()
        {
        }

        InMemoryTestHarness _harness;
        ConsumerTestHarness<TestConsumer> _consumer;
        EntityFrameworkAuditStore _store;

        [OneTimeSetUp]
        public async Task Send_message_to_test_consumer()
        {
            Database.SetInitializer(new DropCreateDatabaseAlways<AuditDbContext>());

            _store = new EntityFrameworkAuditStore(
                LocalDbConnectionStringProvider.GetLocalDbConnectionString(), "audit");
            using (var dbContext = _store.AuditContext)
            {
                dbContext.Database.Initialize(true);
                await dbContext.Database.ExecuteSqlCommandAsync("TRUNCATE TABLE audit");
            }

            _harness = new InMemoryTestHarness();
            _harness.OnConnectObservers += bus =>
            {
                bus.ConnectSendAuditObservers(_store);
                bus.ConnectConsumeAuditObserver(_store);
            };
            _consumer = _harness.Consumer<TestConsumer>();

            await _harness.Start();

            await _harness.InputQueueSendEndpoint.Send(new A());
        }

        [OneTimeTearDown]
        public async Task Teardown()
        {
            await _harness.Stop();
        }

        async Task<int> GetAuditRecords(string contextType)
        {
            using (var dbContext = _store.AuditContext)
                return await dbContext.Set<AuditRecord>()
                    .Where(x => x.ContextType == contextType)
                    .CountAsync();
        }


        class TestConsumer : IConsumer<A>,
            IConsumer<B>
        {
            public async Task Consume(ConsumeContext<A> context) =>
                await context.RespondAsync(new B());

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
