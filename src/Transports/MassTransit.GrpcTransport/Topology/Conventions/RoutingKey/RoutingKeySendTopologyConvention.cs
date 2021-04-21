namespace MassTransit.GrpcTransport.Topology.Conventions.RoutingKey
{
    using MassTransit.Topology;
    using MassTransit.Topology.Conventions;


    public class RoutingKeySendTopologyConvention :
        IRoutingKeySendTopologyConvention
    {
        readonly IConventionTypeCache<IMessageSendTopologyConvention> _typeCache;

        public RoutingKeySendTopologyConvention()
        {
            DefaultFormatter = new EmptyRoutingKeyFormatter();

            _typeCache = new ConventionTypeCache<IMessageSendTopologyConvention>(typeof(IRoutingKeyMessageSendTopologyConvention<>), new CacheFactory());
        }

        bool IMessageSendTopologyConvention.TryGetMessageSendTopologyConvention<T>(out IMessageSendTopologyConvention<T> convention)
        {
            return _typeCache.GetOrAdd<T, IMessageSendTopologyConvention<T>>().TryGetMessageSendTopologyConvention(out convention);
        }

        public IRoutingKeyFormatter DefaultFormatter { get; set; }


        class CacheFactory :
            IConventionTypeCacheFactory<IMessageSendTopologyConvention>
        {
            IMessageSendTopologyConvention IConventionTypeCacheFactory<IMessageSendTopologyConvention>.Create<T>()
            {
                return new RoutingKeyMessageSendTopologyConvention<T>(null);
            }
        }
    }
}
