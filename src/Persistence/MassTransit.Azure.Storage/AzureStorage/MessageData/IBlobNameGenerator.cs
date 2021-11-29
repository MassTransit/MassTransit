namespace MassTransit.AzureStorage.MessageData
{
    public interface IBlobNameGenerator
    {
        string GenerateBlobName();
    }
}
