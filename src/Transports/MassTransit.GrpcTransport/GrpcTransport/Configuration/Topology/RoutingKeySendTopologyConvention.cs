namespace MassTransit.GrpcTransport.Configuration
{
    using MassTransit.Configuration;
    using Topology;


    public class RoutingKeySendTopologyConvention :
        IRoutingKeySendTopologyConvention
    {
        readonly ITopologyConventionCache<IMessageSendTopologyConvention> _cache;

        public RoutingKeySendTopologyConvention()
        {
            DefaultFormatter = new EmptyRoutingKeyFormatter();

            _cache = new TopologyConventionCache<IMessageSendTopologyConvention>(typeof(IRoutingKeyMessageSendTopologyConvention<>), new Factory());
        }

        bool IMessageSendTopologyConvention.TryGetMessageSendTopologyConvention<T>(out IMessageSendTopologyConvention<T> convention)
        {
            return _cache.GetOrAdd<T, IMessageSendTopologyConvention<T>>().TryGetMessageSendTopologyConvention(out convention);
        }

        public IRoutingKeyFormatter DefaultFormatter { get; set; }


        class Factory :
            IConventionTypeFactory<IMessageSendTopologyConvention>
        {
            IMessageSendTopologyConvention IConventionTypeFactory<IMessageSendTopologyConvention>.Create<T>()
            {
                return new RoutingKeyMessageSendTopologyConvention<T>(null);
            }
        }
    }
}
