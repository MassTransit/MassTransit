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
namespace MassTransit.Audit.Tests
{
    using System.Linq;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using Shouldly;
    using Testing;


    [TestFixture]
    public class Audit_Specs
    {
        InMemoryTestHarness _harness;
        InMemoryAuditStore _store;

        [OneTimeSetUp]
        public async Task Send_message_to_test_consumer()
        {
            _store = new InMemoryAuditStore();
            _harness = new InMemoryTestHarness();
            _harness.OnConnectObservers += bus =>
            {
                bus.UseSendAudit(_store);
                bus.UseConsumeAudit(_store);
            };
            _harness.Consumer<TestConsumer>();

            await _harness.Start();

            await _harness.InputQueueSendEndpoint.Send(new A());
        }

        [Test]
        public async Task Should_audit_sent_messages()
        {
            var sentCount = _harness.Sent.Select().Count();
            _store.Audit.Count(x => x.Metadata.ContextType == "Send").ShouldBe(sentCount);
        }

        [Test]
        public async Task Should_audit_consumed_messages()
        {
            var consumedCount = _harness.Consumed.Select().Count();
            _store.Audit.Count(x => x.Metadata.ContextType == "Consume").ShouldBe(consumedCount);
        }


        class TestConsumer : IConsumer<A>
        {
            public async Task Consume(ConsumeContext<A> context) =>
                await context.RespondAsync(new B());
        }


        class A
        {
        }


        class B
        {
        }
    }
}