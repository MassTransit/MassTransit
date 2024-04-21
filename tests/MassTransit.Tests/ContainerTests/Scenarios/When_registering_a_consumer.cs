namespace MassTransit.Tests.ContainerTests.Scenarios
{
    using System.Threading.Tasks;
    using NUnit.Framework;
    using TestFramework;


    [TestFixture]
    public abstract class When_registering_a_consumer :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_receive_using_the_first_consumer()
        {
            const string name = "Joe";

            await InputQueueSendEndpoint.Send(new SimpleMessageClass(name));

            var lastConsumer = await SimpleConsumer.LastConsumer;
            Assert.That(lastConsumer, Is.Not.Null);

            var last = await lastConsumer.Last;
            Assert.That(last.Name, Is.EqualTo(name));

            var wasDisposed = await lastConsumer.Dependency.WasDisposed;
            Assert.Multiple(() =>
            {
                Assert.That(wasDisposed, Is.True, "Dependency was not disposed");

                Assert.That(lastConsumer.Dependency.SomethingDone, Is.True, "Dependency was disposed before consumer executed");
            });
        }
    }


    [TestFixture]
    public abstract class When_registering_a_consumer_by_interface :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_receive_using_the_first_consumer()
        {
            const string name = "Joe";

            await InputQueueSendEndpoint.Send(new SimpleMessageClass(name));

            var lastConsumer = await SimpleConsumer.LastConsumer;
            Assert.That(lastConsumer, Is.Not.Null);

            var last = await lastConsumer.Last;
            Assert.That(last.Name, Is.EqualTo(name));

            var wasDisposed = await lastConsumer.Dependency.WasDisposed;
            Assert.Multiple(() =>
            {
                Assert.That(wasDisposed, Is.True, "Dependency was not disposed");

                Assert.That(lastConsumer.Dependency.SomethingDone, Is.True, "Dependency was disposed before consumer executed");
            });
        }
    }
}
