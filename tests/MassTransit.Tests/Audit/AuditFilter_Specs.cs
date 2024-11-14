namespace MassTransit.Tests.Audit
{
    using System.Linq;
    using System.Threading.Tasks;
    using MassTransit.Testing;
    using NUnit.Framework;


    [TestFixture]
    public class AuditFilter_Specs
    {
        [Test]
        public async Task Should_audit_and_filter_consumed_messages()
        {
            var expected = _harness.Consumed.Select<A>().Any();
            var expectedB = _harness.Consumed.Select<B>().Any();

            Assert.That(_store.Count(x => x.Result.Metadata.ContextType == "Consume"), Is.EqualTo(1));
        }

        [Test]
        public async Task Should_audit_and_filter_sent_messages()
        {
            var expected = _harness.Sent.Select<A>().Any();
            var expectedB = _harness.Sent.Select<B>().Any();

            Assert.That(_store.Count(x => x.Result.Metadata.ContextType == "Send"), Is.EqualTo(1));
        }

        InMemoryTestHarness _harness;
        InMemoryAuditStore _store;

        [OneTimeSetUp]
        public async Task Send_message_to_test_consumer()
        {
            _store = new InMemoryAuditStore();
            _harness = new InMemoryTestHarness();
            _harness.OnConfigureInMemoryBus += configurator =>
            {
                configurator.ConnectSendAuditObservers(_store, c => c.Exclude<B>());
                configurator.ConnectConsumeAuditObserver(_store, c => c.Exclude<A>());
            };
            _harness.Consumer<TestConsumer>();

            await _harness.Start();

            Task<ConsumeContext<B>> response = _harness.SubscribeHandler<B>();

            await _harness.InputQueueSendEndpoint.Send(new A(), x => x.ResponseAddress = _harness.BusAddress);

            await response;
        }

        [OneTimeTearDown]
        public async Task OneTimeTearDown()
        {
            await _harness.Stop();

            _harness.Dispose();
        }

        class TestConsumer : IConsumer<A>
        {
            public async Task Consume(ConsumeContext<A> context)
            {
                await context.RespondAsync(new B());
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
