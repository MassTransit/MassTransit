namespace MassTransit.MongoDbIntegration.Tests.Audit
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using MongoDbIntegration.Audit;
    using Newtonsoft.Json;
    using NUnit.Framework;
    using Testing;
    using static MongoDbAuditStoreFixture;


    [TestFixture]
    public class Produces_an_audit_record_for_a_sent_message
    {
        [Test]
        public async Task Audit_document_gets_created()
        {
            Assert.Multiple(() =>
            {
                Assert.That(_auditDocument.ContextType, Is.EqualTo("Send"));
                Assert.That(_auditDocument.MessageId, Is.EqualTo(_sent.Context.MessageId.Value.ToString()));
                Assert.That(_auditDocument.ConversationId, Is.EqualTo(_sent.Context.ConversationId.Value.ToString()));
                Assert.That(_auditDocument.DestinationAddress, Is.EqualTo(_sent.Context.DestinationAddress.ToString()));
                Assert.That(_auditDocument.MessageType, Is.EqualTo(typeof(A).FullName));
            });
        }

        [Test]
        public void Message_payload_matches_sent_message()
        {
            Assert.That(JsonConvert.DeserializeObject<A>(_auditDocument.Message).Data, Is.EqualTo(_sent.Context.Message.Data));
        }

        [Test]
        public void Metadata_deserializes_and_matches()
        {
            Assert.That(
                _sent.Context.Headers.Select(x => (x.Key, x.Value)),
                Is.EquivalentTo(_auditDocument.Headers.Select(x => (x.Key, x.Value)))
            );
        }

        InMemoryTestHarness _harness;
        ISentMessage<A> _sent;
        AuditDocument _auditDocument;

        const string TestData = "test data";

        [OneTimeSetUp]
        public async Task Setup()
        {
            _harness = new InMemoryTestHarness();
            _harness.OnConfigureInMemoryBus += configurator => configurator.ConnectSendAuditObservers(AuditStore);
            _harness.Consumer<TestConsumer>();

            await _harness.Start();

            await _harness.InputQueueSendEndpoint.Send(new A { Data = TestData });

            _sent = _harness.Sent.Select<A>().First();
            List<AuditDocument> audit = await GetAuditRecords("Send");
            _auditDocument = audit.Single();
        }

        [OneTimeTearDown]
        public Task Teardown()
        {
            return Task.WhenAll(_harness.Stop(), Cleanup());
        }
    }


    [TestFixture]
    public class Produces_an_audit_record_for_a_consumed_message
    {
        [Test]
        public async Task Audit_document_gets_created()
        {
            Assert.Multiple(() =>
            {
                Assert.That(_auditDocument.ContextType, Is.EqualTo("Consume"));
                Assert.That(_auditDocument.MessageId, Is.EqualTo(_consumed.Context.MessageId.Value.ToString()));
                Assert.That(_auditDocument.ConversationId, Is.EqualTo(_consumed.Context.ConversationId.Value.ToString()));
                Assert.That(_auditDocument.InputAddress, Is.EqualTo(_consumed.Context.ReceiveContext.InputAddress.ToString()));
                Assert.That(_auditDocument.DestinationAddress, Is.EqualTo(_consumed.Context.DestinationAddress.ToString()));
                Assert.That(_auditDocument.MessageType, Is.EqualTo(typeof(A).FullName));
            });
        }

        [Test]
        public void Message_payload_matches_sent_message()
        {
            Assert.That(JsonConvert.DeserializeObject<A>(_auditDocument.Message).Data, Is.EqualTo(_consumed.Context.Message.Data));
        }

        [Test]
        public void Metadata_deserializes_and_matches()
        {
            Assert.That(
                _consumed.Context.Headers.Select(x => (x.Key, x.Value)),
                Is.EquivalentTo(_auditDocument.Headers.Select(x => (x.Key, x.Value)))
            );
        }

        IReceivedMessage<A> _consumed;
        AuditDocument _auditDocument;
        InMemoryTestHarness _harness;

        [OneTimeSetUp]
        public async Task Send_message_to_test_consumer()
        {
            _harness = new InMemoryTestHarness();
            _harness.OnConfigureInMemoryBus += configurator => configurator.ConnectConsumeAuditObserver(AuditStore);

            ConsumerTestHarness<TestConsumer> consumer = _harness.Consumer<TestConsumer>();

            await _harness.Start();

            await _harness.InputQueueSendEndpoint.Send(new A());

            _consumed = consumer.Consumed.Select<A>().First();
            List<AuditDocument> audit = await GetAuditRecords("Consume");
            _auditDocument = audit.Single();
        }

        [OneTimeTearDown]
        public Task Teardown()
        {
            return Task.WhenAll(_harness.Stop(), Cleanup());
        }
    }


    [TestFixture]
    public class Can_store_two_records_for_both_send_and_consume
    {
        [Test]
        public void Should_have_the_consume_record()
        {
            var sentRecord = _audit.FirstOrDefault(x => x.ContextType == "Send");

            Assert.That(sentRecord, Is.Not.Null);
        }

        [Test]
        public void Should_have_the_sent_record()
        {
            var consumedRecord = _audit.FirstOrDefault(x => x.ContextType == "Consume");

            Assert.That(consumedRecord, Is.Not.Null);
        }

        [Test]
        public void The_number_of_records_is_two()
        {
            Assert.That(_audit, Has.Count.EqualTo(2));
        }

        InMemoryTestHarness _harness;
        List<AuditDocument> _audit;

        [OneTimeSetUp]
        public async Task Send_message_to_test_consumer()
        {
            _harness = new InMemoryTestHarness();
            _harness.OnConfigureInMemoryBus += configurator => configurator.UseMongoDbAuditStore(Database, AuditCollectionName);

            ConsumerTestHarness<TestConsumer> consumer = _harness.Consumer<TestConsumer>();

            await _harness.Start();

            await _harness.InputQueueSendEndpoint.Send(new A());

            IReceivedMessage<A> consumed = consumer.Consumed.Select<A>().First();
            _audit = await GetAuditRecordsForMessage(consumed.Context.MessageId.Value);
        }

        [OneTimeTearDown]
        public Task Teardown()
        {
            return Task.WhenAll(_harness.Stop(), Cleanup());
        }
    }


    class TestConsumer : IConsumer<A>
    {
        public Task Consume(ConsumeContext<A> context)
        {
            return Task.CompletedTask;
        }
    }


    class A
    {
        public string Data { get; set; }
    }
}
