namespace MassTransit
{
    using System;
    using Configuration;
    using MongoDbIntegration.MessageData;


    public static class MongoDbMessageDataConfigurationExtensions
    {
        /// <summary>
        /// Use MongoDB for message data storage, via GridFS
        /// </summary>
        /// <param name="selector"></param>
        /// <param name="connectionString"></param>
        /// <param name="containerName">Specify the container name (defaults to message-data)</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IMessageDataRepository MongoDb(this IMessageDataRepositorySelector selector, string connectionString,
            string containerName = default)
        {
            if (selector is null)
                throw new ArgumentNullException(nameof(selector));

            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentNullException(nameof(connectionString));

            return new MongoDbMessageDataRepository(connectionString, containerName ?? "message-data");
        }
    }
}
