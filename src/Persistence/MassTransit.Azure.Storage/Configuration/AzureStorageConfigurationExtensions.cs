using BlobServiceClient = Azure.Storage.Blobs.BlobServiceClient;


namespace MassTransit
{
    using System;
    using Azure.Storage.MessageData;


    public static class AzureStorageConfigurationExtensions
    {
        public static AzureStorageMessageDataRepository CreateMessageDataRepository(string connectionString, string containerName)
        {
            return new AzureStorageMessageDataRepository(connectionString, containerName);
        }

        public static AzureStorageMessageDataRepository CreateMessageDataRepository(Uri serviceUri, string containerName, string accountName, string accountKey)
        {
            return new AzureStorageMessageDataRepository(serviceUri, containerName, accountName, accountKey);
        }

        public static AzureStorageMessageDataRepository CreateMessageDataRepository(Uri serviceUri, string containerName, string signature)
        {
            return new AzureStorageMessageDataRepository(serviceUri, containerName, signature);
        }

        public static AzureStorageMessageDataRepository CreateMessageDataRepository(Uri serviceUri, string containerName, string tenantId, string clientId,
            string clientSecret)
        {
            return new AzureStorageMessageDataRepository(serviceUri, containerName, tenantId, clientId, clientSecret);
        }

        public static AzureStorageMessageDataRepository CreateMessageDataRepository(this BlobServiceClient client, string containerName)
        {
            return new AzureStorageMessageDataRepository(client, containerName);
        }
    }
}
