namespace MassTransit.Tests.CachingTests
{
    using System;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using Util.Caching;


    [TestFixture]
    public class Tests
    {
        [Test]
        public async Task Test1()
        {
            var cache = new GreenCache<Endpoint>(100, TimeSpan.FromSeconds(60), TimeSpan.FromMinutes(30),
                () => DateTime.UtcNow);

            var addressIndex =  cache.AddIndex("address", x => x.Address);

            var address = new Uri("rabbitmq://localhost/vhost/input-queue");

            cache.Add(new Endpoint {Address = address});


            var endpoint = await addressIndex.Get(address, key => Task.FromResult(new Endpoint {Address = key}));
        }
    }


    namespace TestValueObjects
    {
        public class SimpleValue
        {
            public string Id { get; set; }
            public string Value { get; set; }
        }


        public class SmartValue :
            INotifyValueUsed
        {
            readonly string _id;
            readonly string _value;

            public SmartValue(string id, string value)
            {
                _id = id;
                _value = value;
            }

            public event Action Used;

            public string Id => _id;

            public string Value
            {
                get
                {
                    Used?.Invoke();

                    return _value;
                }
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