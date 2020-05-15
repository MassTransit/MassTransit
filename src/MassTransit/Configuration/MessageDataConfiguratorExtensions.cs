namespace MassTransit
{
    using MessageData;
    using MessageData.Conventions;
    using Transformation.TransformConfigurators;


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
    }
}
