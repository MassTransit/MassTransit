namespace MassTransit.DependencyInjection.Registration
{
    using System;
    using System.Collections.Generic;
    using Configuration;


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
