namespace MassTransit.MessageData.Configuration
{
    using System;
    using System.IO;
    using Serialization;


    public static class MessageDataRepositorySelectorExtensions
    {
        public static IMessageDataRepository WithInMemoryStorage(this IMessageDataRepositorySelector selector)
        {
            if (selector is null)
            {
                throw new ArgumentNullException(nameof(selector));
            }

            var repository = new InMemoryMessageDataRepository();

            return repository;
        }

        public static IMessageDataRepository WithFileSystemStorage(this IMessageDataRepositorySelector selector, string path)
        {
            if (selector is null)
            {
                throw new ArgumentNullException(nameof(selector));
            }

            if (path is null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            var dataDirectory = new DirectoryInfo(path);

            var repository = new FileSystemMessageDataRepository(dataDirectory);

            return repository;
        }

        public static IMessageDataRepository WithEncryptedStorage(this IMessageDataRepositorySelector selector, IMessageDataRepository innerRepository, ICryptoStreamProvider streamProvider)
        {
            if (selector is null)
            {
                throw new ArgumentNullException(nameof(selector));
            }

            if (innerRepository is null)
            {
                throw new ArgumentNullException(nameof(innerRepository));
            }

            if (streamProvider is null)
            {
                throw new ArgumentNullException(nameof(streamProvider));
            }

            var repository = new EncryptedMessageDataRepository(innerRepository, streamProvider);

            return repository;
        }
    }
}
