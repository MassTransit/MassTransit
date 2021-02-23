namespace MassTransit.Azure.Storage
{
    using MessageData;
    using global::Azure.Storage.Blobs;

    public static class AzureStorageConfigurationExtensions
    {
        public static AzureStorageMessageDataRepository CreateMessageDataRepository(this BlobServiceClient account, string containerName)
        {
            return new AzureStorageMessageDataRepository(account, containerName, new NewIdBlobNameGenerator());
        }
    }
}
