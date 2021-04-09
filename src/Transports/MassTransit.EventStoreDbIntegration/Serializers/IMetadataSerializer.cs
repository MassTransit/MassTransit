namespace MassTransit.EventStoreDbIntegration.Serializers
{
    public interface IMetadataSerializer
    {
        byte[] Serialize(SendContext context);
    }
}
