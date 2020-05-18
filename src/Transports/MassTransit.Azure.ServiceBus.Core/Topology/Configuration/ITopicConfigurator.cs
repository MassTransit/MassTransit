namespace MassTransit.Azure.ServiceBus.Core.Topology
{
    using GreenPipes;


    public interface ITopicConfigurator :
        IMessageEntityConfigurator,
        ISpecification
    {
        /// <summary>
        /// If messages should be filtered before publishing
        /// </summary>
        bool? EnableFilteringMessagesBeforePublishing { set; }
    }
}
