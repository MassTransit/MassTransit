namespace MassTransit.Tests.Middleware.Caching
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using MassTransit.Caching;
    using MassTransit.Caching.Internals;
    using NUnit.Framework;
    using TestValueObjects;
    using Util;


    [TestFixture]
    public class Adding_nodes_to_a_bucket
    {
        [Test]
        public async Task Should_add_the_first_node()
        {
            var settings = new CacheSettings(1000, TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(60));
            var manager = new NodeTracker<SimpleValue>(settings);

            var bucket = new Bucket<SimpleValue>(manager);

            var valueNode = new BucketNode<SimpleValue>(await SimpleValueFactory.Healthy("Hello"));
            bucket.Push(valueNode);
        }

        [Test]
        public async Task Should_fill_them_even_fuller()
        {
            var settings = new TestCacheSettings(100, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(60));

            var cache = new GreenCache<SimpleValue>(settings);

            IIndex<string, SimpleValue> index = cache.AddIndex("id", x => x.Id);


            var addedObserver = new NodeAddedCountObserver(200);
            cache.Connect(addedObserver);

            var removedObserver = new NodeRemovedCountObserver<SimpleValue>(99);
            cache.Connect(removedObserver);

            for (var i = 0; i < 200; i++)
            {
                var simpleValue = await index.Get($"key{i}", SimpleValueFactory.Healthy);

                if (i % 2 == 0)
                    settings.CurrentTime += TimeSpan.FromSeconds(1);
            }

            await addedObserver.Completed;
            await removedObserver.Completed;

            Assert.That(cache.Statistics.Count, Is.EqualTo(101));

            Task<SimpleValue>[] values = cache.GetAll().ToArray();

            Assert.That(values.Length, Is.EqualTo(101));
        }

        [Test]
        public async Task Should_fill_up_a_bunch_of_buckets()
        {
            var addedObserver = new NodeAddedCountObserver(100);
            var removedObserver = new NodeRemovedCountObserver<SimpleValue>(40);

            var settings = new TestCacheSettings(100, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(60));

            var cache = new GreenCache<SimpleValue>(settings);

            IIndex<string, SimpleValue> index = cache.AddIndex("id", x => x.Id);

            cache.Connect(addedObserver);
            cache.Connect(removedObserver);

            for (var i = 0; i < 100; i++)
            {
                var simpleValue = await index.Get($"key{i}", SimpleValueFactory.Healthy);

                settings.CurrentTime += TimeSpan.FromSeconds(1);
            }

            await addedObserver.Completed;
            await removedObserver.Completed;

            Assert.That(cache.Statistics.Count, Is.EqualTo(60));

            Task<SimpleValue>[] values = cache.GetAll().ToArray();

            Assert.That(values.Length, Is.EqualTo(60));
        }

        [Test]
        public async Task Should_fill_up_the_buckets()
        {
            var settings = new TestCacheSettings(100, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(60));

            var cache = new GreenCache<SimpleValue>(settings);

            IIndex<string, SimpleValue> index = cache.AddIndex("id", x => x.Id);

            var observer = new NodeAddedCountObserver(100);
            cache.Connect(observer);

            for (var i = 0; i < 100; i++)
            {
                var simpleValue = await index.Get($"key{i}", SimpleValueFactory.Healthy);
            }

            await observer.Completed;

            Assert.That(cache.Statistics.Count, Is.EqualTo(100));
        }

        [Test]
        public async Task Should_fill_up_the_buckets_over_time()
        {
            var settings = new TestCacheSettings(100, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(300));

            var cache = new GreenCache<SimpleValue>(settings);

            IIndex<string, SimpleValue> index = cache.AddIndex("id", x => x.Id);

            var observer = new NodeAddedCountObserver(100);
            cache.Connect(observer);

            for (var i = 0; i < 100; i++)
            {
                var simpleValue = await index.Get($"key{i}", SimpleValueFactory.Healthy);

                settings.CurrentTime += TimeSpan.FromSeconds(1);
            }

            await observer.Completed;

            Assert.That(cache.Statistics.Count, Is.EqualTo(100));
        }

        [Test]
        public async Task Should_fill_up_the_buckets_over_time_and_remove_old_entries()
        {
            var settings = new TestCacheSettings(100, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(300));

            var cache = new GreenCache<SimpleValue>(settings);

            IIndex<string, SimpleValue> index = cache.AddIndex("id", x => x.Id);

            var observer = new NodeAddedCountObserver(200);
            cache.Connect(observer);

            var removed = new NodeRemovedCountObserver<SimpleValue>(100);
            cache.Connect(removed);

            for (var i = 0; i < 200; i++)
            {
                var simpleValue = await index.Get($"key{i}", SimpleValueFactory.Healthy);

                settings.CurrentTime += TimeSpan.FromSeconds(1);
            }

            await observer.Completed;
            await removed.Completed;

            Assert.That(cache.Statistics.Count, Is.EqualTo(100));
        }

        [Test]
        public async Task Should_fill_up_the_buckets_with_smart_values_over_time_and_remove_old_entries()
        {
            var settings = new TestCacheSettings(100, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(300));

            var cache = new GreenCache<SmartValue>(settings);

            IIndex<string, SmartValue> index = cache.AddIndex("id", x => x.Id);

            var observer = new SmartAddedCountObserver(200);
            cache.Connect(observer);

            var removed = new NodeRemovedCountObserver<SmartValue>(100);
            cache.Connect(removed);


            for (var i = 0; i < 200; i++)
            {
                var simpleValue = await index.Get($"key{i}", SmartValueFactory.Healthy);

                settings.CurrentTime += TimeSpan.FromSeconds(1);
            }

            await observer.Completed;
            await removed.Completed;

            Assert.That(cache.Statistics.Count, Is.EqualTo(100));
        }


        class NodeAddedCountObserver :
            ICacheValueObserver<SimpleValue>
        {
            readonly int _expected;
            readonly TaskCompletionSource<bool> _source;
            int _count;

            public NodeAddedCountObserver(int expected)
            {
                _expected = expected;
                _source = TaskUtil.GetTask();
            }

            public Task<bool> Completed => _source.Task;

            public void ValueAdded(INode<SimpleValue> node, SimpleValue value)
            {
                if (Interlocked.Increment(ref _count) == _expected)
                    _source.SetCompleted();
            }

            public void ValueRemoved(INode<SimpleValue> node, SimpleValue value)
            {
            }

            public void CacheCleared()
            {
            }
        }


        class SmartAddedCountObserver :
            ICacheValueObserver<SmartValue>
        {
            readonly int _expected;
            readonly TaskCompletionSource<bool> _source;
            int _count;

            public SmartAddedCountObserver(int expected)
            {
                _expected = expected;
                _source = TaskUtil.GetTask();
            }

            public Task<bool> Completed => _source.Task;

            public void ValueAdded(INode<SmartValue> node, SmartValue value)
            {
                if (Interlocked.Increment(ref _count) == _expected)
                    _source.SetCompleted();
            }

            public void ValueRemoved(INode<SmartValue> node, SmartValue value)
            {
            }

            public void CacheCleared()
            {
            }
        }


        class NodeRemovedCountObserver<T> :
            ICacheValueObserver<T>
            where T : class
        {
            readonly int _expected;
            readonly TaskCompletionSource<bool> _source;
            int _count;

            public NodeRemovedCountObserver(int expected)
            {
                _expected = expected;
                _source = TaskUtil.GetTask();
            }

            public Task<bool> Completed => _source.Task;

            public void ValueAdded(INode<T> node, T value)
            {
            }

            public void ValueRemoved(INode<T> node, T value)
            {
                if (Interlocked.Increment(ref _count) == _expected)
                    _source.SetCompleted();
            }

            public void CacheCleared()
            {
            }
        }
    }
}
