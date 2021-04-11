namespace MassTransit.EventStoreDbIntegration.Serializers
{
    public interface IHeadersSerializer
    {
        byte[] Serialize(SendContext context);
    }
}
