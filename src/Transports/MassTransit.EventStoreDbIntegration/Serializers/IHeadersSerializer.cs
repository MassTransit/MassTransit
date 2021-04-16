namespace MassTransit.EventStoreDbIntegration.Serializers
{
    public interface IHeadersSerializer
    {
        byte[] Serialize<T>(SendContext<T> context) where T : class;
    }
}
