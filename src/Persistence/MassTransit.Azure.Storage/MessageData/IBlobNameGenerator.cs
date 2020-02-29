namespace MassTransit.Azure.Storage.MessageData
{
    public interface IBlobNameGenerator
    {
        string GenerateBlobName();
    }
}
