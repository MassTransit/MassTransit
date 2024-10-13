namespace MassTransit.Serialization.MessagePackFormatters;

using MessagePack;
using MessagePack.Formatters;


public class InterfaceConcreteMapFormatter<TInterface, TImplementation> :
    IMessagePackFormatter<TInterface>
    where TImplementation : TInterface
{
    public virtual void Serialize(ref MessagePackWriter writer, TInterface value, MessagePackSerializerOptions options)
    {
        IMessagePackFormatter<TImplementation> innerFormatter = options.Resolver.GetFormatterWithVerify<TImplementation>();

        if (value is TImplementation implementation)
            innerFormatter.Serialize(ref writer, implementation, options);
        else
            innerFormatter.Serialize(ref writer, (TImplementation)value!, options);
    }

    public virtual TInterface Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
    {
        IMessagePackFormatter<TImplementation> innerFormatter = options.Resolver.GetFormatterWithVerify<TImplementation>();

        return innerFormatter.Deserialize(ref reader, options);
    }
}
