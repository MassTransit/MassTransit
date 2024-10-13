namespace MassTransit;

using Serialization;


public static class MessagePackConfigurationExtensions
{
    public static void UseMessagePack(this IReceiveEndpointConfigurator configurator, bool isDefault = false)
    {
        var factory = new MessagePackSerializerFactory();

        configurator.AddSerializer(factory, isDefault);
        configurator.AddDeserializer(factory, isDefault);
    }

    public static void UseMessagePack(this IBusFactoryConfigurator configurator, bool isDefault = false)
    {
        var factory = new MessagePackSerializerFactory();

        configurator.AddSerializer(factory, isDefault);
        configurator.AddDeserializer(factory, isDefault);
    }
}
