namespace MassTransit.Courier.Serialization
{
    using Newtonsoft.Json.Linq;


    /// <summary>
    /// Default conversion of properties using standard serialization approach
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DefaultJsonTypeConverter<T> :
        JsonTypeConverter<T>
        where T : class
    {
        public T Convert(JToken token)
        {
            if (token.Type == JTokenType.Null)
                token = new JObject();

            using (var jsonReader = new JTokenReader(token))
            {
                return (T)SerializerCache.Deserializer.Deserialize(jsonReader, typeof(T));
            }
        }
    }
}
