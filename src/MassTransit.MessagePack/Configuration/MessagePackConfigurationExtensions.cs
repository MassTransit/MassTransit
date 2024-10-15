namespace MassTransit;

using Serialization;


public static class MessagePackConfigurationExtensions
{
    /// <summary>
    /// Use the MessagePack serializer as the default serializer for the receive endpoint
    /// </summary>
    /// <param name="configurator"></param>
    /// <param name="isDefault"></param>
    public static void UseMessagePackSerializer(this IReceiveEndpointConfigurator configurator, bool isDefault = true)
    {
        var factory = new MessagePackSerializerFactory();

        configurator.AddSerializer(factory, isDefault);
        configurator.AddDeserializer(factory, isDefault);
    }

    /// <summary>
    /// Use the MessagePack serializer
    /// </summary>
    /// <param name="configurator"></param>
    /// <param name="isDefault"></param>
    public static void UseMessagePackSerializer(this IBusFactoryConfigurator configurator, bool isDefault = true)
    {
        var factory = new MessagePackSerializerFactory();

        configurator.AddSerializer(factory, isDefault);
        configurator.AddDeserializer(factory, isDefault);
    }

    /// <summary>
    /// Use the MessagePack deserializer, optionally setting it as the default message deserializer if no content type is found.
    /// </summary>
    /// <param name="configurator"></param>
    /// <param name="isDefault"></param>
    public static void UseMessagePackDeserializer(this IBusFactoryConfigurator configurator, bool isDefault = false)
    {
        var factory = new MessagePackSerializerFactory();

        configurator.AddDeserializer(factory, isDefault);
    }
}
