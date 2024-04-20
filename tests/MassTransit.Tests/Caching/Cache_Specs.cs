namespace MassTransit.Tests.Caching
{
    using System;
    using System.Collections.Concurrent;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;
    using Internals.Caching;
    using MathNet.Numerics.Distributions;
    using NUnit.Framework;


    public class Caching_a_value
    {
        ICache<Guid, Item> _newCache;

        [OneTimeSetUp]
        public void Setup()
        {
            _newCache = new MassTransitCache<Guid, Item, CacheValue<Item>>(new UsageCachePolicy<Item>());
        }

        [Test]
        public async Task Should_get_and_add()
        {
            await _newCache.GetOrAdd(Guid.NewGuid(), async key => new Item(key));
        }

        [Test]
        public async Task Should_get_and_add_and_get()
        {
            var key = Guid.NewGuid();
            await _newCache.GetOrAdd(key, async key => new Item(key));

            await _newCache.Get(key);
        }

        [Test]
        public async Task Should_get_and_add_and_get_same_time()
        {
            var key = Guid.NewGuid();

            var timer = Stopwatch.StartNew();

            Task<Item> first = _newCache.GetOrAdd(key, async key =>
            {
                await Task.Delay(1000);

                return new Item(key) { Value = "First" };
            });

            Task<Item> second = _newCache.GetOrAdd(key, async key =>
            {
                return new Item(key) { Value = "Second" };
            });

            var item = await second;

            timer.Stop();

            await first;

            await _newCache.Get(key);

            Assert.Multiple(() =>
            {
                Assert.That(timer.Elapsed + TimeSpan.FromSeconds(0.1), Is.GreaterThanOrEqualTo(TimeSpan.FromSeconds(1)));

                Assert.That(item.Value, Is.EqualTo("First"));
            });
        }
    }


    public class Caching_a_bunch_of_values
    {
        ICache<Guid, Item> _newCache;

        [OneTimeSetUp]
        public void Setup()
        {
            _newCache = new MassTransitCache<Guid, Item, CacheValue<Item>>(new UsageCachePolicy<Item>(), new CacheOptions { Capacity = 1000 });
        }

        [Test]
        public async Task Should_stay_under_half_capacity()
        {
            for (var i = 0; i < 1010; i++)
                await _newCache.GetOrAdd(Guid.NewGuid(), async key => new Item(key));

            Assert.That(_newCache.Count, Is.EqualTo(500));

            TestContext.Out.WriteLine("Hit Ratio: {0:P}", _newCache.HitRatio);
        }
    }


    public class Caching_values_and_using_them_too
    {
        ICache<Guid, Item> _newCache;

        [OneTimeSetUp]
        public void Setup()
        {
            _newCache = new MassTransitCache<Guid, Item, CacheValue<Item>>(new UsageCachePolicy<Item>(), new CacheOptions { Capacity = 1000 });
        }

        [Test]
        public async Task Should_stay_under_capacity()
        {
            Guid[] ids = Enumerable.Range(0, 1000).Select(n => Guid.NewGuid()).ToArray();

            for (var i = 0; i < 500; i++)
            {
                await _newCache.GetOrAdd(ids[i], key => Task.FromResult(new Item(key)));
                await _newCache.GetOrAdd(ids[i], key => Task.FromResult(new Item(key)));
            }

            for (var i = 500; i < 1000; i++)
                await _newCache.GetOrAdd(ids[i], key => Task.FromResult(new Item(key)));

            Assert.That(_newCache.Count, Is.EqualTo(1000));

            TestContext.Out.WriteLine("Hit Ratio: {0:P}", _newCache.HitRatio);
        }
    }


    public class Should_handle_a_random_distribution
    {
        const double S = 0.86;
        const int N = 500;
        const int SampleCount = 10000;
        ICache<Guid, Item> _newCache;
        int[] _samples;

        [OneTimeSetUp]
        public void Setup()
        {
            _newCache = new MassTransitCache<Guid, Item, CacheValue<Item>>(new UsageCachePolicy<Item>(), new CacheOptions { Capacity = 1000 });

            _samples = new int[SampleCount];

            Zipf.Samples(_samples, S, N);
        }

        [Test]
        public async Task Should_stay_under_capacity()
        {
            var map = new ConcurrentDictionary<int, Guid>();

            Guid[] ids = _samples.Select(x => map.GetOrAdd(x, _ => Guid.NewGuid())).ToArray();

            var distinctCount = ids.Distinct().Count();
            var expectedHitRatio = (SampleCount - distinctCount) / (double)SampleCount;

            Task<Item>[] tasks = ids.Select(id => _newCache.GetOrAdd(id, key => Task.FromResult(new Item(key)))).ToArray();

            await Task.WhenAll(tasks);

            TestContext.Out.WriteLine("Hit Ratio: {0:P}", _newCache.HitRatio);

            Assert.That(_newCache.Count, Is.EqualTo(distinctCount));
            Assert.That(_newCache.HitRatio, Is.EqualTo(expectedHitRatio));
        }
    }


    public class ValueTracker_Specs
    {
        [Test]
        public async Task Should_not_die()
        {
            var tracker = new ValueTracker<Item, CacheValue<Item>>(new UsageCachePolicy<Item>(), 1000);

            void Remove()
            {
            }

            for (var i = 0; i < 10000; i++)
                await tracker.Add(new CacheValue<Item>(Remove));
        }
    }


    public class Should_handle_a_random_distribution_with_time_to_live
    {
        const double S = 0.86;
        const int N = 500;
        const int SampleCount = 10000;
        ICache<Guid, Item> _massTransitCache;
        int[] _samples;

        [OneTimeSetUp]
        public void Setup()
        {
            _massTransitCache = new MassTransitCache<Guid, Item, ITimeToLiveCacheValue<Item>>(new TimeToLiveCachePolicy<Item>(TimeSpan.FromSeconds(30)),
                new CacheOptions { Capacity = 1000 });

            _samples = new int[SampleCount];

            Zipf.Samples(_samples, S, N);
        }

        [Test]
        public async Task Should_stay_under_capacity()
        {
            var map = new ConcurrentDictionary<int, Guid>();

            Guid[] ids = _samples.Select(x => map.GetOrAdd(x, _ => Guid.NewGuid())).ToArray();

            var distinctCount = ids.Distinct().Count();
            var expectedHitRatio = (SampleCount - distinctCount) / (double)SampleCount;

            for (var i = 0; i < SampleCount; i++)
            {
                var item = await _massTransitCache.GetOrAdd(ids[i], key => Task.FromResult(new Item(key)));
            }

            // var tasks = ids.Select(id => _massTransitCache.GetOrAdd(id, key => Task.FromResult(new Item(key)))).ToArray();
            //
            // await Task.WhenAll(tasks);

            TestContext.Out.WriteLine("Hit Ratio: {0:P}", _massTransitCache.HitRatio);

            Assert.That(_massTransitCache.Count, Is.EqualTo(distinctCount));
            Assert.That(_massTransitCache.HitRatio, Is.EqualTo(expectedHitRatio));
        }
    }


    class Item
    {
        public Item(Guid key)
        {
            Key = key;
            Value = key.ToString("N");
        }

        public string Value { get; set; }

        public Guid Key { get; }
    }
}
