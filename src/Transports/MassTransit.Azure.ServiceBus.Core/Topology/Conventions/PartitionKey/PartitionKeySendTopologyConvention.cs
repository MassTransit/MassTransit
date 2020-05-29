namespace MassTransit.Azure.ServiceBus.Core.Topology.Conventions.PartitionKey
{
    using MassTransit.Topology;
    using MassTransit.Topology.Conventions;


    public class PartitionKeySendTopologyConvention :
        IPartitionKeySendTopologyConvention
    {
        readonly IConventionTypeCache<IMessageSendTopologyConvention> _typeCache;

        public PartitionKeySendTopologyConvention()
        {
            DefaultFormatter = new EmptyPartitionKeyFormatter();

            _typeCache = new ConventionTypeCache<IMessageSendTopologyConvention>(typeof(IPartitionKeyMessageSendTopologyConvention<>), new CacheFactory());
        }

        bool IMessageSendTopologyConvention.TryGetMessageSendTopologyConvention<T>(out IMessageSendTopologyConvention<T> convention)
        {
            return _typeCache.GetOrAdd<T, IMessageSendTopologyConvention<T>>().TryGetMessageSendTopologyConvention(out convention);
        }

        public IPartitionKeyFormatter DefaultFormatter { get; set; }


        class CacheFactory :
            IConventionTypeCacheFactory<IMessageSendTopologyConvention>
        {
            IMessageSendTopologyConvention IConventionTypeCacheFactory<IMessageSendTopologyConvention>.Create<T>()
            {
                return new PartitionKeyMessageSendTopologyConvention<T>(null);
            }
        }
    }
}
