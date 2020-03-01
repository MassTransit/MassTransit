namespace MassTransit.Azure.Storage
{
    using MessageData;
    using Microsoft.Azure.Storage;


    public static class AzureStorageConfigurationExtensions
    {
        public static AzureStorageMessageDataRepository CreateMessageDataRepository(this CloudStorageAccount account, string containerName)
        {
            return new AzureStorageMessageDataRepository(account.BlobEndpoint, containerName, account.Credentials, new NewIdBlobNameGenerator());
        }
    }
}
