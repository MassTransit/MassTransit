namespace MassTransit.Tests.CachingTests
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using TestValueObjects;
    using Util.Caching;


    [TestFixture]
    public class Adding_an_item_through_an_index
    {
        [Test]
        public async Task Should_update_the_second_index()
        {
            DateTime currentTime = DateTime.UtcNow;

            var cache = new GreenCache<SimpleValue>(100, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(60), () => currentTime);

            var index =  cache.AddIndex("id", x => x.Id);
            var valueIndex =  cache.AddIndex("value", x => x.Value);

            for (int i = 0; i < 100; i++)
            {
                SimpleValue simpleValue = await index.Get($"key{i}", SimpleValueFactory.Healthy);
            }

            await Task.Delay(100);

            Assert.That(cache.Statistics.Count, Is.EqualTo(100));

            var result = await valueIndex.Get("The key is key27");

            Assert.That(result.Id, Is.EqualTo("key27"));
        }

        [Test]
        public async Task Should_honor_the_clear()
        {
            DateTime currentTime = DateTime.UtcNow;

            var cache = new GreenCache<SimpleValue>(100, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(60), () => currentTime);

            var index =  cache.AddIndex("id", x => x.Id);
            var valueIndex =  cache.AddIndex("value", x => x.Value);

            for (int i = 0; i < 100; i++)
            {
                SimpleValue simpleValue = await index.Get($"key{i}", SimpleValueFactory.Healthy);
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
            DateTime currentTime = DateTime.UtcNow;

            var cache = new GreenCache<SimpleValue>(100, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(60), () => currentTime);

            var index =  cache.AddIndex("id", x => x.Id);
            var valueIndex =  cache.AddIndex("value", x => x.Value);

            for (int i = 0; i < 100; i++)
            {
                SimpleValue simpleValue = await index.Get($"key{i}", SimpleValueFactory.Healthy);
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

            for (int i = 0; i < 100; i++)
            {
                SimpleValue simpleValue = await index.Get($"key{i}", SimpleValueFactory.Healthy);
            }

            await Task.Delay(100);

            Assert.That(cache.Statistics.Count, Is.EqualTo(100));


        }

        [Test]
        public async Task Should_update_the_second_index_once_removed()
        {
            DateTime currentTime = DateTime.UtcNow;

            var cache = new GreenCache<SimpleValue>(100, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(60), () => currentTime);

            var index =  cache.AddIndex("id", x => x.Id);
            var valueIndex =  cache.AddIndex("value", x => x.Value);

            for (int i = 0; i < 100; i++)
            {
                SimpleValue simpleValue = await index.Get($"key{i}", SimpleValueFactory.Healthy);
            }

            await Task.Delay(100);

            Assert.That(cache.Statistics.Count, Is.EqualTo(100));

            var result = await valueIndex.Get("The key is key27");

            Assert.That(result.Id, Is.EqualTo("key27"));

            valueIndex.Remove("The key is key29");

            await Task.Delay(10);

            Assert.That(async () => await index.Get("key29"), Throws.TypeOf<KeyNotFoundException>());

            Assert.That(cache.Statistics.Count, Is.EqualTo(99));
            Assert.That(cache.Statistics.Hits, Is.EqualTo(1));
            Assert.That(cache.Statistics.Misses, Is.EqualTo(101));
        }
    }
}