using MassTransit.Serialization;

namespace MassTransit.Configuration
{
    public static class MassTransitSerilizationOptionsExtensions
    {
        public static void UseDotNetXmlSerilaizer(this BusConfiguration cfg)
        {
            cfg.UseCustomSerializer<DotNotXmlMessageSerializer>();
        }
        public static void UseJsonSerializer(this BusConfiguration cfg)
        {
            cfg.UseCustomSerializer<JsonMessageSerializer>();
        }
        public static void UseXmlSerializer(this BusConfiguration cfg)
        {
            cfg.UseCustomSerializer<XmlMessageSerializer>();
        }
        public static void UseBinarySerializer(this BusConfiguration cfg)
        {
            cfg.UseCustomSerializer<BinaryMessageSerializer>();
        }       
    }
}