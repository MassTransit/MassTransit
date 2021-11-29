namespace MassTransit
{
    public interface IServiceBusTopicConfigurator :
        IServiceBusMessageEntityConfigurator,
        ISpecification
    {
        /// <summary>
        /// If messages should be filtered before publishing
        /// </summary>
        bool? EnableFilteringMessagesBeforePublishing { set; }
    }
}
