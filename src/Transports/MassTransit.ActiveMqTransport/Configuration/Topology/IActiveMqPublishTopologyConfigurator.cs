namespace MassTransit
{
    using ActiveMqTransport.Topology;


    public interface IActiveMqPublishTopologyConfigurator :
        IPublishTopologyConfigurator,
        IActiveMqPublishTopology
    {
        /// <summary>
        /// ActiveMQ supports publish/subscribe using virtual topics. By default, virtual topics
        /// must be prefixed with "VirtualTopic." however those defaults can be changed. Changing
        /// this setting will use the specified prefix instead (or "", if so specified).
        /// </summary>
        new string VirtualTopicPrefix { set; }

        new IActiveMqMessagePublishTopologyConfigurator<T> GetMessageTopology<T>()
            where T : class;
    }
}
