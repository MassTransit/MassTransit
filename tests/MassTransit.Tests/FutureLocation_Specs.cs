namespace MassTransit.Tests
{
    using System;
    using NUnit.Framework;


    [TestFixture]
    public class FutureLocation_Specs
    {
        [Test]
        public void Should_round_trip_without_issue()
        {
            var address = new Uri("loopback://localhost/input-queue");
            var id = NewId.NextGuid();

            var futureLocation = new FutureLocation(id, address);

            Uri location = futureLocation;

            var returnedLocation = new FutureLocation(location);

            Assert.Multiple(() =>
            {
                Assert.That(returnedLocation.Id, Is.EqualTo(id));
                Assert.That(returnedLocation.Address, Is.EqualTo(new Uri("queue:input-queue")));
            });
        }
    }
}
