namespace MassTransit.MongoDbIntegration.Tests.Courier
{
    using System;
    using System.Threading.Tasks;
    using MassTransit.Courier.Contracts;
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization;
    using MongoDB.Bson.Serialization.Serializers;
    using MongoDB.Driver;
    using MongoDbIntegration.Courier;
    using MongoDbIntegration.Courier.Consumers;
    using MongoDbIntegration.Courier.Documents;
    using MongoDbIntegration.Courier.Events;
    using NUnit.Framework;
    using Testing;


    [TestFixture]
    public class When_a_routing_slip_is_completed :
        MongoDbTestFixture
    {
        [Test]
        public async Task Should_process_the_event()
        {
            ConsumeContext<RoutingSlipCompleted> context = await _completed;

            Assert.Multiple(() =>
            {
                Assert.That(context.Message.TrackingNumber, Is.EqualTo(_trackingNumber));

                Assert.That(context.CorrelationId.HasValue, Is.True);
                Assert.That(context.CorrelationId.Value, Is.EqualTo(_trackingNumber));
            });
        }

        [Test]
        public async Task Should_upsert_the_event_into_the_routing_slip()
        {
            ConsumeContext<RoutingSlipCompleted> context = await _completed;

            await Task.Delay(1000);

            FilterDefinition<RoutingSlipDocument> query = Builders<RoutingSlipDocument>.Filter.Eq(x => x.TrackingNumber, _trackingNumber);

            var routingSlip = await (await _collection.FindAsync(query).ConfigureAwait(false)).SingleOrDefaultAsync().ConfigureAwait(false);

            Assert.That(routingSlip, Is.Not.Null);
            Assert.That(routingSlip.Events, Is.Not.Null);
            Assert.That(routingSlip.Events, Has.Length.EqualTo(1));

            var completed = routingSlip.Events[0] as RoutingSlipCompletedDocument;
            Assert.That(completed, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(completed.Variables.ContainsKey("Client"), Is.True);
                Assert.That(completed.Variables["Client"], Is.EqualTo(27));
            });
            //Assert.AreEqual(received.Timestamp.ToMongoDbDateTime(), read.Timestamp);
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _trackingNumber = NewId.NextGuid();

            Console.WriteLine("Tracking Number: {0}", _trackingNumber);

            await Bus.Publish<RoutingSlipCompleted>(new RoutingSlipCompletedEvent(_trackingNumber, DateTime.UtcNow, TimeSpan.FromSeconds(1)));
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            base.ConfigureInMemoryReceiveEndpoint(configurator);

            _collection = Database.GetCollection<RoutingSlipDocument>(EventCollectionName);

            var persister = new RoutingSlipEventPersister(_collection);

            configurator.Consumer(() => new RoutingSlipCompletedConsumer(persister));

            _completed = Handled<RoutingSlipCompleted>(configurator, x => x.Message.TrackingNumber == _trackingNumber);
        }

        IMongoCollection<RoutingSlipDocument> _collection;
        Guid _trackingNumber;
        Task<ConsumeContext<RoutingSlipCompleted>> _completed;

        protected override void SetupActivities(BusTestHarness testHarness)
        {
        }
    }
}
