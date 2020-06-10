namespace MassTransit.Registration
{
    using System;
    using System.Collections;
    using System.Collections.Generic;


    public class RegistrationCache<T> :
        IRegistrationCache<T>
    {
        readonly IDictionary<Type, T> _dictionary;
        readonly Func<Type, T> _missingRegistrationFactory;

        public RegistrationCache(Func<Type, T> missingRegistrationFactory = default)
        {
            _missingRegistrationFactory = missingRegistrationFactory;
            _dictionary = new Dictionary<Type, T>();
        }

        public IEnumerator<KeyValuePair<Type, T>> GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Count => _dictionary.Count;

        public bool ContainsKey(Type key)
        {
            return _dictionary.ContainsKey(key);
        }

        public bool TryGetValue(Type key, out T value)
        {
            return _dictionary.TryGetValue(key, out value);
        }

        public T this[Type key] => _dictionary[key];

        public IEnumerable<Type> Keys => _dictionary.Keys;

        public IEnumerable<T> Values => _dictionary.Values;

        public T GetOrAdd(Type type, Func<Type, T> missingRegistrationFactory = default)
        {
            lock (_dictionary)
            {
                if (_dictionary.TryGetValue(type, out var value))
                    return value;

                Func<Type, T> factory = missingRegistrationFactory ?? _missingRegistrationFactory
                    ?? throw new ArgumentNullException(nameof(missingRegistrationFactory));

                value = factory(type);
                _dictionary.Add(type, value);

                return value;
            }
        }
    }
}
