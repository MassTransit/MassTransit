namespace MassTransit
{
    using System;
    using Audit;
    using Audit.MetadataFactories;
    using Audit.Observers;
    using Configuration;
    using Util;


    public static class AuditConfigurationExtensions
    {
        /// <summary>
        /// Adds observers that will audit all published and sent messages, sending them to the message audit store after they are sent/published.
        /// </summary>
        /// <param name="connector">The bus</param>
        /// <param name="store">Audit store</param>
        /// <param name="configureFilter">Filter configuration delegate</param>
        /// <param name="metadataFactory">Message metadata factory. If omitted, the default one will be used.</param>
        public static ConnectHandle ConnectSendAuditObservers<T>(this T connector, IMessageAuditStore store,
            Action<IMessageFilterConfigurator> configureFilter = null, ISendMetadataFactory metadataFactory = null)
            where T : ISendObserverConnector, IPublishObserverConnector
        {
            var specification = new SendMessageFilterConfigurator();
            configureFilter?.Invoke(specification);

            var factory = metadataFactory ?? new DefaultSendMetadataFactory();

            var sendHandle = connector.ConnectSendObserver(new AuditSendObserver(store, factory, specification.Filter));
            var publishHandle = connector.ConnectPublishObserver(new AuditPublishObserver(store, factory, specification.Filter));

            return new MultipleConnectHandle(sendHandle, publishHandle);
        }

        /// <summary>
        /// Add an observer that will audit consumed messages, sending them to the message audit store prior to consumption by the consumer
        /// </summary>
        /// <param name="connector">The bus or endpoint</param>
        /// <param name="store">The audit store</param>
        /// <param name="configureFilter">Filter configuration delegate</param>
        /// <param name="metadataFactory">Message metadata factory. If omitted, the default one will be used.</param>
        public static ConnectHandle ConnectConsumeAuditObserver(this IConsumeObserverConnector connector, IMessageAuditStore store,
            Action<IMessageFilterConfigurator> configureFilter = null, IConsumeMetadataFactory metadataFactory = null)
        {
            if (connector == null)
                throw new ArgumentNullException(nameof(connector));
            if (store == null)
                throw new ArgumentNullException(nameof(store));

            var filterConfigurator = new ConsumeMessageFilterConfigurator();
            configureFilter?.Invoke(filterConfigurator);

            var factory = metadataFactory ?? new DefaultConsumeMetadataFactory();

            return connector.ConnectConsumeObserver(new AuditConsumeObserver(store, factory, filterConfigurator.Filter));
        }
    }
}
