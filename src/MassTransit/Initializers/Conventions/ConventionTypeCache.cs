namespace MassTransit.Initializers.Conventions
{
    using System;
    using System.Collections.Concurrent;


    public class ConventionTypeCache<TValue> :
        IConventionTypeCache<TValue>
        where TValue : class
    {
        readonly IInitializerConvention _convention;
        readonly ConcurrentDictionary<Type, Cached> _dictionary;
        readonly IConventionTypeCacheFactory<TValue> _typeFactory;

        public ConventionTypeCache(IConventionTypeCacheFactory<TValue> typeFactory, IInitializerConvention convention)
        {
            _typeFactory = typeFactory ?? throw new ArgumentNullException(nameof(typeFactory));
            _convention = convention;

            _dictionary = new ConcurrentDictionary<Type, Cached>();
        }

        TResult IConventionTypeCache<TValue>.GetOrAdd<T, TResult>()
        {
            var result = _dictionary.GetOrAdd(typeof(T), add => new CachedValue(() => _typeFactory.Create<T>(_convention))).Value as TResult;
            if (result == null)
                throw new ArgumentException($"The specified result type was invalid: {TypeCache<TResult>.ShortName}");

            return result;
        }


        interface Cached
        {
            TValue Value { get; }
        }


        class CachedValue :
            Cached
        {
            readonly Lazy<TValue> _value;

            public CachedValue(Func<TValue> valueFactory)
            {
                _value = new Lazy<TValue>(valueFactory);
            }

            public TValue Value => _value.Value;
        }
    }
}
