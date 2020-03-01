namespace MassTransit.Tests.Audit
{
    using System.Linq;
    using System.Threading.Tasks;
    using MassTransit.Testing;
    using NUnit.Framework;
    using Shouldly;


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
            _store.Count(x => x.Result.Metadata.ContextType == "Send").ShouldBe(2);
        }

        [Test]
        public async Task Should_audit_consumed_messages()
        {
            bool expected = _harness.Consumed.Select<A>().Any();
            bool expectedB = _harness.Consumed.Select<B>().Any();
            _store.Count(x => x.Result.Metadata.ContextType == "Consume").ShouldBe(2);
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
