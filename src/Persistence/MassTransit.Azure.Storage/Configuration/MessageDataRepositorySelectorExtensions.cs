using BlobServiceClient = Azure.Storage.Blobs.BlobServiceClient;


namespace MassTransit
{
    using System;
    using MessageData;


    public static class MessageDataRepositorySelectorExtensions
    {
        public static IMessageDataRepository WithAzureStorage(this IMessageDataRepositorySelector selector, string connectionString, string containerName = default)
        {
            if (selector is null)
            {
                throw new ArgumentNullException(nameof(selector));
            }

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentNullException(nameof(connectionString));
            }

            var client = new BlobServiceClient(connectionString);

            var repository = client.CreateMessageDataRepository(containerName ?? "message-data");

            return repository;
        }
    }
}
