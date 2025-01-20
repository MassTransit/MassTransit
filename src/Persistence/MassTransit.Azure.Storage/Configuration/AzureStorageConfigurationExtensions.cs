namespace MassTransit
{
    using Azure.Storage.Blobs;
    using AzureStorage.MessageData;


    public static class AzureStorageConfigurationExtensions
    {
        public static AzureStorageMessageDataRepository CreateMessageDataRepository(this BlobServiceClient client, string containerName, bool compress = false)
        {
            return new AzureStorageMessageDataRepository(client, containerName, compress);
        }
    }
}
