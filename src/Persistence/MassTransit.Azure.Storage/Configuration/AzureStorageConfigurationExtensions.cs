using BlobServiceClient = Azure.Storage.Blobs.BlobServiceClient;


namespace MassTransit
{
    using Azure.Storage.MessageData;


    public static class AzureStorageConfigurationExtensions
    {
        public static AzureStorageMessageDataRepository CreateMessageDataRepository(this BlobServiceClient client, string containerName)
        {
            return new AzureStorageMessageDataRepository(client, containerName);
        }
    }
}
