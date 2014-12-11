namespace MassTransit.Internals.Mapping
{
    using System.Collections.Generic;


    public class ObjectArrayValueProvider :
        IArrayValueProvider
    {
        readonly object[] _values;

        public ObjectArrayValueProvider(object[] values)
        {
            _values = values;
        }

        public bool TryGetValue(int index, out object value)
        {
            if (index < 0 || index >= _values.Length)
            {
                value = null;
                return false;
            }

            value = _values[index];
            if (value is IDictionary<string, object>)
                value = new DictionaryObjectValueProvider((IDictionary<string, object>)value);
            else if (value is object[])
                value = new ObjectArrayValueProvider((object[])value);

            return true;
        }

        public bool TryGetValue<T>(int index, out T value)
        {
            object obj;
            if (TryGetValue(index, out obj))
            {
                if (obj is T)
                {
                    value = (T)obj;
                    return true;
                }
            }

            value = default(T);
            return false;
        }
    }
}