namespace MassTransit.LegacySupport.SerializationCustomization
{
    using System.Runtime.Serialization;

    public interface LegacySurrogate :
        ISerializationSurrogate
    {
        string SurrogateTypeName { get; }
    }
}