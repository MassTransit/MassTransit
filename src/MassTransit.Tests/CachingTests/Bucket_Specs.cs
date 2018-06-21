namespace MassTransit.Tests.CachingTests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using TestValueObjects;
    using Util.Caching;


    [TestFixture]
    public class Adding_nodes_to_a_bucket
    {
        [Test]
        public async Task Should_add_the_first_node()
        {
            var manager = new NodeTracker<SimpleValue>(1000, TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(60), () => DateTime.UtcNow);

            Bucket<SimpleValue> bucket = new Bucket<SimpleValue>(manager);

            var valueNode = new BucketNode<SimpleValue>(await SimpleValueFactory.Healthy("Hello"));
            bucket.Push(valueNode);
        }

        [Test]
        public async Task Should_fill_up_the_buckets()
        {
            DateTime currentTime = DateTime.UtcNow;

            var cache = new GreenCache<SimpleValue>(100, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(60), () => currentTime);

            var index = cache.AddIndex("id", x => x.Id);

            for (int i = 0; i < 100; i++)
            {
                SimpleValue simpleValue = await index.Get($"key{i}", SimpleValueFactory.Healthy);
            }

            await Task.Delay(100);

            Assert.That(cache.Statistics.Count, Is.EqualTo(100));
        }

        [Test]
        public async Task Should_fill_up_a_bunch_of_buckets()
        {
            DateTime currentTime = DateTime.UtcNow;

            var cache = new GreenCache<SimpleValue>(100, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(60), () => currentTime);

            var index = cache.AddIndex("id", x => x.Id);

            for (int i = 0; i < 100; i++)
            {
                SimpleValue simpleValue = await index.Get($"key{i}", SimpleValueFactory.Healthy);

                currentTime = currentTime.Add(TimeSpan.FromSeconds(1));
            }

            await Task.Delay(2000);

            Assert.That(cache.Statistics.Count, Is.EqualTo(60));

            Assert.That(cache.Statistics.ValidityCheckInterval, Is.EqualTo(TimeSpan.FromMilliseconds(250)));
            Assert.That(cache.Statistics.OldestBucketIndex, Is.EqualTo(40));
            Assert.That(cache.Statistics.CurrentBucketIndex, Is.EqualTo(100));


            var values = cache.GetAll().ToArray();

            Assert.That(values.Length, Is.EqualTo(60));
        }

        [Test]
        public async Task Should_fill_them_even_fuller()
        {
            DateTime currentTime = DateTime.UtcNow;

            var cache = new GreenCache<SimpleValue>(100, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(60), () => currentTime);

            var index = cache.AddIndex("id", x => x.Id);

            for (int i = 0; i < 200; i++)
            {
                SimpleValue simpleValue = await index.Get($"key{i}", SimpleValueFactory.Healthy);

                if(i%2 == 0)
                    currentTime = currentTime.Add(TimeSpan.FromSeconds(1));
            }

            await Task.Delay(300);

            Assert.That(cache.Statistics.Count, Is.EqualTo(101));

            Assert.That(cache.Statistics.ValidityCheckInterval, Is.EqualTo(TimeSpan.FromMilliseconds(250)));
            Assert.That(cache.Statistics.OldestBucketIndex, Is.EqualTo(50));
            Assert.That(cache.Statistics.CurrentBucketIndex, Is.EqualTo(100));


            var values = cache.GetAll().ToArray();

            Assert.That(values.Length, Is.EqualTo(101));
        }
    }
}