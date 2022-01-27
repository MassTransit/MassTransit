namespace MassTransit.DynamoDb
{
    using Newtonsoft.Json;
    using Serialization;


    static class SagaSerializer
    {
        internal static string Serialize<T>(T value)
        {
            return JsonConvert.SerializeObject(value, typeof(T), JsonMessageSerializer.SerializerSettings);
        }

        internal static T Deserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json, JsonMessageSerializer.DeserializerSettings);
        }
    }
}
