namespace MassTransit
{
    public interface IServiceBusTopicConfigurator :
        IServiceBusMessageEntityConfigurator,
        ISpecification
    {
        /// <summary>
        /// If True, the topic will deliver messages to subscriptions in order
        /// </summary>
        bool? SupportOrdering { set; }
    }
}
