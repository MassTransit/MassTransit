namespace MassTransit
{
    using System;
    using Azure.Storage.Blobs;
    using Configuration;


    public static class MessageDataRepositorySelectorExtensions
    {
        /// <summary>
        /// Use Azure Blob Storage for message data storage
        /// </summary>
        /// <param name="selector"></param>
        /// <param name="connectionString"></param>
        /// <param name="containerName">Specify the container name (defaults to message-data)</param>
        /// <param name="compress">Specify if the file should be compressed (defaults to false)</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IMessageDataRepository AzureStorage(this IMessageDataRepositorySelector selector, string connectionString, string containerName = default, bool compress = false)
        {
            if (selector is null)
                throw new ArgumentNullException(nameof(selector));

            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentNullException(nameof(connectionString));

            var client = new BlobServiceClient(connectionString);

            var repository = client.CreateMessageDataRepository(containerName ?? "message-data", compress);

            return repository;
        }
    }
}
