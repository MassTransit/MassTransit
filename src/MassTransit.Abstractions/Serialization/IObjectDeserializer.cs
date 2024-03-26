namespace MassTransit
{
    public interface IObjectDeserializer
    {
        T? DeserializeObject<T>(object? value, T? defaultValue = null)
            where T : class;

        T? DeserializeObject<T>(object? value, T? defaultValue = null)
            where T : struct;

        /// <summary>
        /// Serialize the dictionary to a message body, using the underlying serializer to ensure objects are properly serialized.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        MessageBody SerializeObject(object? value);
    }
}
