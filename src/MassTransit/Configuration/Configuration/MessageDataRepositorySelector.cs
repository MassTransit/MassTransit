namespace MassTransit.Configuration
{
    class MessageDataRepositorySelector :
        IMessageDataRepositorySelector
    {
        public MessageDataRepositorySelector(IBusFactoryConfigurator configurator)
        {
            Configurator = configurator;
        }

        public IBusFactoryConfigurator Configurator { get; }
    }
}
