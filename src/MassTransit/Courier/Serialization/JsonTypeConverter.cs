namespace MassTransit.Courier.Serialization
{
    using Newtonsoft.Json.Linq;


    /// <summary>
    /// Converts from a JToken to the requested type
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface JsonTypeConverter<out T>
        where T : class
    {
        T Convert(JToken token);
    }
}
