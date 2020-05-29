namespace MassTransit.Util
{
    using System;
    using System.Collections.Generic;
    using Context;


    public interface IObjectTypeDeserializer
    {
        T Deserialize<T>(IDictionary<string, object> dictionary, string key, T defaultValue);
        T Deserialize<T>(IHeaderProvider headerProvider, string key, T defaultValue);

        T Deserialize<T>(IDictionary<string, object> dictionary, string key);

        T Deserialize<T>(IDictionary<string, object> dictionary);
        T Deserialize<T>(object value);
        T Deserialize<T>(object value, T defaultValue);
        object Deserialize(object value, Type objectType, bool allowNull = false);
    }
}
