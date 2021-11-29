namespace MassTransit
{
    using Azure.Storage.Blobs;
    using AzureStorage.MessageData;


    public static class AzureStorageConfigurationExtensions
    {
        public static AzureStorageMessageDataRepository CreateMessageDataRepository(this BlobServiceClient client, string containerName)
        {
            return new AzureStorageMessageDataRepository(client, containerName);
        }
    }
}
