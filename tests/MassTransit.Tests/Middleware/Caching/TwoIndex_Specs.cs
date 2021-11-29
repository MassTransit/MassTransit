namespace MassTransit.Tests.Middleware.Caching
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using MassTransit.Caching;
    using NUnit.Framework;
    using TestValueObjects;


    [TestFixture]
    public class Adding_an_item_through_an_index
    {
        [Test]
        public async Task Should_honor_the_clear()
        {
            var settings = new TestCacheSettings(100, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(60));

            var cache = new GreenCache<SimpleValue>(settings);

            IIndex<string, SimpleValue> index = cache.AddIndex("id", x => x.Id);
            IIndex<string, SimpleValue> valueIndex = cache.AddIndex("value", x => x.Value);

            for (var i = 0; i < 100; i++)
            {
                var simpleValue = await index.Get($"key{i}", SimpleValueFactory.Healthy);
            }

            await Task.Delay(100);

            Assert.That(cache.Statistics.Count, Is.EqualTo(100));

            var result = await valueIndex.Get("The key is key27");

            Assert.That(result.Id, Is.EqualTo("key27"));

            cache.Clear();

            Assert.That(async () => await index.Get("key27"), Throws.TypeOf<KeyNotFoundException>());
            Assert.That(async () => await valueIndex.Get("The key is key27"), Throws.TypeOf<KeyNotFoundException>());
        }

        [Test]
        public async Task Should_honor_the_clear_and_still_allow_adding_nodes()
        {
            var settings = new TestCacheSettings(100, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(60));

            var cache = new GreenCache<SimpleValue>(settings);

            IIndex<string, SimpleValue> index = cache.AddIndex("id", x => x.Id);
            IIndex<string, SimpleValue> valueIndex = cache.AddIndex("value", x => x.Value);

            for (var i = 0; i < 100; i++)
            {
                var simpleValue = await index.Get($"key{i}", SimpleValueFactory.Healthy);
            }

            await Task.Delay(100);

            Assert.That(cache.Statistics.Count, Is.EqualTo(100));
            Assert.That(cache.Statistics.Misses, Is.EqualTo(100));

            var result = await valueIndex.Get("The key is key27");

            Assert.That(result.Id, Is.EqualTo("key27"));

            Assert.That(cache.Statistics.Hits, Is.EqualTo(1));

            cache.Clear();

            Assert.That(async () => await index.Get("key27"), Throws.TypeOf<KeyNotFoundException>());
            Assert.That(async () => await valueIndex.Get("The key is key27"), Throws.TypeOf<KeyNotFoundException>());

            for (var i = 0; i < 100; i++)
            {
                var simpleValue = await index.Get($"key{i}", SimpleValueFactory.Healthy);
            }

            await Task.Delay(100);

            Assert.That(cache.Statistics.Count, Is.EqualTo(100));
        }

        [Test]
        public async Task Should_update_the_second_index()
        {
            var settings = new TestCacheSettings(100, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(60));

            var cache = new GreenCache<SimpleValue>(settings);

            IIndex<string, SimpleValue> index = cache.AddIndex("id", x => x.Id);
            IIndex<string, SimpleValue> valueIndex = cache.AddIndex("value", x => x.Value);

            for (var i = 0; i < 100; i++)
            {
                var simpleValue = await index.Get($"key{i}", SimpleValueFactory.Healthy);
            }

            await Task.Delay(100);

            Assert.That(cache.Statistics.Count, Is.EqualTo(100));

            var result = await valueIndex.Get("The key is key27");

            Assert.That(result.Id, Is.EqualTo("key27"));
        }

        [Test]
        public async Task Should_update_the_second_index_once_removed()
        {
            var settings = new TestCacheSettings(100, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(60));

            var cache = new GreenCache<SimpleValue>(settings);

            IIndex<string, SimpleValue> index = cache.AddIndex("id", x => x.Id);
            IIndex<string, SimpleValue> valueIndex = cache.AddIndex("value", x => x.Value);

            for (var i = 0; i < 100; i++)
            {
                var simpleValue = await index.Get($"key{i}", SimpleValueFactory.Healthy);
            }

            await Task.Delay(100);

            Assert.That(cache.Statistics.Count, Is.EqualTo(100));

            var result = await valueIndex.Get("The key is key27");

            Assert.That(result.Id, Is.EqualTo("key27"));

            valueIndex.Remove("The key is key29");

            await Task.Delay(300);

            Assert.That(async () => await index.Get("key29"), Throws.TypeOf<KeyNotFoundException>());

            Assert.That(cache.Statistics.Count, Is.EqualTo(99));
            Assert.That(cache.Statistics.Hits, Is.EqualTo(1));
            Assert.That(cache.Statistics.Misses, Is.EqualTo(101));
        }
    }
}
