namespace MassTransit.EntityFrameworkCoreIntegration
{
    using Newtonsoft.Json;
    using Serialization;


    static class JsonHelper
    {
        public static T Deserialize<T>(string json)
            where T : class
        {
            return string.IsNullOrWhiteSpace(json) ? null : JsonConvert.DeserializeObject<T>(json, JsonMessageSerializer.DeserializerSettings);
        }

        public static string Serialize<T>(T obj)
            where T : class
        {
            return obj == null ? null : JsonConvert.SerializeObject(obj, Formatting.None, JsonMessageSerializer.SerializerSettings);
        }
    }
}
