namespace MassTransit.Topology.Topologies
{
    using System;
    using Configuration;
    using Util;


    public abstract class HostTopology :
        IHostTopology
    {
        protected static readonly INewIdFormatter Formatter = FormatUtil.Formatter;

        readonly IHostConfiguration _hostConfiguration;
        readonly ITopologyConfiguration _topologyConfiguration;

        protected HostTopology(IHostConfiguration hostConfiguration, ITopologyConfiguration topologyConfiguration)
        {
            _hostConfiguration = hostConfiguration;
            _topologyConfiguration = topologyConfiguration;
        }

        public IPublishTopology PublishTopology => _topologyConfiguration.Publish;
        public ISendTopology SendTopology => _topologyConfiguration.Send;

        public IMessagePublishTopology<T> Publish<T>()
            where T : class
        {
            return _topologyConfiguration.Publish.GetMessageTopology<T>();
        }

        public IMessageSendTopology<T> Send<T>()
            where T : class
        {
            return _topologyConfiguration.Send.GetMessageTopology<T>();
        }

        public IMessageTopology<T> Message<T>()
            where T : class
        {
            return _topologyConfiguration.Message.GetMessageTopology<T>();
        }

        public virtual bool TryGetPublishAddress(Type messageType, out Uri publishAddress)
        {
            return _topologyConfiguration.Publish.TryGetPublishAddress(messageType, _hostConfiguration.HostAddress, out publishAddress);
        }

        public virtual bool TryGetPublishAddress<T>(out Uri publishAddress)
            where T : class
        {
            return _topologyConfiguration.Publish.GetMessageTopology<T>().TryGetPublishAddress(_hostConfiguration.HostAddress, out publishAddress);
        }
    }
}
