namespace MassTransit.MessageData.Configuration
{
    using System;
    using System.IO;
    using MassTransit.Configuration;
    using Serialization;


    public static class MessageDataRepositorySelectorExtensions
    {
        public static IMessageDataRepository InMemory(this IMessageDataRepositorySelector selector)
        {
            if (selector is null)
                throw new ArgumentNullException(nameof(selector));

            return new InMemoryMessageDataRepository();
        }

        public static IMessageDataRepository FileSystem(this IMessageDataRepositorySelector selector, string path)
        {
            if (selector is null)
                throw new ArgumentNullException(nameof(selector));
            if (path is null)
                throw new ArgumentNullException(nameof(path));

            var dataDirectory = new DirectoryInfo(path);

            return new FileSystemMessageDataRepository(dataDirectory);
        }

        public static IMessageDataRepository Encrypted(this IMessageDataRepositorySelector selector, ICryptoStreamProvider streamProvider,
            Func<IMessageDataRepositorySelector, IMessageDataRepository> innerSelector)
        {
            if (selector is null)
                throw new ArgumentNullException(nameof(selector));
            if (streamProvider is null)
                throw new ArgumentNullException(nameof(streamProvider));
            if (innerSelector == null)
                throw new ArgumentNullException(nameof(innerSelector));

            var innerRepository = innerSelector(selector);
            if (innerRepository is EncryptedMessageDataRepository)
                throw new ArgumentException("Nesting encrypted repositories is not supported", nameof(innerSelector));

            if (innerRepository is IBusObserver observer)
                selector.Configurator.ConnectBusObserver(observer);

            return new EncryptedMessageDataRepository(innerRepository, streamProvider);
        }
    }
}
