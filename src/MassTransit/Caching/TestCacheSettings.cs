namespace MassTransit.Caching
{
    using System;


    public class TestCacheSettings :
        CacheSettings
    {
        readonly TestTimeProvider _provider;

        public TestCacheSettings(int capacity = 10000, TimeSpan? minAge = default, TimeSpan? maxAge = default)
            : this(capacity, minAge, maxAge, new TestTimeProvider())
        {
        }

        TestCacheSettings(int capacity, TimeSpan? minAge, TimeSpan? maxAge, TestTimeProvider provider)
            : base(capacity, minAge, maxAge, provider.Now)
        {
            _provider = provider;
        }

        public DateTime CurrentTime
        {
            get => _provider.Current;
            set => _provider.Current = value;
        }


        class TestTimeProvider
        {
            public DateTime Current;

            public TestTimeProvider()
            {
                Current = DateTime.UtcNow;
            }

            public DateTime Now()
            {
                return Current;
            }
        }
    }
}
