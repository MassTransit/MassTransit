namespace MassTransit.MongoDbIntegration.Tests.Courier
{
    using System;
    using System.Threading.Tasks;
    using MassTransit.Courier.Contracts;
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

            Assert.AreEqual(_trackingNumber, context.Message.TrackingNumber);

            Assert.IsTrue(context.CorrelationId.HasValue);
            Assert.AreEqual(_trackingNumber, context.CorrelationId.Value);
        }

        [Test]
        public async Task Should_upsert_the_event_into_the_routing_slip()
        {
            ConsumeContext<RoutingSlipCompleted> context = await _completed;

            await Task.Delay(1000);

            FilterDefinition<RoutingSlipDocument> query = Builders<RoutingSlipDocument>.Filter.Eq(x => x.TrackingNumber, _trackingNumber);

            var routingSlip = await (await _collection.FindAsync(query).ConfigureAwait(false)).SingleOrDefaultAsync().ConfigureAwait(false);

            Assert.IsNotNull(routingSlip);
            Assert.IsNotNull(routingSlip.Events);
            Assert.AreEqual(1, routingSlip.Events.Length);

            var completed = routingSlip.Events[0] as RoutingSlipCompletedDocument;
            Assert.IsNotNull(completed);
            Assert.IsTrue(completed.Variables.ContainsKey("Client"));
            Assert.AreEqual(27, completed.Variables["Client"]);
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
