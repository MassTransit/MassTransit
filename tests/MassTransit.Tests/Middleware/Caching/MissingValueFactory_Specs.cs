namespace MassTransit.Tests.Middleware.Caching
{
    using System.Threading.Tasks;
    using MassTransit.Caching.Internals;
    using NUnit.Framework;
    using TestValueObjects;


    [TestFixture]
    public class Using_the_missing_value_factory
    {
        [Test]
        public async Task Should_complete_with_the_second_factory_once_it_works()
        {
            var helloKey = "Hello";

            var faultyValue = new PendingValue<string, SimpleValue>(helloKey, SimpleValueFactory.Faulty);

            var nodeValueFactory = new NodeValueFactory<SimpleValue>(faultyValue, 100);

            var healthyValue = new PendingValue<string, SimpleValue>(helloKey, SimpleValueFactory.Healthy);

            nodeValueFactory.Add(healthyValue);

            var value = await nodeValueFactory.CreateValue().ConfigureAwait(false);

            Assert.That(value, Is.Not.Null);
            Assert.That(value.Id, Is.EqualTo(helloKey));
            Assert.That(value.Value, Is.EqualTo("The key is Hello"));

            Assert.That(async () => await faultyValue.Value, Throws.TypeOf<TestException>());

            var healthy = await healthyValue.Value;
        }

        [Test]
        public async Task Should_fail_when_the_factory_fails()
        {
            var helloKey = "Hello";

            var pendingValue = new PendingValue<string, SimpleValue>(helloKey, SimpleValueFactory.Faulty);

            var nodeValueFactory = new NodeValueFactory<SimpleValue>(pendingValue, 100);

            Assert.That(async () => await nodeValueFactory.CreateValue(), Throws.TypeOf<TestException>());
        }

        [Test]
        public void Should_fault_if_there_is_a_problem()
        {
            Assert.That(async () => await SimpleValueFactory.Faulty("Hello"), Throws.TypeOf<TestException>());
        }

        [Test]
        public async Task Should_return_the_created_value()
        {
            var value = await SimpleValueFactory.Healthy("Hello");

            Assert.That(value, Is.Not.Null);
            Assert.That(value.Id, Is.EqualTo("Hello"));
            Assert.That(value.Value, Is.EqualTo("The key is Hello"));
        }

        [Test]
        public async Task Should_return_the_value_through_the_pending_value()
        {
            var helloKey = "Hello";

            var pendingValue = new PendingValue<string, SimpleValue>(helloKey, SimpleValueFactory.Healthy);

            var nodeValueFactory = new NodeValueFactory<SimpleValue>(pendingValue, 100);

            var value = await nodeValueFactory.CreateValue().ConfigureAwait(false);

            Assert.That(value, Is.Not.Null);
            Assert.That(value.Id, Is.EqualTo(helloKey));
            Assert.That(value.Value, Is.EqualTo("The key is Hello"));
        }
    }
}
