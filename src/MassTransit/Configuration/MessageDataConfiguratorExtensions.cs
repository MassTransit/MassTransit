namespace MassTransit
{
    using System;
    using Configuration;
    using MessageData.Configuration;
    using MessageData.Conventions;


    public static class MessageDataConfiguratorExtensions
    {
        /// <summary>
        /// Enable the loading of message data for the any message type that includes a MessageData property.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="repository"></param>
        public static void UseMessageData(this IBusFactoryConfigurator configurator, IMessageDataRepository repository)
        {
            if (configurator.ConsumeTopology.TryAddConvention(new MessageDataConsumeTopologyConvention(repository))
                && configurator.SendTopology.TryAddConvention(new MessageDataSendTopologyConvention(repository)))
            {
                // Courier does not use ConsumeContext, so it needs to be special
                var observer = new CourierMessageDataConfigurationObserver(configurator, repository, false);
            }
        }

        /// <summary>
        /// Enable the loading of message data for the any message type that includes a MessageData property.
        /// </summary>
        /// <param name="configurator">The bus factory configurator.</param>
        /// <param name="selector">
        /// The repository selector.
        /// See extension methods, e.g. <see cref="MessageDataRepositorySelectorExtensions.FileSystem" />.
        /// </param>
        public static IMessageDataRepository UseMessageData(this IBusFactoryConfigurator configurator,
            Func<IMessageDataRepositorySelector, IMessageDataRepository> selector)
        {
            if (configurator is null)
                throw new ArgumentNullException(nameof(configurator));

            if (selector is null)
                throw new ArgumentNullException(nameof(selector));

            var repository = selector(new MessageDataRepositorySelector(configurator));

            UseMessageData(configurator, repository);

            if (repository is IBusObserver observer)
                configurator.ConnectBusObserver(observer);

            return repository;
        }
    }
}
