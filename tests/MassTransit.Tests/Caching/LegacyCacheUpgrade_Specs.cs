namespace MassTransit.Tests.Caching
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Internals.Caching;
    using Middleware.Caching;
    using Middleware.Caching.TestValueObjects;
    using NUnit.Framework;


    [TestFixture]
    public class Adding_items_to_the_cache
    {
        [Test]
        public async Task Should_not_find_a_faulted_value()
        {
            var cache = new MassTransitCache<string, SimpleValue, CacheValue<SimpleValue>>(new UsageCachePolicy<SimpleValue>());

            var helloKey = "Hello";

            Task<SimpleValue> valueTask = cache.GetOrAdd(helloKey, SimpleValueFactory.Faulty);

            Assert.That(async () => await valueTask, Throws.TypeOf<TestException>());

            Assert.That(async () => await cache.Get(helloKey), Throws.TypeOf<KeyNotFoundException>());
        }

        [Test]
        public async Task Should_support_a_simple_addition()
        {
            var cache = new MassTransitCache<string, SimpleValue, CacheValue<SimpleValue>>(new UsageCachePolicy<SimpleValue>());

            var helloKey = "Hello";

            var value = await cache.GetOrAdd(helloKey, SimpleValueFactory.Healthy);

            Assert.That(value, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(value.Id, Is.EqualTo(helloKey));
                Assert.That(value.Value, Is.EqualTo("The key is Hello"));
            });
        }

        [Test]
        public async Task Should_support_a_simple_addition_and_access()
        {
            var cache = new MassTransitCache<string, SimpleValue, CacheValue<SimpleValue>>(new UsageCachePolicy<SimpleValue>());

            var helloKey = "Hello";

            var value = await cache.GetOrAdd(helloKey, SimpleValueFactory.Healthy);

            Task<SimpleValue> readValueTask = cache.Get(helloKey);

            Assert.That(value, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(value.Id, Is.EqualTo(helloKey));
                Assert.That(value.Value, Is.EqualTo("The key is Hello"));
            });

            var readValue = await readValueTask;

            Assert.That(readValue, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(readValue.Id, Is.EqualTo(helloKey));
                Assert.That(readValue.Value, Is.EqualTo("The key is Hello"));
            });
        }

        [Test]
        public async Task Should_support_access_to_eventual_success()
        {
            var cache = new MassTransitCache<string, SimpleValue, CacheValue<SimpleValue>>(new UsageCachePolicy<SimpleValue>());

            var helloKey = "Hello";

            Task<SimpleValue> valueTask = cache.GetOrAdd(helloKey, SimpleValueFactory.Faulty);


            Task<SimpleValue> goodValueTask = cache.GetOrAdd(helloKey, SimpleValueFactory.Healthy);

            Task<SimpleValue> readValueTask = cache.Get(helloKey);

            Assert.That(async () => await valueTask, Throws.TypeOf<TestException>());

            var value = await goodValueTask;

            Assert.That(value, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(value.Id, Is.EqualTo(helloKey));
                Assert.That(value.Value, Is.EqualTo("The key is Hello"));
            });

            var readValue = await readValueTask;

            Assert.That(readValue, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(readValue.Id, Is.EqualTo(helloKey));
                Assert.That(readValue.Value, Is.EqualTo("The key is Hello"));
            });
        }

        [Test]
        public async Task Should_support_access_to_eventual_success_after_waiting()
        {
            var cache = new MassTransitCache<string, SimpleValue, CacheValue<SimpleValue>>(new UsageCachePolicy<SimpleValue>());

            var helloKey = "Hello";

            Task<SimpleValue> valueTask = cache.GetOrAdd(helloKey, SimpleValueFactory.Faulty);
            Assert.That(async () => await valueTask, Throws.TypeOf<TestException>());

            Task<SimpleValue> valueTask2 = cache.GetOrAdd(helloKey, SimpleValueFactory.Faulty);
            Assert.That(async () => await valueTask2, Throws.TypeOf<TestException>());

            Task<SimpleValue> goodValueTask = cache.GetOrAdd(helloKey, SimpleValueFactory.Healthy);

            var value = await goodValueTask;

            Assert.That(value, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(value.Id, Is.EqualTo(helloKey));
                Assert.That(value.Value, Is.EqualTo("The key is Hello"));
            });

            Task<SimpleValue> readValueTask = cache.Get(helloKey);

            var readValue = await readValueTask;

            Assert.That(readValue, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(readValue.Id, Is.EqualTo(helloKey));
                Assert.That(readValue.Value, Is.EqualTo("The key is Hello"));
            });
        }
    }
}
