namespace MassTransit.Internals.Mapping
{
    using System.Collections.Generic;


    public class DictionaryObjectValueProvider :
        IObjectValueProvider
    {
        readonly IDictionary<string, object> _dictionary;

        public DictionaryObjectValueProvider(IDictionary<string, object> dictionary)
        {
            _dictionary = dictionary;
        }

        public bool TryGetValue(string name, out object value)
        {
            bool found = _dictionary.TryGetValue(name, out value);

            if (found)
            {
                if (value is IDictionary<string, object>)
                    value = new DictionaryObjectValueProvider((IDictionary<string, object>)value);

                if (value is object[])
                    value = new ObjectArrayValueProvider((object[])value);
            }

            return found;
        }

        public bool TryGetValue<T>(string name, out T value)
        {
            object obj;
            if (TryGetValue(name, out obj))
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