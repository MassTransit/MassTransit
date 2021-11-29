namespace MassTransit.Tests.Middleware.Caching
{
    using System;
    using System.Threading.Tasks;
    using MassTransit.Caching;
    using NUnit.Framework;


    [TestFixture]
    public class Tests
    {
        [Test]
        public async Task Test1()
        {
            var settings = new CacheSettings(100, TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(30));
            var cache = new GreenCache<Endpoint>(settings);

            IIndex<Uri, Endpoint> addressIndex = cache.AddIndex("address", x => x.Address);

            var address = new Uri("rabbitmq://localhost/vhost/input-queue");

            cache.Add(new Endpoint {Address = address});


            var endpoint = await addressIndex.Get(address, key => Task.FromResult(new Endpoint {Address = key}));
        }
    }


    namespace TestValueObjects
    {
        using System;
        using MassTransit.Caching;


        public class SimpleValue
        {
            public string Id { get; set; }
            public string Value { get; set; }
        }


        public class SmartValue :
            INotifyValueUsed,
            IAsyncDisposable
        {
            readonly string _id;
            readonly string _value;

            public SmartValue(string id, string value)
            {
                _id = id;
                _value = value;
            }

            public string Id => _id;

            public string Value
            {
                get
                {
                    Used?.Invoke();

                    return _value;
                }
            }

            public ValueTask DisposeAsync()
            {
                return default;
            }

            public event Action Used;
        }


        public static class SmartValueFactory
        {
            public static async Task<SmartValue> Healthy(string id)
            {
                await Task.Delay(10).ConfigureAwait(false);

                return new SmartValue(id, $"The key is {id}");
            }
        }


        public static class SimpleValueFactory
        {
            public static async Task<SimpleValue> Healthy(string id)
            {
                await Task.Delay(10).ConfigureAwait(false);

                return new SimpleValue
                {
                    Id = id,
                    Value = $"The key is {id}"
                };
            }

            public static async Task<SimpleValue> Faulty(string id)
            {
                await Task.Delay(10).ConfigureAwait(false);

                throw new TestException("The SimpleValue factory is quite faulty at the moment.");
            }
        }
    }
}
