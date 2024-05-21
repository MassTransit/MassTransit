namespace MassTransit.Configuration
{
    public class PartitionKeySendTopologyConvention :
        IPartitionKeySendTopologyConvention
    {
        readonly ITopologyConventionCache<IMessageSendTopologyConvention> _cache;

        public PartitionKeySendTopologyConvention()
        {
            _cache = new TopologyConventionCache<IMessageSendTopologyConvention>(typeof(IPartitionKeyMessageSendTopologyConvention<>), new Factory());
        }

        bool IMessageSendTopologyConvention.TryGetMessageSendTopologyConvention<T>(out IMessageSendTopologyConvention<T> convention)
        {
            return _cache.GetOrAdd<T, IMessageSendTopologyConvention<T>>().TryGetMessageSendTopologyConvention(out convention);
        }


        class Factory :
            IConventionTypeFactory<IMessageSendTopologyConvention>
        {
            IMessageSendTopologyConvention IConventionTypeFactory<IMessageSendTopologyConvention>.Create<T>()
            {
                return new PartitionKeyMessageSendTopologyConvention<T>(null);
            }
        }
    }
}
