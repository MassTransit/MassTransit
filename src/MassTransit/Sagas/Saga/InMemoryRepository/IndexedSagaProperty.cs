namespace MassTransit.Saga
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Internals;


    /// <summary>
    /// A dictionary index of the sagas
    /// </summary>
    /// <typeparam name="TSaga"></typeparam>
    /// <typeparam name="TProperty"></typeparam>
    public class IndexedSagaProperty<TSaga, TProperty> :
        IIndexedSagaProperty<TSaga>
        where TSaga : class, ISaga
    {
        readonly Func<TSaga, TProperty> _getProperty;
        readonly IDictionary<TProperty, HashSet<SagaInstance<TSaga>>> _values;

        /// <summary>
        /// Creates an index for the specified property of a saga
        /// </summary>
        /// <param name="propertyInfo"></param>
        public IndexedSagaProperty(PropertyInfo propertyInfo)
        {
            _values = new Dictionary<TProperty, HashSet<SagaInstance<TSaga>>>();
            _getProperty = GetGetMethod(propertyInfo);
        }

        public int Count => _values.Count;

        public SagaInstance<TSaga> this[object key]
        {
            get
            {
                var keyValue = (TProperty)key;

                if (_values.TryGetValue(keyValue, out HashSet<SagaInstance<TSaga>> result))
                    return result.SingleOrDefault();

                return null;
            }
        }

        public void Add(SagaInstance<TSaga> newItem)
        {
            var key = _getProperty(newItem.Instance);

            if (!_values.TryGetValue(key, out HashSet<SagaInstance<TSaga>> hashSet))
            {
                hashSet = new HashSet<SagaInstance<TSaga>>();
                _values.Add(key, hashSet);
            }

            hashSet.Add(newItem);
        }

        public void Remove(SagaInstance<TSaga> instance)
        {
            var key = _getProperty(instance.Instance);

            if (!_values.TryGetValue(key, out HashSet<SagaInstance<TSaga>> hashSet))
                return;

            if (hashSet.Remove(instance) && hashSet.Count == 0)
                _values.Remove(key);
        }

        public IEnumerable<SagaInstance<TSaga>> Where(Func<TSaga, bool> filter)
        {
            return _values.Values.SelectMany(x => x).Where(x => filter(x.Instance));
        }

        public IEnumerable<SagaInstance<TSaga>> Where(object key, Func<TSaga, bool> filter)
        {
            var keyValue = (TProperty)key;

            if (_values.TryGetValue(keyValue, out HashSet<SagaInstance<TSaga>> resultSet))
                return resultSet.Where(x => filter(x.Instance));

            return Enumerable.Empty<SagaInstance<TSaga>>();
        }

        public IEnumerable<TResult> Select<TResult>(Func<TSaga, TResult> transformer)
        {
            return _values.Values.SelectMany(x => x).Select(x => transformer(x.Instance));
        }

        static Func<TSaga, TProperty> GetGetMethod(PropertyInfo property)
        {
            return ReadPropertyCache<TSaga>.GetProperty<TProperty>(property).Get;
        }
    }
}
