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
    using MassTransit.Tests.Audit;
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
                bus.ConnectSendAuditObservers(_store);
                bus.ConnectConsumeAuditObserver(_store);
            };
            _harness.Consumer<TestConsumer>();

            await _harness.Start();

            var response = _harness.SubscribeHandler<B>();

            await _harness.InputQueueSendEndpoint.Send(new A(), x => x.ResponseAddress = _harness.BusAddress);

            await response;
        }

        [Test]
        public async Task Should_audit_sent_messages()
        {
            var expected = _harness.Sent.Select<A>().Any();
            var expectedB = _harness.Sent.Select<B>().Any();
            _store.Count(x => x.Metadata.ContextType == "Send").ShouldBe(3);
        }

        [Test]
        public async Task Should_audit_consumed_messages()
        {
            bool expected = _harness.Consumed.Select<A>().Any();
            bool expectedB = _harness.Consumed.Select<B>().Any();
            _store.Count(x => x.Metadata.ContextType == "Consume").ShouldBe(2);
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