namespace MassTransit.MongoDbIntegration.Configuration
{
    using System;
    using MassTransit.MessageData;
    using MessageData;


    public static class MessageDataRepositorySelectorExtensions
    {
        public static IMessageDataRepository UsingMongoDbStorage(this IMessageDataRepositorySelector selector, string connectionString, string containerName = default)
        {
            if (selector is null)
            {
                throw new ArgumentNullException(nameof(selector));
            }

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentNullException(nameof(connectionString));
            }

            var repository = new MongoDbMessageDataRepository(connectionString, containerName ?? "message-data");

            return repository;
        }
    }
}
