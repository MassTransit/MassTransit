namespace MassTransit.Internals
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;


    public interface IReadOnlyPropertyCache<T> :
        IEnumerable<ReadOnlyProperty<T>>
    {
        bool TryGetValue(string key, out ReadOnlyProperty<T> value);
    }


    public class ReadOnlyPropertyCache<T> :
        IReadOnlyPropertyCache<T>
    {
        readonly IDictionary<string, ReadOnlyProperty<T>> _properties;

        public ReadOnlyPropertyCache()
        {
            _properties = CreatePropertyCache();
        }

        public IEnumerator<ReadOnlyProperty<T>> GetEnumerator()
        {
            return _properties.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool TryGetValue(string key, out ReadOnlyProperty<T> value)
        {
            try
            {
                return _properties.TryGetValue(key, out value);
            }
            catch (KeyNotFoundException)
            {
                throw new KeyNotFoundException($"The read only property {key} was not found.");
            }
        }

        static IDictionary<string, ReadOnlyProperty<T>> CreatePropertyCache()
        {
            return new Dictionary<string, ReadOnlyProperty<T>>(typeof(T).GetAllProperties()
                .Where(x => x.CanRead)
                .GroupBy(x => x.Name, StringComparer.OrdinalIgnoreCase)
                .Select(x => x.Last())
                .Select(x => new ReadOnlyProperty<T>(x))
                .ToDictionary(x => x.Property.Name));
        }

        public object Get(Expression<Func<T, object>> propertyExpression, T instance)
        {
            return _properties[propertyExpression.GetMemberName()].Get(instance);
        }
    }
}
