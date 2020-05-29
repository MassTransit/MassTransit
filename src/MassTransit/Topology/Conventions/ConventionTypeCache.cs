namespace MassTransit.Topology.Conventions
{
    using System;
    using System.Collections.Concurrent;
    using System.Reflection;
    using Metadata;


    public class ConventionTypeCache<TValue> :
        IConventionTypeCache<TValue>
        where TValue : class
    {
        readonly ConcurrentDictionary<Type, Cached> _dictionary;
        readonly IConventionTypeCacheFactory<TValue> _typeFactory;

        public ConventionTypeCache(Type genericType, IConventionTypeCacheFactory<TValue> typeFactory)
        {
            if (genericType == null)
                throw new ArgumentNullException(nameof(genericType));
            if (typeFactory == null)
                throw new ArgumentNullException(nameof(typeFactory));

            var typeInfo = genericType.GetTypeInfo();

            if (typeInfo.GenericTypeParameters == null)
                throw new ArgumentException("The type specified must be a generic type", nameof(genericType));
            if (typeInfo.GenericTypeParameters.Length != 1)
                throw new ArgumentException("The generic type must have a single generic argument", nameof(genericType));
            if (!typeof(TValue).IsAssignableFrom(genericType))
                throw new ArgumentException("The generic type must be assignable to T", nameof(genericType));

            _dictionary = new ConcurrentDictionary<Type, Cached>();
            _typeFactory = typeFactory;
        }

        TResult IConventionTypeCache<TValue>.GetOrAdd<T, TResult>()
        {
            var result = _dictionary.GetOrAdd(typeof(T), add => new CachedValue(() => _typeFactory.Create<T>())).Value as TResult;
            if (result == null)
                throw new ArgumentException($"The specified result type was invalid: {TypeMetadataCache<TResult>.ShortName}");

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
