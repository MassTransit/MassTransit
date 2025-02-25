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
    using MongoDbIntegration.Courier.Documents;
    using NUnit.Framework;
    using Testing;


    [TestFixture]
    public class When_a_complete_routing_slip_is_completed :
        MongoDbTestFixture
    {
        [Test]
        public async Task Should_complete_the_activity()
        {
            ConsumeContext<RoutingSlipActivityCompleted> context = await _prepareCompleted;

            Assert.Multiple(() =>
            {
                Assert.That(context.Message.TrackingNumber, Is.EqualTo(_trackingNumber));

                Assert.That(context.CorrelationId.HasValue, Is.True);
                Assert.That(context.CorrelationId.Value, Is.Not.EqualTo(_trackingNumber));
            });
        }

        [Test]
        public async Task Should_complete_the_routing_slip()
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
        public async Task Should_complete_the_second_activity()
        {
            ConsumeContext<RoutingSlipActivityCompleted> context = await _sendCompleted;

            Assert.Multiple(() =>
            {
                Assert.That(context.Message.TrackingNumber, Is.EqualTo(_trackingNumber));

                Assert.That(context.CorrelationId.HasValue, Is.True);
                Assert.That(context.CorrelationId.Value, Is.Not.EqualTo(_trackingNumber));
            });
        }

        [Test]
        public async Task Should_upsert_the_event_into_the_routing_slip()
        {
            await _completed;
            await Task.Delay(2000);
            await _prepareCompleted;
            await Task.Delay(2000);
            await _sendCompleted;
            await Task.Delay(2000);

            FilterDefinition<RoutingSlipDocument> query = Builders<RoutingSlipDocument>.Filter.Eq(x => x.TrackingNumber, _trackingNumber);

            var routingSlip = await (await _collection.FindAsync(query).ConfigureAwait(false)).SingleOrDefaultAsync().ConfigureAwait(false);

            Assert.That(routingSlip, Is.Not.Null);
            Assert.That(routingSlip.Events, Is.Not.Null);
            Assert.That(routingSlip.Events, Has.Length.EqualTo(3));
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _trackingNumber = NewId.NextGuid();

            Console.WriteLine("Tracking Number: {0}", _trackingNumber);

            await Bus.Publish<RoutingSlipCompleted>(new RoutingSlipCompletedEvent(_trackingNumber, DateTime.UtcNow, TimeSpan.FromSeconds(1)));

            await Bus.Publish<RoutingSlipActivityCompleted>(new RoutingSlipActivityCompletedEvent(_trackingNumber,
                "Prepare",
                NewId.NextGuid(), DateTime.UtcNow));

            await Bus.Publish<RoutingSlipActivityCompleted>(new RoutingSlipActivityCompletedEvent(_trackingNumber, "Send",
                NewId.NextGuid(), DateTime.UtcNow));
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            base.ConfigureInMemoryReceiveEndpoint(configurator);

            _collection = Database.GetCollection<RoutingSlipDocument>(EventCollectionName);

            var persister = new RoutingSlipEventPersister(_collection);

            configurator.UseMessageRetry(x =>
            {
                x.Handle<MongoWriteException>();
                x.Interval(10, TimeSpan.FromMilliseconds(20));
            });

            var partitioner = configurator.CreatePartitioner(16);

            configurator.RoutingSlipEventConsumers(persister, partitioner);
            configurator.RoutingSlipActivityEventConsumers(persister, partitioner);

            _completed = Handled<RoutingSlipCompleted>(configurator);
            _prepareCompleted = Handled<RoutingSlipActivityCompleted>(configurator, x => x.Message.ActivityName == "Prepare");
            _sendCompleted = Handled<RoutingSlipActivityCompleted>(configurator, x => x.Message.ActivityName == "Send");
        }

        IMongoCollection<RoutingSlipDocument> _collection;
        Guid _trackingNumber;
        Task<ConsumeContext<RoutingSlipCompleted>> _completed;
        Task<ConsumeContext<RoutingSlipActivityCompleted>> _prepareCompleted;
        Task<ConsumeContext<RoutingSlipActivityCompleted>> _sendCompleted;

        protected override void SetupActivities(BusTestHarness testHarness)
        {
        }
    }
}
